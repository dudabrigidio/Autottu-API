using AutoTTU.Models;

namespace AutoTTU.Repository
{
    /// <summary>
    /// Interface que define os contratos de acesso a dados para a entidade Slots
    /// </summary>
    public interface ISlotRepository
    {
        /// <summary>
        /// Busca todos os slots cadastrados
        /// </summary>
        Task<IEnumerable<Slot>> GetAllAsync();

        /// <summary>
        /// Busca um slot pelo ID
        /// </summary>
        /// <param name="id">ID do slot</param>
        Task<Slot?> GetByIdAsync(int id);

        /// <summary>
        /// Adiciona um slot ao banco de dados
        /// </summary>
        /// <param name="slot">Objeto Slot a ser adicionado</param>
        Task<Slot> AddAsync(Slot slot);

        /// <summary>
        /// Atualiza um slot existente
        /// </summary>
        /// <param name="slot">Objeto Slot com dados atualizados</param>
        Task UpdateAsync(Slot slot);



        /// <summary>
        /// Remove um Slot do banco de dados pelo ID
        /// </summary>
        /// <param name="id">ID do Slot a ser removido</param>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Verifica se um slot existe pelo ID
        /// </summary>
        /// <param name="id">ID da moto</param>
        Task<bool> SlotExisteAsync(int id);

        /// <summary>
        /// Verifica se já existe uma moto no slot
        /// </summary>
        /// <param name="id">id da moto a ser verificada</param>
        Task<bool> ExisteMotoAsync(int id);

        /// <summary>
        /// Verifica se uma moto está em outro slot (excluindo um slot específico)
        /// </summary>
        /// <param name="idMoto">ID da moto a ser verificada</param>
        /// <param name="idSlotExcluir">ID do slot a ser excluído da verificação</param>
        Task<bool> ExisteMotoEmOutroSlotAsync(int idMoto, int idSlotExcluir);

    }
}