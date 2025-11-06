using AutoTTU.Dto;
using AutoTTU.Models;
using AutoTTU.Repository;

namespace AutoTTU.Service
{
    /// <summary>
    /// Implementação do serviço para a entidade Usuario
    /// Contém a lógica de negócio e validações
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retorna todos os usuários cadastrados
        /// </summary>
        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// Busca um usuário pelo ID
        /// </summary>
        public async Task<Usuario?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Cria um novo usuário após validar os dados
        /// </summary>
        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            // Validação: verifica se o usuário não é nulo
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario), "Usuário não pode ser nulo");

            // Validação: verifica se o e-mail já está cadastrado
            var emailExists = await _repository.EmailExisteAsync(usuario.Email);
            if (emailExists)
                throw new InvalidOperationException($"Já existe um usuário cadastrado com o e-mail: {usuario.Email}");

            // Validação: verifica se campos obrigatórios estão preenchidos
            if (string.IsNullOrWhiteSpace(usuario.Nome))
                throw new ArgumentException("Nome do usuário é obrigatório", nameof(usuario.Nome));

            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new ArgumentException("E-mail do usuário é obrigatório", nameof(usuario.Email));

            if (string.IsNullOrWhiteSpace(usuario.Senha))
                throw new ArgumentException("Senha do usuário é obrigatória", nameof(usuario.Senha));

            // Adiciona o usuário ao banco
            return await _repository.AddAsync(usuario);
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        public async Task UpdateAsync(int id, Usuario usuario)
        {
            // Validação: verifica se o usuário existe
            var existingUsuario = await _repository.GetByIdAsync(id);
            if (existingUsuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");

            // Validação: se o e-mail foi alterado, verifica se não está em uso por outro usuário
            if (existingUsuario.Email != usuario.Email)
            {
                var emailExists = await _repository.EmailExisteAsync(usuario.Email);
                if (emailExists)
                    throw new InvalidOperationException($"Já existe um usuário cadastrado com o e-mail: {usuario.Email}");
            }

            // Validação: verifica campos obrigatórios
            if (string.IsNullOrWhiteSpace(usuario.Nome))
                throw new ArgumentException("Nome do usuário é obrigatório", nameof(usuario.Nome));

            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new ArgumentException("E-mail do usuário é obrigatório", nameof(usuario.Email));

            // Atualiza o usuário existente com os novos dados
            existingUsuario.Nome = usuario.Nome;
            existingUsuario.Email = usuario.Email;
            existingUsuario.Senha = usuario.Senha;
            existingUsuario.Telefone = usuario.Telefone;

            // Salva as alterações
            await _repository.UpdateAsync(existingUsuario);
        }

        /// <summary>
        /// Remove um usuário do sistema
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            // Validação: verifica se o ID é válido
            if (id <= 0)
                throw new ArgumentException("ID inválido", nameof(id));

            // Busca o usuário
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null)
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");

            // Remove o usuário
            await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Realiza o login de um usuário
        /// </summary>
        public async Task<Usuario?> LoginAsync(LoginDto loginDto)
        {
            // Validação: verifica se os dados de login foram fornecidos
            if (loginDto == null)
                throw new ArgumentNullException(nameof(loginDto), "Dados de login não podem ser nulos");

            if (string.IsNullOrWhiteSpace(loginDto.Email))
                throw new ArgumentException("E-mail é obrigatório", nameof(loginDto.Email));

            if (string.IsNullOrWhiteSpace(loginDto.Senha))
                throw new ArgumentException("Senha é obrigatória", nameof(loginDto.Senha));

            // Busca o usuário pelo e-mail
            var usuario = await _repository.GetByEmailAsync(loginDto.Email);

            // Verifica se o usuário existe e se a senha confere
            if (usuario == null || usuario.Senha != loginDto.Senha)
                return null; // Credenciais inválidas

            return usuario; // Login bem-sucedido
        }
    }
}
