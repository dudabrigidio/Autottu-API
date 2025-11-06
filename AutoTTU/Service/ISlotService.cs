using AutoTTU.Models;

namespace AutoTTU.Service
{
    /// <summary>
    /// Interface que define os contratos de lógica de negócio para a entidade Slot
    /// </summary>
    public interface ISlotService
    {
        /// <summary>
        /// Retorna todas as motos cadastradas
        /// </summary>
        Task<IEnumerable<Slot>> GetAllAsync();

        /// <summary>
        /// Busca um slot pelo ID
        /// </summary>
        /// <param name="id">ID do slot</param>
        Task<Slot?> GetByIdAsync(int id);

        /// <summary>
        /// Cria um slot após validar os dados
        /// </summary>
        /// <param name="slot">Objeto Slot a ser criado</param>
        Task<Slot> CreateAsync(Slot slot);

        /// <summary>
        /// Atualiza slot existente
        /// </summary>
        /// <param name="id">ID do slot a ser atualizado</param>
        /// <param name="slot">Objeto Slot com dados atualizados</param>
        Task UpdateAsync(int id, Slot slot);

        /// <summary>
        /// Remove um slot do sistema
        /// </summary>
        /// <param name="id">ID do Slot a ser removido</param>
        Task DeleteAsync(int id);


    }
}