using Microsoft.AspNetCore.Mvc;
using AutoTTU.Models;
using AutoTTU.Service;
using AutoTTU.Dto;

namespace AutoTTU.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar os usuários do sistema e autenticação
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _service;

        
        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista de todos os usuários cadastrados
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuario()
        {
            var usuarios = await _service.GetAllAsync();
            return Ok(usuarios);
        }

        /// <summary>
        /// Busca de um usuário pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _service.GetByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        /// <summary>
        /// Cadastro de um usuário
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(UsuarioInputDto usuarioDto)
        {
            try
            {
                // Converte DTO para modelo, garantindo que o ID seja 0 (banco vai gerar automaticamente)
                var usuario = new Usuario
                {
                    
                    Nome = usuarioDto.Nome,
                    Email = usuarioDto.Email,
                    Senha = usuarioDto.Senha,
                    Telefone = usuarioDto.Telefone
                };

                var novoUsuario = await _service.CreateAsync(usuario);
                return CreatedAtAction("GetUsuario", new { id = novoUsuario.IdUsuario }, novoUsuario);
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
        /// Alteração de um usuário já cadastrado
        /// </summary>
        /// <param name="id">ID do usuário</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioInputDto usuarioDto)
        {
            var usuario = new Usuario
            {
                IdUsuario = id,
                Nome = usuarioDto.Nome,
                Email = usuarioDto.Email,
                Senha = usuarioDto.Senha,
                Telefone = usuarioDto.Telefone
            };

            try
            {
                await _service.UpdateAsync(id, usuario);
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
        /// Apagar um usuário pelo ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
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

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var usuario = await _service.LoginAsync(loginDto);

                if (usuario == null)
                    return Unauthorized(new { message = "Email ou senha inválidos" });

                var idToken = Guid.NewGuid().ToString();
                return Ok(new { message = "Login bem-sucedido", idToken = idToken });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}