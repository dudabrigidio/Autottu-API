using Xunit;
using Moq;
using FluentAssertions;
using AutoTTU.Service;
using AutoTTU.Models;
using AutoTTU.Repository;

namespace AutoTTU.Tests.Services;

/// <summary>
/// Testes UNITÁRIOS para CheckinService
/// </summary>
public class CheckinServiceTests
{
    private readonly Mock<ICheckinRepository> _mockRepository;
    private readonly CheckinService _service;


    public CheckinServiceTests()
    {
        _mockRepository = new Mock<ICheckinRepository>();
        _service = new CheckinService(_mockRepository.Object);

    }

    #region CreateAsync - Testes de Criação

    /// <summary>
    /// TESTE: Criar checkin com dados válidos
    /// </summary>
    [Fact]
    public async Task CreateAsync_DeveSalvarCheckin_QuandoValido()
    {
        var checkin = new Checkin
        {
            IdMoto = 1,
            IdUsuario = 2,
            AtivoChar = "S",
            Observacao = "Moto ok",
            TimeStamp = DateTime.Now,
            ImagensUrl = "img.jpg"
        };

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Checkin>()))
            .ReturnsAsync(checkin);

        var result = await _service.CreateAsync(checkin);

        result.Should().NotBeNull();

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Checkin>()), Times.Once);
    }

    /// <summary>
    /// TESTE: Criar checkin sem TimeStamp (deve definir automaticamente)
    /// </summary>
    [Fact]
    public async Task CreateAsync_DeveDefinirTimeStamp_QuandoNaoFornecido()
    {
        var checkin = new Checkin
        {
            IdMoto = 1,
            IdUsuario = 2,
            AtivoChar = "S",
            Observacao = "Moto ok",
            TimeStamp = default,
            ImagensUrl = "img.jpg"
        };

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Checkin>()))
            .ReturnsAsync((Checkin c) => c);

        var result = await _service.CreateAsync(checkin);

        result.TimeStamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }



    /// <summary>
    /// TESTE: Tentar criar checkin com AtivoChar inválido
    /// </summary>
    [Fact]
    public async Task CreateAsync_DeveLancarExcecao_QuandoAtivoCharInvalido()
    {
        // ARRANGE
        var checkin = new Checkin
        {
            IdMoto = 1,
            IdUsuario = 2,
            AtivoChar = "X", // INVÁLIDO: deve ser "S" ou "N"
            Observacao = "Moto ok",
            TimeStamp = DateTime.Now,
            ImagensUrl = "img.jpg"
        };

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(checkin));

        exception.ParamName.Should().Be("AtivoChar");
    }


    /// <summary>
    /// TESTE: Listar todos os checkins quando existem
    /// </summary>
    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosCheckins_QuandoExistirem()
    {
        var checkins = new List<Checkin>
        {
            new Checkin { IdCheckin = 1, IdMoto = 1, IdUsuario = 1, AtivoChar = "S", Observacao = "OK", TimeStamp = DateTime.Now, ImagensUrl = "url1" },
            new Checkin { IdCheckin = 2, IdMoto = 2, IdUsuario = 2, AtivoChar = "N", Observacao = "OK", TimeStamp = DateTime.Now, ImagensUrl = "url2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(checkins);

        // ACT
        var result = await _service.GetAllAsync();

        // ASSERT
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Verifica se tem 2 itens
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once); // Verifica se foi chamado
    }


    /// <summary>
    /// TESTE: Buscar checkin por ID válido
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_DeveRetornarCheckin_QuandoIdValido()
    {
        // ARRANGE
        var checkin = new Checkin
        {
            IdCheckin = 1,
            IdMoto = 1,
            IdUsuario = 1,
            AtivoChar = "S",
            Observacao = "OK",
            TimeStamp = DateTime.Now,
            ImagensUrl = "url1"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(checkin);

        // ACT
        var result = await _service.GetByIdAsync(1);

        // ASSERT
        result.Should().NotBeNull();
        result!.IdCheckin.Should().Be(1);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// TESTE: Buscar checkin com ID inválido (deve retornar null)
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoIdInvalido()
    {
        var result = await _service.GetByIdAsync(0);

        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }



    /// <summary>
    /// TESTE: Atualizar checkin com dados válidos
    /// </summary>
    [Fact]
    public async Task UpdateAsync_DeveAtualizarCheckin_QuandoValido()
    {
        // ARRANGE
        var existingCheckin = new Checkin
        {
            IdCheckin = 1,
            IdMoto = 1,
            IdUsuario = 1,
            AtivoChar = "S",
            Observacao = "Antiga",
            TimeStamp = DateTime.Now,
            ImagensUrl = "url1"
        };

        var updatedCheckin = new Checkin
        {
            IdMoto = 2,
            IdUsuario = 2,
            AtivoChar = "N",
            Observacao = "Nova",
            TimeStamp = DateTime.Now,
            ImagensUrl = "url2"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingCheckin);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Checkin>())).Returns(Task.CompletedTask);

        // ACT
        await _service.UpdateAsync(1, updatedCheckin);

        // ASSERT: Verifica se os métodos foram chamados
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Checkin>()), Times.Once);
    }

    /// <summary>
    /// TESTE: Tentar atualizar checkin que não existe (deve lançar exceção)
    /// </summary>
    [Fact]
    public async Task UpdateAsync_DeveLancarExcecao_QuandoCheckinNaoExiste()
    {
        // ARRANGE
        var checkin = new Checkin
        {
            IdMoto = 1,
            IdUsuario = 2,
            AtivoChar = "S",
            Observacao = "OK",
            TimeStamp = DateTime.Now,
            ImagensUrl = "url1"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Checkin?)null);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateAsync(999, checkin));

        exception.Message.Should().Contain("999"); // Verifica se a mensagem contém o ID
    }

    #endregion

    #region DeleteAsync - Testes de Exclusão

    /// <summary>
    /// TESTE: Deletar checkin com ID válido
    /// </summary>
    [Fact]
    public async Task DeleteAsync_DeveRemoverCheckin_QuandoIdValido()
    {
        // ARRANGE
        var checkin = new Checkin
        {
            IdCheckin = 1,
            IdMoto = 1,
            IdUsuario = 1,
            AtivoChar = "S",
            Observacao = "OK",
            TimeStamp = DateTime.Now,
            ImagensUrl = "url1"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(checkin);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // ACT
        await _service.DeleteAsync(1);

        // ASSERT
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    /// <summary>
    /// TESTE: Tentar deletar checkin com ID inválido (deve lançar exceção)
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
