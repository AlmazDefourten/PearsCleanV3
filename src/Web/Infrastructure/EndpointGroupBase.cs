using Microsoft.AspNetCore.Mvc;

namespace PearsCleanV3.Web.Infrastructure;

[IgnoreAntiforgeryToken]
public abstract class EndpointGroupBase
{
    public abstract void Map(WebApplication app);
}
