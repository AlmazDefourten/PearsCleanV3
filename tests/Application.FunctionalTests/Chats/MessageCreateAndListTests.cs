using PearsCleanV3.Application.Chats.Commands;
using PearsCleanV3.Application.Chats.Queries;
using PearsCleanV3.Application.FunctionalTests.Mocks;

namespace PearsCleanV3.Application.FunctionalTests.Chats;

using static Testing;

public class MessageCreateAndListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldCreateMessageAndGetMessageInList()
    {
        await RunAsDefaultUserAsync();
        var userCreated = await RunAsAdministratorAsync();

        var command = new CreateMessageCommand
        {
            file = FileMocksFactory.CreateFormFileMock(),
            message = "test",
            userToId = userCreated.Id
        };

        await SendAsync(command);

        var query = new GetMessagesQuery { UserId = userCreated.Id };
        
        var list = await SendAsync(query);
        
        var messageCreated = list.FirstOrDefault(m => m.Content == command.message);

        messageCreated.Should().NotBeNull();
        messageCreated!.Content.Should().Be("test");
    }
}
