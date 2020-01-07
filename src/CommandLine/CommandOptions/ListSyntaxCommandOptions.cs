﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Orang.Syntax;

namespace Orang.CommandLine
{
    internal sealed class ListSyntaxCommandOptions : CommonListCommandOptions
    {
        internal ListSyntaxCommandOptions()
        {
        }

        public ImmutableArray<SyntaxSection> Sections { get; internal set; }
    }
}
