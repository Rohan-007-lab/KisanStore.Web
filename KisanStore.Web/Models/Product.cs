using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace KisanStore.Web.Models
{
    public partial class Product
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public int? CategoryId { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public int StockQuantity { get; set; }

        public string? Unit { get; set; }

        public string? ImageUrl { get; set; }

        public string? Manufacturer { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsFeatured { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

        public virtual Category? Category { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
