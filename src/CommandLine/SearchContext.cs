﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Threading;

namespace Orang.FileSystem
{
    internal class SearchContext
    {
        public SearchContext(
            SearchTelemetry telemetry = null,
            TextWriter output = null,
            in CancellationToken cancellationToken = default)
        {
            Telemetry = telemetry ?? new SearchTelemetry();
            Output = output;
            CancellationToken = cancellationToken;
        }

        public SearchTelemetry Telemetry { get; }

        public TextWriter Output { get; }

        public CancellationToken CancellationToken { get; }

        public SearchState State { get; set; }
    }
}
