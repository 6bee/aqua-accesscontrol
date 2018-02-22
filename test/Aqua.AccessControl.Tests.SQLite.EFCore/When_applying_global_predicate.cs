namespace Aqua.AccessControl.Tests.SQLite.EFCore
{
    public class When_applying_global_predicate : Tests.When_applying_global_predicate
    {
        protected override IDataProvider DataProvider { get; } = new SQLiteDataProvider();
    }
}
