﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;
using Orang.FileSystem;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal class FindContentCommand : CommonFindContentCommand<FindCommandOptions>
    {
        private AskMode _askMode;
        private IResultStorage _storage;
        private OutputSymbols _symbols;

        public FindContentCommand(FindCommandOptions options) : base(options)
        {
        }

        private OutputSymbols Symbols => _symbols ?? (_symbols = OutputSymbols.Create(Options.HighlightOptions));

        protected override void ExecuteCore(SearchContext context)
        {
            context.Telemetry.MatchingLineCount = -1;

            if (ConsoleOut.Verbosity >= Verbosity.Minimal)
                _askMode = Options.AskMode;

            if (Options.OutputPath != null
                && Options.Output.IncludeContent)
            {
                _storage = new TextWriterResultStorage(context.Output);
            }

            base.ExecuteCore(context);
        }

        protected override void ExecuteFile(string filePath, SearchContext context)
        {
            context.Telemetry.FileCount++;

            string input = ReadFile(filePath, null, Options.DefaultEncoding);

            if (input != null)
            {
                Match match = Options.ContentFilter.Regex.Match(input);

                if (Options.ContentFilter.IsMatch(match))
                {
                    if (!Options.OmitPath)
                        WritePath(filePath, colors: Colors.Matched_Path, verbosity: Verbosity.Minimal);

                    WriteMatches(input, match, FileWriterOptions, context);

                    if (Options.MaxMatchingFiles == context.Telemetry.MatchingFileCount)
                        context.State = SearchState.MaxReached;

                    context.Output?.WriteLineIf(Options.Output.IncludePath, filePath);
                }
            }
        }

        protected override void ExecuteDirectory(string directoryPath, SearchContext context, FileSystemFinderProgressReporter progress)
        {
            Regex regex = Options.ContentFilter.Regex;
            string basePath = (Options.PathDisplayStyle == PathDisplayStyle.Full) ? null : directoryPath;
            string indent = (Options.PathDisplayStyle == PathDisplayStyle.Relative) ? Options.Indent : "";

            foreach (FileSystemFinderResult result in Find(directoryPath, progress, context.CancellationToken))
            {
                string input = ReadFile(result.Path, basePath, Options.DefaultEncoding, progress, indent);

                if (input == null)
                    continue;

                Match match = regex.Match(input);

                if (Options.ContentFilter.IsMatch(match))
                {
                    EndProgress(progress);

                    if (!Options.OmitPath)
                        WritePath(result, basePath, colors: Colors.Matched_Path, matchColors: (Options.HighlightMatch) ? Colors.Match_Path : default, indent: indent, verbosity: Verbosity.Minimal);

                    context.Output?.WriteLineIf(Options.Output.IncludePath, result.Path);

                    if (Options.ContentFilter.IsNegative)
                    {
                        context.Telemetry.MatchingFileCount++;

                        WriteLineIf(!Options.OmitPath, Verbosity.Minimal);
                    }
                    else
                    {
                        WriteMatches(input, match, DirectoryWriterOptions, context);

                        if (context.State == SearchState.Canceled)
                            break;
                    }

                    if (Options.MaxMatchingFiles == context.Telemetry.MatchingFileCount)
                        context.State = SearchState.MaxReached;

                    if (context.State == SearchState.MaxReached)
                        break;

                    if (_askMode == AskMode.File)
                    {
                        try
                        {
                            if (ConsoleHelpers.Question("Continue without asking?", indent))
                                _askMode = AskMode.None;
                        }
                        catch (OperationCanceledException)
                        {
                            context.State = SearchState.Canceled;
                            break;
                        }
                    }
                }
            }

            context.Telemetry.SearchedDirectoryCount = progress.SearchedDirectoryCount;
            context.Telemetry.DirectoryCount = progress.DirectoryCount;
            context.Telemetry.FileCount = progress.FileCount;
        }

        private void WriteMatches(
            string input,
            Match match,
            MatchWriterOptions writerOptions,
            SearchContext context)
        {
            SearchTelemetry telemetry = context.Telemetry;

            telemetry.MatchingFileCount++;

            int fileMatchCount = 0;
            var maxReason = MaxReason.None;

            if (_storage != null
                || Options.AskMode == AskMode.Value
                || ShouldLog(Verbosity.Normal))
            {
                if (!Options.OmitPath)
                {
                    if (Options.AskMode == AskMode.Value)
                    {
                        ConsoleOut.WriteLine();
                    }
                    else if (ConsoleOut.Verbosity >= Verbosity.Normal)
                    {
                        ConsoleOut.WriteLine(Verbosity.Normal);
                    }

                    if (Out?.Verbosity >= Verbosity.Normal)
                        Out.WriteLine(Verbosity.Normal);
                }

                MatchOutputInfo outputInfo = Options.CreateOutputInfo(input, match);

                using (MatchWriter matchWriter = MatchWriter.CreateFind(Options.ContentDisplayStyle, input, writerOptions, _storage, outputInfo, ask: _askMode == AskMode.Value))
                {
                    maxReason = WriteMatches(matchWriter, match, context);
                    fileMatchCount += matchWriter.MatchCount;

                    if (matchWriter.MatchingLineCount >= 0)
                    {
                        if (telemetry.MatchingLineCount == -1)
                            telemetry.MatchingLineCount = 0;

                        telemetry.MatchingLineCount += matchWriter.MatchingLineCount;
                    }

                    if (_askMode == AskMode.Value)
                    {
                        if (matchWriter is AskValueMatchWriter askValueMatchWriter)
                        {
                            if (!askValueMatchWriter.Ask)
                                _askMode = AskMode.None;
                        }
                        else if (matchWriter is AskLineMatchWriter askLineMatchWriter)
                        {
                            if (!askLineMatchWriter.Ask)
                                _askMode = AskMode.None;
                        }
                    }
                }
            }
            else
            {
                fileMatchCount = EnumerateValues();
            }

            if (Options.AskMode != AskMode.Value
                && !Options.OmitPath)
            {
                if (ConsoleOut.Verbosity == Verbosity.Minimal)
                {
                    ConsoleOut.Write($" {fileMatchCount.ToString("n0")}", Colors.Message_OK);
                    ConsoleOut.WriteIf(maxReason == MaxReason.CountExceedsMax, "+", Colors.Message_OK);
                    ConsoleOut.WriteLine();
                }

                if (Out?.Verbosity == Verbosity.Minimal)
                {
                    Out.Write($" {fileMatchCount.ToString("n0")}");
                    Out.WriteIf(maxReason == MaxReason.CountExceedsMax, "+");
                    Out.WriteLine();
                }
            }

            telemetry.MatchCount += fileMatchCount;

            int EnumerateValues()
            {
                using (var matchWriter = new EmptyMatchWriter(null, writerOptions))
                {
                    maxReason = WriteMatches(matchWriter, match, context);

                    if (matchWriter.MatchingLineCount >= 0)
                    {
                        if (telemetry.MatchingLineCount == -1)
                            telemetry.MatchingLineCount = 0;

                        telemetry.MatchingLineCount += matchWriter.MatchingLineCount;
                    }

                    return matchWriter.MatchCount;
                }
            }
        }

        protected override void WriteSummary(SearchTelemetry telemetry)
        {
            WriteSearchedFilesAndDirectories(telemetry, Options.SearchTarget);

            if (!ShouldLog(Verbosity.Minimal))
                return;

            WriteLine(Verbosity.Minimal);
            WriteCount("Matches", telemetry.MatchCount, Colors.Message_OK, Verbosity.Minimal);
            Write("  ", Colors.Message_OK, Verbosity.Minimal);

            if (telemetry.MatchingLineCount > 0)
            {
                WriteCount("Matching lines", telemetry.MatchingLineCount, Colors.Message_OK, Verbosity.Minimal);
                Write("  ", Colors.Message_OK, Verbosity.Minimal);
            }

            if (telemetry.MatchingFileCount > 0)
                WriteCount("Matching files", telemetry.MatchingFileCount, Colors.Message_OK, Verbosity.Minimal);

            WriteLine(Verbosity.Minimal);
        }

        protected override MatchWriterOptions CreateMatchWriteOptions(string indent)
        {
            int groupNumber = Options.ContentFilter.GroupNumber;

            GroupDefinition? groupDefinition = (groupNumber >= 0)
                ? new GroupDefinition(groupNumber, Options.ContentFilter.GroupName)
                : default(GroupDefinition?);

            return new MatchWriterOptions(
                format: Options.Format,
                groupDefinition,
                symbols: Symbols,
                highlightOptions: Options.HighlightOptions,
                indent: indent);
        }
    }
}
