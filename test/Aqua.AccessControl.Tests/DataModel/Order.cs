// Copyright (c) Christof Senn. All rights reserved. 

using System.Collections.Generic;

namespace Aqua.AccessControl.Tests.DataModel
{
    public class Order : AggregateRoot
    {
        public ICollection<OrderItem> Items { set; get; }
    }
}
