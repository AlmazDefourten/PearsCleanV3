using Microsoft.AspNetCore.SignalR;

namespace PearsCleanV3.Web.Infrastructure;

/// <summary>
/// Класс, предоставляющий возможность идентифицировать пользователя SignalR
/// </summary>
public sealed class CustomUserIdProvider: IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.Claims.First().Value;
    }
}
