using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoTTU.Models
{
    public class Slot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSlot { get; set; }

        [ForeignKey("Motos")]
        public int IdMoto { get; set; }

        [Required]
        [MaxLength(1)]
        public string AtivoChar { get; set; }  // Armazena "S" ou "N" no banco

        [NotMapped]
        public bool Ocupado
        {
            get => AtivoChar.ToLower() == "s";
            set => AtivoChar = value ? "s" : "n";
        }


    }
}


