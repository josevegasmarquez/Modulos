using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace Modulos.Client.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public InventoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Products()
        {
            var client = GetHttpClient();
            var response = await client.GetAsync("api/Inventory/products");
            if (response.IsSuccessStatusCode)
            {
                var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
                return View(products);
            }
            return View(new List<ProductDto>());
        }

        public async Task<IActionResult> Warehouses()
        {
            var client = GetHttpClient();
            var response = await client.GetAsync("api/Inventory/warehouses");
            if (response.IsSuccessStatusCode)
            {
                var warehouses = await response.Content.ReadFromJsonAsync<IEnumerable<WarehouseDto>>();
                return View(warehouses);
            }
            return View(new List<WarehouseDto>());
        }

        public async Task<IActionResult> Movements()
        {
            var client = GetHttpClient();
            var response = await client.GetAsync("api/Inventory/movements");
            if (response.IsSuccessStatusCode)
            {
                var movements = await response.Content.ReadFromJsonAsync<IEnumerable<MovementDto>>();
                return View(movements);
            }
            return View(new List<MovementDto>());
        }

        private HttpClient GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient("ModulosAPI");
            var token = HttpContext.Session.GetString("JWTToken");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        // --- Client DTOs ---

        public class ProductDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Brand { get; set; } = string.Empty;
            public string? Features { get; set; }
            public decimal Price { get; set; }
            public bool IsActive { get; set; }
            public int TotalStock { get; set; }
        }

        public class WarehouseDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public bool IsActive { get; set; }
        }

        public class MovementDto
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public MovementType Type { get; set; }
            public string? FromWarehouseName { get; set; } // Simplified for view
            public string? ToWarehouseName { get; set; }
            public string? Observations { get; set; }
            public string? UserName { get; set; }
            public int DetailsCount { get; set; }
            
            // To handle API response directly if needed
            public WarehouseDto? FromWarehouse { get; set; }
            public WarehouseDto? ToWarehouse { get; set; }
            public UserDto? User { get; set; }
            public List<object>? Details { get; set; }
        }

        public enum MovementType
        {
            Purchase = 0,
            Sale = 1,
            Transfer = 2,
            Adjustment = 3
        }
    }
}
