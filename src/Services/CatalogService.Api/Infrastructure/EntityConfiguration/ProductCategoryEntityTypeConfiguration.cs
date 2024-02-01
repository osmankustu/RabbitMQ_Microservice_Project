using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Api.Core.Domain;
using ProductService.Api.Infrastructure.Context;

namespace ProductService.Api.Infrastructure.EntityConfiguration
{
    public class ProductCategoryEntityTypeConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            builder.ToTable("ProductCategory", ProductContext.DEFAULT_SCHEMA);

            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.Id)
                .UseHiLo("product_category_hilo")
                .IsRequired();

            builder.Property(pc => pc.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
