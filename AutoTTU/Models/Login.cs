using System.ComponentModel.DataAnnotations;

namespace AutoTTU.Models
{
    public class Login
    {
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Senha { get; set; }
    }
}
