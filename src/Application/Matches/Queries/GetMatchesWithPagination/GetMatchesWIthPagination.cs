﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

public class GetMatchesQueryHandler : IRequestHandler<GetMatchesWithPaginationQuery, List<MatchesDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorage _fileStorage;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public GetMatchesQueryHandler(IApplicationDbContext context, IFileStorage fileStorage, 
        UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
    {
        _context = context;
        _fileStorage = fileStorage;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public async Task<List<MatchesDto>> Handle(GetMatchesWithPaginationQuery request, CancellationToken cancellationToken)
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
        
        var data = await _context.Matches
            .Where(x => x.MatchedUser != null && x.MatchedUser.Id == currentUser.Id && x.SwipedUser != null)
            .Join(_context.Users, m => m.SwipedUser!.Id, u => u.Id,
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
                match.File = await _fileStorage.GetPicture(match.ProfilePictureUrl);
        }

        return data;
    }
}
