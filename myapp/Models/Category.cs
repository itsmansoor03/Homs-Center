using System.ComponentModel.DataAnnotations;

namespace myapp.Models
{
    public class Category
    {
        
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        //Navigation        
        public ICollection<Product>? Products { get; set; }
    }
}
