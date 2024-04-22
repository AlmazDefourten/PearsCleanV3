using System.ComponentModel;
using System.Composition.Hosting.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using PearsCleanV3.Application.Users.Commands.SetProfilePicture;

namespace PearsCleanV3.Web.Endpoints;

public class Profile() : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .DisableAntiforgery()
            .MapPost(UpdateProfile, "UpdateProfile");
    }
    
    public async Task UpdateProfile(ISender sender, [FromForm] UpdateProfileCommand command)
    {
        await sender.Send(command);
    }
}
