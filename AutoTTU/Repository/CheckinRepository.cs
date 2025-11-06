using AutoTTU.Connection;
using AutoTTU.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoTTU.Repository
{
    /// <summary>
    /// Implementação do repositório para a entidade Checkin
    /// Responsável apenas pelo acesso direto aos dados no banco
    /// </summary>
    public class CheckinRepository : ICheckinRepository
    {
        private readonly AppDbContext _context;

        public CheckinRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca todos os checkins realizados
        /// </summary>
        public async Task<IEnumerable<Checkin>> GetAllAsync()
        {
            return await _context.Checkin.ToListAsync();
        }

        /// <summary>
        /// Busca um checkin pelo ID
        /// </summary>
        public async Task<Checkin?> GetByIdAsync(int id)
        {
            return await _context.Checkin.FindAsync(id);
        }

        /// <summary>
        /// Adiciona um novo checkin ao banco de dados
        /// </summary>
        public async Task<Checkin> AddAsync(Checkin checkin)
        {
            _context.Checkin.Add(checkin);
            await _context.SaveChangesAsync();
            return checkin;
        }

        /// <summary>
        /// Atualiza um checkin existente
        /// </summary>
        public async Task UpdateAsync(Checkin checkin)
        {
            // Marca o checkin como modificado no contexto
            _context.Entry(checkin).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove um checkin do banco de dados pelo ID
        /// </summary>
        /// <returns>True se o checkin foi removido, False se não foi encontrado</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            // Busca o checkin pelo ID
            var checkin = await GetByIdAsync(id);

            // Se não encontrou, retorna false
            if (checkin == null)
                return false;

            // Remove o checkin encontrado
            _context.Checkin.Remove(checkin);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Verifica se um checkin existe pelo ID
        /// </summary>
        public async Task<bool> CheckinExisteAsync(int id)
        {
            return await _context.Checkin.AnyAsync(e => e.IdCheckin == id);
        }

        
    }
}