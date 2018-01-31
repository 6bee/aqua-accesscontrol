namespace Aqua.AccessControl
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        public static readonly IEqualityComparer<T> Instance = new ReferenceEqualityComparer<T>();

        private ReferenceEqualityComparer()
        {
        }

        public bool Equals(T x, T y) => ReferenceEquals(x, y);

        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }
}