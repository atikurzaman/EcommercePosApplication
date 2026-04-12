using System.Reflection;
using EcommercePos.Shared.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EcommercePos.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditableEntityInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyAuditInfo(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyAuditInfo(DbContext? context)
    {
        if (context == null) return;

        var userId = _currentUserService.UserId;
        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            var entity = entry.Entity;
            var entityType = entity.GetType();

            if (entry.State == EntityState.Added)
            {
                SetPropertyIfExists(entityType, entity, "CreatedAt", now);
                if (userId.HasValue)
                    SetPropertyIfExists(entityType, entity, "CreatedBy", userId.Value);
            }

            if (entry.State == EntityState.Modified)
            {
                SetPropertyIfExists(entityType, entity, "UpdatedAt", (DateTime?)now);
                if (userId.HasValue)
                    SetPropertyIfExists(entityType, entity, "UpdatedBy", userId.Value);
            }
        }
    }

    private static void SetPropertyIfExists(Type type, object entity, string propertyName, object? value)
    {
        var prop = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(entity, value);
        }
    }
}
