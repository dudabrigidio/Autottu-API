using Microsoft.AspNetCore.Mvc;
using AutoTTU.Models;
using AutoTTU.Service;
using AutoTTU.Dto;

namespace AutoTTU.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar os slots de estacionamento
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly ISlotService _service;


        public SlotController(ISlotService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista de todos os slots cadastrados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slot>>> GetSlot()
        {
            var slots = await _service.GetAllAsync();
            return Ok(slots);
        }

        /// <summary>
        /// Busca de um slot pelo ID
        /// </summary>
        /// <param name="id">ID do slot</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Slot>> GetSlot(int id)
        {
            var slot = await _service.GetByIdAsync(id);

            if (slot == null)
            {
                return NotFound();
            }

            return Ok(slot);
        }

        /// <summary>
        /// Cadastro de um slot
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST/api/Slots
        ///        {
        ///             "idMoto": 1,
        ///             "ativoChar": "s",
        ///        }     
        /// </remarks>

        [HttpPost]

        public async Task<ActionResult<Slot>> PostSlot(SlotsInputDto slotDto)
        {
            try
            {
                var slot = new Slot
                {
                    IdMoto = slotDto.IdMoto,
                    AtivoChar = slotDto.AtivoChar
                };

                var novoSlot = await _service.CreateAsync(slot);
                return CreatedAtAction("GetSlot", new { id = novoSlot.IdSlot }, novoSlot);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Alteração de um slot já cadastrado
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     PUT/api/Slots
        ///        {
        ///             "idMoto": 1,
        ///             "ativoChar": "s",
        ///        }
        ///       
        /// </remarks>
        /// 
        /// <param name="id">ID do usuário</param>

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSlot(int id, SlotsInputDto slotDto)
        {
            var slot = new Slot
            {
                IdMoto = slotDto.IdMoto,
                AtivoChar = slotDto.AtivoChar
            };

            try
            {
                await _service.UpdateAsync(id, slot);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Apagar slot pelo ID
        /// </summary>
        /// <param name="id">ID do Slot</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }


    }
}