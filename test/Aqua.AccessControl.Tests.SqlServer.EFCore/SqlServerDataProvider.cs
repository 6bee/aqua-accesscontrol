namespace Aqua.AccessControl.Tests.SqlServer.EFCore
{
    using Aqua.AccessControl.Tests.DataModel;
    using System;
    using System.Linq;

    public class SqlServerDataProvider : Disposable, IDataProvider
    {
        private readonly SqlServerDataContext _dataContext;

        public SqlServerDataProvider()
            : this(null, null)
        {
        }

        public SqlServerDataProvider(string username, string passeword)
        {
            var connectionString =
                  $"Server=.;Database=testdb-{Guid.NewGuid()};User Id={username ?? "sa"};Password = {passeword ?? "sa(!)Password"};";
            _dataContext = new SqlServerDataContext(connectionString);
            var created = _dataContext.Database.EnsureCreated();
            if (created)
            {
                new SqlServerDataSeeder().Seed(_dataContext);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !Disposed)
            {
                _dataContext.Database.EnsureDeleted();
            }
        }

        public IQueryable<Tenant> Tenants => _dataContext.Tenants;
        public IQueryable<Claim> Claims => _dataContext.Claims;
        public IQueryable<ProductCategory> ProductCategories => _dataContext.ProductCategories;
        public IQueryable<Product> Products => _dataContext.Products;
        public IQueryable<Order> Orders => _dataContext.Orders;
    }
}
