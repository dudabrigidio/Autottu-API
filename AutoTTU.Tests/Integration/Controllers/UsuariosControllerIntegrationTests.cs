using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using AutoTTU.Tests.Integration;
using AutoTTU.Tests.Helpers;
using AutoTTU.Dto;

namespace AutoTTU.Tests.Integration.Controllers;

/// <summary>
/// Testes de INTEGRAÇÃO para UsuariosController
/// </summary>
public class UsuariosControllerIntegrationTests : IntegrationTestBase
{
    public UsuariosControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        : base(factory)
    {
    }

    /// <summary>
    /// TESTE: GET /api/v1/Usuarios - Listar todos os usuários
    /// </summary>
    [Fact]
    public async Task GetUsuarios_DeveRetornarListaVazia_QuandoNaoHaUsuarios()
    {
        CleanDatabase();

        var response = await Client.GetAsync("/api/v1/Usuarios");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var usuarios = await response.Content.ReadFromJsonAsync<List<AutoTTU.Models.Usuario>>();

        usuarios.Should().NotBeNull();
        usuarios.Should().BeEmpty();
    }

    /// <summary>
    /// TESTE: POST /api/v1/Usuarios - Cadastrar um novo usuário
    /// </summary>
    [Fact]
    public async Task PostUsuario_DeveCriarUsuario_QuandoDadosValidos()
    {
        CleanDatabase();

        var usuarioDto = new UsuarioInputDto
        {
            Nome = "João Silva",
            Email = "joao@test.com",
            Senha = "123456",
            Telefone = "11999999999"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Usuarios", usuarioDto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var usuario = await response.Content.ReadFromJsonAsync<AutoTTU.Models.Usuario>();

        usuario.Should().NotBeNull();
        usuario!.Nome.Should().Be("João Silva");
        usuario.Email.Should().Be("joao@test.com");
    }

    /// <summary>
    /// TESTE: GET /api/v1/Usuarios/{id} - Buscar usuário por ID
    /// </summary>
    [Fact]
    public async Task GetUsuario_DeveRetornarUsuario_QuandoIdExiste()
    {
        CleanDatabase();

        var usuario = TestDataBuilder.CreateUsuario();
        DbContext.Usuario.Add(usuario);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"/api/v1/Usuarios/{usuario.IdUsuario}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AutoTTU.Models.Usuario>();
        result.Should().NotBeNull();
        result!.IdUsuario.Should().Be(usuario.IdUsuario);
        result.Nome.Should().Be(usuario.Nome);
    }

    /// <summary>
    /// TESTE: GET /api/v1/Usuarios/{id} - Buscar usuário que não existe
    /// </summary>
    [Fact]
    public async Task GetUsuario_DeveRetornarNotFound_QuandoIdNaoExiste()
    {
        CleanDatabase();

        var response = await Client.GetAsync("/api/v1/Usuarios/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// TESTE: POST /api/v1/Usuarios - Criar usuário com dados inválidos
    /// </summary>
    [Fact]
    public async Task PostUsuario_DeveRetornarBadRequest_QuandoDadosInvalidos()
    {
        CleanDatabase();

        var usuarioDto = new UsuarioInputDto
        {
            Nome = "",
            Email = "",
            Senha = "",
            Telefone = ""
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Usuarios", usuarioDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// TESTE: POST /api/v1/Usuarios - Criar usuário com email duplicado
    /// </summary>
    [Fact]
    public async Task PostUsuario_DeveRetornarConflict_QuandoEmailJaExiste()
    {
        CleanDatabase();

        var usuarioExistente = TestDataBuilder.CreateUsuario(email: "joao@test.com");
        DbContext.Usuario.Add(usuarioExistente);
        await DbContext.SaveChangesAsync();

        var usuarioDto = new UsuarioInputDto
        {
            Nome = "Maria Silva",
            Email = "joao@test.com",
            Senha = "123456",
            Telefone = "11988888888"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Usuarios", usuarioDto);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    /// <summary>
    /// TESTE: PUT /api/v1/Usuarios/{id} - Atualizar usuário
    /// </summary>
    [Fact]
    public async Task PutUsuario_DeveAtualizarUsuario_QuandoDadosValidos()
    {
        CleanDatabase();

        var usuario = TestDataBuilder.CreateUsuario(
            nome: "João Silva",
            email: "joao@test.com"
        );
        DbContext.Usuario.Add(usuario);
        await DbContext.SaveChangesAsync();

        var updateDto = new UsuarioInputDto
        {
            Nome = "João Silva Santos",
            Email = "joao@test.com",
            Senha = "654321",
            Telefone = "11977777777"
        };

        var response = await Client.PutAsJsonAsync($"/api/v1/Usuarios/{usuario.IdUsuario}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Recarrega o contexto para ver as mudanças
        await DbContext.Entry(usuario).ReloadAsync();
        var updatedUsuario = await DbContext.Usuario.FindAsync(usuario.IdUsuario);
        updatedUsuario.Should().NotBeNull();
        updatedUsuario!.Nome.Should().Be("João Silva Santos");
    }

    /// <summary>
    /// TESTE: DELETE /api/v1/Usuarios/{id} - Deletar usuário
    /// </summary>
    [Fact]
    public async Task DeleteUsuario_DeveRemoverUsuario_QuandoIdExiste()
    {
        CleanDatabase();

        var usuario = TestDataBuilder.CreateUsuario();
        DbContext.Usuario.Add(usuario);
        await DbContext.SaveChangesAsync();

        var response = await Client.DeleteAsync($"/api/v1/Usuarios/{usuario.IdUsuario}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Limpa o contexto para garantir que a consulta venha do banco
        DbContext.ChangeTracker.Clear();
        var deletedUsuario = await DbContext.Usuario.FindAsync(usuario.IdUsuario);
        deletedUsuario.Should().BeNull();
    }

    /// <summary>
    /// TESTE: DELETE /api/v1/Usuarios/{id} - Deletar usuário que não existe
    /// </summary>
    [Fact]
    public async Task DeleteUsuario_DeveRetornarNotFound_QuandoIdNaoExiste()
    {
        CleanDatabase();

        var response = await Client.DeleteAsync("/api/v1/Usuarios/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// TESTE: POST /api/v1/Usuarios/Login - Login com credenciais válidas
    /// </summary>
    [Fact]
    public async Task Login_DeveRetornarOk_QuandoCredenciaisValidas()
    {
        CleanDatabase();

        var usuario = TestDataBuilder.CreateUsuario(
            email: "joao@test.com",
            senha: "123456"
        );
        DbContext.Usuario.Add(usuario);
        await DbContext.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            Email = "joao@test.com",
            Senha = "123456"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Usuarios/Login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    /// <summary>
    /// TESTE: POST /api/v1/Usuarios/Login - Login com credenciais inválidas
    /// </summary>
    [Fact]
    public async Task Login_DeveRetornarUnauthorized_QuandoCredenciaisInvalidas()
    {
        CleanDatabase();

        var usuario = TestDataBuilder.CreateUsuario(
            email: "joao@test.com",
            senha: "123456"
        );
        DbContext.Usuario.Add(usuario);
        await DbContext.SaveChangesAsync();

        var loginDto = new LoginDto
        {
            Email = "joao@test.com",
            Senha = "senhaerrada"
        };

        var response = await Client.PostAsJsonAsync("/api/v1/Usuarios/Login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
