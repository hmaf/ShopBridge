using Microsoft.EntityFrameworkCore;
using ShopBridge.Domain.Entities;
using ShopBridge.Domain.Entities.Commons;
using ShopBridge.Infra.Persistence.Configurations;
using System.Linq.Expressions;

namespace ShopBridge.Infra.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products => Set<Product>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        BaseEntityConfiguration.Apply(modelBuilder);

        ApplySoftDeleteFilters(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var currentTime = DateTime.Now;

        foreach (var entry in ChangeTracker.Entries()
                     .Where(e => e.Entity is BaseEntity<Guid>))
        {
            var entity = (BaseEntity<Guid>)entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.SetCreated(currentTime, null);
                    break;

                case EntityState.Modified:
                    entity.SetUpdated(currentTime, null);
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entity.SetDeleted(currentTime, null);
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (IsBaseEntity(entityType.ClrType))
            {
                ApplySoftDeleteFilter(modelBuilder, entityType.ClrType);
            }
        }
    }

    private static bool IsBaseEntity(Type type)
    {
        while (type is not null)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(BaseEntity<>))
            {
                return true;
            }

            type = type.BaseType!;
        }

        return false;
    }

    private static void ApplySoftDeleteFilter(
        ModelBuilder modelBuilder,
        Type entityType)
    {
        var parameter = Expression.Parameter(entityType, "entity");

        var deletedDate = Expression.Property(
            parameter,
            nameof(BaseEntity<Guid>.DDate));

        var filter = Expression.Lambda(
            Expression.Equal(
                deletedDate,
                Expression.Constant(null, typeof(DateTime?))),
            parameter);

        modelBuilder.Entity(entityType)
            .HasQueryFilter(filter);
    }
}
