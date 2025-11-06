using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AutoTTU.Connection;

namespace AutoTTU.Tests.Integration
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            // 🔹 Sobrescreve a configuração apenas em memória
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "ApiSettings:ApiKey", "TestApiKey123" },
                    // Passa o nome do banco para o Program.cs usar
                    { "TestDatabaseName", _databaseName }
                });
            });

            // 🔹 Sobrescreve a configuração do DbContext para usar o mesmo banco
            builder.ConfigureServices(services =>
            {
                // Remove o DbContext registrado pelo Program.cs
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(AppDbContext));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                var optionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (optionsDescriptor != null)
                {
                    services.Remove(optionsDescriptor);
                }

                // Registra novamente com o nome fixo do banco
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
            });
        }
    }
}
