using Enhanzer.Api.DTOs;
using Enhanzer.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Enhanzer.Api.Controllers;

[ApiController]
[Route("api/audit-logs")]
public class AuditLogsController(IAuditLogService auditLogService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuditLogs(CancellationToken cancellationToken)
    {
        var logs = await auditLogService.GetAuditLogsAsync(cancellationToken);
        return Ok(logs);
    }
}
