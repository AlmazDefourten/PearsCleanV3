using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Users.Queries;

public class UserDto
{
    public string? Id { get; init; }

    public string? RealName { get; init; }

    public string? Description { get; init; }

    public string? ProfilePictureUrl { get; init; }
    
    public byte[]? File { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
