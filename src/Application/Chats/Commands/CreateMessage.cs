using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Chats.Commands;

public record CreateMessageCommand : IRequest
{
    public string? Message { get; init; }
    public string? UserToId { get; set; }
}

public class CreateMatchCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor) : IRequestHandler<CreateMessageCommand>
{
    public async Task Handle(CreateMessageCommand request, CancellationToken cancellationToken)
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
        
        var entity = new Message
        {
            UserTo = await context.Users.FirstAsync(
                x => x.Id == request.UserToId, cancellationToken: cancellationToken),
            UserFrom = currentUser,
            Content = request.Message,
            CreateTime = DateTime.Now.ToUniversalTime()
        };

        // TODO: сделать логгирование через events, сделать валидирование
        // entity.AddDomainEvent(new TodoItemCreatedEvent(entity));
        
        context.Messages.Add(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}
