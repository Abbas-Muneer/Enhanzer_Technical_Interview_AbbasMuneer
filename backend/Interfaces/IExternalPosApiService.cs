using Enhanzer.Api.Models.DTOs;

namespace Enhanzer.Api.Interfaces;

public interface IExternalPosApiService
{
    Task<ExternalLoginResponseDto> LoginAsync(ExternalLoginRequestDto request, CancellationToken cancellationToken = default);
}
