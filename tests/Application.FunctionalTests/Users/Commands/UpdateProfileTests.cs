using PearsCleanV3.Application.FunctionalTests.Mocks;
using PearsCleanV3.Application.Users.Commands.SetProfilePicture;
using PearsCleanV3.Domain.Entities;

namespace PearsCleanV3.Application.FunctionalTests.Users.Commands;

using static Testing;

public class UpdateProfileTests : BaseTestFixture
{
    [Test]
    public async Task ShouldUpdateProfile()
    {
        var user = await RunAsDefaultUserAsync();

        var command = new UpdateProfileCommand
        {
            file = FileMocksFactory.CreateFormFileMock(),
            description = "Some descr",
            realName = "Ivan Ivanov"
        };

        await SendAsync(command);

        var userProfileUpdated = await FindAsync<ApplicationUser>(user.Id);

        userProfileUpdated.Should().NotBeNull();
        userProfileUpdated!.ProfilePictureUrl.Should().NotBeNullOrEmpty();
        userProfileUpdated.Description.Should().Be("Some descr");
        userProfileUpdated.RealName.Should().Be("Ivan Ivanov");
    }
}
