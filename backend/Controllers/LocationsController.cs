using Enhanzer.Api.DTOs;
using Enhanzer.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Enhanzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController(IMasterDataService masterDataService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LocationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocations(CancellationToken cancellationToken)
    {
        var locations = await masterDataService.GetLocationsAsync(cancellationToken);
        return Ok(locations);
    }
}
