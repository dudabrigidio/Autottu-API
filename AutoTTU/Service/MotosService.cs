using AutoTTU.Dto;
using AutoTTU.Models;
using AutoTTU.Repository;

namespace AutoTTU.Service
{
    /// <summary>
    /// Implementação do serviço para a entidade Usuario
    /// Contém a lógica de negócio e validações
    /// </summary>
    public class MotosService : IMotosService
    {
        private readonly IMotosRepository _repository;

        public MotosService(IMotosRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retorna todas motos cadastradas
        /// </summary>
        public async Task<IEnumerable<Motos>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// Busca uma moto pelo ID
        /// </summary>
        public async Task<Motos?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Cria uma nova moto após validar os dados
        /// </summary>
        public async Task<Motos> CreateAsync(Motos moto)
        {
            if (moto == null)
                throw new ArgumentNullException(nameof(moto));

            if (string.IsNullOrWhiteSpace(moto.Modelo))
                throw new ArgumentException("Modelo é obrigatório", nameof(moto.Modelo));

            if (string.IsNullOrWhiteSpace(moto.Marca))
                throw new ArgumentException("Marca é obrigatória", nameof(moto.Marca));

            if (string.IsNullOrWhiteSpace(moto.Placa))
                throw new ArgumentException("Placa é obrigatória", nameof(moto.Placa));

            if (moto.AtivoChar.ToLower() != "s" && moto.AtivoChar.ToLower() != "n")
                throw new ArgumentException("AtivoChar deve ser 'S' ou 'N'", nameof(moto.AtivoChar));

            if (moto.Ano < 1900 || moto.Ano > DateTime.Now.Year + 1)
                throw new ArgumentException("Ano da moto inválido", nameof(moto.Ano));

            if (string.IsNullOrWhiteSpace(moto.FotoUrl))
                throw new ArgumentException("Url das fotos é obrigatória", nameof(moto.FotoUrl));


            // Valida placa única
            var placaExiste = await _repository.PlacaExisteAsync(moto.Placa);
            if (placaExiste)
                throw new InvalidOperationException($"Já existe uma moto cadastrada com a placa: {moto.Placa}");

            // Se passou em todas as validações, adiciona no banco
            return await _repository.AddAsync(moto);

        }

        /// <summary>
        /// Atualiza uma moto existente
        /// </summary>
        public async Task UpdateAsync(int id, Motos moto)
        {
            // Validação: verifica se a moto existe
            var existingMoto = await _repository.GetByIdAsync(id);
            if (existingMoto == null)
                throw new KeyNotFoundException($"Moto com ID {id} não encontrado");

            // Validação: se o e-mail foi alterado, verifica se não está em uso por outro usuário
            if (existingMoto.Placa != moto.Placa)
            {
                var placaExiste = await _repository.PlacaExisteAsync(moto.Placa);
                if (placaExiste)
                    throw new InvalidOperationException($"Já existe uma moto cadastrada com a placa: {moto.Placa}");
            }

            // Validação: verifica campos obrigatórios
            if (string.IsNullOrWhiteSpace(moto.Modelo))
                throw new ArgumentException("Modelo é obrigatório", nameof(moto.Modelo));

            if (string.IsNullOrWhiteSpace(moto.Marca))
                throw new ArgumentException("Marca é obrigatória", nameof(moto.Marca));

            if (string.IsNullOrWhiteSpace(moto.Placa))
                throw new ArgumentException("Placa é obrigatória", nameof(moto.Placa));

            if (moto.AtivoChar.ToLower() != "s" && moto.AtivoChar.ToLower() != "n")
                throw new ArgumentException("AtivoChar deve ser 'S' ou 'N'", nameof(moto.AtivoChar));

            if (moto.Ano < 1900 || moto.Ano > DateTime.Now.Year + 1)
                throw new ArgumentException("Ano da moto inválido", nameof(moto.Ano));

            if (string.IsNullOrWhiteSpace(moto.FotoUrl))
                throw new ArgumentException("Url das fotos é obrigatória", nameof(moto.FotoUrl));

            // Atualiza o usuário existente com os novos dados
            existingMoto.Modelo = moto.Modelo;
            existingMoto.Marca = moto.Marca;
            existingMoto.Placa = moto.Placa;
            existingMoto.AtivoChar = moto.AtivoChar;
            existingMoto.Ano = moto.Ano;
            existingMoto.FotoUrl = moto.FotoUrl;


            // Salva as alterações
            await _repository.UpdateAsync(existingMoto);
        }

        /// <summary>
        /// Remove uma moto do sistema
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            // Validação: verifica se o ID é válido
            if (id <= 0)
                throw new ArgumentException("ID inválido", nameof(id));

            // Busca a moto
            var motos = await _repository.GetByIdAsync(id);
            if (motos == null)
                throw new KeyNotFoundException($"Moto com ID {id} não encontrado");

            // Remove a moto
            await _repository.DeleteAsync(id);
        }

        
    }
}
