using Microsoft.AspNetCore.Mvc;
using ServerSideApplication.Service;
using ModelClasses;

namespace ServerSideApplication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroceryController : ControllerBase
    {
        private readonly IGroceryService _groceryService;

        public GroceryController(IGroceryService groceryService)
        {
            _groceryService = groceryService;
        }

        [HttpPost]
        public async Task<IActionResult> PostGroceryDataAsync([FromBody] GroceryList groceryList)
        {
            var error = await _groceryService.PostGroceryListData(groceryList);
            return Ok(error);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroceryDataAsync()
        {
            var groceryList = await _groceryService.GetGroceryData();
            return Ok(groceryList);
        }
    }
}
