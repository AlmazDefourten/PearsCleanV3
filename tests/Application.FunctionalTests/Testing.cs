using System.Security.Claims;
using PearsCleanV3.Domain.Constants;
using PearsCleanV3.Infrastructure.Data;
using PearsCleanV3.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.FunctionalTests;

[SetUpFixture]
public class Testing
{
    private static ITestDatabase s_database = null!;
    private static CustomWebApplicationFactory s_factory = null!;
    private static IServiceScopeFactory s_scopeFactory = null!;
    private static string? s_userId;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        s_database = await TestDatabaseFactory.CreateAsync();

        s_factory = new CustomWebApplicationFactory(s_database.GetConnection());

        s_scopeFactory = s_factory.Services.GetRequiredService<IServiceScopeFactory>();
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = s_scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task SendAsync(IBaseRequest request)
    {
        using var scope = s_scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        await mediator.Send(request);
    }

    public static string? GetUserId()
    {
        return s_userId;
    }

    public static async Task<ApplicationUser> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", Array.Empty<string>(), "1001");
    }

    public static async Task<ApplicationUser> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@local", "Administrator1234!", new[] { Roles.Administrator }, "1002");
    }

    public static async Task<ApplicationUser> RunAsUserAsync(string userName, string password, string[] roles, string? id = null)
    {
        using var scope = s_scopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        var claims = new List<Claim>
        { 
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.NameIdentifier, id ?? GenerateUserId()),
            new("Email", userName),
            new("RealName", userName)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var userFound = await userManager.GetUserAsync(claimsPrincipal);

        if (userFound != null)
        {
            return userFound;
        }
        
        var user = new ApplicationUser { Id = id ?? GenerateUserId(), UserName = userName, Email = userName };

        var result = await userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return user;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Не удалось создать пользователя {userName}.{Environment.NewLine}{errors}");
    }

    private static string GenerateUserId()
    {
        return Guid.NewGuid().ToString();
    }

    public static async Task ResetState()
    {
        try
        {
            await s_database.ResetAsync();
        }
        catch (Exception)
        {
            // ignored
        }

        s_userId = null;
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = s_scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = s_scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = s_scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await s_database.DisposeAsync();
        await s_factory.DisposeAsync();
    }
}
