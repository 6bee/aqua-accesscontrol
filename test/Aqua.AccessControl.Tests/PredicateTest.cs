// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Tests
{
    public abstract class PredicateTest : Disposable
    {
        protected abstract IDataProvider DataProvider { get; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                DataProvider.Dispose();
            }
        }
    }
}
