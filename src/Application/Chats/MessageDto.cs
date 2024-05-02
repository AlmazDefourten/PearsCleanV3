using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.Chats;

public class MessageDto
{
    public string? Content { get; set; }
    
    public string? UserFromId { get; set; }
    
    public string? UserToId { get; set; }
    
    public DateTime? CreateTime { get; set; }
    
    public string? PictureUrl { get; set; }
    
    public byte[]? File { get; set; }
    
    public bool Me { get; set; }
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Message, MessageDto>();
        }
    }
}
