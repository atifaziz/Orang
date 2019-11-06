﻿# `orang replace`

Searches the file system for files and replaces its content\.

## Synopsis

```
orang replace [<PATH>]
[   --ask]                <ASK_MODE>
[-a|--attributes]         <ATTRIBUTES>
[   --attributes-to-skip] <ATTRIBUTES>
 -c|--content             <REGEX>
[-y|--display]            <DISPLAY_OPTIONS>
[-d|--dry-run]
[   --encoding]           <ENCODING>
[   --evaluator]          <EVALUATOR>
[-e|--extension]          <EXTENSION_FILTER>
[-h|--highlight]          <HIGHLIGHT>
[-i|--include-directory]  <REGEX>
[   --input]              <INPUT>
[-l|--line-number]
[   --max-count]          <MAX_OPTIONS>
[-n|--name]               <REGEX>
[   --no-recurse]
[   --progress]
[-r|--replacement]        <REPLACEMENT>
[-v|--verbosity]          <VERBOSITY>
[   --file-log]           <FILE_LOG>
```

## Arguments

**`<PATH>`**

Path to one or more files and/or directories that should be searched\.

## Options

**`[--ask] <ASK_MODE>`**

Ask for a permission to update each file or value\. Allowed values are \[f\]ile and \[v\]alue\.

**`[-a|--attributes] <ATTRIBUTES>`**

File attributes that are required\. Allowed values are archive, compressed, \[d\]irectory, \[e\]mpty, encrypted, \[f\]ile, \[h\]idden, normal, offline, \[r\]ead\-only, \[s\]ystem and temporary\.

**`[--attributes-to-skip] <ATTRIBUTES>`**

File attributes that should be skipped\. Allowed values are archive, compressed, \[e\]mpty, encrypted, \[h\]idden, normal, offline, \[r\]ead\-only, \[s\]ystem and temporary\.

**`-c|--content <REGEX>`**

Regular expression for files' content\. Syntax is \<PATTERN> \[\<PATTERN\_OPTIONS>\]\. Allowed values are compiled, \[c\]ulture\-\[i\]nvariant, \[e\]cma\-\[s\]cript, \[n\] explicit\-capture, \[f\]rom\-file, \[i\]gnore\-case, \[x\] ignore\-pattern\-whitespace, \[li\]st, \[li\]st\-\[s\]eparator, \[l\]iteral, \[m\]ultiline, \[r\]ight\-to\-left, \[s\]ingleline, timeout=\<NUM>, \[w\]hole\-\[i\]nput, \[w\]hole\-\[l\]ine and \[w\]hole\-word\.

**`[-y|--display] <DISPLAY_OPTIONS>`**

Display of the results\. Allowed values are \[c\]ontent=\<CONTENT\_DISPLAY> and \[p\]ath=\<PATH\_DISPLAY>\.

**`[-d|--dry-run]`**

Display which files should be updated but do not actually update any file\.

**`[--encoding] <ENCODING>`**

Encoding to use when a file does not contain byte order mark\. Default encoding is UTF\-8\.

**`[--evaluator] <EVALUATOR>`**

Path to the evaluator method to compute replacements\. The format is "LibraryPath,FullTypeName\.MethodName"\.

**`[-e|--extension] <EXTENSION_FILTER>`**

A filter for file extensions\. Syntax is EXT1\[,EXT2,\.\.\.\] \[\<EXTENSION\_OPTIONS>\]\. Allowed values are \[c\]ulture\-\[i\]nvariant, \[f\]rom\-file, \[i\]gnore\-case, \[li\]st\-\[s\]eparator, \[l\]iteral, \[neg\]ative and timeout=\<NUM>\.

**`[-h|--highlight] <HIGHLIGHT>`**

Parts of the output to highlight\. Allowed values are \[n\]one, \[m\]atch, \[r\]eplacement, \[e\]mpty\-\[m\]atch, \[e\]mpty\-\[r\]eplacement, \[e\]mpty\-\[s\]plit, \[e\]mpty, \[b\]oundary, \[t\]ab, \[c\]arriage\-\[r\]eturn, \[l\]ine\[f\]eed, \[n\]ew\-\[l\]ine and space\.

**`[-i|--include-directory] <REGEX>`**

Regular expression for a directory name\. Syntax is \<PATTERN> \[\<PATTERN\_OPTIONS>\]\. Allowed values are compiled, \[c\]ulture\-\[i\]nvariant, \[e\]cma\-\[s\]cript, \[n\] explicit\-capture, \[f\]rom\-file, \[g\]roup=\<GROUP\_NAME>, \[i\]gnore\-case, \[x\] ignore\-pattern\-whitespace, \[li\]st, \[li\]st\-\[s\]eparator, \[l\]iteral, \[m\]ultiline, \[neg\]ative, \[p\]art=\<NAME\_PART>, \[r\]ight\-to\-left, \[s\]ingleline, timeout=\<NUM>, \[w\]hole\-\[i\]nput, \[w\]hole\-\[l\]ine and \[w\]hole\-word\.

**`[--input] <INPUT>`**

Text to search\.

**`[-l|--line-number]`**

Include line number\.

**`[--max-count] <MAX_OPTIONS>`**

Stop searching after specified number is reached\. Allowed values are \[m\]atches=\<NUM>, \[m\]atches\-\[i\]n\-\[f\]ile and \[m\]atching\-\[f\]iles\.

**`[-n|--name] <REGEX>`**

Regular expression for file or directory name\. Syntax is \<PATTERN> \[\<PATTERN\_OPTIONS>\]\. Allowed values are compiled, \[c\]ulture\-\[i\]nvariant, \[e\]cma\-\[s\]cript, \[n\] explicit\-capture, \[f\]rom\-file, \[g\]roup=\<GROUP\_NAME>, \[i\]gnore\-case, \[x\] ignore\-pattern\-whitespace, \[li\]st, \[li\]st\-\[s\]eparator, \[l\]iteral, \[m\]ultiline, \[neg\]ative, \[p\]art=\<NAME\_PART>, \[r\]ight\-to\-left, \[s\]ingleline, timeout=\<NUM>, \[w\]hole\-\[i\]nput, \[w\]hole\-\[l\]ine and \[w\]hole\-word\.

**`[--no-recurse]`**

Do not search subdirectories\.

**`[--progress]`**

Display dot \(\.\) for every tenth searched directory\.

**`[-r|--replacement] <REPLACEMENT>`**

Replacement pattern\. Syntax is \<REPLACEMENT> \[\<REPLACEMENT\_OPTIONS>\]\. Allowed values are \[f\]rom\-file, \[l\]iteral and \[m\]ultiline\.

**`[-v|--verbosity] <VERBOSITY>`**

The amount of information to display in the log\. Allowed values are \[q\]uiet, \[m\]inimal, \[n\]ormal, \[d\]etailed and \[diag\]nostic\.

**`[--file-log] <FILE_LOG>`**

Syntax is \<LOG\_PATH> \[\<LOG\_OPTIONS>\]\. Allowed values are \[v\]erbosity=\<VERBOSITY> and \[a\]ppend\.

## Samples

### Sample

Update version in csproj and vbproj files in a current directory.

Current directory contains file **pattern.txt** with a following content:

```
(?mx)
(?<=
  ^\ *\<Version\>
)
\d+\.\d+\.\d+\.\d+
(?=
  \</Version\>
)
```

#### Syntax

```
orang replace ^
 --extension csproj,vbproj ^
 --content "pattern.txt" from-file ^
 --replacement "1.2.3.0" ^
 --highlight match replacement
```

#### Short Syntax

```
orang replace ^
 -e csproj,vbproj ^
 -c "pattern.txt" f ^
 -r "1.2.3.0" ^
 -h m r
```

### Sample

Remove duplicate words in C# comments from source files in a current directory.

Current directory contains file **pattern.txt** with a following content:

```
(?mx)
(?<=
  ^
  (\ |\t)*
  //[^\r\n]*\b(?<g>\w+)\b
)
\ +
\b\k<g>\b
```

#### Syntax

```
orang replace ^
 --extension cs ^
 --content "pattern.txt" ^
 --include-directory ".git" whole-input negative ^
 --highlight match
```

#### Short Syntax

```
orang replace ^
 -e cs ^
 -c "pattern.txt" ^
 -i ".git" wi neg ^
 -h m

```

### Sample

Normalize newline to CR+LF for all files in a current directory.

#### Syntax

```
orang replace ^
 --content "(?<!\r)\n" ^
 --replacement "\r\n" multiline ^
 --verbosity minimal
```

#### Short Syntax

```
orang replace ^
 -c "(?<!\r)\n" ^
 -r "\r\n" m ^
 -v m
```
*\(Generated with [DotMarkdown](http://github.com/JosefPihrt/DotMarkdown)\)*