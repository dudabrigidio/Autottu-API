using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using AutoTTU.Tests.Integration;
using AutoTTU.Tests.Helpers;
using AutoTTU.Dto;
using AutoTTU.Connection;
using Microsoft.EntityFrameworkCore;

namespace AutoTTU.Tests.Integration.Controllers;

/// <summary>
/// Testes de INTEGRAÇÃO para CheckinsController
/// </summary>
public class CheckinsControllerIntegrationTests : IntegrationTestBase
{
    public CheckinsControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    /// <summary>
    /// TESTE: GET /api/v1/Checkins - Listar todos os checkins
    /// </summary>
    [Fact]
    public async Task GetCheckin_DeveRetornarListaVazia_QuandoNaoHaCheckins()
    {
        CleanDatabase();

        var response = await Client.GetAsync("/api/v1/Checkins");

        response.StatusCode.Should().Be(HttpStatusCode.OK); // Status deve ser 200
        
        var checkins = await response.Content.ReadFromJsonAsync<List<AutoTTU.Models.Checkin>>();
        
        checkins.Should().NotBeNull();
        checkins.Should().BeEmpty(); 
    }

    /// <summary>
    /// TESTE: POST /api/v1/Checkins - Criar um novo checkin
    /// </summary>
    [Fact]
    public async Task PostCheckin_DeveCriarCheckin_QuandoDadosValidos()
    {
        CleanDatabase();


        var moto = TestDataBuilder.CreateMoto();
        var usuario = TestDataBuilder.CreateUsuario();
        
        DbContext.Motos.Add(moto);
        DbContext.Usuario.Add(usuario);
        await DbContext.SaveChangesAsync();

        var checkinDto = new CheckinInputDto
        {
            IdMoto = moto.IdMoto,
            IdUsuario = usuario.IdUsuario,
            AtivoChar = "S",
            Observacao = "Moto em bom estado",
            TimeStamp = DateTime.Now,
            ImagensUrl = "https://example.com/image.jpg"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Checkins", checkinDto);

        response.StatusCode.Should().Be(HttpStatusCode.Created); // Status deve ser 201
        
        var checkin = await response.Content.ReadFromJsonAsync<AutoTTU.Models.Checkin>();
        
        checkin.Should().NotBeNull();
        checkin!.Observacao.Should().Be("Moto em bom estado");
    }

    /// <summary>
    /// TESTE: GET /api/v1/Checkins/{id} - Buscar checkin por ID

    /// </summary>
    [Fact]
    public async Task GetCheckin_DeveRetornarCheckin_QuandoIdExiste()
    {
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto();
        var usuario = TestDataBuilder.CreateUsuario();
        
        DbContext.Motos.Add(moto);
        DbContext.Usuario.Add(usuario);
        await DbContext.SaveChangesAsync();

        var checkin = TestDataBuilder.CreateCheckin(
            idMoto: moto.IdMoto,
            idUsuario: usuario.IdUsuario
        );
        
        DbContext.Checkin.Add(checkin);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"/api/v1/Checkins/{checkin.IdCheckin}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<AutoTTU.Models.Checkin>();
        result.Should().NotBeNull();
        result!.IdCheckin.Should().Be(checkin.IdCheckin);
    }

    /// <summary>
    /// TESTE: GET /api/v1/Checkins/{id} - Buscar checkin que não existe

    /// </summary>
    [Fact]
    public async Task GetCheckin_DeveRetornarNotFound_QuandoIdNaoExiste()
    {
        CleanDatabase();

        var response = await Client.GetAsync("/api/v1/Checkins/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// TESTE: POST /api/v1/Checkins - Criar checkin com dados inválidos
    /// </summary>
    [Fact]
    public async Task PostCheckin_DeveRetornarBadRequest_QuandoDadosInvalidos()
    {
        CleanDatabase();

        var checkinDto = new CheckinInputDto
        {
            IdMoto = 0, 
            IdUsuario = 0, 
            AtivoChar = "x",
            Observacao = "", 
            TimeStamp = DateTime.Now,
            ImagensUrl = "" 
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Checkins", checkinDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// TESTE: PUT /api/v1/Checkins/{id} - Atualizar checkin
    /// </summary>
    [Fact]
    public async Task PutCheckin_DeveAtualizarCheckin_QuandoDadosValidos()
    {
        // ARRANGE
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto();
        var usuario = TestDataBuilder.CreateUsuario();
        
        DbContext.Motos.Add(moto);
        DbContext.Usuario.Add(usuario);
        await DbContext.SaveChangesAsync();

        var checkin = TestDataBuilder.CreateCheckin(
            idMoto: moto.IdMoto,
            idUsuario: usuario.IdUsuario,
            observacao: "Observação original"
        );
        
        DbContext.Checkin.Add(checkin);
        await DbContext.SaveChangesAsync();

        var updateDto = new CheckinInputDto
        {
            IdMoto = moto.IdMoto,
            IdUsuario = usuario.IdUsuario,
            AtivoChar = "N",
            Observacao = "Observação atualizada",
            TimeStamp = DateTime.Now,
            ImagensUrl = "https://example.com/new-image.jpg"
        };

        var response = await Client.PutAsJsonAsync($"/api/v1/Checkins/{checkin.IdCheckin}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent); // Status deve ser 204

        var updatedCheckin = await DbContext.Checkin.FindAsync(checkin.IdCheckin);
        updatedCheckin.Should().NotBeNull();
        updatedCheckin!.Observacao.Should().Be("Observação atualizada");
    }

    /// <summary>
    /// TESTE: DELETE /api/v1/Checkins/{id} - Deletar checkin

    /// </summary>
    [Fact]
    public async Task DeleteCheckin_DeveRemoverCheckin_QuandoIdExiste()
    {
        CleanDatabase();

        var moto = TestDataBuilder.CreateMoto();
        var usuario = TestDataBuilder.CreateUsuario();
        
        DbContext.Motos.Add(moto);
        DbContext.Usuario.Add(usuario);
        await DbContext.SaveChangesAsync();

        var checkin = TestDataBuilder.CreateCheckin(
            idMoto: moto.IdMoto,
            idUsuario: usuario.IdUsuario
        );
        
        DbContext.Checkin.Add(checkin);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"/api/v1/Checkins/{checkin.IdCheckin}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent); // Status deve ser 204

        var deletedCheckin = await DbContext.Checkin.FindAsync(checkin.IdCheckin);
        deletedCheckin.Should().BeNull(); 
        
    }

    /// <summary>
    /// TESTE: DELETE /api/v1/Checkins/{id} - Deletar checkin que não existe

    /// </summary>
    [Fact]
    public async Task DeleteCheckin_DeveRetornarNotFound_QuandoIdNaoExiste()
    {
        CleanDatabase();

        var response = await Client.DeleteAsync("/api/v1/Checkins/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
