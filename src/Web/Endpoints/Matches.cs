using PearsCleanV3.Application.Common.Models;
using PearsCleanV3.Application.Matches.Commands;
using PearsCleanV3.Application.Matches.Queries.GetMatchesWithPagination;

namespace PearsCleanV3.Web.Endpoints;

public class Matches : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetMatchesWithPagination)
            .MapPost(CreateMatch);
    }

    public async Task<List<MatchesDto>> GetMatchesWithPagination(ISender sender, [AsParameters] GetMatchesWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    
    public async Task<int> CreateMatch(ISender sender, CreateMatchCommand command)
    {
        return await sender.Send(command);
    }
}
