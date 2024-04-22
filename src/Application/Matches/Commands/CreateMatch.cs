using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Application.TodoItems.Commands.CreateTodoItem;
using PearsCleanV3.Domain.Entities;
using PearsCleanV3.Domain.Events;

namespace PearsCleanV3.Application.Matches.Commands;

public record CreateMatchCommand : IRequest<int>
{
    public string? MatchedUserId { get; init; }
}

public class CreateMatchCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor) : IRequestHandler<CreateMatchCommand, int>
{
    public async Task<int> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
    {
        var user = contextAccessor.HttpContext?.User;

        if (user is null)
        {
            throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
        }
        
        var currentUser = await userManager.GetUserAsync(user);

        if (currentUser == null)
        {
            throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
        }
        
        var entity = new Match
        {
            MatchedUser = await context.Users.FirstAsync(
                x => x.Id == request.MatchedUserId, cancellationToken: cancellationToken),
            SwipedUser = currentUser
        };

        if (await context.Matches.AnyAsync(x =>
                x.MatchedUser != null && x.SwipedUser != null && x.SwipedUser.Id == currentUser.Id &&
                entity.MatchedUser.Id == x.MatchedUser.Id, cancellationToken: cancellationToken))
        {
            return 0;
        }

        // TODO: сделать логгирование через events, сделать валидирование
        // entity.AddDomainEvent(new TodoItemCreatedEvent(entity));
        
        context.Matches.Add(entity);

        await context.SaveChangesAsync(cancellationToken);
        
        var twoMatchesFrom = await context.Matches.AnyAsync(m => m.SwipedUser != null &&
                                                                 m.SwipedUser!.Id == currentUser.Id &&
                                                                 m.MatchedUser!.Id == context.Users.First(
                                                                     x => x.Id == request.MatchedUserId).Id, cancellationToken: cancellationToken);
        
        var twoMatchesTo = await context.Matches.AnyAsync(m => m.SwipedUser != null &&
                                                               m.MatchedUser!.Id == currentUser.Id &&
                                                               m.SwipedUser!.Id == context.Users.First(
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
                UserFrom = await context.Users.FirstAsync(
                    x => x.Id == request.MatchedUserId, cancellationToken: cancellationToken),
                UserTo = currentUser
            };
        
            var messageTo = new Message
            {
                Content = "У вас мэтч! Начинайте общаться :)",
                Created = DateTime.Now.ToUniversalTime(),
                CreateTime = DateTime.Now.ToUniversalTime(),
                UserFrom = currentUser,
                UserTo = await context.Users.FirstAsync(
                    x => x.Id == request.MatchedUserId, cancellationToken: cancellationToken)
            };

            context.Messages.Add(messageFrom);
            context.Messages.Add(messageTo);
            
            await context.SaveChangesAsync(cancellationToken);
        }

        return entity.Id;
    }
}
