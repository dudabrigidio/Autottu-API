using AutoTTU.Dto;
using AutoTTU.Models;
using AutoTTU.Repository;

namespace AutoTTU.Service
{
    /// <summary>
    /// Implementação do serviço para a entidade Slot
    /// Contém a lógica de negócio e validações
    /// </summary>
    public class SlotService : ISlotService
    {
        private readonly ISlotRepository _repository;
        private readonly IMotosRepository _motosRepository;

        public SlotService(ISlotRepository repository, IMotosRepository motosRepository)
        {
            _repository = repository;
            _motosRepository = motosRepository;
        }

        /// <summary>
        /// Retorna slots cadastrados
        /// </summary>
        public async Task<IEnumerable<Slot>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// Busca um slot pelo ID
        /// </summary>
        public async Task<Slot?> GetByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _repository.GetByIdAsync(id);
        }

        /// <summary>
        /// Cria uma nova moto após validar os dados
        /// </summary>
        public async Task<Slot> CreateAsync(Slot slot)
        {
            
            // Valida moto - verifica se a moto já está em outro slot
            var motoExisteSlot = await _repository.ExisteMotoAsync(slot.IdMoto);
            if (motoExisteSlot)
                throw new InvalidOperationException($"Moto já está em outro slot");
            
            var motoExiste = await _motosRepository.MotoExisteAsync(slot.IdMoto);
            if (!motoExiste)
                throw new ArgumentException($"Moto com ID {slot.IdMoto} não encontrada", nameof(slot.IdMoto));
            

            if (slot.AtivoChar.ToLower() != "s" && slot.AtivoChar.ToLower() != "n")
                throw new ArgumentException("AtivoChar deve ser 'S' ou 'N'", nameof(slot.AtivoChar));

            // Se passou em todas as validações, adiciona no banco
            return await _repository.AddAsync(slot);

        }

        /// <summary>
        /// Atualiza um slot existente
        /// </summary>
        public async Task UpdateAsync(int id, Slot slot)
        {
            // Validação: verifica se o slot existe
            var existingSlot = await _repository.GetByIdAsync(id);
            if (existingSlot == null)
                throw new KeyNotFoundException($"Slot com ID {id} não encontrado");

            // Valida moto - verifica se a nova moto já está em outro slot (excluindo o slot atual)
            // Se a moto não mudou, não precisa verificar
            if (existingSlot.IdMoto != slot.IdMoto)
            {
                var motoExisteEmOutroSlot = await _repository.ExisteMotoEmOutroSlotAsync(slot.IdMoto, id);
                if (motoExisteEmOutroSlot)
                    throw new InvalidOperationException($"Moto já está em outro slot");
            }

            var motoExiste = await _motosRepository.MotoExisteAsync(slot.IdMoto);
            if (!motoExiste)
                throw new ArgumentException($"Moto com ID {slot.IdMoto} não encontrada", nameof(slot.IdMoto));


            if (slot.AtivoChar.ToLower() != "s" && slot.AtivoChar.ToLower() != "n")
                throw new ArgumentException("AtivoChar deve ser 'S' ou 'N'", nameof(slot.AtivoChar));

            // Atualiza o slot existente com os novos dados
            existingSlot.IdMoto = slot.IdMoto;
            existingSlot.AtivoChar = slot.AtivoChar;
            

            // Salva as alterações
            await _repository.UpdateAsync(existingSlot);
        }

        /// <summary>
        /// Remove um slot do sistema
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            // Validação: verifica se o ID é válido
            if (id <= 0)
                throw new ArgumentException("ID inválido", nameof(id));

            // Busca slot
            var slot = await _repository.GetByIdAsync(id);
            if (slot == null)
                throw new KeyNotFoundException($"Slot com ID {id} não encontrado");

            // Remove slot
            await _repository.DeleteAsync(id);
        }


    }
}
