using KisanStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KisanStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7090/api";

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/Products/featured");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<Product>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(products);
            }
            return View(new List<Product>());
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }

}
