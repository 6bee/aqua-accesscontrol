namespace Aqua.AccessControl.Tests.SQLite.EF6
{
    public class When_applying_global_predicate : Tests.When_applying_global_predicate
    {
        protected override IDataProvider DataProvider { get; } = new SQLiteDataProvider();
    }
}
