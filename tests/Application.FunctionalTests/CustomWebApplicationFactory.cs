using System.Data.Common;
using System.Security.Claims;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.FunctionalTests;

using static Testing;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection;

    public CustomWebApplicationFactory(DbConnection connection)
    {
        _connection = connection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services
                .RemoveAll<IUser>()
                .AddTransient(_ => Mock.Of<IUser>(s => s.Id == GetUserId()));

            services
                .RemoveAll<DbContextOptions<ApplicationDbContext>>()
                .AddDbContext<ApplicationDbContext>((sp, options) =>
                {
                    options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                    options.UseNpgsql(_connection);
                });
            
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            var claims = new List<Claim>
            { 
                new(ClaimTypes.Name, "test@local"),
                new(ClaimTypes.NameIdentifier, "1001"),
                new("Email", "test@local")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            httpContext.User = claimsPrincipal;
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);
            services.AddSingleton(httpContextAccessorMock.Object);
        });
    }
}
