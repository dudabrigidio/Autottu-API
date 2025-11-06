using Microsoft.EntityFrameworkCore;
using AutoTTU.Connection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoTTU.Tests.Integration;

public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
{
    protected readonly CustomWebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly AppDbContext DbContext;

    protected IntegrationTestBase(CustomWebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = Factory.CreateClient();
        
        // Adiciona a API Key para todas as requisições
        Client.DefaultRequestHeaders.Add("X-API-Key", "TestApiKey123");

        // Obtém o DbContext para manipulação direta se necessário
        var scope = Factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected void CleanDatabase()
    {
        DbContext.Checkin.RemoveRange(DbContext.Checkin);
        DbContext.Motos.RemoveRange(DbContext.Motos);
        DbContext.Slot.RemoveRange(DbContext.Slot);
        DbContext.Usuario.RemoveRange(DbContext.Usuario);
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        CleanDatabase();
        Client.Dispose();
    }
}
