// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests;

using System;
using System.Text.RegularExpressions;

public static class Helper
{
    public static string GetCleanMessage(this Exception exception)
    {
        var message = exception?.Message.Replace("\r\n", "\n");
        if (!string.IsNullOrEmpty(message))
        {
            message = Regex.Replace(message, @"^(.*)\nParameter name: ([^\n]*)$", @"$1 (Parameter '$2')", RegexOptions.Multiline);
        }

        return message;
    }
}