﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using CommandLine;
using static Orang.CommandLine.ParseHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    [OptionValueProvider(nameof(Display), OptionValueProviderNames.Display_MatchAndSplit)]
    internal abstract class RegexCommandLineOptions : CommonRegexCommandLineOptions
    {
        [Value(index: 0,
            HelpText = "Path to a file that should be analyzed.",
            MetaName = ArgumentMetaNames.Path)]
        public string Path { get; set; }

        [Option(longName: OptionNames.Input,
            HelpText = "Text to search.",
            MetaValue = MetaValues.Input)]
        public string Input { get; set; }

        [Option(longName: OptionNames.Modify,
            HelpText = "Functions to modify results.",
            MetaValue = MetaValues.ModifyOptions)]
        public IEnumerable<string> Modify { get; set; }

        public bool TryParse(RegexCommandOptions options)
        {
            var baseOptions = (CommonRegexCommandOptions)options;

            if (!TryParse(baseOptions))
                return false;

            options = (RegexCommandOptions)baseOptions;

            string input = Input;

            if (!string.IsNullOrEmpty(Path))
            {
                if (input != null)
                {
                    WriteError($"Option '{OptionNames.GetHelpText(OptionNames.Input)}' and argument '{ArgumentMetaNames.Path}' cannot be set both at the same time.");
                    return false;
                }

                try
                {
                    input = File.ReadAllText(Path);
                }
                catch (Exception ex) when (ex is ArgumentException
                    || ex is IOException
                    || ex is UnauthorizedAccessException)
                {
                    WriteError(ex);
                    return false;
                }
            }
            else if (string.IsNullOrEmpty(input))
            {
                input = ConsoleHelpers.ReadRedirectedInput();

                if (input == null)
                {
                    WriteError("Input is missing.");
                    return false;
                }
            }

            if (!TryParseDisplay(
                values: Display,
                optionName: OptionNames.Display,
                contentDisplayStyle: out ContentDisplayStyle? contentDisplayStyle,
                pathDisplayStyle: out PathDisplayStyle? _,
                lineDisplayOptions: out LineDisplayOptions lineDisplayOptions,
                displayParts: out DisplayParts displayParts,
                fileProperties: out ImmutableArray<FileProperty> fileProperties,
                indent: out string indent,
                separator: out string separator,
                contentDisplayStyleProvider: OptionValueProviders.ContentDisplayStyleProvider_WithoutLineAndUnmatchedLinesAndOmit,
                pathDisplayStyleProvider: OptionValueProviders.PathDisplayStyleProvider))
            {
                return false;
            }

            if (!TryParseModifyOptions(Modify, OptionNames.Modify, out ModifyOptions modifyOptions, out bool aggregateOnly))
                return false;

            if (modifyOptions.HasAnyFunction
                && contentDisplayStyle == ContentDisplayStyle.ValueDetail)
            {
                contentDisplayStyle = ContentDisplayStyle.Value;
            }

            if (aggregateOnly)
                ConsoleOut.Verbosity = Orang.Verbosity.Minimal;

            options.Format = new OutputDisplayFormat(
                contentDisplayStyle: contentDisplayStyle ?? ContentDisplayStyle.Value,
                pathDisplayStyle: PathDisplayStyle.Full,
                lineOptions: lineDisplayOptions,
                displayParts: displayParts,
                fileProperties: fileProperties,
                indent: indent,
                separator: separator ?? Environment.NewLine);

            options.ModifyOptions = modifyOptions;
            options.Input = input;

            return true;
        }
    }
}
