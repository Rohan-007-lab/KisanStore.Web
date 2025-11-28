using KisanStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KisanStore.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7090/api";

        public ProductsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index(int? categoryId, string? search)
        {
            var url = $"{_apiUrl}/Products?";
            if (categoryId.HasValue)
                url += $"categoryId={categoryId}&";
            if (!string.IsNullOrEmpty(search))
                url += $"search={search}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<Product>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Get categories for filter
                var catResponse = await _httpClient.GetAsync($"{_apiUrl}/Categories");
                if (catResponse.IsSuccessStatusCode)
                {
                    var catJson = await catResponse.Content.ReadAsStringAsync();
                    ViewBag.Categories = JsonSerializer.Deserialize<List<Category>>(catJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                return View(products);
            }
            return View(new List<Product>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/Products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(product);
            }
            return NotFound();
        }
    }

}
