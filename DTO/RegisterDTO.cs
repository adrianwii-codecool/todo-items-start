using System.ComponentModel.DataAnnotations;

namespace TodoItems.DTO
{
    public class RegisterDTO
    {
        [Required]
        [MaxLength(255)]
        public string UsernName { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
         [MinLength(1)]
          [MaxLength(16)]
       public ICollection<string> Roles { get; set; } = new List<string>();
    }
}
