using Enhanzer.Api.Data;
using Enhanzer.Api.Interfaces;
using Enhanzer.Api.Models.DTOs;
using Enhanzer.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Enhanzer.Api.Services;

public class LocationService(ApplicationDbContext dbContext) : ILocationService
{
    public async Task ReplaceLocationsAsync(IEnumerable<UserLocationDto> locations, CancellationToken cancellationToken = default)
    {
        var normalizedLocations = locations
            .Where(location => !string.IsNullOrWhiteSpace(location.Location_Code) && !string.IsNullOrWhiteSpace(location.Location_Name))
            .Select(location => new LocationDetail
            {
                Location_Code = location.Location_Code.Trim(),
                Location_Name = location.Location_Name.Trim(),
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        dbContext.LocationDetails.RemoveRange(dbContext.LocationDetails);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (normalizedLocations.Count == 0)
        {
            return;
        }

        await dbContext.LocationDetails.AddRangeAsync(normalizedLocations, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LocationDto>> GetLocationsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.LocationDetails
            .AsNoTracking()
            .OrderBy(location => location.Location_Name)
            .Select(location => new LocationDto
            {
                Id = location.Id,
                LocationCode = location.Location_Code,
                LocationName = location.Location_Name
            })
            .ToListAsync(cancellationToken);
    }
}
