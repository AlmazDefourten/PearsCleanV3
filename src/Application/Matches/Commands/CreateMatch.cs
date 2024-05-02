using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Matches.Commands;

public record CreateMatchCommand : IRequest<int>
{
    public string? SwipedUserId { get; init; }
    
    public string? MatchedUserId { get; init; }
}

public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public CreateMatchCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public async Task<int> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
    {
        var user = _contextAccessor.HttpContext?.User;

        if (user is null)
        {
            throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
        }
        
        var currentUser = request.SwipedUserId == null 
            ? await _userManager.GetUserAsync(user)
            : await _context.Users.FindAsync(new object?[] { request.SwipedUserId }, cancellationToken: cancellationToken);

        if (currentUser == null)
        {
            throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
        }
        
        var entity = new Match
        {
            MatchedUser = await _context.Users.FirstAsync(
                x => x.Id == request.MatchedUserId, cancellationToken: cancellationToken),
            SwipedUser = currentUser
        };

        if (await _context.Matches.AnyAsync(x =>
                x.MatchedUser != null && x.SwipedUser != null && x.SwipedUser.Id == currentUser.Id &&
                entity.MatchedUser.Id == x.MatchedUser.Id, cancellationToken: cancellationToken))
        {
            return 0;
        }

        // TODO: сделать логгирование через events, сделать валидирование
        // entity.AddDomainEvent(new TodoItemCreatedEvent(entity));
        
        _context.Matches.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);
        
        var twoMatchesFrom = await _context.Matches.AnyAsync(m => m.SwipedUser != null &&
                                                                 m.SwipedUser!.Id == currentUser.Id &&
                                                                 m.MatchedUser!.Id == _context.Users.First(
                                                                     x => x.Id == request.MatchedUserId).Id, cancellationToken: cancellationToken);
        
        var twoMatchesTo = await _context.Matches.AnyAsync(m => m.SwipedUser != null &&
                                                               m.MatchedUser!.Id == currentUser.Id &&
                                                               m.SwipedUser!.Id == _context.Users.First(
                                                                   x => x.Id == request.MatchedUserId).Id, cancellationToken: cancellationToken);

        if (!twoMatchesFrom || !twoMatchesTo)
        {
            return entity.Id;
        }

        {
            var messageFrom = new Message
            {
                Content = "У вас мэтч! Начинайте общаться :)",
                Created = DateTime.Now.ToUniversalTime(),
                CreateTime = DateTime.Now.ToUniversalTime(),
                UserFrom = await _context.Users.FirstAsync(
                    x => x.Id == request.MatchedUserId, cancellationToken: cancellationToken),
                UserTo = currentUser
            };
        
            var messageTo = new Message
            {
                Content = "У вас мэтч! Начинайте общаться :)",
                Created = DateTime.Now.ToUniversalTime(),
                CreateTime = DateTime.Now.ToUniversalTime(),
                UserFrom = currentUser,
                UserTo = await _context.Users.FirstAsync(
                    x => x.Id == request.MatchedUserId, cancellationToken: cancellationToken)
            };

            _context.Messages.Add(messageFrom);
            _context.Messages.Add(messageTo);
            
            await _context.SaveChangesAsync(cancellationToken);
        }

        return entity.Id;
    }
}
