namespace PearsCleanV3.Application.Chats.Queries;

public class ChatDto
{
    public string? Id { get; init; }

    public string? Title { get; init; }

    public bool Active { get; init; }

    public string? ProfilePictureUrl { get; init; }

    public byte[]? IconFile { get; set; }

    public string? LastMessage { get; set; }
}
