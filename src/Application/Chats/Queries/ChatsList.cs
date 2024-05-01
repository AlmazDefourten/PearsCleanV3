using PearsCleanV3.Application.Common.Interfaces;
using PearsCleanV3.Application.Users.Queries;
using PearsCleanV3.Application.Users.Queries.GetUsers;
using PearsCleanV3.Domain.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace PearsCleanV3.Application.Chats.Queries;

public record GetChatsQuery: IRequest<List<ChatDto>>
{
    public string? UserId { get; init; }
}

public class GetChatsList(IApplicationDbContext context, IUser currentUser, IFileStorage fileStorage) : IRequestHandler<GetChatsQuery, List<ChatDto>>
{
    public async Task<List<ChatDto>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
    {
        var chats = await context.Messages
            .Where(x => x.UserTo != null && x.UserTo.Id == currentUser.Id && x.UserFrom != null)
            .Select(x => new ChatDto()
            {
                Id = x.UserFrom!.Id,
                Title = x.UserFrom!.RealName,
                ProfilePictureUrl = x.UserFrom!.ProfilePictureUrl,
                LastMessage = context.Messages
                    .Where(lm => lm.UserFrom!.Id == x.UserFrom!.Id || (lm.UserFrom!.Id == currentUser.Id && lm.UserTo!.Id == x.UserFrom!.Id))
                    .OrderByDescending(lm => lm.Id)
                    .FirstOrDefault() == null 
                    ? null 
                    : context.Messages
                    .Where(lm => lm.UserFrom!.Id == x.UserFrom!.Id || (lm.UserFrom!.Id == currentUser.Id && lm.UserTo!.Id == x.UserFrom!.Id))
                    .OrderByDescending(lm => lm.Id)
                    .FirstOrDefault()!
                    .Content!.Substring(0, 20)
            })
            .ToListAsync(cancellationToken: cancellationToken);
        chats = chats.DistinctBy(x => x.Id).ToList();
        
        foreach (var chat in chats)
        {
            if (chat.ProfilePictureUrl != null)
            {
                chat.IconFile = RoundImageProcessor.CropToSquare(await fileStorage.GetPicture(chat.ProfilePictureUrl));
            }
        }
        
        return chats;
    }
}

static class RoundImageProcessor
{
    public static byte[] CropToSquare(byte[] imageBytes)
    {
        using Image image = Image.Load(imageBytes);
        int minSide = Math.Min(image.Width, image.Height);
        int xx = (image.Width - minSide) / 2;
        int y = (image.Height - minSide) / 2;

        image.Mutate(x => x.Crop(new Rectangle(xx, y, minSide, minSide)));

        using MemoryStream ms = new();
        image.SaveAsJpeg(ms);
        return ms.ToArray();
    }
}
