using AutoTTU.Connection;
using AutoTTU.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoTTU.Repository
{
    /// <summary>
    /// Implementação do repositório para a entidade Moto
    /// Responsável apenas pelo acesso direto aos dados no banco
    /// </summary>
    public class SlotRepository : ISlotRepository
    {
        private readonly AppDbContext _context;

        public SlotRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca todos os slots cadastrados
        /// </summary>
        public async Task<IEnumerable<Slot>> GetAllAsync()
        {
            return await _context.Slot.ToListAsync();
        }

        /// <summary>
        /// Busca um slot pelo ID
        /// </summary>
        public async Task<Slot?> GetByIdAsync(int id)
        {
            // Usa FirstOrDefaultAsync ao invés de FindAsync para melhor compatibilidade com Oracle
            return await _context.Slot
                .FirstOrDefaultAsync(s => s.IdSlot == id);
        }


        /// <summary>
        /// Adiciona um slot moto ao banco de dados
        /// </summary>
        public async Task<Slot> AddAsync(Slot slot)
        {
            _context.Slot.Add(slot);
            await _context.SaveChangesAsync();
            return slot;
        }


        /// <summary>
        /// Atualiza um slot existente
        /// </summary>
        public async Task UpdateAsync(Slot slot)
        {
            // Marca o slot como modificado no contexto
            _context.Entry(slot).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Remove um slot do banco de dados pelo ID
        /// </summary>
        /// <returns>True se o slot foi removido, False se não foi encontrada</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            // Busca um slot pelo ID
            var slot = await GetByIdAsync(id);

            // Se não encontrou, retorna false
            if (slot == null)
                return false;

            // Remove a moto encontrado
            _context.Slot.Remove(slot);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Verifica se um slot existe pelo ID
        /// </summary>
        public async Task<bool> SlotExisteAsync(int id)
        {
            // Usa CountAsync ao invés de AnyAsync para evitar problemas com Oracle
            var count = await _context.Slot
                .Where(e => e.IdSlot == id)
                .CountAsync();
            return count > 0;
        }

        public async Task<bool> ExisteMotoAsync(int id)
        {
            var count = await _context.Slot
                .Where(u => u.IdMoto == id)
                .CountAsync();
            return count > 0;
        }

        /// <summary>
        /// Verifica se uma moto está em outro slot (excluindo um slot específico)
        /// </summary>
        public async Task<bool> ExisteMotoEmOutroSlotAsync(int idMoto, int idSlotExcluir)
        {
            var count = await _context.Slot
                .Where(s => s.IdMoto == idMoto && s.IdSlot != idSlotExcluir)
                .CountAsync();
            return count > 0;
        }
    }
}