using Microsoft.AspNetCore.Identity;

namespace PearsCleanV3.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? RealName { get; set; }
    
    public string? Description { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
}
