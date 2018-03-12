// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests
{
    public static class Helper
    {
        public static string Clean(this string text)
            => text?.Replace("\r\n", "\n");
    }
}
