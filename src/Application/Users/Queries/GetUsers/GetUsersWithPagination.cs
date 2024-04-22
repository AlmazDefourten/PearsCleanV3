using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Application.Common.Mappings;
using PearsCleanV3.Application.Common.Models;
using PearsCleanV3.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using PearsCleanV3.Domain.Common;

namespace PearsCleanV3.Application.Users.Queries.GetUsers;

public record GetUsersWithPaginationQuery : IRequest<List<UserDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetUsersWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IFileStorage fileStorage, IUser currentUser) : IRequestHandler<GetUsersWithPaginationQuery, List<UserDto>>
{
    private const string Adminid = "665d5a02-b4f9-4fa2-9bbe-065fec73bc1a";

    public async Task<List<UserDto>> Handle(GetUsersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var users = context.Users
            .Where(x => x.Id != Adminid && x.Id != currentUser.Id)
            .OrderBy(x => x.Id)
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .ToList();

        foreach (var user in users)
        {
            if (user.ProfilePictureUrl != null)
            {
                user.File = await fileStorage.GetPicture(user.ProfilePictureUrl);
            }
        }
        
        
        return users;
    }
}
