// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Tests.DataModel
{
    public class Product : AggregateRoot
    {
        public ProductCategory ProductCategory { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
