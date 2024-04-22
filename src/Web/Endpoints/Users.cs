using PearsCleanV3.Domain.Entities;
using PearsCleanV3.Infrastructure.Identity;

namespace PearsCleanV3.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapIdentityApi<ApplicationUser>();
    }
}
