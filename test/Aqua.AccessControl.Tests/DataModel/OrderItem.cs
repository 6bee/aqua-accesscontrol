// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.DataModel;

public class OrderItem : Entity
{
    public long OrderId { get; set; }

    public long ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}