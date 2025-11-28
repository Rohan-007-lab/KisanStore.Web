using KisanStore.API.Models;
using KisanStore.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KisanStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly KisanStoreDbContext _context;

        public CartController(KisanStoreDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cartItems = await _context.Cart
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
            return Ok(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Cart cart)
        {
            var existing = await _context.Cart
                .FirstOrDefaultAsync(c => c.UserId == cart.UserId && c.ProductId == cart.ProductId);

            if (existing != null)
            {
                existing.Quantity += cart.Quantity;
            }
            else
            {
                _context.Cart.Add(cart);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
                return NotFound();

            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

