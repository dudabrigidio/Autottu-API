using AutoTTU.Models;

namespace AutoTTU.Repository
{
    /// <summary>
    /// Interface que define os contratos de acesso a dados para a entidade Moto
    /// </summary>
    public interface IMotosRepository
    {
        /// <summary>
        /// Busca todos os motos cadastrados
        /// </summary>
        Task<IEnumerable<Motos>> GetAllAsync();

        /// <summary>
        /// Busca uma moto pelo ID
        /// </summary>
        /// <param name="id">ID da moto</param>
        Task<Motos?> GetByIdAsync(int id);

        /// <summary>
        /// Adiciona uma nova moto ao banco de dados
        /// </summary>
        /// <param name="moto">Objeto Motos a ser adicionado</param>
        Task<Motos> AddAsync(Motos moto);

        /// <summary>
        /// Atualiza uma moto existente
        /// </summary>
        /// <param name="moto">Objeto Usuario com dados atualizados</param>
        Task UpdateAsync(Motos moto);



        /// <summary>
        /// Remove uma moto do banco de dados pelo ID
        /// </summary>
        /// <param name="id">ID da moto a ser removido</param>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica se uma moto existe pelo ID
        /// </summary>
        /// <param name="id">ID da moto</param>
        Task<bool> MotoExisteAsync(int id);

        /// <summary>
        /// Verifica se já existe uma moto com o a placa informada
        /// </summary>
        /// <param name="placa">Placa a ser verificada</param>
        Task<bool> PlacaExisteAsync(string placa);
    }
}