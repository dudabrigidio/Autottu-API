using AutoTTU.Models;

namespace AutoTTU.Service
{
    /// <summary>
    /// Interface que define os contratos de lógica de negócio para a entidade Moto
    /// </summary>
    public interface IMotosService
    {
        /// <summary>
        /// Retorna todas as motos cadastradas
        /// </summary>
        Task<IEnumerable<Motos>> GetAllAsync();

        /// <summary>
        /// Busca uma moto pelo ID
        /// </summary>
        /// <param name="id">ID da moto</param>
        Task<Motos?> GetByIdAsync(int id);

        /// <summary>
        /// Cria uma nova moto após validar os dados
        /// </summary>
        /// <param name="moto">Objeto Moto a ser criado</param>
        Task<Motos> CreateAsync(Motos moto);

        /// <summary>
        /// Atualiza uma moto existente
        /// </summary>
        /// <param name="id">ID da moto a ser atualizado</param>
        /// <param name="moto">Objeto moto com dados atualizados</param>
        Task UpdateAsync(int id, Motos moto);

        /// <summary>
        /// Remove uma moto do sistema
        /// </summary>
        /// <param name="id">ID da moto a ser removido</param>
        Task DeleteAsync(int id);

      
    }
}