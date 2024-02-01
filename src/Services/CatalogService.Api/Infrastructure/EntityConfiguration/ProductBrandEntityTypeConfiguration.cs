using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;  
using ProductService.Api.Core.Domain;
using ProductService.Api.Infrastructure.Context;

namespace ProductService.Api.Infrastructure.EntityConfiguration
{
    public class ProductBrandEntityTypeConfiguration : IEntityTypeConfiguration<ProductBrand>
    {
        public void Configure(EntityTypeBuilder<ProductBrand> builder)
        {
            builder.ToTable("ProductBrand", ProductContext.DEFAULT_SCHEMA);

            builder.HasKey(pb => pb.Id);

            builder.Property(pb => pb.Id)
                .UseHiLo("product_brand_hilo")
                .IsRequired();

            builder.Property(pb => pb.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
