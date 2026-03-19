using Enhanzer.Api.Models.DTOs;

namespace Enhanzer.Api.Interfaces;

public interface ILocationService
{
    Task ReplaceLocationsAsync(IEnumerable<UserLocationDto> locations, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LocationDto>> GetLocationsAsync(CancellationToken cancellationToken = default);
}
