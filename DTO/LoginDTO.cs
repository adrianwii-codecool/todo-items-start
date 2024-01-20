using System.ComponentModel.DataAnnotations;

namespace TodoItems.DTO
{
    public class LoginDTO
    {
        [Required]
        [MaxLength(255)]

        public string UserName { get; set; } = String.Empty;
        [Required]
        [MaxLength(100)]

        public string Password { get; set; } = String.Empty;
    }
}
