using Microsoft.AspNetCore.Mvc;
using AutoTTU.Models;
using AutoTTU.Service;
using AutoTTU.Dto;

namespace AutoTTU.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar os check-ins de motos
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CheckinsController : ControllerBase
    {
        private readonly ICheckinService _service;


        public CheckinsController(ICheckinService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista de todos os CheckIn's realizados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Checkin>>> GetCheckin()
        {
            var checkins = await _service.GetAllAsync();
            return Ok(checkins);
        }

        /// <summary>
        /// Busca de CheckIn pelo ID
        /// </summary>
        /// <param name="id">ID do CheckIn</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Checkin>> GetCheckin(int id)
        {
            var checkin = await _service.GetByIdAsync(id);

            if (checkin == null)
            {
                return NotFound();
            }

            return Ok(checkin);
        }

        

        /// <summary>
        /// Realização de CheckIn
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST/api/CheckIn
        ///        {
        ///             "idMoto": 1,
        ///             "idUsuario": 1,
        ///             "ativoChar" (se foi violada = s, se não foi violada = n): "s",
        ///             "observacao": "Moto danificada",
        ///             "timeStamp": "2025-09-18",
        ///             "imagensUrl": "string"
        ///         }
        ///       
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult<Checkin>> PostCheckin(CheckinInputDto checkinDto)
        {
            try
            {
                // Converte DTO para modelo
                var checkin = new Checkin
                {
                    IdMoto = checkinDto.IdMoto,
                    IdUsuario = checkinDto.IdUsuario,
                    AtivoChar = checkinDto.AtivoChar,
                    Observacao = checkinDto.Observacao,
                    TimeStamp = checkinDto.TimeStamp ?? DateTime.Now,
                    ImagensUrl = checkinDto.ImagensUrl
                };

                var novoCheckin = await _service.CreateAsync(checkin);
                return CreatedAtAction("GetCheckin", new { id = novoCheckin.IdCheckin }, novoCheckin);
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
        /// Alteração de CheckIn já realizado
        /// </summary>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     PUT/api/CheckIn
        ///        {
        ///             "idMoto": 1,
        ///             "idUsuario": 1,
        ///             "ativoChar" (se foi violada = s, se não foi violada = n): "s",
        ///             "observacao": "Moto danificada",
        ///             "timeStamp": "2025-09-18",
        ///             "imagensUrl": "string"
        ///         }
        ///       
        /// </remarks>
        /// <param name="id">ID do CheckIn</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCheckin(int id, CheckinInputDto checkinDto)
        {
            var checkin = new Checkin
            {
                IdMoto = checkinDto.IdMoto,
                IdUsuario = checkinDto.IdUsuario,
                AtivoChar = checkinDto.AtivoChar,
                Observacao = checkinDto.Observacao,
                TimeStamp = checkinDto.TimeStamp ?? DateTime.Now,
                ImagensUrl = checkinDto.ImagensUrl
            };

            try
            {
                await _service.UpdateAsync(id, checkin);
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
        /// Apagar CheckIn pelo ID
        /// </summary>
        /// <param name="id">ID do CheckIn</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCheckin(int id)
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
