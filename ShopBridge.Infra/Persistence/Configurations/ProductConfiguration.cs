

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Infra.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.Price)
                .HasPrecision(18, 2);

            builder.Property(o => o.Stock)
                .IsRequired();
        }
    }
}
