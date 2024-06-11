// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.DataModel;

public class Claim : AggregateRoot
{
    public string Type { get; set; }

    public string Value { get; set; }

    public string Subject { get; set; }
}
