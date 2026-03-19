using Enhanzer.Api.Data;
using Enhanzer.Api.Models.DTOs;
using Enhanzer.Api.Models.Entities;
using Enhanzer.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Enhanzer.Api.Tests;

public class LocationServiceTests
{
    [Fact]
    public async Task ReplaceLocationsAsync_ClearsOldRowsAndInsertsFreshLocations()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var dbContext = new ApplicationDbContext(options);
        dbContext.LocationDetails.Add(new LocationDetail
        {
            Location_Code = "OLD",
            Location_Name = "Old Branch"
        });
        await dbContext.SaveChangesAsync();

        var service = new LocationService(dbContext);

        await service.ReplaceLocationsAsync(
        [
            new UserLocationDto { Location_Code = "NEW1", Location_Name = "North Branch" },
            new UserLocationDto { Location_Code = "NEW2", Location_Name = "South Branch" }
        ]);

        var savedLocations = await dbContext.LocationDetails.OrderBy(location => location.Location_Code).ToListAsync();

        Assert.Equal(2, savedLocations.Count);
        Assert.DoesNotContain(savedLocations, location => location.Location_Code == "OLD");
        Assert.Equal("NEW1", savedLocations[0].Location_Code);
        Assert.Equal("NEW2", savedLocations[1].Location_Code);
    }
}
