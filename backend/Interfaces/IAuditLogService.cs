using Enhanzer.Api.DTOs;

namespace Enhanzer.Api.Interfaces;

public interface IAuditLogService
{
    Task<IReadOnlyList<AuditLogDto>> GetAuditLogsAsync(CancellationToken cancellationToken = default);
}
