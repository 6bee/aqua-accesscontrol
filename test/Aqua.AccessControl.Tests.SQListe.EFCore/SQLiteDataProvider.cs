namespace Aqua.AccessControl.Tests.SQListe.EFCore
{
    using Aqua.AccessControl.Tests.DataModel;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using System.Linq;

    public class SQLiteDataProvider : DbContext, IDataProvider
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite("DataSource=sampledb.sqlite;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Conventions
            //    .Remove<PluralizingTableNameConvention>();

            //modelBuilder
            //    .Entity<Entity>()
            //    .Property(x => x.Id)
            //    .ValueGeneratedNever();

            var tenantModel = modelBuilder.Entity<Tenant>();
            //tenantModel.Property(x => x.Id).ValueGeneratedNever();

            var claimModel = modelBuilder.Entity<Claim>();
            //claimModel.Property(x => x.Id).ValueGeneratedNever();

            var productCategoryModel = modelBuilder.Entity<ProductCategory>();
            //productCategoryModel.Property(x => x.Id).ValueGeneratedNever();

            var productModel = modelBuilder.Entity<Product>();
            //productModel.Property(x => x.Id).ValueGeneratedNever();
            productModel
                .HasOne(x => x.ProductCategory)
                .WithMany()
                .HasForeignKey("ProductCategoryId");

            var orderModel = modelBuilder.Entity<Order>();
            //orderModel.Property(x => x.Id).ValueGeneratedNever();
            orderModel
                .HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.OrderId)
                .IsRequired(true);

            var orderItemModel = modelBuilder.Entity<OrderItem>();
            //orderItemModel.Property(x => x.Id).ValueGeneratedNever();

            var pkProperty = typeof(Entity).GetProperty(nameof(Entity.Id));
            var primaryKeys = (
                from type in modelBuilder.Model.GetEntityTypes()
                from property in type.GetProperties()
                where property.PropertyInfo?.Name == pkProperty.Name 
                   && property.PropertyInfo.DeclaringType == pkProperty.DeclaringType
                select property
                ).ToArray();
            foreach (var k in primaryKeys)
            {
                k.ValueGenerated = ValueGenerated.Never;
            }
        }

        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<Claim> Claims { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        IQueryable<Tenant> IDataProvider.Tenants => Tenants;
        IQueryable<Claim> IDataProvider.Claims => Claims;
        IQueryable<ProductCategory> IDataProvider.ProductCategories => ProductCategories;
        IQueryable<Product> IDataProvider.Products => Products;
        IQueryable<Order> IDataProvider.Orders => Orders;
    }
}
