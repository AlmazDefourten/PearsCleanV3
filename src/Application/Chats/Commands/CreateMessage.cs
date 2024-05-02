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

public class CreateMatchCommandHandler : IRequestHandler<CreateMessageCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IFileStorage _fileStorage;

    public CreateMatchCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager, 
        IHttpContextAccessor contextAccessor, IFileStorage fileStorage)
    {
        _context = context;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _fileStorage = fileStorage;
    }

    public async Task Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var user = _contextAccessor.HttpContext?.User;

        if (user is null)
        {
            throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
        }
        
        var currentUser = await _userManager.GetUserAsync(user);

        if (currentUser == null)
        {
            throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
        }
        
        var entity = new Message
        {
            UserTo = await _context.Users.FirstAsync(
                x => x.Id == request.userToId, cancellationToken: cancellationToken),
            UserFrom = currentUser,
            Content = request.message,
            CreateTime = DateTime.Now.ToUniversalTime()
        };
        
        if (request.file != null)
        {
            var url = Guid.NewGuid().ToString();
            await _fileStorage.UploadPicture(url, request.file);
            entity.PictureUrl = url;
        }

        // TODO: сделать логгирование через events, сделать валидирование
        // entity.AddDomainEvent(new TodoItemCreatedEvent(entity));
        
        _context.Messages.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);
        
        
    }
}
