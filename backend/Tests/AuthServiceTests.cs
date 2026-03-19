using Enhanzer.Api.Interfaces;
using Enhanzer.Api.Models.DTOs;
using Enhanzer.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Enhanzer.Api.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WhenExternalLoginSucceeds_PersistsLocationsAndReturnsSuccess()
    {
        var externalService = new Mock<IExternalPosApiService>();
        var locationService = new Mock<ILocationService>();
        var logger = new Mock<ILogger<AuthService>>();

        externalService
            .Setup(service => service.LoginAsync(It.IsAny<ExternalLoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalLoginResponseDto
            {
                Success = true,
                Message = "ok",
                UserLocations =
                [
                    new UserLocationDto { Location_Code = "B001", Location_Name = "Main Branch" }
                ]
            });

        var service = new AuthService(externalService.Object, locationService.Object, logger.Object);

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Email = "user@example.com",
            Password = "password"
        });

        Assert.True(result.Success);
        Assert.Single(result.Locations);
        locationService.Verify(service => service.ReplaceLocationsAsync(
            It.Is<IEnumerable<UserLocationDto>>(locations => locations.Any(location => location.Location_Code == "B001")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenExternalLoginFails_DoesNotPersistLocations()
    {
        var externalService = new Mock<IExternalPosApiService>();
        var locationService = new Mock<ILocationService>();
        var logger = new Mock<ILogger<AuthService>>();

        externalService
            .Setup(service => service.LoginAsync(It.IsAny<ExternalLoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ExternalLoginResponseDto
            {
                Success = false,
                Message = "Invalid username or password."
            });

        var service = new AuthService(externalService.Object, locationService.Object, logger.Object);

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Email = "user@example.com",
            Password = "wrong"
        });

        Assert.False(result.Success);
        Assert.Equal("Invalid username or password.", result.Message);
        locationService.Verify(service => service.ReplaceLocationsAsync(It.IsAny<IEnumerable<UserLocationDto>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
