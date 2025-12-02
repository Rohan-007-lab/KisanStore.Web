using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KisanStore.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7090/api";

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult Reports()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");
            return View();
        }


       

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");
            return View();
        }

        // Products Management
        public async Task<IActionResult> Products()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var response = await _httpClient.GetAsync($"{_apiUrl}/Products");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(products);
            }
            return View(new List<ProductViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            // Get categories
            var response = await _httpClient.GetAsync($"{_apiUrl}/Categories");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                ViewBag.Categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductViewModel product)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(product);

            var content = new StringContent(
                JsonSerializer.Serialize(product),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/Products", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Product created successfully";
                return RedirectToAction("Products");
            }

            ModelState.AddModelError("", "Failed to create product");
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var response = await _httpClient.GetAsync($"{_apiUrl}/Products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductViewModel>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Get categories
                var catResponse = await _httpClient.GetAsync($"{_apiUrl}/Categories");
                if (catResponse.IsSuccessStatusCode)
                {
                    var catJson = await catResponse.Content.ReadAsStringAsync();
                    ViewBag.Categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(catJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                return View(product);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, ProductViewModel product)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (id != product.ProductId)
                return BadRequest();

            var content = new StringContent(
                JsonSerializer.Serialize(product),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"{_apiUrl}/Products/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Product updated successfully";
                return RedirectToAction("Products");
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var response = await _httpClient.DeleteAsync($"{_apiUrl}/Products/{id}");
            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Product deleted successfully";

            return RedirectToAction("Products");
        }

        // Categories Management
        public async Task<IActionResult> Categories()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var response = await _httpClient.GetAsync($"{_apiUrl}/Categories");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(categories);
            }
            return View(new List<CategoryViewModel>());
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryViewModel category)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var content = new StringContent(
                JsonSerializer.Serialize(category),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/Categories", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category created successfully";
                return RedirectToAction("Categories");
            }

            return View(category);
        }

        // Orders Management
        public async Task<IActionResult> Orders()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var response = await _httpClient.GetAsync($"{_apiUrl}/Orders");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var orders = JsonSerializer.Deserialize<List<OrderViewModel>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(orders);
            }
            return View(new List<OrderViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var content = new StringContent(
                JsonSerializer.Serialize(status),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"{_apiUrl}/Orders/{orderId}/status", content);
            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Order status updated";

            return RedirectToAction("Orders");
        }
    }

    // ViewModel Classes for MVC
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public CategoryViewModel? Category { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public string? Unit { get; set; }
        public string? ImageUrl { get; set; }
        public string? Manufacturer { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }

    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public UserViewModel? User { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = "Pending";
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? ShippingAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public ProductViewModel? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class UserViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Customer";
    }

    public class CartViewModel
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public ProductViewModel? Product { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; }
    }
}