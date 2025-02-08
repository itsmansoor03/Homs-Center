using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace myapp.Models
{
	public class Customer
	{
		public int Id { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;

		[EmailAddress]
		public string? Email { get; set; }
		public string Address { get; set; } = null!;
		public string? Phone { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Password is required")]
		[StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
		public string Password { get; set; } = null!;

		// Navigation
		public ICollection<Order>? Orders { get; set; }
	}
}
