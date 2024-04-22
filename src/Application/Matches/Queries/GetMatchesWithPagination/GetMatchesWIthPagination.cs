using Microsoft.EntityFrameworkCore.Internal;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Application.Common.Mappings;
using PearsCleanV3.Application.Common.Models;
using PearsCleanV3.Domain.Common;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Matches.Queries.GetMatchesWithPagination;

public record GetMatchesWithPaginationQuery : IRequest<List<MatchesDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetMatchesWithPaginationQueryValidator
{
    public GetMatchesWithPaginationQueryValidator()
    {
        // TODO: сделать валидирование
    }
}

public class GetMatchesQueryHandler(IApplicationDbContext context, IUser user, IFileStorage fileStorage) : IRequestHandler<GetMatchesWithPaginationQuery, List<MatchesDto>>
{
    public async Task<List<MatchesDto>> Handle(GetMatchesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await context.Matches
            .Where(x => x.MatchedUser != null && x.MatchedUser.Id == user.Id && x.SwipedUser != null)
            .Join(context.Users, m => m.SwipedUser!.Id, u => u.Id,
                (m, u) => new MatchesDto()
                {
                    MatchedUserId = m.MatchedUser!.Id,
                    SwipedUserId = m.SwipedUser!.Id,
                    Id = u.Id,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    Description = u.Description,
                    RealName = u.RealName
                })
            .ToListAsync(cancellationToken: cancellationToken);

        foreach (var match in data)
        {
            if (match.ProfilePictureUrl != null)
                match.File = await fileStorage.GetPicture(match.ProfilePictureUrl);
        }

        return data;
    }
}
