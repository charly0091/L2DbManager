using GameDbManager.API.Models.Items;
using GameDbManager.API.Services.Items;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GameDbManager.API.Controllers.Items
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _itemService;

        public ItemController(ItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public ActionResult<List<Item>> GetAllItems()
        {
            var items = _itemService.GetAllItems();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public ActionResult<Item> GetItemById(int id)
        {
            var item = _itemService.GetItemById(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public ActionResult CreateItem(Item item)
        {
            _itemService.CreateItem(item);
            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateItem(int id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _itemService.UpdateItem(item);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            var item = _itemService.GetItemById(id);
            if (item == null)
            {
                return NotFound();
            }

            _itemService.DeleteItem(id);
            return NoContent();
        }

        [HttpPost("import")]
        public IActionResult ImportItems([FromBody] string xmlContent)
        {
            var message = _itemService.ImportItemsFromXml(xmlContent);
            return Ok(message);
        }
    }
}