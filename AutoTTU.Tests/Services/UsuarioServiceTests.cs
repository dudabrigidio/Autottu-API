using Xunit;
using Moq;
using FluentAssertions;
using AutoTTU.Service;
using AutoTTU.Models;
using AutoTTU.Repository;

namespace AutoTTU.Tests.Services;

/// <summary>
/// Testes UNITÁRIOS para UsuarioService
/// </summary>
public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _mockRepository;
    private readonly UsuarioService _service;


    public UsuarioServiceTests()
    {
        _mockRepository = new Mock<IUsuarioRepository>();
        _service = new UsuarioService(_mockRepository.Object);

    }

    #region CreateAsync - Testes de Criação

    /// <summary>
    /// TESTE: Cadastrar usuario com dados válidos
    /// </summary>
    [Fact]
    public async Task CreateAsync_DeveSalvarUsuario_QuandoValido()
    {
        var usuario = new Usuario
        {
            IdUsuario = 1,
            Nome = "João Silva",
            Email = "joao@test.com",
            Senha = "123456",
            Telefone = "11999999999",
        }
        ;

    
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Usuario>()))
            .ReturnsAsync(usuario);

        var result = await _service.CreateAsync(usuario);

        result.Should().NotBeNull();

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
    }



    /// <summary>
    /// TESTE: Tentar cadastrar usuario com email inválido
    /// </summary>
    [Fact]
    public async Task CreateAsync_DeveLancarExcecao_QuandoAtivoCharInvalido()
    {
        var usuario = new Usuario
        {
            IdUsuario = 1,
            Nome = "João Silva",
            Email = "joaotest.com",
            Senha = "123456",
            Telefone = "11999999999",
        }
        ;

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(usuario));

        exception.ParamName.Should().Be("Email");

    }


    /// <summary>
    /// TESTE: Listar todos os usuarios quando existem
    /// </summary>
    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosUsuarios_QuandoExistirem()
    {

        var usuario = new List<Usuario>
        {
            new Usuario {IdUsuario = 1, Nome = "João Silva",Email = "joao@test.com",Senha = "123456",Telefone = "11999999999" },
            new Usuario {IdUsuario = 2, Nome = "Maria Silva",Email = "maria@test.com",Senha = "123456",Telefone = "11999999999" }
        }
        ;

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(usuario);

        var result = await _service.GetAllAsync();

        // ASSERT
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Verifica se tem 2 itens
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once); // Verifica se foi chamado
    }


    /// <summary>
    /// TESTE: Buscar usuario por ID válido
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_DeveRetornarUsuario_QuandoIdValido()
    {

        var usuario = new Usuario
        {
            IdUsuario = 1,
            Nome = "João Silva",
            Email = "joao@test.com",
            Senha = "123456",
            Telefone = "11999999999",
        }
        ;

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(usuario);

        // ACT
        var result = await _service.GetByIdAsync(1);

        // ASSERT
        result.Should().NotBeNull();
        result!.IdUsuario.Should().Be(1);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// TESTE: Buscar usuario com ID inválido (deve retornar null)
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoIdInvalido()
    {
        var result = await _service.GetByIdAsync(0);

        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }



    /// <summary>
    /// TESTE: Atualizar usuario com dados válidos
    /// </summary>
    [Fact]
    public async Task UpdateAsync_DeveAtualizaUsuario_QuandoValido()
    {
        // ARRANGE
        var existingUsuario = new Usuario
        {
            IdUsuario = 1,
            Nome = "João Silva",
            Email = "joao@test.com",
            Senha = "123456",
            Telefone = "11999999999",
        };

        var updatedUsuario = new Usuario
        {
            IdUsuario = 1,
            Nome = "João Morais",
            Email = "joao@test.com",
            Senha = "123456",
            Telefone = "11999999999",
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingUsuario);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

        // ACT
        await _service.UpdateAsync(1, updatedUsuario);

        // ASSERT: Verifica se os métodos foram chamados
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Once);
    }

    /// <summary>
    /// TESTE: Tentar atualizar usuario que não existe (deve lançar exceção)
    /// </summary>
    [Fact]
    public async Task UpdateAsync_DeveLancarExcecao_QuandoUsuarioNaoExiste()
    {
        // ARRANGE
        var usuario = new Usuario
        {
            IdUsuario = 1,
            Nome = "João Morais",
            Email = "joao@test.com",
            Senha = "123456",
            Telefone = "11999999999",
        };

        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Usuario?)null);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateAsync(999, usuario));

        exception.Message.Should().Contain("999");
    }

    #endregion

    #region DeleteAsync - Testes de Exclusão

    /// <summary>
    /// TESTE: Deletar usuario com ID válido
    /// </summary>
    [Fact]
    public async Task DeleteAsync_DeveRemoverUsuario_QuandoIdValido()
    {
        // ARRANGE
        var usuario = new Usuario
        {
            IdUsuario = 1,
            Nome = "João Morais",
            Email = "joao@test.com",
            Senha = "123456",
            Telefone = "11999999999",
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(usuario);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // ACT
        await _service.DeleteAsync(1);

        // ASSERT
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    /// <summary>
    /// TESTE: Tentar deletar usuario com ID inválido (deve lançar exceção)
    /// </summary>
    [Fact]
    public async Task DeleteAsync_DeveLancarExcecao_QuandoIdInvalido()
    {
        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.DeleteAsync(0));

        exception.ParamName.Should().Be("id");
    }

    #endregion

}
