using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Application.Common.Mappings;
using PearsCleanV3.Application.Common.Models;
using PearsCleanV3.Application.Users.Queries.GetUsers;
using PearsCleanV3.Domain.Common;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Users.Commands.SetProfilePicture;

public record UpdateProfileCommand : IRequest
{
    public string? realName { get; init; }
    public string? description { get; init; }
    public IFormFile? file { get; init; }
}

public class UpdateProfile : IRequestHandler<UpdateProfileCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public UpdateProfile(IApplicationDbContext context, IFileStorage fileStorage, 
        UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _fileStorage = fileStorage;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = _contextAccessor.HttpContext?.User;

        if (user is null)
        {
            throw new ArgumentNullException("Не найден текущий пользователь для создания совпадения");
        }
        
        var currentUser = await _userManager.GetUserAsync(user);

        var url = Guid.NewGuid().ToString();
        
        // TODO: logging
        var userDomain = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == currentUser!.Id, cancellationToken: cancellationToken);

        if (request.file != null)
        {
            await _fileStorage.UploadPicture(url, request.file);
            userDomain!.ProfilePictureUrl = url;
        }

        userDomain!.RealName = request.realName;
        userDomain!.Description = request.description;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
