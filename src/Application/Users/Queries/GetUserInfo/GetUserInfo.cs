using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Application.Common.Mappings;
using PearsCleanV3.Application.Common.Models;
using PearsCleanV3.Domain.Common;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Users.Queries.GetUserInfo;

public record GetUserInfoQuery : IRequest<UserDto>
{
    public string? Id { get; init; }
}

public class GetUserInfo : IRequestHandler<GetUserInfoQuery, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IFileStorage _fileStorage;

    public GetUserInfo(IApplicationDbContext context, IMapper mapper, 
        UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IFileStorage fileStorage)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _fileStorage = fileStorage;
    }

    public async Task<UserDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        ApplicationUser? user;
        if (request.Id == null)
        {
            var userContext = _contextAccessor.HttpContext?.User;

            if (userContext is null)
            {
                throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
            }

            user = await _userManager.GetUserAsync(userContext);
        }
        else
        {
            user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id,
                cancellationToken: cancellationToken);
        }

        if (user == null)
        {
            throw new ArgumentNullException();
        }

        var data = await _context.Users
            .OrderBy(x => x.Id)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstAsync(x => x.Id == user.Id, cancellationToken: cancellationToken);

        if (user.ProfilePictureUrl != null)
        {
            data.File = await _fileStorage.GetPicture(user.ProfilePictureUrl);
        }

        return data;
    }
}
