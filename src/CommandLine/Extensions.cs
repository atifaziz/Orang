﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Orang.FileSystem;

namespace Orang
{
    internal static class Extensions
    {
        public static void WritePath(
            this TextWriterWithVerbosity writer,
            string path,
            string basePath,
            bool relativePath = false,
            string indent = null,
            Verbosity verbosity = Verbosity.Quiet)
        {
            if (!writer.ShouldWrite(verbosity))
                return;

            writer.Write(indent, verbosity);

            int startIndex = 0;

            if (string.Equals(path, basePath, StringComparison.OrdinalIgnoreCase))
            {
                writer.Write((relativePath) ? "." : path, verbosity);
                return;
            }

            if (basePath != null
                && path.Length > basePath.Length
                && path.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                startIndex = basePath.Length;

                if (FileSystemHelpers.IsDirectorySeparator(path[startIndex]))
                    startIndex++;
            }

            if (!relativePath)
                writer.Write(path, 0, startIndex, Colors.BasePath, verbosity);

            writer.Write(path, startIndex, path.Length - startIndex, verbosity);
        }

        public static void WriteLineIf(this TextWriter writer, bool condition, string value)
        {
            if (condition)
                writer.WriteLine(value);
        }

        public static IEnumerable<RegexOptions> GetFlags(this RegexOptions options)
        {
            return GetFlags();

            IEnumerable<RegexOptions> GetFlags()
            {
                if ((options & RegexOptions.IgnoreCase) != 0)
                    yield return RegexOptions.IgnoreCase;

                if ((options & RegexOptions.Multiline) != 0)
                    yield return RegexOptions.Multiline;

                if ((options & RegexOptions.ExplicitCapture) != 0)
                    yield return RegexOptions.ExplicitCapture;

                if ((options & RegexOptions.Compiled) != 0)
                    yield return RegexOptions.Compiled;

                if ((options & RegexOptions.Singleline) != 0)
                    yield return RegexOptions.Singleline;

                if ((options & RegexOptions.IgnorePatternWhitespace) != 0)
                    yield return RegexOptions.IgnorePatternWhitespace;

                if ((options & RegexOptions.RightToLeft) != 0)
                    yield return RegexOptions.RightToLeft;

                if ((options & RegexOptions.ECMAScript) != 0)
                    yield return RegexOptions.ECMAScript;

                if ((options & RegexOptions.CultureInvariant) != 0)
                    yield return RegexOptions.CultureInvariant;
            }
        }

        public static int GetDigitCount(this int value)
        {
            if (value < 0)
                value = -value;

            if (value < 10)
                return 1;

            if (value < 100)
                return 2;

            if (value < 1000)
                return 3;

            if (value < 10000)
                return 4;

            if (value < 100000)
                return 5;

            if (value < 1000000)
                return 6;

            if (value < 10000000)
                return 7;

            if (value < 100000000)
                return 8;

            if (value < 1000000000)
                return 9;

            return 10;
        }

        public static OperationCanceledException GetOperationCanceledException(this AggregateException aggregateException)
        {
            OperationCanceledException operationCanceledException = null;

            foreach (Exception ex in aggregateException.InnerExceptions)
            {
                if (ex is OperationCanceledException operationCanceledException2)
                {
                    if (operationCanceledException == null)
                        operationCanceledException = operationCanceledException2;
                }
                else if (ex is AggregateException aggregateException2)
                {
                    foreach (Exception ex2 in aggregateException2.InnerExceptions)
                    {
                        if (ex2 is OperationCanceledException operationCanceledException3)
                        {
                            if (operationCanceledException == null)
                                operationCanceledException = operationCanceledException3;
                        }
                        else
                        {
                            return null;
                        }
                    }

                    return operationCanceledException;
                }
                else
                {
                    return null;
                }
            }

            return null;
        }
    }
}
