using KisanStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KisanStore.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7090/api";

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var response = await _httpClient.GetAsync($"{_apiUrl}/Cart/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var cartItems = JsonSerializer.Deserialize<List<Cart>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(cartItems);
            }
            return View(new List<Cart>());
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "Please login first" });

            var cart = new Cart
            {
                UserId = int.Parse(userId),
                ProductId = productId,
                Quantity = quantity
            };

            var content = new StringContent(
                JsonSerializer.Serialize(cart),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/Cart", content);
            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Added to cart" });

            return Json(new { success = false, message = "Failed to add" });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/Cart/{id}");
            return RedirectToAction("Index");
        }
    }
}

