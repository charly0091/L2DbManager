using GameDbManager.API.Services.Items;
using Microsoft.AspNetCore.Mvc;

namespace GameDbManager.API.Controllers.Items
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly ItemService _itemService;

        public ImportController(ItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpPost("import")]
        public IActionResult ImportItems([FromBody] string xmlFilePath)
        {
            _itemService.ImportItemsFromXml(xmlFilePath);
            return Ok("Items imported successfully.");
        }
    }
}
