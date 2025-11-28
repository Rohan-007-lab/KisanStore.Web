using System.ComponentModel.DataAnnotations;

namespace KisanStore.Web.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
