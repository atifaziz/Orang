﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orang.FileSystem
{
    internal enum ConflictResolution
    {
        Ask = 0,
        Overwrite = 1,
        Skip = 2,
        Rename = 3,
    }
}
