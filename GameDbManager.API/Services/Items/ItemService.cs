using GameDbManager.API.Data;
using GameDbManager.API.Models.Items;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace GameDbManager.API.Services.Items
{
    public class ItemService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<ItemService> _logger;

        public ItemService(GameDbContext context, ILogger<ItemService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Importa items desde un archivo XML
        /// </summary>
        /// <param name="xmlContent">Contenido XML con los items</param>
        /// <param name="overwriteExisting">Si se deben sobrescribir items existentes</param>
        /// <returns>Mensaje con el resultado de la importación</returns>
        public async Task<string> ImportItemsFromXml(string xmlContent, bool overwriteExisting = false)
        {
            try
            {
                // Limpiamos el XML de caracteres inválidos
                xmlContent = SanitizeXml(xmlContent);

                // Utilizamos un enfoque más robusto para procesar elementos individuales
                var items = new List<Item>();
                int processedCount = 0;
                int successCount = 0;
                int errorCount = 0;
                var errors = new List<string>();

                // En lugar de parsear todo el XML de una vez, extraeremos cada item individualmente
                var itemRegex = new Regex(@"<(armor|weapon|accessory|accessorie|jewelry|etc)\s+[^>]*>.*?</(armor|weapon|accessory|accessorie|jewelry|etc)>",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);

                var matches = itemRegex.Matches(xmlContent);
                processedCount = matches.Count;

                foreach (Match match in matches)
                {
                    try
                    {
                        var itemXml = CleanupItemXml(match.Value);
                        var item = ParseSingleItem(itemXml);

                        if (item != null)
                        {
                            items.Add(item);
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        errors.Add($"Error parsing item at position {match.Index}: {ex.Message}");
                        _logger.LogError(ex, "Error parsing item at position {Position}", match.Index);
                    }
                }

                // Ahora importamos los items que se pudieron parsear correctamente
                int importedCount = 0;
                int failedCount = 0;

                if (items.Any())
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Items ON");

                            foreach (var item in items)
                            {
                                try
                                {
                                    // Verificar si el item ya existe
                                    var existingItem = await _context.Items.FindAsync(item.Id);

                                    if (existingItem != null)
                                    {
                                        if (overwriteExisting)
                                        {
                                            // Eliminar las relaciones del item existente
                                            await DeleteItemRelations(existingItem.Id);

                                            // Eliminar el item existente
                                            _context.Items.Remove(existingItem);
                                            await _context.SaveChangesAsync();

                                            // Agregar el nuevo item
                                            await _context.Items.AddAsync(item);
                                        }
                                        else
                                        {
                                            failedCount++;
                                            errors.Add($"Item with ID {item.Id} already exists. Use overwrite option to replace it.");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        // Agregar nuevo item
                                        await _context.Items.AddAsync(item);
                                    }

                                    await _context.SaveChangesAsync();
                                    importedCount++;
                                }
                                catch (Exception ex)
                                {
                                    failedCount++;
                                    errors.Add($"Error importing item {item.Id} - {item.Name}: {ex.Message}");
                                    _logger.LogError(ex, "Error importing item {ItemId}", item.Id);

                                    // Limpiar el contexto para continuar con el siguiente item
                                    _context.ChangeTracker.Clear();
                                }
                            }

                            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Items OFF");
                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return $"Error during database operation: {ex.Message}";
                        }
                    }
                }

                // Generar reporte
                var result = new StringBuilder();
                result.AppendLine($"XML import results:");
                result.AppendLine($"- Found: {processedCount} items");
                result.AppendLine($"- Successfully parsed: {successCount} items");
                result.AppendLine($"- Successfully imported to database: {importedCount} items");
                result.AppendLine($"- Failed to parse: {errorCount} items");
                result.AppendLine($"- Failed to import: {failedCount} items");

                if (errors.Any())
                {
                    result.AppendLine("\nErrors (limited to first 10):");
                    foreach (var error in errors.Take(10))
                    {
                        result.AppendLine($"- {error}");
                    }

                    if (errors.Count > 10)
                    {
                        result.AppendLine($"... and {errors.Count - 10} more errors.");
                    }
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error during import");
                return $"Critical error during import: {ex.Message}";
            }
        }

        /// <summary>
        /// Valida el formato del XML
        /// </summary>
        public bool ValidateXml(string xmlContent)
        {
            try
            {
                // Sanitizar el XML
                xmlContent = SanitizeXml(xmlContent);

                // Intentar cargar el XML en un XDocument para verificar que sea válido
                XDocument.Parse(xmlContent);

                // También verificar que tenga elementos de item
                var hasItems = Regex.IsMatch(xmlContent, "<(armor|weapon|accessory|accessorie|jewelry|etc)\\s+");

                return hasItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "XML validation error");
                return false;
            }
        }

        /// <summary>
        /// Elimina las relaciones de un item (stats, skills)
        /// </summary>
        private async Task DeleteItemRelations(int itemId)
        {
            // Eliminar stats relacionados
            var stats = await _context.ItemStats
                .Where(s => s.ItemId == itemId)
                .ToListAsync();

            if (stats.Any())
            {
                _context.ItemStats.RemoveRange(stats);
            }

            // Eliminar skills relacionados
            var skills = await _context.ItemSkills
                .Where(s => s.ItemId == itemId)
                .ToListAsync();

            if (skills.Any())
            {
                _context.ItemSkills.RemoveRange(skills);
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Limpia el XML de un item específico
        /// </summary>
        private string CleanupItemXml(string itemXml)
        {
            // Buscar la primera etiqueta de apertura válida
            var match = Regex.Match(itemXml, @"<(armor|weapon|accessory|accessorie|jewelry|etc)");
            if (match.Success && match.Index > 0)
            {
                // Eliminar cualquier contenido antes del inicio de la etiqueta
                itemXml = itemXml.Substring(match.Index);
            }
            return itemXml;
        }

        /// <summary>
        /// Parsea un único item desde su XML
        /// </summary>
        private Item ParseSingleItem(string itemXml)
        {
            try
            {
                XElement itemElement = XElement.Parse(itemXml);
                string itemType = itemElement.Name.LocalName.ToLower();

                // Crear la instancia del tipo correcto
                Item item = null;
                switch (itemType)
                {
                    case "armor":
                        item = new Armor();
                        if (itemElement.Attribute("armorType") != null)
                        {
                            ((Armor)item).ArmorType = itemElement.Attribute("armorType").Value;
                        }
                        break;
                    case "weapon":
                        item = new Weapon();
                        if (itemElement.Attribute("weaponType") != null)
                        {
                            ((Weapon)item).WeaponType = itemElement.Attribute("weaponType").Value;
                        }
                        break;
                    case "accessory":
                    case "accessorie":
                        item = new Accessory();
                        break;
                    case "jewelry":
                        item = new Jewelry();
                        break;
                    case "etc":
                        item = new Etc();
                        if (itemElement.Attribute("itemType") != null)
                        {
                            ((Etc)item).ItemType = itemElement.Attribute("itemType").Value;
                        }
                        break;
                    default:
                        return null;
                }

                // Propiedades básicas
                item.Id = int.Parse(itemElement.Attribute("id")?.Value ?? "0");
                item.Name = itemElement.Attribute("name")?.Value ?? "Unknown";

                // Procesar elementos set
                foreach (var setElement in itemElement.Elements("set"))
                {
                    ProcessSetElement(setElement, item);
                }

                // Procesar elementos stats
                var statsElement = itemElement.Element("stats");
                if (statsElement != null)
                {
                    ProcessStatsElement(statsElement, item);
                }

                // Procesar elementos skills
                var skillsElement = itemElement.Element("skills");
                if (skillsElement != null)
                {
                    ProcessSkillsElement(skillsElement, item);
                }

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing item: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Procesa un elemento set y aplica sus valores al item
        /// </summary>
        private void ProcessSetElement(XElement setElement, Item item)
        {
            // Procesamos cada atributo posible
            var icon = setElement.Attribute("icon");
            if (icon != null)
            {
                item.Icon = icon.Value;
            }

            var weight = setElement.Attribute("weight");
            if (weight != null && int.TryParse(weight.Value, out int weightVal))
            {
                item.Weight = weightVal;
            }

            var price = setElement.Attribute("price");
            if (price != null && int.TryParse(price.Value, out int priceVal))
            {
                item.Price = priceVal;
            }

            var sellable = setElement.Attribute("sellable");
            if (sellable != null)
            {
                item.Sellable = sellable.Value.ToLower() == "true";
            }

            var tradeable = setElement.Attribute("tradeable");
            if (tradeable != null)
            {
                item.Tradeable = tradeable.Value.ToLower() == "true";
            }

            var dropable = setElement.Attribute("dropable");
            if (dropable != null)
            {
                item.Dropable = dropable.Value.ToLower() == "true";
            }

            var destroyable = setElement.Attribute("destroyable");
            if (destroyable != null)
            {
                item.Destroyable = destroyable.Value.ToLower() == "true";
            }

            // Procesar bodyPart, grade y crystallizable (ahora en la clase base)
            var bodyPart = setElement.Attribute("bodyPart");
            if (bodyPart != null)
            {
                item.BodyPart = bodyPart.Value;
            }

            var grade = setElement.Attribute("grade");
            if (grade != null)
            {
                item.Grade = grade.Value;
            }

            var crystallizable = setElement.Attribute("crystallizable");
            if (crystallizable != null)
            {
                item.Crystallizable = crystallizable.Value.ToLower() == "true";
            }

            // Procesar stackable específico para Etc
            if (item is Etc etc)
            {
                var stackable = setElement.Attribute("stackable");
                if (stackable != null)
                {
                    etc.Stackable = stackable.Value.ToLower() == "true";
                }
            }
        }

        /// <summary>
        /// Procesa las estadísticas de un item
        /// </summary>
        private void ProcessStatsElement(XElement statsElement, Item item)
        {
            foreach (var statElement in statsElement.Elements())
            {
                try
                {
                    var statType = statElement.Name.LocalName; // add, set, sub, enchant
                    var stat = statElement.Attribute("stat")?.Value;
                    var order = statElement.Attribute("order")?.Value;
                    var val = statElement.Attribute("val")?.Value;

                    if (!string.IsNullOrEmpty(stat) && !string.IsNullOrEmpty(val))
                    {
                        var itemStat = new ItemStat
                        {
                            ItemId = item.Id,
                            StatType = statType,
                            Stat = stat,
                            Order = order,
                            Value = val
                        };

                        item.Stats.Add(itemStat);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing stat element for item {ItemId}", item.Id);
                }
            }
        }

        /// <summary>
        /// Procesa las habilidades de un item
        /// </summary>
        private void ProcessSkillsElement(XElement skillsElement, Item item)
        {
            foreach (var skillElement in skillsElement.Elements("skill"))
            {
                try
                {
                    var skillId = skillElement.Attribute("id")?.Value;
                    var level = skillElement.Attribute("level")?.Value;

                    if (!string.IsNullOrEmpty(skillId) && int.TryParse(skillId, out var skillIdValue) &&
                        !string.IsNullOrEmpty(level) && int.TryParse(level, out var levelValue))
                    {
                        var itemSkill = new ItemSkill
                        {
                            ItemId = item.Id,
                            SkillId = skillIdValue,
                            SkillLevel = levelValue
                        };

                        item.Skills.Add(itemSkill);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing skill element for item {ItemId}", item.Id);
                }
            }
        }

        /// <summary>
        /// Limpia y repara el XML
        /// </summary>
        private string SanitizeXml(string xmlContent)
        {
            // Eliminar caracteres inválidos
            var sanitized = new string(xmlContent.Where(c => c > 0x1F || c == 0x09 || c == 0x0A || c == 0x0D).ToArray());

            // Asegurar que inicia con declaración XML correcta
            if (!sanitized.TrimStart().StartsWith("<?xml"))
            {
                sanitized = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" + sanitized;
            }

            // Si no hay un elemento raíz, envolvemos todo en <list>
            if (!sanitized.Contains("<list>"))
            {
                var firstItemMatch = Regex.Match(sanitized, @"<(armor|weapon|accessory|accessorie|jewelry|etc)");
                if (firstItemMatch.Success)
                {
                    int index = firstItemMatch.Index;
                    sanitized = sanitized.Insert(index, "<list>\n");

                    // Verificar si ya hay un cierre de </list> o añadirlo
                    if (!sanitized.Contains("</list>"))
                    {
                        sanitized += "\n</list>";
                    }
                }
            }

            // Corregir posible XML truncado
            var openList = sanitized.IndexOf("<list>");
            var closeList = sanitized.LastIndexOf("</list>");

            if (openList != -1 && closeList == -1)
            {
                sanitized += "\n</list>";
            }

            return sanitized;
        }

        // Métodos CRUD estándar
        public List<Item> GetAllItems()
        {
            return _context.Items
                .Include(i => i.Stats)
                .Include(i => i.Skills)
                .ToList();
        }

        public Item GetItemById(int id)
        {
            return _context.Items
                .Include(i => i.Stats)
                .Include(i => i.Skills)
                .FirstOrDefault(i => i.Id == id);
        }

        public void CreateItem(Item item)
        {
            _context.Items.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(Item item)
        {
            // Manejar estadísticas y habilidades para evitar duplicados
            var existingItem = _context.Items
                .Include(i => i.Stats)
                .Include(i => i.Skills)
                .FirstOrDefault(i => i.Id == item.Id);

            if (existingItem != null)
            {
                // Actualizar propiedades básicas
                _context.Entry(existingItem).CurrentValues.SetValues(item);

                // Actualizar estadísticas
                UpdateItemStats(existingItem, item);

                // Actualizar habilidades
                UpdateItemSkills(existingItem, item);
            }
            else
            {
                _context.Items.Update(item);
            }

            _context.SaveChanges();
        }

        private void UpdateItemStats(Item existingItem, Item updatedItem)
        {
            // Eliminar estadísticas que ya no están presentes
            var statsToRemove = existingItem.Stats.Where(s =>
                !updatedItem.Stats.Any(us => us.Id == s.Id)).ToList();

            foreach (var stat in statsToRemove)
            {
                _context.ItemStats.Remove(stat);
            }

            // Actualizar o agregar estadísticas nuevas
            foreach (var stat in updatedItem.Stats)
            {
                var existingStat = existingItem.Stats.FirstOrDefault(s => s.Id == stat.Id);

                if (existingStat != null)
                {
                    // Actualizar estadística existente
                    _context.Entry(existingStat).CurrentValues.SetValues(stat);
                }
                else
                {
                    // Agregar nueva estadística
                    existingItem.Stats.Add(stat);
                }
            }
        }

        private void UpdateItemSkills(Item existingItem, Item updatedItem)
        {
            // Eliminar habilidades que ya no están presentes
            var skillsToRemove = existingItem.Skills.Where(s =>
                !updatedItem.Skills.Any(us => us.Id == s.Id)).ToList();

            foreach (var skill in skillsToRemove)
            {
                _context.ItemSkills.Remove(skill);
            }

            // Actualizar o agregar habilidades nuevas
            foreach (var skill in updatedItem.Skills)
            {
                var existingSkill = existingItem.Skills.FirstOrDefault(s => s.Id == skill.Id);

                if (existingSkill != null)
                {
                    // Actualizar habilidad existente
                    _context.Entry(existingSkill).CurrentValues.SetValues(skill);
                }
                else
                {
                    // Agregar nueva habilidad
                    existingItem.Skills.Add(skill);
                }
            }
        }

        public void DeleteItem(int id)
        {
            var item = _context.Items.Find(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}