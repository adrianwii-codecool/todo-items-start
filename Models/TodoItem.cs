using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TodoItems.Models
{
    public class TodoItem
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
        public string? Secret { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
