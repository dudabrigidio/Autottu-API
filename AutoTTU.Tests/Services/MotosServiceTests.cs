using Xunit;
using Moq;
using FluentAssertions;
using AutoTTU.Service;
using AutoTTU.Models;
using AutoTTU.Repository;

namespace AutoTTU.Tests.Services;

/// <summary>
/// Testes UNITÁRIOS para MotosService
/// </summary>
public class MotosServiceTests
{
    private readonly Mock<IMotosRepository> _mockRepository;
    private readonly MotosService _service;


    public MotosServiceTests()
    {
        _mockRepository = new Mock<IMotosRepository>();
        _service = new MotosService(_mockRepository.Object);

    }

    #region CreateAsync - Testes de Criação

    /// <summary>
    /// TESTE: Cadastrar moto com dados válidos
    /// </summary>
    [Fact]
    public async Task CreateAsync_DeveSalvarMoto_QuandoValido()
    {
        var moto = new Motos
        {
            IdMoto = 1,
            Modelo = "H2",
            Marca = "Honda",
            Ano = 2020,
            Placa = "FDP3467",
            AtivoChar = "s",
            FotoUrl = "www.google.com/123456plcg",
        }
        ;

        _mockRepository
            .Setup(r => r.PlacaExisteAsync("FDP3467"))
            .ReturnsAsync(false);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Motos>()))
            .ReturnsAsync(moto);

        var result = await _service.CreateAsync(moto);

        result.Should().NotBeNull();

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Motos>()), Times.Once);
    }



    /// <summary>
    /// TESTE: Tentar cadastrar moto com AtivoChar inválido
    /// </summary>
    [Fact]
    public async Task CreateAsync_DeveLancarExcecao_QuandoAtivoCharInvalido()
    {
        var moto = new Motos
        {
            IdMoto = 1,
            Modelo = "H2",
            Marca = "Honda",
            Ano = 2020,
            Placa = "FDP3467",
            AtivoChar = "x",
            FotoUrl = "www.google.com/123456plcg",
        }
        ;

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(moto));

        exception.ParamName.Should().Be("AtivoChar");
    }


    /// <summary>
    /// TESTE: Listar todas as motos quando existem
    /// </summary>
    [Fact]
    public async Task GetAllAsync_DeveRetornarTodasMotos_QuandoExistirem()
    {

        var motos = new List<Motos>
        {
            new Motos { IdMoto = 1,Modelo = "H2",Marca = "Honda", Ano = 2020,Placa = "FDP3467", AtivoChar = "s", FotoUrl = "www.google.com/123456plcg" },
            new Motos { IdMoto = 2, Modelo = "H4",Marca = "Wolkswagen", Ano = 2019,Placa = "PLK876", AtivoChar = "n", FotoUrl = "www.google.com/123456plcg" },

        }
        ;

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(motos);

        var result = await _service.GetAllAsync();

        // ASSERT
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Verifica se tem 2 itens
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once); // Verifica se foi chamado
    }


    /// <summary>
    /// TESTE: Buscar moto por ID válido
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_DeveRetornarMoto_QuandoIdValido()
    {

        var moto = new Motos
        {
            IdMoto = 1,
            Modelo = "H2",
            Marca = "Honda",
            Ano = 2020,
            Placa = "FDP3467",
            AtivoChar = "s",
            FotoUrl = "www.google.com/123456plcg",
        }
        ;

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(moto);

        // ACT
        var result = await _service.GetByIdAsync(1);

        // ASSERT
        result.Should().NotBeNull();
        result!.IdMoto.Should().Be(1);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// TESTE: Buscar moto com ID inválido (deve retornar null)
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoIdInvalido()
    {
        var result = await _service.GetByIdAsync(0);

        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }



    /// <summary>
    /// TESTE: Atualizar moto com dados válidos
    /// </summary>
    [Fact]
    public async Task UpdateAsync_DeveAtualizarMoto_QuandoValido()
    {
        // ARRANGE
        var existingMoto = new Motos
        {
            IdMoto = 1,
            Modelo = "H2",
            Marca = "Honda",
            Ano = 2020,
            Placa = "FDP3467",
            AtivoChar = "s",
            FotoUrl = "www.google.com/123456plcg",
        }
        ;

        var updatedMoto = new Motos
        {
            IdMoto = 1,
            Modelo = "H3",
            Marca = "Wolksvagem",
            Ano = 2020,
            Placa = "FDP3467",
            AtivoChar = "s",
            FotoUrl = "www.google.com/123456plcg",
        }
        ;

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingMoto);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Motos>())).Returns(Task.CompletedTask);

        // ACT
        await _service.UpdateAsync(1, updatedMoto);

        // ASSERT: Verifica se os métodos foram chamados
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Motos>()), Times.Once);
    }

    /// <summary>
    /// TESTE: Tentar atualizar moto que não existe (deve lançar exceção)
    /// </summary>
    [Fact]
    public async Task UpdateAsync_DeveLancarExcecao_QuandoMotoNaoExiste()
    {
        // ARRANGE
        var moto = new Motos
        {
            IdMoto = 1,
            Modelo = "H2",
            Marca = "Honda",
            Ano = 2020,
            Placa = "FDP3467",
            AtivoChar = "s",
            FotoUrl = "www.google.com/123456plcg",
        }
        ;

        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Motos?)null);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateAsync(999, moto));

        exception.Message.Should().Contain("999"); // Verifica se a mensagem contém o ID
    }

    #endregion

    #region DeleteAsync - Testes de Exclusão

    /// <summary>
    /// TESTE: Deletar moto com ID válido
    /// </summary>
    [Fact]
    public async Task DeleteAsync_DeveRemoverMoto_QuandoIdValido()
    {
        // ARRANGE
        var moto = new Motos
        {
            IdMoto = 1,
            Modelo = "H2",
            Marca = "Honda",
            Ano = 2020,
            Placa = "FDP3467",
            AtivoChar = "s",
            FotoUrl = "www.google.com/123456plcg",
        }
        ;

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(moto);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // ACT
        await _service.DeleteAsync(1);

        // ASSERT
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    /// <summary>
    /// TESTE: Tentar deletar moto com ID inválido (deve lançar exceção)
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
