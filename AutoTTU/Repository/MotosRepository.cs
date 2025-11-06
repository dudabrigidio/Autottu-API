using AutoTTU.Connection;
using AutoTTU.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoTTU.Repository
{
    /// <summary>
    /// Implementação do repositório para a entidade Moto
    /// Responsável apenas pelo acesso direto aos dados no banco
    /// </summary>
    public class MotosRepository : IMotosRepository
    {
        private readonly AppDbContext _context;

        public MotosRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca todos as motos cadastradas
        /// </summary>
        public async Task<IEnumerable<Motos>> GetAllAsync()
        {
            return await _context.Motos.ToListAsync();
        }

        /// <summary>
        /// Busca uma moto pelo ID
        /// </summary>
        public async Task<Motos?> GetByIdAsync(int id)
        {
            return await _context.Motos.FindAsync(id);
        }


        /// <summary>
        /// Adiciona uma nova moto ao banco de dados
        /// </summary>
        public async Task<Motos> AddAsync(Motos moto)
        {
            _context.Motos.Add(moto);
            await _context.SaveChangesAsync();
            return moto;
        }


        /// <summary>
        /// Atualiza uma moto existente
        /// </summary>
        public async Task UpdateAsync(Motos moto)
        {
            // Marca a moto como modificado no contexto
            _context.Entry(moto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Remove uma moto do banco de dados pelo ID
        /// </summary>
        /// <returns>True se a moto foi removido, False se não foi encontrada</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            // Busca a moto pelo ID
            var moto = await GetByIdAsync(id);

            // Se não encontrou, retorna false
            if (moto == null)
                return false;

            // Remove a moto encontrado
            _context.Motos.Remove(moto);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Verifica se uma moto existe pelo ID
        /// </summary>
        public async Task<bool> MotoExisteAsync(int id)
        {
            return await _context.Motos.AnyAsync(e => e.IdMoto == id);
        }

        /// <summary>
        /// Verifica se já existe uma moto com a placa informado
        /// </summary>
        public async Task<bool> PlacaExisteAsync(string placa)
        {
            var count = await _context.Motos
                .Where(u => u.Placa == placa)
                .CountAsync();
            return count > 0;
        }
    }
}