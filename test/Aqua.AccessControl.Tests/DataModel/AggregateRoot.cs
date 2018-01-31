// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Tests.DataModel
{
    public abstract class AggregateRoot : Entity
    {
        public int TenantId { get; set; }
    }
}
