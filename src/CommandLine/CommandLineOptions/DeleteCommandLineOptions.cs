﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using CommandLine;
using static Orang.CommandLine.ParseHelpers;

namespace Orang.CommandLine
{
    [Verb("delete", HelpText = "Deletes files and directories.")]
    internal class DeleteCommandLineOptions : CommonFindCommandLineOptions
    {
        [Option(longName: OptionNames.Ask,
            HelpText = "Ask for a permission to delete file or directory.")]
        public bool Ask { get; set; }

        [Option(shortName: OptionShortNames.Content, longName: OptionNames.Content,
            HelpText = "Regular expression for files' content. Syntax is <PATTERN> [<PATTERN_OPTIONS>].",
            MetaValue = MetaValues.Regex)]
        [OptionValueProvider(OptionValueProviderNames.PatternOptionsWithoutPart)]
        public IEnumerable<string> Content { get; set; }

        [Option(longName: OptionNames.ContentOnly,
            HelpText = "Delete content of a file or directory but not the file or directory itself.")]
        public bool ContentOnly { get; set; }

        [Option(shortName: OptionShortNames.DryRun, longName: OptionNames.DryRun,
            HelpText = "Display which files or directories should be deleted but do not actually delete any file or directory.")]
        public bool DryRun { get; set; }

        [Option(shortName: OptionShortNames.Highlight, longName: OptionNames.Highlight,
            HelpText = "Parts of the output to highlight.",
            MetaValue = MetaValues.Highlight)]
        [OptionValueProvider(OptionValueProviderNames.DeleteHighlightOptions)]
        public IEnumerable<string> Highlight { get; set; }

        [Option(longName: OptionNames.IncludingBom,
            HelpText = "Delete byte order mark (BOM) when deleting file's content.")]
        public bool IncludingBom { get; set; }

        [Option(longName: OptionNames.MaxCount,
            HelpText = "Stop renaming after specified number is reached.",
            MetaValue = MetaValues.Number)]
        public int MaxCount { get; set; }

        [Option(shortName: OptionShortNames.Name, longName: OptionNames.Name,
            Required = true,
            HelpText = "Regular expression for file or directory name. Syntax is <PATTERN> [<PATTERN_OPTIONS>].",
            MetaValue = MetaValues.Regex)]
        public IEnumerable<string> Name { get; set; }

        [Option(shortName: OptionShortNames.Output, longName: OptionNames.Output,
            HelpText = "Path to a file that should store results.",
            MetaValue = MetaValues.Path)]
        public string Output { get; set; }

        public bool TryParse(ref DeleteCommandOptions options)
        {
            var baseOptions = (CommonFindCommandOptions)options;

            if (!TryParse(ref baseOptions))
                return false;

            options = (DeleteCommandOptions)baseOptions;

            if (!TryParseAsEnumFlags(Highlight, OptionNames.Highlight, out HighlightOptions highlightOptions, defaultValue: HighlightOptions.Match, provider: OptionValueProviders.DeleteHighlightOptionsProvider))
                return false;

            if (!TryParseFilter(Name, OptionNames.Name, out Filter nameFilter))
                return false;

            Filter contentFilter = null;

            if (Content.Any()
                && !TryParseFilter(Content, OptionNames.Content, out contentFilter))
            {
                return false;
            }

            string outputPath = null;

            if (Output != null
                && !TryEnsureFullPath(Output, out outputPath))
            {
                return false;
            }

            if (!TryParseDisplay(
                values: Display,
                optionName: OptionNames.Display,
                contentDisplayStyle: out ContentDisplayStyle _,
                pathDisplayStyle: out PathDisplayStyle pathDisplayStyle,
                defaultContentDisplayStyle: 0,
                defaultPathDisplayStyle: PathDisplayStyle.Relative,
                contentDisplayStyleProvider: OptionValueProviders.ContentDisplayStyleProvider,
                pathDisplayStyleProvider: OptionValueProviders.PathDisplayStyleProvider_WithoutOmit))
            {
                return false;
            }

            options.Format = new OutputDisplayFormat(ContentDisplayStyle.None, pathDisplayStyle);
            options.Ask = Ask;
            options.DryRun = DryRun;
            options.HighlightOptions = highlightOptions;
            options.SearchTarget = GetSearchTarget();
            options.NameFilter = nameFilter;
            options.ContentFilter = contentFilter;
            options.ContentOnly = ContentOnly;
            options.IncludingBom = IncludingBom;
            options.MaxMatchingFiles = MaxCount;
            options.OutputPath = outputPath;

            return true;
        }
    }
}