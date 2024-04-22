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

public class GetUserInfo(IApplicationDbContext context, IMapper mapper, 
    UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IFileStorage fileStorage) : IRequestHandler<GetUserInfoQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        ApplicationUser? user;
        if (request.Id == null)
        {
            var userContext = contextAccessor.HttpContext?.User;

            if (userContext is null)
            {
                throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
            }

            user = await userManager.GetUserAsync(userContext);
        }
        else
        {
            user = await context.Users.FirstOrDefaultAsync(x => x.Id == request.Id,
                cancellationToken: cancellationToken);
        }

        if (user == null)
        {
            throw new ArgumentNullException();
        }

        var data = await context.Users
            .OrderBy(x => x.Id)
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .FirstAsync(x => x.Id == user.Id, cancellationToken: cancellationToken);

        if (user.ProfilePictureUrl != null)
        {
            data.File = await fileStorage.GetPicture(user.ProfilePictureUrl);
        }

        return data;
    }
}
