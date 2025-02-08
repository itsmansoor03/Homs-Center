using System.ComponentModel.DataAnnotations.Schema;

namespace myapp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Column(TypeName = "decimal(6,  2)")] 
        public decimal Price { get; set; }

        //navigation
		[ForeignKey(nameof(Category))]

		public int CategoryId { get; set; }

        public Category? Category { get; set; } 

        public ICollection<OrderItem>? OrderItems { get; set; } 

    }
}
