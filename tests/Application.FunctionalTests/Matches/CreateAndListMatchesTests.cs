using PearsCleanV3.Application.Matches.Commands;
using PearsCleanV3.Application.Matches.Queries.GetMatchesWithPagination;

namespace PearsCleanV3.Application.FunctionalTests.Matches;

using static Testing;

public class CreateAndListMatchesTests : BaseTestFixture
{
    [Test]
    public async Task ShouldCreateMessageAndGetMessageInList()
    {
        var swipedUser = await RunAsDefaultUserAsync();
        var matchedUser = await RunAsAdministratorAsync();

        var commandFirstSwipe = new CreateMatchCommand
        {
            SwipedUserId = swipedUser.Id,
            MatchedUserId = matchedUser.Id
        };

        await SendAsync(commandFirstSwipe);
        
        var commandSecondSwipe = new CreateMatchCommand
        {
            SwipedUserId = matchedUser.Id,
            MatchedUserId = swipedUser.Id
        };

        await SendAsync(commandSecondSwipe);

        var query = new GetMatchesWithPaginationQuery();
        
        var list = await SendAsync(query);
        
        var match = list.FirstOrDefault(m => m.MatchedUserId == swipedUser.Id && m.SwipedUserId == matchedUser.Id);

        match.Should().NotBeNull();
        match!.MatchedUserId.Should().Be(swipedUser.Id);
        match.SwipedUserId.Should().Be(matchedUser.Id);
    }
}
