using PearsCleanV3.Application.Users.Queries;
using PearsCleanV3.Application.Users.Queries.GetUserInfo;
using PearsCleanV3.Application.Users.Queries.GetUsers;

namespace PearsCleanV3.Web.Endpoints;

public class Cards : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetUsersWithPagination)
            .MapGet(GetUserInfo, "GetUserInfo");
    }
    
    public async Task<List<UserDto>> GetUsersWithPagination(ISender sender)
    {
        return await sender.Send(new GetUsersWithPaginationQuery());
    }
    
    public async Task<UserDto> GetUserInfo(ISender sender)
    {
        return await sender.Send(new GetUserInfoQuery());
    }
}
