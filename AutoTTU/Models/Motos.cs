using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoTTU.Models
{
    public class Motos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int IdMoto { get; set; }

        [Required]
        [MaxLength(100)]
        public string Modelo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Marca { get; set; }

        [Required]
        public int Ano { get; set; }

        [MaxLength(10)]
        public string Placa { get; set; }

        [Required]
        [MaxLength(1)]
        public string AtivoChar { get; set; }  // Armazena "S" ou "N" no banco

        [NotMapped]
        public bool Status
        {
            get => AtivoChar.ToLower() == "s";
            set => AtivoChar = value ? "s" : "n";
        }

        [Required]
        [MaxLength(2048)]
        public string FotoUrl { get; set; }

    }
}
