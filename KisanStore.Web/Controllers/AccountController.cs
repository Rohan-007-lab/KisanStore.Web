using KisanStore.Web.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KisanStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7090/api";

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/Auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                HttpContext.Session.SetString("UserId", user["userId"].ToString());
                HttpContext.Session.SetString("UserName", user["fullName"].ToString());
                HttpContext.Session.SetString("UserRole", user["role"].ToString());

                if (user["role"].ToString() == "Admin")
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var content = new StringContent(
                JsonSerializer.Serialize(model),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/Auth/register", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
