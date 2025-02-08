using System.ComponentModel.DataAnnotations.Schema;

namespace myapp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderPlaced { get; set; }
        public DateTime? OrderFulfilled { get; set; }
        [Column(TypeName = "decimal(6,  2)")]
        public decimal? TotalAmount { get; set;  }
        public string? OrderStatus { get; set; }
        
		//Navigation
		[ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; } 
        public ICollection<OrderItem>? OrderItems { get; set; } 


    }
}
