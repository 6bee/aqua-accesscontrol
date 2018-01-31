namespace Aqua.AccessControl.Tests.SQLite.EF6
{
    public class When_applying_type_predicate : Tests.When_applying_type_predicate
    {
        protected override IDataProvider DataProvider { get; } = new SQLiteDataProvider();
    }
}
