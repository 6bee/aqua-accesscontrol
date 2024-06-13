// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.DataModel;

public static class ClaimTypes
{
    private static readonly string Base = "https://github.com/6bee/aqua-accesscontrol/2018-01";

    public static readonly string Tenant = Base + "/tenant";

    public static class EntityAccess
    {
        private static readonly string Base = ClaimTypes.Base + "/entityaccess";

        public static readonly string Create = Base + "/create";
        public static readonly string Read = Base + "/read";
        public static readonly string Update = Base + "/update";
        public static readonly string Delete = Base + "/delete";
    }

    public static class PropertyAccess
    {
        private static readonly string Base = ClaimTypes.Base + "/propertyaccess";

        public static readonly string Read = Base + "/read";
        public static readonly string Write = Base + "/write";
    }

    public static class Operation
    {
        private static readonly string Base = ClaimTypes.Base + "/operation";

        public static readonly string Read = Base + "/read";
        public static readonly string Write = Base + "/write";
        public static readonly string Exeute = Base + "/execute";
    }
}