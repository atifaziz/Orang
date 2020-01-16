@echo off

set _msbuildPath=C:\Program Files\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin
set _roslynatorPath=..\..\Roslynator\src

"%_msbuildPath%\msbuild" "%_roslynatorPath%\CommandLine.sln" /t:Build /p:Configuration=Release /v:m /m

"%_roslynatorPath%\CommandLine\bin\Release\net472\roslynator" loc "..\src\Orang.sln" ^
 --msbuild-path "%_msbuildPath%" ^
 --ignore-block-boundary ^
 --verbosity d

pause
