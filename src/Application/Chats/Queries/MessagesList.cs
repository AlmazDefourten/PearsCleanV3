using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Application.Users.Queries;
using PearsCleanV3.Application.Users.Queries.GetUsers;
using PearsCleanV3.Domain.Common;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Chats.Queries;

public record GetMessagesQuery: IRequest<List<MessageDto>>
{
    public string? UserId { get; init; }
}

public class GetMessagesList : IRequestHandler<GetMessagesQuery, List<MessageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileStorage _fileStorage;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetMessagesList(IApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, 
        IHttpContextAccessor contextAccessor, IFileStorage fileStorage)
    {
        _context = context;
        _mapper = mapper;
        _fileStorage = fileStorage;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public async Task<List<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var user = _contextAccessor.HttpContext?.User;

        if (user is null)
        {
            throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
        }
        
        var currentUser = await _userManager.GetUserAsync(user);

        if (currentUser == null)
        {
            throw new NullReferenceException();
        }
        
        var messages = await _context.Messages
            .Where(x => x.UserTo != null && request.UserId != null && x.UserFrom != null 
                && ((x.UserFrom.Id == request.UserId && x.UserTo.Id == currentUser.Id) 
                || (x.UserTo!.Id == request.UserId && x.UserFrom!.Id == currentUser.Id)))
            .OrderBy(x => x.CreateTime)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken);
        
        foreach (var msg in messages)
        {
            msg.Me = msg.UserFromId == currentUser.Id;
            if (msg.PictureUrl != null)
            {
                msg.File = await _fileStorage.GetPicture(msg.PictureUrl);
            }
        }
        
        return messages;
    }
}
