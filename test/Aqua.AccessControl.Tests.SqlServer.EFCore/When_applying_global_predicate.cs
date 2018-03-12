// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SqlServer.EFCore
{
    public class When_applying_global_predicate : Tests.When_applying_global_predicate
    {
        protected override IDataProvider DataProvider { get; } = new SqlServerDataProvider();
    }
}