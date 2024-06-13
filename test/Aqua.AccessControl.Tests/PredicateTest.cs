// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests;

using System;

public abstract class PredicateTest : IDisposable
{
    protected abstract IDataProvider DataProvider { get; }

    public void Dispose() => DataProvider.Dispose();
}
