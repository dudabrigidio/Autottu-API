using Microsoft.EntityFrameworkCore;
using AutoTTU.Connection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTTU.Tests.Integration;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    protected readonly CustomWebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly IServiceScope Scope;
    protected readonly AppDbContext DbContext;

    protected IntegrationTestBase(CustomWebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();

        // Adiciona a API Key padrão
        Client.DefaultRequestHeaders.Add("X-API-Key", "TestApiKey123");

        // Cria um novo escopo (não usa o provider raiz!)
        Scope = Factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Garante que o banco de dados seja criado
        DbContext.Database.EnsureCreated();
    }

    protected void CleanDatabase()
    {
        if (DbContext == null) return;
        
        try
        {
            DbContext.Checkin.RemoveRange(DbContext.Checkin);
            DbContext.Motos.RemoveRange(DbContext.Motos);
            DbContext.Slot.RemoveRange(DbContext.Slot);
            DbContext.Usuario.RemoveRange(DbContext.Usuario);
            DbContext.SaveChanges();
        }
        catch
        {
            // Ignora erros ao limpar o banco de dados
        }
    }

    public void Dispose()
    {
        try
        {
            CleanDatabase();
        }
        catch
        {
            // Ignora erros ao limpar o banco de dados
        }
        finally
        {
            DbContext?.Dispose();
            Scope?.Dispose();
            Client?.Dispose();
        }
    }
}
