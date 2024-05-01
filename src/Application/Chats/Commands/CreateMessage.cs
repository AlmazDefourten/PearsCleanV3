using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Domain.Common;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Chats.Commands;

public record CreateMessageCommand : IRequest
{
    public string? message { get; init; }
    public string? userToId { get; set; }
    
    public IFormFile? file { get; init; }
}

public class CreateMatchCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, 
    IHttpContextAccessor contextAccessor, IFileStorage fileStorage) : IRequestHandler<CreateMessageCommand>
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
                x => x.Id == request.userToId, cancellationToken: cancellationToken),
            UserFrom = currentUser,
            Content = request.message,
            CreateTime = DateTime.Now.ToUniversalTime()
        };
        if (request.file != null)
        {
            var url = Guid.NewGuid().ToString();
            await fileStorage.UploadPicture(url, request.file);
            entity.PictureUrl = url;
        }

        // TODO: сделать логгирование через events, сделать валидирование
        // entity.AddDomainEvent(new TodoItemCreatedEvent(entity));
        
        context.Messages.Add(entity);

        await context.SaveChangesAsync(cancellationToken);
        
        
    }
}
