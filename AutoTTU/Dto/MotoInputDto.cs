using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoTTU.Dto
{
    /// <summary>
    /// DTO para resposta de criação de usuário (POST) - sem ID e sem senha
    /// </summary>
    public class MotoInputDto
    {
        /// <summary>
        /// Modelo da moto
        /// </summary>
        public string Modelo { get; set; }


        /// <summary>
        /// Marca da moto
        /// </summary>
        public string Marca { get; set; }


        /// <summary>
        /// Ano da moto
        /// </summary>
        public int Ano { get; set; }


        /// <summary>
        /// Placa da moto
        /// </summary>
        public string Placa { get; set; }


        /// <summary>
        /// Se a moto esta ativa
        /// </summary>
        public string AtivoChar { get; set; }  // Armazena "S" ou "N" no banco


        /// <summary>
        /// Url da foto
        /// </summary>
        public string FotoUrl { get; set; }
    }
}
