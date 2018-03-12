// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.DataModel
{
    public static class ClaimTypes
    {
        private const string Base = "https://github.com/6bee/aqua-accesscontrol/2018-01";

        public const string Tenant = Base + "/tenant";

        public static class EntityAccess
        {
            private const string Base = ClaimTypes.Base + "/entityaccess";

            public const string Create = Base + "/create";
            public const string Read = Base + "/read";
            public const string Update = Base + "/update";
            public const string Delete = Base + "/delete";
        }

        public static class PropertyAccess
        {
            private const string Base = ClaimTypes.Base + "/propertyaccess";

            public const string Read = Base + "/read";
            public const string Write = Base + "/write";
        }

        public static class Operation
        {
            private const string Base = ClaimTypes.Base + "/operation";

            public const string Read = Base + "/read";
            public const string Write = Base + "/write";
            public const string Exeute = Base + "/execute";
        }
    }
}
