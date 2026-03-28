using Enhanzer.Api.DTOs;
using Enhanzer.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Enhanzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController(IMasterDataService masterDataService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItems(CancellationToken cancellationToken)
    {
        var items = await masterDataService.GetItemsAsync(cancellationToken);
        return Ok(items);
    }
}
