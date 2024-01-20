using System.ComponentModel.DataAnnotations;

namespace TodoItems.Configurations.Options
{
    public class CorsConfiguration
    {
        [Required]
        [MinLength(1)]
        public string[] Origins { get; set; } = Array.Empty<string>();
    }
}
