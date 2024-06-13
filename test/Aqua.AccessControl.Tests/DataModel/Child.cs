// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.DataModel;

public class Child : Entity
{
    public Parent Parent { get; set; }

    public Child Self { get; set; }
}