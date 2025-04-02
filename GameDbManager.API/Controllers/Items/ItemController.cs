using GameDbManager.API.Models.Items;
using GameDbManager.API.Models.Items.Dtos;
using GameDbManager.API.Services.Items;
using GameDbManager.API.Services.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GameDbManager.API.Controllers.Items
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _itemService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(ItemService itemService, ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ItemDto>> GetAllItems()
        {
            try
            {
                var items = _itemService.GetAllItems();
                var itemDtos = items.Select(ItemMapper.ToDto).ToList();

                // Usar opciones específicas que NO preservan referencias para esta respuesta
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null,
                    // Nota: NO incluimos ReferenceHandler.Preserve aquí
                };

                // Serializar manualmente con opciones sin preservación de referencias
                var json = JsonSerializer.Serialize(itemDtos, options);
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving items");
                return StatusCode(500, "An error occurred while retrieving items");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<ItemDto> GetItemById(int id)
        {
            try
            {
                var item = _itemService.GetItemById(id);
                if (item == null)
                {
                    return NotFound();
                }

                var dto = ItemMapper.ToDto(item);

                // Usar las mismas opciones que en GetAllItems para mantener consistencia
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = null,
                    // Sin ReferenceHandler.Preserve
                };

                var json = JsonSerializer.Serialize(dto, options);
                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving item {id}");
                return StatusCode(500, $"An error occurred while retrieving item {id}");
            }
        }

        [HttpPost]
        public ActionResult<ItemDto> CreateItem(ItemDto itemDto)
        {
            try
            {
                var item = ItemMapper.FromDto(itemDto);
                _itemService.CreateItem(item);

                var createdDto = ItemMapper.ToDto(item);
                return CreatedAtAction(nameof(GetItemById), new { id = createdDto.Id }, createdDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating item");
                return StatusCode(500, "An error occurred while creating the item");
            }
        }

        [HttpPut("{id}")]
        public ActionResult<ItemDto> UpdateItem(int id, ItemDto itemDto)
        {
            try
            {
                if (id != itemDto.Id)
                {
                    return BadRequest("ID mismatch between URL and item data");
                }

                var existingItem = _itemService.GetItemById(id);
                if (existingItem == null)
                {
                    return NotFound();
                }

                var item = ItemMapper.FromDto(itemDto);
                _itemService.UpdateItem(item);

                var updatedDto = ItemMapper.ToDto(item);
                return Ok(updatedDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating item {id}");
                return StatusCode(500, $"An error occurred while updating item {id}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            try
            {
                var item = _itemService.GetItemById(id);
                if (item == null)
                {
                    return NotFound();
                }

                _itemService.DeleteItem(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting item {id}");
                return StatusCode(500, $"An error occurred while deleting item {id}");
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportItems([FromBody] ImportXmlRequest request)
        {
            if (string.IsNullOrEmpty(request.XmlContent))
            {
                return BadRequest("XML content is empty");
            }

            try
            {
                var result = await _itemService.ImportItemsFromXml(request.XmlContent, request.OverwriteExisting);

                // Comprobar si realmente hubo éxito basado en los datos importados
                bool actualSuccess = !result.Contains("Failed to import:") ||
                                     result.Contains("Successfully imported to database: 0 items") == false;

                if (!actualSuccess)
                {
                    return StatusCode(500, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing items");

                // Capturar y devolver también las excepciones internas para diagnóstico
                var detailedError = $"Error importing items: {ex.Message}";

                if (ex.InnerException != null)
                {
                    detailedError += $"\nInner exception: {ex.InnerException.Message}";
                }

                return StatusCode(500, detailedError);
            }
        }

        [HttpPost("validate")]
        public IActionResult ValidateXml([FromBody] ValidateXmlRequest request)
        {
            if (string.IsNullOrEmpty(request.XmlContent))
            {
                return BadRequest("XML content is empty");
            }

            try
            {
                var isValid = _itemService.ValidateXml(request.XmlContent);
                return Ok(isValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating XML");
                return BadRequest($"XML validation error: {ex.Message}");
            }
        }

        public class ImportXmlRequest
        {
            public string XmlContent { get; set; }
            public bool OverwriteExisting { get; set; }
        }

        public class ValidateXmlRequest
        {
            public string XmlContent { get; set; }
        }
    }
}