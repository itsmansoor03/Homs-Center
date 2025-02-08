using System.ComponentModel.DataAnnotations.Schema;

namespace donate.Models
{
    public class Donation
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Image { get; set; }
        //navigation
        public string? Status { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Request>? Requests { get; set; }

    }
}
