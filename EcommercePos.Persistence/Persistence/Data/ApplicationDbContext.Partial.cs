using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using EcommercePos.Domain.Entities;

namespace EcommercePos.Persistence.Data;

public partial class ApplicationDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        ApplyGlobalSoftDeleteFilter(modelBuilder);
    }

    private static void ApplyGlobalSoftDeleteFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var isDeletedProperty = entityType.FindProperty("IsDeleted");
            if (isDeletedProperty != null && isDeletedProperty.ClrType == typeof(bool))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, "IsDeleted");
                var falseConstant = Expression.Constant(false);
                var filter = Expression.Lambda(Expression.Equal(property, falseConstant), parameter);
                entityType.SetQueryFilter(filter);
            }
        }
    }
}
