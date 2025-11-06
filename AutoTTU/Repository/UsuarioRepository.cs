using AutoTTU.Connection;
using AutoTTU.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoTTU.Repository
{
    /// <summary>
    /// Implementação do repositório para a entidade Usuario
    /// Responsável apenas pelo acesso direto aos dados no banco
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca todos os usuários cadastrados
        /// </summary>
        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuario.ToListAsync();
        }

        /// <summary>
        /// Busca um usuário pelo ID
        /// </summary>
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuario.FindAsync(id);
        }

        /// <summary>
        /// Busca um usuário pelo e-mail
        /// </summary>
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Adiciona um novo usuário ao banco de dados
        /// </summary>
        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }


        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        public async Task UpdateAsync(Usuario usuario)
        {
            // Marca o usuário como modificado no contexto
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Remove um usuário do banco de dados pelo ID
        /// </summary>
        /// <returns>True se o usuário foi removido, False se não foi encontrado</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            // Busca o usuário pelo ID
            var usuario = await GetByIdAsync(id);
            
            // Se não encontrou, retorna false
            if (usuario == null)
                return false;

            // Remove o usuário encontrado
            _context.Usuario.Remove(usuario);
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// Verifica se um usuário existe pelo ID
        /// </summary>
        public async Task<bool> ExisteAsync(int id)
        {
            return await _context.Usuario.AnyAsync(e => e.IdUsuario == id);
        }

        /// <summary>
        /// Verifica se já existe um usuário com o e-mail informado
        /// </summary>
        public async Task<bool> EmailExisteAsync(string email)
        {
            var count = await _context.Usuario
                .Where(u => u.Email == email)
                .CountAsync();
            return count > 0;        
        }
    }
}