using Xunit;
using Moq;
using FluentAssertions;
using AutoTTU.Service;
using AutoTTU.Models;
using AutoTTU.Repository;

namespace AutoTTU.Tests.Services;

/// <summary>
/// Testes UNITÁRIOS para SlotService
/// </summary>
public class SlotServiceTests
{
    private readonly Mock<ISlotRepository> _mockRepository;
    private readonly SlotService _service;


    public SlotServiceTests()
    {
        _mockRepository = new Mock<ISlotRepository>();
        _service = new SlotService(_mockRepository.Object);

    }

    #region CreateAsync - Testes de Criação

    /// <summary>
    /// TESTE: Cadastrar slot com dados válidos
    /// </summary>
    [Fact]
    public async Task CreateAsync_DeveSalvarSlot_QuandoValido()
    {
        var slot = new Slot
        {
            IdSlot = 1,
            IdMoto = 1,
            AtivoChar = "s",
        };

        _mockRepository
            .Setup(r => r.ExisteMotoAsync(1))
            .ReturnsAsync(false);
        
        _mockMotosRepository
            .Setup(r => r.MotoExisteAsync(1))
            .ReturnsAsync(true);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Slot>()))
            .ReturnsAsync(slot);

        var result = await _service.CreateAsync(slot);

        result.Should().NotBeNull();

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Slot>()), Times.Once);
    }



    /// <summary>
    /// TESTE: Tentar cadastrar slot com AtivoChar inválido
    /// </summary>
    [Fact]
    public async Task CreateAsync_DeveLancarExcecao_QuandoAtivoCharInvalido()
    {
        var slot = new Slot
        {
            IdSlot = 1,
            IdMoto = 1,
            AtivoChar = "x",
        }
         ;

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(slot));

        exception.ParamName.Should().Be("AtivoChar");
    }


    /// <summary>
    /// TESTE: Listar todos os slots quando existem
    /// </summary>
    [Fact]
    public async Task GetAllAsync_DeveRetornarTodosSlots_QuandoExistirem()
    {
        var slot = new List<Slot>
        {
            new Slot {IdSlot = 1, IdMoto = 1,AtivoChar = "x" },
            new Slot {IdSlot = 2, IdMoto = 2,AtivoChar = "x" }
        }
        ;
       
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(slot);

        var result = await _service.GetAllAsync();

        // ASSERT
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Verifica se tem 2 itens
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once); // Verifica se foi chamado
    }


    /// <summary>
    /// TESTE: Buscar slot por ID válido
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_DeveRetornarSlot_QuandoIdValido()
    {

        var slot = new Slot
        {
            IdSlot = 1,
            IdMoto = 1,
            AtivoChar = "x",
        }
        ;

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(slot);

        // ACT
        var result = await _service.GetByIdAsync(1);

        // ASSERT
        result.Should().NotBeNull();
        result!.IdMoto.Should().Be(1);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// TESTE: Buscar slot com ID inválido (deve retornar null)
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoIdInvalido()
    {
        var result = await _service.GetByIdAsync(0);

        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }



    /// <summary>
    /// TESTE: Atualizar slot com dados válidos
    /// </summary>
    [Fact]
    public async Task UpdateAsync_DeveAtualizarSlot_QuandoValido()
    {
        // ARRANGE
        var existingSlot = new Slot
        {
            IdSlot = 1,
            IdMoto = 1,
            AtivoChar = "x",
        };

        var updatedSlot = new Slot
        {
            IdSlot = 1,
            IdMoto = 1,
            AtivoChar = "x",
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingSlot);
        _mockMotosRepository.Setup(r => r.MotoExisteAsync(1)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Slot>())).Returns(Task.CompletedTask);

        // ACT
        await _service.UpdateAsync(1, updatedSlot);

        // ASSERT: Verifica se os métodos foram chamados
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Slot>()), Times.Once);
    }

    /// <summary>
    /// TESTE: Tentar atualizar slot que não existe (deve lançar exceção)
    /// </summary>
    [Fact]
    public async Task UpdateAsync_DeveLancarExcecao_QuandoSlotNaoExiste()
    {
        // ARRANGE
        var slot = new Slot
        {
            IdSlot = 1,
            IdMoto = 1,
            AtivoChar = "s",
        }
        ;

        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Slot?)null);

        // ACT & ASSERT
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateAsync(999, slot));

        exception.Message.Should().Contain("999"); 
    }

    #endregion

    #region DeleteAsync - Testes de Exclusão

    /// <summary>
    /// TESTE: Deletar slot com ID válido
    /// </summary>
    [Fact]
    public async Task DeleteAsync_DeveRemoverSlot_QuandoIdValido()
    {
        // ARRANGE
        var slot = new Slot
        {
            IdSlot = 1,
            IdMoto = 1,
            AtivoChar = "x",
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(slot);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // ACT
        await _service.DeleteAsync(1);

        // ASSERT
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    /// <summary>
    /// TESTE: Tentar deletar slot com ID inválido (deve lançar exceção)
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
