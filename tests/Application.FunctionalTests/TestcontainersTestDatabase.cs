using System.Data.Common;
using PearsCleanV3.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.PostgreSql;

namespace PearsCleanV3.Application.FunctionalTests;

public class TestcontainersTestDatabase : ITestDatabase
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithAutoRemove(true)
        .Build();
    private DbConnection _connection = null!;
    private string _connectionString = null!;
    private Respawner _respawner = null!;

    public async Task InitialiseAsync()
    {
        await _container.StartAsync();

        _connectionString = _container.GetConnectionString();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        var context = new ApplicationDbContext(options);
        
        context.Database.Migrate();
        
        _connection = context.Database.GetDbConnection();
        _connection.Open();
        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            SchemasToInclude = new []
            {
                "public"
            },
            DbAdapter = DbAdapter.Postgres
        });
    }

    public DbConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(_connection);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _container.DisposeAsync();
    }
}
