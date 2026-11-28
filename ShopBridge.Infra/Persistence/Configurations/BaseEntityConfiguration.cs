using Microsoft.EntityFrameworkCore;
using ShopBridge.Domain.Entities.Commons;

namespace ShopBridge.Infra.Persistence.Configurations;

public static class BaseEntityConfiguration
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!IsBaseEntity(entityType.ClrType))
                continue;

            var builder = modelBuilder.Entity(entityType.ClrType);

            builder.Property(nameof(BaseEntity<Guid>.CDate))
                .HasColumnType("timestamp without time zone");

            builder.Property(nameof(BaseEntity<Guid>.UDate))
                .HasColumnType("timestamp without time zone");

            builder.Property(nameof(BaseEntity<Guid>.DDate))
                .HasColumnType("timestamp without time zone");
        }
    }

    private static bool IsBaseEntity(Type type)
    {
        while (type is not null)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(BaseEntity<>))
                return true;

            type = type.BaseType!;
        }

        return false;
    }
}
