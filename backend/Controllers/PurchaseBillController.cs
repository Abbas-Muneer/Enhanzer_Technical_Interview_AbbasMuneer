using Enhanzer.Api.DTOs;
using Enhanzer.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Enhanzer.Api.Controllers;

[ApiController]
[Route("api/purchase-bill")]
public class PurchaseBillController(IPurchaseBillService purchaseBillService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PurchaseBillListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurchaseBills(CancellationToken cancellationToken)
    {
        var purchaseBills = await purchaseBillService.GetPurchaseBillsAsync(cancellationToken);
        return Ok(purchaseBills);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PurchaseBillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPurchaseBill(int id, CancellationToken cancellationToken)
    {
        var purchaseBill = await purchaseBillService.GetPurchaseBillAsync(id, cancellationToken);
        if (purchaseBill is null)
        {
            return NotFound(new ErrorResponseDto { Message = "Purchase bill not found." });
        }

        return Ok(purchaseBill);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PurchaseBillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePurchaseBill([FromBody] SavePurchaseBillRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await purchaseBillService.CreatePurchaseBillAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PurchaseBillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePurchaseBill(int id, [FromBody] SavePurchaseBillRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await purchaseBillService.UpdatePurchaseBillAsync(id, request, cancellationToken);
        if (response is null)
        {
            return NotFound(new ErrorResponseDto { Message = "Purchase bill not found." });
        }

        return Ok(response);
    }
}
