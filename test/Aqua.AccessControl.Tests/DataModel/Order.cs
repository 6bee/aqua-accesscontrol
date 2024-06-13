// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.DataModel;

using System.Collections.Generic;

public class Order : AggregateRoot
{
    public ICollection<OrderItem> Items { get; set; }
}