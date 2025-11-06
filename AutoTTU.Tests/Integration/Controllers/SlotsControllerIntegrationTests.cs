using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using AutoTTU.Tests.Integration;
using AutoTTU.Tests.Helpers;
using AutoTTU.Dto;

namespace AutoTTU.Tests.Integration.Controllers;

/// <summary>
/// Testes de INTEGRAÇÃO para SlotController
/// </summary>
public class SlotsControllerIntegrationTests : IntegrationTestBase
{
    public SlotsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    /// <summary>
    /// TESTE: GET /api/v1/Slots - Listar todos os slots
    /// </summary>
    [Fact]
    public async Task GetSlots_DeveRetornarListaVazia_QuandoNaoHaSlots()
    {
        CleanDatabase();

        var response = await Client.GetAsync("/api/v1/Slots");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var slots = await response.Content.ReadFromJsonAsync<List<AutoTTU.Models.Slot>>();

        slots.Should().NotBeNull();
        slots.Should().BeEmpty();
    }

    /// <summary>
    /// TESTE: POST /api/v1/Slots - Cadastrar um novo slot
    /// </summary>
    [Fact]
    public async Task PostSlot_DeveCriarSlot_QuandoDadosValidos()
    {
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto();
        DbContext.Motos.Add(moto);
        await DbContext.SaveChangesAsync();

        var slotDto = new SlotsInputDto
        {
            IdMoto = moto.IdMoto,
            AtivoChar = "S"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Slots", slotDto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var slot = await response.Content.ReadFromJsonAsync<AutoTTU.Models.Slot>();

        slot.Should().NotBeNull();
        slot!.IdMoto.Should().Be(moto.IdMoto);
        slot.AtivoChar.Should().Be("S");
    }

    /// <summary>
    /// TESTE: GET /api/v1/Slots/{id} - Buscar slot por ID
    /// </summary>
    [Fact]
    public async Task GetSlot_DeveRetornarSlot_QuandoIdExiste()
    {
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto();
        DbContext.Motos.Add(moto);
        await DbContext.SaveChangesAsync();

        var slot = TestDataBuilder.CreateSlot(idMoto: moto.IdMoto);
        DbContext.Slot.Add(slot);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"/api/v1/Slots/{slot.IdSlot}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AutoTTU.Models.Slot>();
        result.Should().NotBeNull();
        result!.IdSlot.Should().Be(slot.IdSlot);
    }

    /// <summary>
    /// TESTE: GET /api/v1/Slots/{id} - Buscar slot que não existe
    /// </summary>
    [Fact]
    public async Task GetSlot_DeveRetornarNotFound_QuandoIdNaoExiste()
    {
        CleanDatabase();

        var response = await Client.GetAsync("/api/v1/Slots/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// TESTE: POST /api/v1/Slots - Criar slot com dados inválidos
    /// </summary>
    [Fact]
    public async Task PostSlot_DeveRetornarBadRequest_QuandoDadosInvalidos()
    {
        CleanDatabase();

        var slotDto = new SlotsInputDto
        {
            IdMoto = 0,
            AtivoChar = "X"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Slots", slotDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// TESTE: POST /api/v1/Slots - Criar slot com moto já em outro slot
    /// </summary>
    [Fact]
    public async Task PostSlot_DeveRetornarConflict_QuandoMotoJaEstaEmOutroSlot()
    {
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto();
        DbContext.Motos.Add(moto);
        await DbContext.SaveChangesAsync();

        var slotExistente = TestDataBuilder.CreateSlot(idMoto: moto.IdMoto);
        DbContext.Slot.Add(slotExistente);
        await DbContext.SaveChangesAsync();

        var slotDto = new SlotsInputDto
        {
            IdMoto = moto.IdMoto,
            AtivoChar = "S"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Slots", slotDto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// TESTE: PUT /api/v1/Slots/{id} - Atualizar slot
    /// </summary>
    [Fact]
    public async Task PutSlot_DeveAtualizarSlot_QuandoDadosValidos()
    {
        CleanDatabase();

        var moto1 = TestDataBuilder.CreateMoto(placa: "ABC1234");
        var moto2 = TestDataBuilder.CreateMoto(placa: "XYZ5678");
        DbContext.Motos.Add(moto1);
        DbContext.Motos.Add(moto2);
        await DbContext.SaveChangesAsync();

        var slot = TestDataBuilder.CreateSlot(idMoto: moto1.IdMoto, ativoChar: "S");
        DbContext.Slot.Add(slot);
        await DbContext.SaveChangesAsync();

        var updateDto = new SlotsInputDto
        {
            IdMoto = moto2.IdMoto,
            AtivoChar = "N"
        };

        var response = await Client.PutAsJsonAsync($"/api/v1/Slots/{slot.IdSlot}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updatedSlot = await DbContext.Slot.FindAsync(slot.IdSlot);
        updatedSlot.Should().NotBeNull();
        updatedSlot!.IdMoto.Should().Be(moto2.IdMoto);
        updatedSlot.AtivoChar.Should().Be("N");
    }

    /// <summary>
    /// TESTE: DELETE /api/v1/Slots/{id} - Deletar slot
    /// </summary>
    [Fact]
    public async Task DeleteSlot_DeveRemoverSlot_QuandoIdExiste()
    {
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto();
        DbContext.Motos.Add(moto);
        await DbContext.SaveChangesAsync();

        var slot = TestDataBuilder.CreateSlot(idMoto: moto.IdMoto);
        DbContext.Slot.Add(slot);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"/api/v1/Slots/{slot.IdSlot}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deletedSlot = await DbContext.Slot.FindAsync(slot.IdSlot);
        deletedSlot.Should().BeNull();
    }

    /// <summary>
    /// TESTE: DELETE /api/v1/Slots/{id} - Deletar slot que não existe
    /// </summary>
    [Fact]
    public async Task DeleteSlot_DeveRetornarNotFound_QuandoIdNaoExiste()
    {
        CleanDatabase();

        var response = await Client.DeleteAsync("/api/v1/Slots/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
