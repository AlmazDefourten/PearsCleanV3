using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Application.Users.Queries;
using PearsCleanV3.Application.Users.Queries.GetUsers;
using PearsCleanV3.Domain.Common;

namespace PearsCleanV3.Application.Chats.Queries;

public record GetMessagesQuery: IRequest<List<MessageDto>>
{
    public string? UserId { get; init; }
}

public class GetMessagesList(IApplicationDbContext context, IMapper mapper, IUser currentUser) : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await context.Messages
            .Where(x => x.UserTo != null && request.UserId != null && x.UserFrom != null 
                && ((x.UserFrom.Id == request.UserId && x.UserTo.Id == currentUser.Id) 
                || (x.UserTo!.Id == request.UserId && x.UserFrom!.Id == currentUser.Id)))
            .OrderBy(x => x.CreateTime)
            .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken);
        
        foreach (var msg in messages)
        {
            msg.Me = msg.UserFromId == currentUser.Id;
        }
        
        return messages;
    }
}
