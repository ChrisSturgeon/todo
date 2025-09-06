using api.Data;
using api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace api.IntegrationTests;

public class TodoApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16.1")
        .WithDatabase("test_db")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString());
        builder.ConfigureServices(services =>
        {
            // Get the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to get the DbContext
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;

            try
            {
                var dbContext = scopedServices.GetRequiredService<TodoDbContext>();
                
                // This is the key part: apply the database migrations
                dbContext.Database.Migrate();

                // You can also add some initial data for your tests here
                // e.g., dbContext.Todos.Add(new Todo { ... });
                // dbContext.SaveChanges();

                dbContext.Todos.Add(new Todo()
                {
                    Id = new Guid("fe84672e-302e-460e-b292-938ac417ccda"),
                    Name = "Walk the dog",
                    Description = "Take the dog for a walk",
                    Completed = false,
                    Position = 0,
                    CreatedAt = new DateTimeOffset(2025, 09, 06, 10, 55, 12, TimeSpan.Zero),
                    UpdatedAt = new DateTimeOffset(2025, 09, 06, 10, 55, 12, TimeSpan.Zero),
                });
                
                dbContext.Todos.Add(new Todo()
                {
                    Id = new Guid("a72d3a45-c580-4152-bf3b-67b33b18a3aa"),
                    Name = "Clean the house",
                    Description = "Deep clean the house including the bathroom",
                    Completed = true,
                    Position = 1,
                    CreatedAt = new DateTimeOffset(2025, 08, 04, 06, 22, 42, TimeSpan.Zero),
                    UpdatedAt = new DateTimeOffset(2025, 08, 04, 06, 22, 42, TimeSpan.Zero),
                });

                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log or handle any exceptions during migration
                Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
            }
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}