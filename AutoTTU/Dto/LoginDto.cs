using System.ComponentModel.DataAnnotations;

namespace AutoTTU.Dto
{
    public class LoginDto
    {
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Senha { get; set; }
    }
}
