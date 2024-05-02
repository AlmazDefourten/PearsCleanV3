using PearsCleanV3.Application.Users.Queries.GetUserInfo;
using PearsCleanV3.Application.Users.Queries.GetUsers;

namespace PearsCleanV3.Application.FunctionalTests.Users.Queries;

using static Testing;

public class GetUsersWithPaginationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnCreatedData()
    {
        var userCreated = await RunAsDefaultUserAsync();

        var query = new GetUsersWithPaginationQuery
        {
            PageNumber = 1,
            PageSize = 25
        };

        var result = await SendAsync(query);

        result.Count.Should().Be(1);
        result.First().Id.Should().Be(userCreated.Id);
    }
}
