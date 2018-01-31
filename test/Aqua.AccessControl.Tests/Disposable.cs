// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Tests
{
    using System;

    public abstract class Disposable : IDisposable
    {
        protected bool Disposed { get; private set; }

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            if (Disposed) return;
            Dispose(true);
            Disposed = true;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
