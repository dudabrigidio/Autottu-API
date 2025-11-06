using System.ComponentModel.DataAnnotations;

namespace AutoTTU.Dto
{
    /// <summary>
    /// DTO para entrada de dados de Checkin (POST/PUT) - sem ID
    /// </summary>
    public class CheckinInputDto
    {
        /// <summary>
        /// ID da moto associada ao check-in
        /// </summary>
        [Required]
        public int IdMoto { get; set; }

        /// <summary>
        /// ID do usuário que realizou o check-in
        /// </summary>
        [Required]
        public int IdUsuario { get; set; }

        /// <summary>
        /// Indica se a moto foi violada durante o check-in ("S" para sim, "N" para não)
        /// </summary>
        [Required]
        [MaxLength(1)]
        public string AtivoChar { get; set; }

        /// <summary>
        /// Observações adicionais sobre o estado da moto durante o check-in
        /// </summary>
        [Required]
        public string Observacao { get; set; }

        /// <summary>
        /// Data e hora em que o check-in foi realizado (opcional - se não informado, será usado o momento atual)
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// URLs das imagens capturadas durante o check-in
        /// </summary>
        [Required]
        [MaxLength(2048)]
        public string ImagensUrl { get; set; }
    }
}

