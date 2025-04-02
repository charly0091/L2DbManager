using GameDbManager.API.Data;
using GameDbManager.API.Models.Items;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace GameDbManager.API.Services.Items
{
    public class ItemService
    {
        private readonly GameDbContext _context;

        public ItemService(GameDbContext context)
        {
            _context = context;
        }

        public string ImportItemsFromXml(string xmlContent)
        {
            var items = ParseXml(xmlContent);
            int importedCount = 0;
            int failedCount = 0;

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Items ON");

                    _context.Items.AddRange(items);
                    importedCount = _context.SaveChanges();

                    _context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Items OFF");

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Error importing items: {ex.Message}");
                    failedCount = items.Count - importedCount;
                }
            }

            return $"Imported {importedCount} items successfully. Failed to import {failedCount} items.";
        }

        public List<Item> ParseXml(string xmlContent)
        {
            var xml = XDocument.Parse(xmlContent);
            var items = new List<Item>();

            foreach (var element in xml.Root.Elements())
            {
                Item item = null;
                try
                {
                    switch (element.Name.LocalName)
                    {
                        case "accessorie":
                            item = ParseAccessory(element);
                            break;
                        case "armor":
                            item = ParseArmor(element);
                            break;
                        case "etc":
                            item = ParseEtc(element);
                            break;
                        case "jewelry":
                            item = ParseJewelry(element);
                            break;
                        case "weapon":
                            item = ParseWeapon(element);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the error appropriately
                    Console.WriteLine($"Error parsing XML for item: {ex.Message}");
                }

                if (item != null)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        private Accessory ParseAccessory(XElement element)
        {
            var accessory = new Accessory
            {
                Id = (int)element.Attribute("id"),
                Name = (string)element.Attribute("name")
            };

            foreach (var setElement in element.Elements("set"))
            {
                if (setElement.Attribute("icon") != null)
                    accessory.Icon = (string)setElement.Attribute("icon");

                if (setElement.Attribute("bodyPart") != null)
                    accessory.BodyPart = (string)setElement.Attribute("bodyPart");

                if (setElement.Attribute("grade") != null)
                    accessory.Grade = (string)setElement.Attribute("grade");

                if (setElement.Attribute("crystallizable") != null)
                    accessory.Crystallizable = (bool)setElement.Attribute("crystallizable");

                if (setElement.Attribute("weight") != null)
                    accessory.Weight = (int)setElement.Attribute("weight");

                if (setElement.Attribute("price") != null)
                    accessory.Price = (int)setElement.Attribute("price");

                if (setElement.Attribute("sellable") != null)
                    accessory.Sellable = (bool)setElement.Attribute("sellable");

                if (setElement.Attribute("tradeable") != null)
                    accessory.Tradeable = (bool)setElement.Attribute("tradeable");

                if (setElement.Attribute("dropable") != null)
                    accessory.Dropable = (bool)setElement.Attribute("dropable");

                if (setElement.Attribute("destroyable") != null)
                    accessory.Destroyable = (bool)setElement.Attribute("destroyable");
            }

            return accessory;
        }

        private Armor ParseArmor(XElement element)
        {
            var armor = new Armor
            {
                Id = (int)element.Attribute("id"),
                Name = (string)element.Attribute("name"),
                ArmorType = (string)element.Attribute("armorType")
            };

            foreach (var setElement in element.Elements("set"))
            {
                if (setElement.Attribute("icon") != null)
                    armor.Icon = (string)setElement.Attribute("icon");

                if (setElement.Attribute("bodyPart") != null)
                    armor.BodyPart = (string)setElement.Attribute("bodyPart");

                if (setElement.Attribute("grade") != null)
                    armor.Grade = (string)setElement.Attribute("grade");

                if (setElement.Attribute("crystallizable") != null)
                    armor.Crystallizable = (bool)setElement.Attribute("crystallizable");

                if (setElement.Attribute("weight") != null)
                    armor.Weight = (int)setElement.Attribute("weight");

                if (setElement.Attribute("price") != null)
                    armor.Price = (int)setElement.Attribute("price");

                if (setElement.Attribute("sellable") != null)
                    armor.Sellable = (bool)setElement.Attribute("sellable");

                if (setElement.Attribute("tradeable") != null)
                    armor.Tradeable = (bool)setElement.Attribute("tradeable");

                if (setElement.Attribute("dropable") != null)
                    armor.Dropable = (bool)setElement.Attribute("dropable");

                if (setElement.Attribute("destroyable") != null)
                    armor.Destroyable = (bool)setElement.Attribute("destroyable");
            }

            foreach (var statElement in element.Element("stats")?.Elements() ?? new List<XElement>())
            {
                var stat = new Stat
                {
                    Name = (string)statElement.Attribute("stat"),
                    Order = (string)statElement.Attribute("order"),
                    Value = (int?)statElement.Attribute("val") ?? 0
                };
                armor.Stats.Add(stat);
            }

            return armor;
        }

        private Etc ParseEtc(XElement element)
        {
            var etc = new Etc
            {
                Id = (int)element.Attribute("id"),
                Name = (string)element.Attribute("name"),
                ItemType = (string)element.Attribute("itemType")
            };

            foreach (var setElement in element.Elements("set"))
            {
                if (setElement.Attribute("icon") != null)
                    etc.Icon = (string)setElement.Attribute("icon");

                if (setElement.Attribute("weight") != null)
                    etc.Weight = (int)setElement.Attribute("weight");

                if (setElement.Attribute("price") != null)
                    etc.Price = (int)setElement.Attribute("price");

                if (setElement.Attribute("stackable") != null)
                    etc.Stackable = (bool)setElement.Attribute("stackable");

                if (setElement.Attribute("sellable") != null)
                    etc.Sellable = (bool)setElement.Attribute("sellable");

                if (setElement.Attribute("tradeable") != null)
                    etc.Tradeable = (bool)setElement.Attribute("tradeable");

                if (setElement.Attribute("dropable") != null)
                    etc.Dropable = (bool)setElement.Attribute("dropable");

                if (setElement.Attribute("destroyable") != null)
                    etc.Destroyable = (bool)setElement.Attribute("destroyable");
            }

            return etc;
        }

        private Jewelry ParseJewelry(XElement element)
        {
            var jewelry = new Jewelry
            {
                Id = (int)element.Attribute("id"),
                Name = (string)element.Attribute("name")
            };

            foreach (var setElement in element.Elements("set"))
            {
                if (setElement.Attribute("icon") != null)
                    jewelry.Icon = (string)setElement.Attribute("icon");

                if (setElement.Attribute("bodyPart") != null)
                    jewelry.BodyPart = (string)setElement.Attribute("bodyPart");

                if (setElement.Attribute("grade") != null)
                    jewelry.Grade = (string)setElement.Attribute("grade");

                if (setElement.Attribute("crystallizable") != null)
                    jewelry.Crystallizable = (bool)setElement.Attribute("crystallizable");

                if (setElement.Attribute("weight") != null)
                    jewelry.Weight = (int)setElement.Attribute("weight");

                if (setElement.Attribute("price") != null)
                    jewelry.Price = (int)setElement.Attribute("price");

                if (setElement.Attribute("sellable") != null)
                    jewelry.Sellable = (bool)setElement.Attribute("sellable");

                if (setElement.Attribute("tradeable") != null)
                    jewelry.Tradeable = (bool)setElement.Attribute("tradeable");

                if (setElement.Attribute("dropable") != null)
                    jewelry.Dropable = (bool)setElement.Attribute("dropable");

                if (setElement.Attribute("destroyable") != null)
                    jewelry.Destroyable = (bool)setElement.Attribute("destroyable");
            }

            foreach (var statElement in element.Element("stats")?.Elements() ?? new List<XElement>())
            {
                var stat = new Stat
                {
                    Name = (string)statElement.Attribute("stat"),
                    Order = (string)statElement.Attribute("order"),
                    Value = (int?)statElement.Attribute("val") ?? 0
                };
                jewelry.Stats.Add(stat);
            }

            return jewelry;
        }

        private Weapon ParseWeapon(XElement element)
        {
            var weapon = new Weapon
            {
                Id = (int)element.Attribute("id"),
                Name = (string)element.Attribute("name"),
                WeaponType = (string)element.Attribute("weaponType")
            };

            foreach (var setElement in element.Elements("set"))
            {
                if (setElement.Attribute("icon") != null)
                    weapon.Icon = (string)setElement.Attribute("icon");

                if (setElement.Attribute("bodyPart") != null)
                    weapon.BodyPart = (string)setElement.Attribute("bodyPart");

                if (setElement.Attribute("grade") != null)
                    weapon.Grade = (string)setElement.Attribute("grade");

                if (setElement.Attribute("crystallizable") != null)
                    weapon.Crystallizable = (bool)setElement.Attribute("crystallizable");

                if (setElement.Attribute("weight") != null)
                    weapon.Weight = (int)setElement.Attribute("weight");

                if (setElement.Attribute("price") != null)
                    weapon.Price = (int)setElement.Attribute("price");

                if (setElement.Attribute("sellable") != null)
                    weapon.Sellable = (bool)setElement.Attribute("sellable");

                if (setElement.Attribute("tradeable") != null)
                    weapon.Tradeable = (bool)setElement.Attribute("tradeable");

                if (setElement.Attribute("dropable") != null)
                    weapon.Dropable = (bool)setElement.Attribute("dropable");

                if (setElement.Attribute("destroyable") != null)
                    weapon.Destroyable = (bool)setElement.Attribute("destroyable");
            }

            foreach (var statElement in element.Element("stats")?.Elements() ?? new List<XElement>())
            {
                var stat = new Stat
                {
                    Name = (string)statElement.Attribute("stat"),
                    Order = (string)statElement.Attribute("order"),
                    Value = (int?)statElement.Attribute("val") ?? 0
                };
                weapon.Stats.Add(stat);
            }

            return weapon;
        }

        public List<Item> GetAllItems()
        {
            return _context.Items.ToList();
        }

        public Item GetItemById(int id)
        {
            return _context.Items.Find(id);
        }

        public void CreateItem(Item item)
        {
            _context.Items.Add(item);
            _context.SaveChanges();
        }

        public void UpdateItem(Item item)
        {
            _context.Items.Update(item);
            _context.SaveChanges();
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