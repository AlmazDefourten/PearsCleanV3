using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Application.Users.Queries;
using PearsCleanV3.Application.Users.Queries.GetUsers;
using PearsCleanV3.Domain.Common;

namespace PearsCleanV3.Application.Chats.Queries;

public record GetChatsQuery: IRequest<List<ChatDto>>
{
    public string? UserId { get; init; }
}

public class GetChatsList(IApplicationDbContext context, IUser currentUser) : IRequestHandler<GetChatsQuery, List<ChatDto>>
{
    public async Task<List<ChatDto>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var chats = await context.Messages
            .Where(x => x.UserTo != null && x.UserTo.Id == currentUser.Id && x.UserFrom != null)
            .Select(x => new ChatDto()
            {
                Id = x.UserFrom!.Id,
                Title = x.UserFrom!.RealName
            })
            .ToListAsync(cancellationToken: cancellationToken);
        chats = chats.DistinctBy(x => x.Id).ToList();
        
        return chats;
    }
}
