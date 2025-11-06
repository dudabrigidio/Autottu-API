using Microsoft.AspNetCore.Mvc;
using AutoTTU.Models;
using AutoTTU.Service;
using AutoTTU.Dto;

namespace AutoTTU.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar as motos cadastradas no sistema
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MotosController : ControllerBase
    {
        private readonly IMotosService _service;


        public MotosController(IMotosService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista de todas as motos cadastradas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Motos>>> GetMotos()
        {
            var motos = await _service.GetAllAsync();
            return Ok(motos);
        }

        /// <summary>
        /// Busca de uma moto pelo ID
        /// </summary>
        /// <param name="id">ID da moto</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Motos>> GetMoto(int id)
        {
            var moto = await _service.GetByIdAsync(id);

            if (moto == null)
            {
                return NotFound();
            }

            return Ok(moto);
        }

        /// <summary>
        /// Cadastro de uma moto
        /// </summary>
        /// /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     POST/api/Motos
        ///        {
        ///          "modelo": "H2",
        ///          "marca": "Honda",
        ///          "ano": 2020,
        ///          "placa": "FDP3467",
        ///          "ativoChar": "s",
        ///          "fotoUrl": "www.google.com/123456plcg"
        ///        }
        ///       
        /// </remarks>

        [HttpPost]

        public async Task<ActionResult<Motos>> PostMoto(MotoInputDto motoDto)
        {
            try
            {
                // Converte DTO para modelo, garantindo que o ID seja 0 (banco vai gerar automaticamente)
                var moto = new Motos
                {
                    Modelo = motoDto.Modelo,
                    Marca = motoDto.Marca,
                    Ano = motoDto.Ano,
                    Placa = motoDto.Placa,
                    AtivoChar = motoDto.AtivoChar,
                    FotoUrl = motoDto.FotoUrl,
                };

                var novaMoto = await _service.CreateAsync(moto);
                return CreatedAtAction("GetMoto", new { id = novaMoto.IdMoto }, novaMoto);
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
        /// Alteração de uma moto já cadastrada
        /// </summary>
        /// /// <remarks>
        /// Exemplo de requisição:
        /// 
        ///     PUT/api/Motos
        ///        {
        ///          "modelo": "H2",
        ///          "marca": "Honda",
        ///          "ano": 2020,
        ///          "placa": "FDP3467",
        ///          "ativoChar": "s",
        ///          "fotoUrl": "123456plcg"
        ///        }
        ///       
        /// </remarks>

        /// <param name="id">ID da moto</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMoto(int id, MotoInputDto motoDto)
        {
            var moto = new Motos
            {
                Modelo = motoDto.Modelo,
                Marca = motoDto.Marca,
                Ano = motoDto.Ano,
                Placa = motoDto.Placa,
                AtivoChar = motoDto.AtivoChar,
                FotoUrl = motoDto.FotoUrl,
            };

            try
            {
                await _service.UpdateAsync(id, moto);
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
        /// Apagar uma moto pelo ID
        /// </summary>
        /// <param name="id">ID da moto</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMoto(int id)
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