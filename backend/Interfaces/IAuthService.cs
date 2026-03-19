using Enhanzer.Api.Models.DTOs;

namespace Enhanzer.Api.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
