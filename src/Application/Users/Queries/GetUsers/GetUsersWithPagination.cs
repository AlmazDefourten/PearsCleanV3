using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Domain.Common;

namespace PearsCleanV3.Application.Users.Queries.GetUsers;

public record GetUsersWithPaginationQuery : IRequest<List<UserDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetUsersWithPaginationQueryHandler : IRequestHandler<GetUsersWithPaginationQuery, List<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorage _fileStorage;
    private readonly IUser _currentUser;

    public GetUsersWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IFileStorage fileStorage, IUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
    }

    private const string Adminid = "665d5a02-b4f9-4fa2-9bbe-065fec73bc1a";

    public async Task<List<UserDto>> Handle(GetUsersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var users = _context.Users
            .Where(x => x.Id != Adminid && x.Id != _currentUser.Id)
            .OrderBy(x => x.Id)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToList();

        foreach (var user in users)
        {
            if (user.ProfilePictureUrl != null)
            {
                user.File = await _fileStorage.GetPicture(user.ProfilePictureUrl);
            }
        }
        
        
        return users;
    }
}
