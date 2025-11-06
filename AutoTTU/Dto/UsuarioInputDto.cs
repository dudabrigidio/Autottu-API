namespace AutoTTU.Dto
{
    /// <summary>
    /// DTO para resposta de criação de usuário (POST) - sem ID e sem senha
    /// </summary>
    public class UsuarioInputDto
    {
        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Endereço de e-mail do usuário
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// senha do usuário
        /// </summary>
        public string Senha { get; set; }
        /// <summary>
        /// Número de telefone de contato do usuário
        /// </summary>
        public string Telefone { get; set; }
    }
}
