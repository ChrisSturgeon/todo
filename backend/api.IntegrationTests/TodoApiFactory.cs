using api.Data;
using api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace api.IntegrationTests;

// This factory no longer implements IAsyncLifetime.
// Its only purpose is to configure the web host.
public class TodoApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public TodoApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContextOptions registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Re-register with the container connection string
            services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
            });
        });
    }
}