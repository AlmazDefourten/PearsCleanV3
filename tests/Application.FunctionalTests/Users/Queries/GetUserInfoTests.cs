using System.Data;
using PearsCleanV3.Application.Users.Queries;
using PearsCleanV3.Application.Users.Queries.GetUserInfo;

namespace PearsCleanV3.Application.FunctionalTests.Users.Queries;

using static Testing;

public class GetUserInfoTests : BaseTestFixture
{
    [Test]
    public async Task ShouldNotReturnWithErrorId()
    {
        const string errorId = "-123";

        var query = new GetUserInfoQuery
        {
            Id = errorId
        };

        try
        {
            await SendAsync<UserDto?>(query);
        }
        catch
        {
            return;
        }

        throw new ArgumentException("При получении записи с данным идентификатором должна была возникнуть ошибка",
            errorId);
    }

    [Test]
    public async Task ShouldReturnCreatedData()
    {
        var userCreated = await RunAsDefaultUserAsync();

        var query = new GetUserInfoQuery
        {
            Id = userCreated.Id
        };

        var result = await SendAsync(query);

        result.Id.Should().Be(userCreated.Id);
        result.RealName.Should().Be(userCreated.RealName);
        result.Description.Should().Be(userCreated.Description);
        result.ProfilePictureUrl.Should().Be(userCreated.ProfilePictureUrl);
    }
}
