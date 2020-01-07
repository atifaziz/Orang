﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.CommandLine
{
    internal class FindCommandOptions : CommonFindCommandOptions
    {
        internal FindCommandOptions()
        {
        }

        public AskMode AskMode { get; internal set; }

        public int MaxMatches { get; internal set; }

        public int MaxMatchesInFile { get; internal set; }
    }
}
