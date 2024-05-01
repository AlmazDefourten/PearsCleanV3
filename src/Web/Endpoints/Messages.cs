using Microsoft.AspNetCore.Mvc;
using PearsCleanV3.Application.Chats;
using PearsCleanV3.Application.Chats.Commands;
using PearsCleanV3.Application.Chats.Queries;
using PearsCleanV3.Application.Common.Models;
using PearsCleanV3.Application.Matches.Commands;
using PearsCleanV3.Application.Matches.Queries.GetMatchesWithPagination;

namespace PearsCleanV3.Web.Endpoints;

public class Messages : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .DisableAntiforgery()
            .MapGet(GetMessages)
            .MapGet(GetChats, "GetChats")
            .MapPost(CreateMessage);
    }

    public async Task<List<MessageDto>> GetMessages(ISender sender, [AsParameters] GetMessagesQuery query)
    {
        return await sender.Send(query);
    }
    
    public async Task<List<ChatDto>> GetChats(ISender sender, [AsParameters] GetChatsQuery query)
    {
        return await sender.Send(query);
    }
    
    public async Task CreateMessage(ISender sender, [FromForm] CreateMessageCommand command)
    {
        await sender.Send(command);
    }
}
