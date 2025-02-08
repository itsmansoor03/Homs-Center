using System.ComponentModel.DataAnnotations;

namespace donate.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? UserType { get; set; }
        //navigation
        public ICollection<Donation>? Donations { get; set; }
        public ICollection<Request>? Requests { get; set; }


    }
}
