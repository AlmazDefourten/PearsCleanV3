namespace PearsCleanV3.Domain.Entities;

public class Message : BaseAuditableEntity
{
    public string? Content { get; set; }
    
    public ApplicationUser? UserFrom { get; set; }
    
    public ApplicationUser? UserTo { get; set; }
    
    public DateTime? CreateTime { get; set; }
}
