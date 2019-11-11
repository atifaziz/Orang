﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using Orang.FileSystem;
using static Orang.CommandLine.LogHelpers;
using static Orang.Logger;

namespace Orang.CommandLine
{
    internal class DeleteCommand : CommonFindCommand<DeleteCommandOptions>, INotifyDirectoryChanged
    {
        public DeleteCommand(DeleteCommandOptions options) : base(options)
        {
        }

        public event EventHandler<DirectoryChangedEventArgs> DirectoryChanged;

        protected override void ExecuteFile(string filePath, SearchContext context)
        {
            SearchTelemetry telemetry = context.Telemetry;
            telemetry.FileCount++;

            const string indent = null;

            FileSystemFinderResult? maybeResult = MatchFile(filePath);

            if (maybeResult == null)
                return;

            FileSystemFinderResult result = maybeResult.Value;

            if (Options.ContentFilter != null)
            {
                string input = ReadFile(filePath, null, Options.DefaultEncoding, null, indent);

                if (input == null)
                    return;

                if (!Options.ContentFilter.IsMatch(input))
                    return;
            }

            telemetry.MatchingFileCount++;

            WritePath(result, null, colors: Colors.Matched_Path, matchColors: (Options.HighlightMatch) ? Colors.Match : default, indent);
            WriteLine();

            bool success = false;

            if (!Options.DryRun
                && (!Options.Ask || AskToDelete(context, indent)))
            {
                try
                {
                    FileSystemHelpers.DeleteFile(
                        filePath,
                        contentOnly: Options.ContentOnly,
                        includingBom: Options.IncludingBom);

                    telemetry.ProcessedFileCount++;
                    success = true;
                }
                catch (Exception ex) when (ex is IOException
                    || ex is UnauthorizedAccessException)
                {
                    WriteFileError(ex, indent: indent);
                }
            }

            if (Options.DryRun
                || success)
            {
                context.Output?.WriteLine(filePath);
            }

            if (Options.MaxMatchingFiles == telemetry.MatchingFileCount + telemetry.MatchingDirectoryCount)
            {
                context.State = SearchState.MaxReached;
            }
        }

        protected override void ExecuteDirectory(string directoryPath, SearchContext context, FileSystemFinderProgressReporter progress)
        {
            SearchTelemetry telemetry = context.Telemetry;
            string basePath = (Options.PathDisplayStyle == PathDisplayStyle.Full) ? null : directoryPath;
            string indent = (Options.PathDisplayStyle == PathDisplayStyle.Relative) ? Options.Indent : "";

            foreach (FileSystemFinderResult result in Find(directoryPath, progress, notifyDirectoryChanged: this, context.CancellationToken))
            {
                string path = result.Path;

                Debug.Assert(path.StartsWith(directoryPath, StringComparison.OrdinalIgnoreCase), $"{directoryPath}\r\n{path}");

                if (result.IsDirectory)
                {
                    telemetry.MatchingDirectoryCount++;
                }
                else
                {
                    if (Options.ContentFilter != null)
                    {
                        string input = ReadFile(path, basePath, Options.DefaultEncoding, progress, indent);

                        if (input == null)
                            continue;

                        if (!Options.ContentFilter.IsMatch(input))
                            continue;
                    }

                    telemetry.MatchingFileCount++;
                }

                EndProgress(progress);

                WritePath(result, basePath, colors: Colors.Matched_Path, matchColors: (Options.HighlightMatch) ? Colors.Match : default, indent);
                WriteLine();

                bool success = false;

                if (!Options.DryRun
                    && (!Options.Ask || AskToDelete(context, indent)))
                {
                    try
                    {
                        FileSystemHelpers.Delete(
                            result,
                            contentOnly: Options.ContentOnly,
                            includingBom: Options.IncludingBom,
                            filesOnly: Options.FilesOnly,
                            directoriesOnly: Options.DirectoriesOnly);

                        if (result.IsDirectory)
                        {
                            telemetry.ProcessedDirectoryCount++;
                        }
                        else
                        {
                            telemetry.ProcessedFileCount++;
                        }

                        success = true;
                    }
                    catch (Exception ex) when (ex is IOException
                        || ex is UnauthorizedAccessException)
                    {
                        WriteFileError(ex, indent: indent);
                    }
                }

                if (Options.DryRun
                    || success)
                {
                    context.Output?.WriteLine(path);
                }

                if (result.IsDirectory
                    && success)
                {
                    OnDirectoryChanged(new DirectoryChangedEventArgs(path, null));
                }

                if (context.State == SearchState.Canceled)
                    break;

                if (Options.MaxMatchingFiles == telemetry.MatchingFileCount + telemetry.MatchingDirectoryCount)
                {
                    context.State = SearchState.MaxReached;
                    break;
                }
            }

            telemetry.SearchedDirectoryCount = progress.SearchedDirectoryCount;
            telemetry.FileCount = progress.FileCount;
            telemetry.DirectoryCount = progress.DirectoryCount;
        }

        private bool AskToDelete(SearchContext context, string indent = null)
        {
            try
            {
                return ConsoleHelpers.Question(
                    (Options.ContentOnly) ? "Delete content?" : "Delete?",
                    indent);
            }
            catch (OperationCanceledException)
            {
                context.State = SearchState.Canceled;
                return false;
            }
        }

        protected virtual void OnDirectoryChanged(DirectoryChangedEventArgs e)
        {
            DirectoryChanged?.Invoke(this, e);
        }

        protected override void WriteSummary(SearchTelemetry telemetry)
        {
            WriteSearchedFilesAndDirectories(telemetry, Options.SearchTarget);

            string filesTitle = (Options.ContentOnly)
                ? "Deleted files content"
                : "Deleted files";

            string directoriesTitle = (Options.ContentOnly)
                ? "Deleted directories content"
                : "Deleted directories";

            WriteProcessedFilesAndDirectories(telemetry, Options.SearchTarget, filesTitle, directoriesTitle);
        }
    }
}
