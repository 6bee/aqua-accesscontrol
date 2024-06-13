// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.DataModel;

public abstract class AggregateRoot : Entity
{
    public int TenantId { get; set; }
}