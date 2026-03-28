using Enhanzer.Api.Entities;

namespace Enhanzer.Api.Interfaces;

public interface IAuditLogRepository
{
    Task<List<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(AuditLog entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
