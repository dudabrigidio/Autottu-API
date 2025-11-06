using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using AutoTTU.Tests.Integration;
using AutoTTU.Tests.Helpers;
using AutoTTU.Dto;

namespace AutoTTU.Tests.Integration.Controllers;

/// <summary>
/// Testes de INTEGRAÇÃO para MotosController
/// </summary>
public class MotosControllerIntegrationTests : IntegrationTestBase
{
    public MotosControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    /// <summary>
    /// TESTE: GET /api/v1/Motos - Listar todas as motos
    /// </summary>
    [Fact]
    public async Task GetMotos_DeveRetornarListaVazia_QuandoNaoHaMotos()
    {
        CleanDatabase();

        var response = await Client.GetAsync("/api/v1/Motos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var motos = await response.Content.ReadFromJsonAsync<List<AutoTTU.Models.Motos>>();

        motos.Should().NotBeNull();
        motos.Should().BeEmpty();
    }

    /// <summary>
    /// TESTE: POST /api/v1/Motos - Cadastrar uma nova moto
    /// </summary>
    [Fact]
    public async Task PostMoto_DeveCriarMoto_QuandoDadosValidos()
    {
        CleanDatabase();

        var motoDto = new MotoInputDto
        {
            Modelo = "CG 160",
            Marca = "Honda",
            Ano = 2023,
            Placa = "ABC1234",
            AtivoChar = "S",
            FotoUrl = "https://example.com/foto.jpg"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Motos", motoDto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var moto = await response.Content.ReadFromJsonAsync<AutoTTU.Models.Motos>();

        moto.Should().NotBeNull();
        moto!.Modelo.Should().Be("CG 160");
        moto.Placa.Should().Be("ABC1234");
    }

    /// <summary>
    /// TESTE: GET /api/v1/Motos/{id} - Buscar moto por ID
    /// </summary>
    [Fact]
    public async Task GetMoto_DeveRetornarMoto_QuandoIdExiste()
    {
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto();
        DbContext.Motos.Add(moto);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"/api/v1/Motos/{moto.IdMoto}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AutoTTU.Models.Motos>();
        result.Should().NotBeNull();
        result!.IdMoto.Should().Be(moto.IdMoto);
        result.Modelo.Should().Be(moto.Modelo);
    }

    /// <summary>
    /// TESTE: GET /api/v1/Motos/{id} - Buscar moto que não existe
    /// </summary>
    [Fact]
    public async Task GetMoto_DeveRetornarNotFound_QuandoIdNaoExiste()
    {
        CleanDatabase();

        var response = await Client.GetAsync("/api/v1/Motos/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// TESTE: POST /api/v1/Motos - Criar moto com dados inválidos
    /// </summary>
    [Fact]
    public async Task PostMoto_DeveRetornarBadRequest_QuandoDadosInvalidos()
    {
        CleanDatabase();

        var motoDto = new MotoInputDto
        {
            Modelo = "",
            Marca = "",
            Ano = 0,
            Placa = "",
            AtivoChar = "X",
            FotoUrl = ""
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Motos", motoDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// TESTE: POST /api/v1/Motos - Criar moto com placa duplicada
    /// </summary>
    [Fact]
    public async Task PostMoto_DeveRetornarConflict_QuandoPlacaJaExiste()
    {
        CleanDatabase();

        var motoExistente = TestDataBuilder.CreateMoto(placa: "ABC1234");
        DbContext.Motos.Add(motoExistente);
        await DbContext.SaveChangesAsync();

        var motoDto = new MotoInputDto
        {
            Modelo = "CB 600",
            Marca = "Honda",
            Ano = 2024,
            Placa = "ABC1234",
            AtivoChar = "S",
            FotoUrl = "https://example.com/foto2.jpg"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Motos", motoDto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// TESTE: PUT /api/v1/Motos/{id} - Atualizar moto
    /// </summary>
    [Fact]
    public async Task PutMoto_DeveAtualizarMoto_QuandoDadosValidos()
    {
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto(
            modelo: "CG 160",
            placa: "ABC1234"
        );
        DbContext.Motos.Add(moto);
        await DbContext.SaveChangesAsync();

        var updateDto = new MotoInputDto
        {
            Modelo = "CB 600",
            Marca = "Honda",
            Ano = 2024,
            Placa = "ABC1234",
            AtivoChar = "N",
            FotoUrl = "https://example.com/new-foto.jpg"
        };

        var response = await Client.PutAsJsonAsync($"/api/v1/Motos/{moto.IdMoto}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Limpa o contexto para garantir que a consulta venha do banco
        DbContext.ChangeTracker.Clear();
        var updatedMoto = await DbContext.Motos.FindAsync(moto.IdMoto);
        updatedMoto.Should().NotBeNull();
        updatedMoto!.Modelo.Should().Be("CB 600");
    }

    /// <summary>
    /// TESTE: DELETE /api/v1/Motos/{id} - Deletar moto
    /// </summary>
    [Fact]
    public async Task DeleteMoto_DeveRemoverMoto_QuandoIdExiste()
    {
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto();
        DbContext.Motos.Add(moto);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"/api/v1/Motos/{moto.IdMoto}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Limpa o contexto para garantir que a consulta venha do banco
        DbContext.ChangeTracker.Clear();
        var deletedMoto = await DbContext.Motos.FindAsync(moto.IdMoto);
        deletedMoto.Should().BeNull();
    }

    /// <summary>
    /// TESTE: DELETE /api/v1/Motos/{id} - Deletar moto que não existe
    /// </summary>
    [Fact]
    public async Task DeleteMoto_DeveRetornarNotFound_QuandoIdNaoExiste()
    {
        CleanDatabase();

        var response = await Client.DeleteAsync("/api/v1/Motos/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
