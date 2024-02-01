using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ProductService.Api.Core.Domain;
using ProductService.Api.Infrastructure.EntityConfiguration;

namespace ProductService.Api.Infrastructure.Context
{
    public class ProductContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "product";

        public ProductContext(DbContextOptions<ProductContext> opts) :base(opts)
        {
            
        }

        public DbSet<ProductBrand> ProductBrands { get; set; }

        public DbSet<ProductCategory>  ProductCategories{ get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductBrandEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
            
        }

        

    }
}
