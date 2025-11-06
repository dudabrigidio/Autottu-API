using AutoTTU.Models;
using AutoTTU.Repository;

namespace AutoTTU.Service
{
    /// <summary>
    /// Implementação do serviço para a entidade Checkin
    /// Contém a lógica de negócio e validações
    /// </summary>
    public class CheckinService : ICheckinService
    {
        private readonly ICheckinRepository _repository;
        private readonly IMotosRepository _motosRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private ICheckinRepository @object;

        public CheckinService(ICheckinRepository repository, IMotosRepository motosRepository, IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _motosRepository = motosRepository;
            _usuarioRepository = usuarioRepository;
        }

        public CheckinService(ICheckinRepository @object)
        {
            this.@object = @object;
        }

        /// <summary>
        /// Retorna todos os checkins realizados
        /// </summary>
        public async Task<IEnumerable<Checkin>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// Busca um checkin pelo ID
        /// </summary>
        public async Task<Checkin?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Cria um novo checkin após validar os dados
        /// </summary>
        public async Task<Checkin> CreateAsync(Checkin checkin)
        {
            // Validação: verifica se o checkin não é nulo
            if (checkin == null)
                throw new ArgumentNullException(nameof(checkin), "Checkin não pode ser nulo");

            // Validação: verifica campos obrigatórios
            if (checkin.IdMoto <= 0)
                throw new ArgumentException("ID da moto é obrigatório e deve ser maior que zero", nameof(checkin.IdMoto));

            if (checkin.IdUsuario <= 0)
                throw new ArgumentException("ID do usuário é obrigatório e deve ser maior que zero", nameof(checkin.IdUsuario));

            if (string.IsNullOrWhiteSpace(checkin.AtivoChar))
                throw new ArgumentException("AtivoChar é obrigatório", nameof(checkin.AtivoChar));

            if (checkin.AtivoChar.ToLower() != "s" && checkin.AtivoChar.ToLower() != "n")
                throw new ArgumentException("AtivoChar deve ser 'S' ou 'N'", nameof(checkin.AtivoChar));

            if (string.IsNullOrWhiteSpace(checkin.Observacao))
                throw new ArgumentException("Observação é obrigatória", nameof(checkin.Observacao));

            if (string.IsNullOrWhiteSpace(checkin.ImagensUrl))
                throw new ArgumentException("URL das imagens é obrigatória", nameof(checkin.ImagensUrl));


            var motoExiste = await _motosRepository.MotoExisteAsync(checkin.IdMoto);
            if (!motoExiste)
                throw new ArgumentException($"Moto com ID {checkin.IdMoto} não encontrada", nameof(checkin.IdMoto));

            // Validação: verifica se o usuário existe
            var usuarioExiste = await _usuarioRepository.ExisteAsync(checkin.IdUsuario);
            if (!usuarioExiste)
                throw new ArgumentException($"Usuário com ID {checkin.IdUsuario} não encontrado", nameof(checkin.IdUsuario));

            // Se o TimeStamp não foi definido, define como agora
            if (checkin.TimeStamp == default(DateTime))
                checkin.TimeStamp = DateTime.Now;

            // Adiciona o checkin ao banco
            return await _repository.AddAsync(checkin);
        }

        /// <summary>
        /// Atualiza um checkin existente
        /// </summary>
        public async Task UpdateAsync(int id, Checkin checkin)
        {
            // Validação: verifica se o checkin existe
            var existingCheckin = await _repository.GetByIdAsync(id);
            if (existingCheckin == null)
                throw new KeyNotFoundException($"Checkin com ID {id} não encontrado");

            // Validação: verifica campos obrigatórios
            if (checkin.IdMoto <= 0)
                throw new ArgumentException("ID da moto é obrigatório e deve ser maior que zero", nameof(checkin.IdMoto));

            if (checkin.IdUsuario <= 0)
                throw new ArgumentException("ID do usuário é obrigatório e deve ser maior que zero", nameof(checkin.IdUsuario));

            if (string.IsNullOrWhiteSpace(checkin.AtivoChar))
                throw new ArgumentException("AtivoChar é obrigatório", nameof(checkin.AtivoChar));

            if (checkin.AtivoChar.ToLower() != "s" && checkin.AtivoChar.ToLower() != "n")
                throw new ArgumentException("AtivoChar deve ser 'S' ou 'N'", nameof(checkin.AtivoChar));

            if (string.IsNullOrWhiteSpace(checkin.Observacao))
                throw new ArgumentException("Observação é obrigatória", nameof(checkin.Observacao));

            if (string.IsNullOrWhiteSpace(checkin.ImagensUrl))
                throw new ArgumentException("URL das imagens é obrigatória", nameof(checkin.ImagensUrl));

            // Validação: verifica se a moto existe
            var motoExiste = await _motosRepository.MotoExisteAsync(checkin.IdMoto);
            if (!motoExiste)
                throw new ArgumentException($"Moto com ID {checkin.IdMoto} não encontrada", nameof(checkin.IdMoto));

            // Validação: verifica se o usuário existe
            var usuarioExiste = await _usuarioRepository.ExisteAsync(checkin.IdUsuario);
            if (!usuarioExiste)
                throw new ArgumentException($"Usuário com ID {checkin.IdUsuario} não encontrado", nameof(checkin.IdUsuario));

            // Atualiza o checkin existente com os novos dados
            existingCheckin.IdMoto = checkin.IdMoto;
            existingCheckin.IdUsuario = checkin.IdUsuario;
            existingCheckin.AtivoChar = checkin.AtivoChar;
            existingCheckin.Observacao = checkin.Observacao;
            existingCheckin.TimeStamp = checkin.TimeStamp;
            existingCheckin.ImagensUrl = checkin.ImagensUrl;

            // Salva as alterações
            await _repository.UpdateAsync(existingCheckin);
        }

        /// <summary>
        /// Remove um checkin do sistema
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            // Validação: verifica se o ID é válido
            if (id <= 0)
                throw new ArgumentException("ID inválido", nameof(id));

            // Busca o checkin
            var checkin = await _repository.GetByIdAsync(id);
            if (checkin == null)
                throw new KeyNotFoundException($"Checkin com ID {id} não encontrado");

            // Remove o checkin
            await _repository.DeleteAsync(id);
        }
    }
}
