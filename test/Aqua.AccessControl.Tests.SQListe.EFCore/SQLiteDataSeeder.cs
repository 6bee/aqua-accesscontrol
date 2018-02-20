namespace Aqua.AccessControl.Tests.SQListe.EFCore
{
    public class SQLiteDataSeeder
    {
        public void Seed(SQLiteDataProvider context)
        {
            using (var source = new InMemoryDataProvider())
            {
                context.Tenants.AddRange(source.Tenants);
                context.SaveChanges();
                context.Claims.AddRange(source.Claims);
                context.SaveChanges();
                context.ProductCategories.AddRange(source.ProductCategories);
                context.SaveChanges();
                context.Products.AddRange(source.Products);
                context.SaveChanges();
                context.Orders.AddRange(source.Orders);

                context.SaveChanges();
            }
        }
    }
}
