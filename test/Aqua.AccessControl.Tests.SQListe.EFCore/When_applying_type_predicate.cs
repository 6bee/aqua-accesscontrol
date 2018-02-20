namespace Aqua.AccessControl.Tests.SQListe.EFCore
{
    public class When_applying_type_predicate : Tests.When_applying_type_predicate
    {
        private readonly SQLiteDataProvider _dataProvider;
        protected override IDataProvider DataProvider => _dataProvider;

        public When_applying_type_predicate()
        {
            _dataProvider = new SQLiteDataProvider();
            var created = _dataProvider.Database.EnsureCreated();
            if (created)
            {
                new SQLiteDataSeeder().Seed(_dataProvider);
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && !Disposed)
        //    {
        //        _dataProvider.Database.EnsureDeleted();
        //    }

        //    base.Dispose(disposing);
        //}
    }
}
