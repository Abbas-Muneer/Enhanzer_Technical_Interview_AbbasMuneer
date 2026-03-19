using Enhanzer.Api.Interfaces;
using Enhanzer.Api.Models.DTOs;

namespace Enhanzer.Api.Services;

public class AuthService(
    IExternalPosApiService externalPosApiService,
    ILocationService locationService,
    ILogger<AuthService> logger) : IAuthService
{
    private static readonly List<UserLocationDto> DemoLocations =
    [
        new() { Location_Code = "BATCH001", Location_Name = "BATCH001" },
        new() { Location_Code = "BATCH002", Location_Name = "BATCH002" },
        new() { Location_Code = "BATCH003", Location_Name = "BATCH003" }
    ];

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.Equals(request.Email, "admin", StringComparison.OrdinalIgnoreCase) &&
            request.Password == "admin@123")
        {
            await locationService.ReplaceLocationsAsync(DemoLocations, cancellationToken);

            return new LoginResponseDto
            {
                Success = true,
                Message = "Demo login successful.",
                Locations = DemoLocations.Select(location => new LocationDto
                {
                    LocationCode = location.Location_Code,
                    LocationName = location.Location_Name
                }).ToList()
            };
        }

        var externalRequest = new ExternalLoginRequestDto
        {
            Company_Code = request.Email,
            API_Body = new ExternalLoginBodyDto
            {
                Username = request.Email,
                Pw = request.Password
            }
        };

        var externalResponse = await externalPosApiService.LoginAsync(externalRequest, cancellationToken);

        if (!externalResponse.Success)
        {
            logger.LogWarning("Login failed for {Email}: {Message}", request.Email, externalResponse.Message);
            return new LoginResponseDto
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(externalResponse.Message)
                    ? "Login failed. Please verify your credentials."
                    : externalResponse.Message
            };
        }

        await locationService.ReplaceLocationsAsync(externalResponse.UserLocations, cancellationToken);

        return new LoginResponseDto
        {
            Success = true,
            Message = "Login successful.",
            Locations = externalResponse.UserLocations.Select(location => new LocationDto
            {
                LocationCode = location.Location_Code,
                LocationName = location.Location_Name
            }).ToList()
        };
    }
}
