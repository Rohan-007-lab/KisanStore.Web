namespace KisanStore.Web.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }

        public int? UserId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = null!;

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        public string? ShippingAddress { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual User? User { get; set; }
    }

}
