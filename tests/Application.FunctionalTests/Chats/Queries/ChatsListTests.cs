using PearsCleanV3.Application.Chats.Commands;
using PearsCleanV3.Application.Chats.Queries;
using PearsCleanV3.Application.FunctionalTests.Mocks;

namespace PearsCleanV3.Application.FunctionalTests.Chats.Queries;

using static Testing;

public class ChatsListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldBeEmptyIfMessagesEmpty()
    {
        await RunAsDefaultUserAsync();

        var query = new GetChatsQuery();
        
        var list = await SendAsync(query);

        list.Should().NotBeNull();
        list.Count.Should().Be(0);
    }
    
    [Test]
    public async Task ShouldOneChatWithOneMessage()
    {
        var curUser = await RunAsDefaultUserAsync();
        
        var command = new CreateMessageCommand
        {
            file = FileMocksFactory.CreateFormFileMock(),
            message = "test",
            userToId = curUser.Id
        };

        await SendAsync(command);
        
        var query = new GetChatsQuery();
        
        var list = await SendAsync(query);

        list.Should().NotBeNull();
        list.Count.Should().Be(1);
    }
}
