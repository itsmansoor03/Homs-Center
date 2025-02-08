using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace myapp.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(6,  2)")]
        public decimal? UnitPrice { get; set; }

		[ForeignKey(nameof(Product))]
		public int ProductId { get; set; }

        [ForeignKey(nameof(Order))] 
        public int OrderId { get; set; }

        //Navigation 
        public Order? Order { get; set; }
        public Product? Product { get; set; }

    }
}