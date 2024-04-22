using Microsoft.AspNetCore.Identity;

namespace PearsCleanV3.Domain.Entities;

public class Match : BaseAuditableEntity
{
    public ApplicationUser? SwipedUser { get; set; }
    
    public ApplicationUser? MatchedUser { get; set; }
}
