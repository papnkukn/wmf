@echo off
rem Run this script at your own risk

:parameters
rem Descriptive script name
set NAME=wmf

set MSBUILD="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set ILMERGE="D:\utils\ilmerge.exe"
set CONFIG=Release
set PLATFORM=AnyCPU

rem /v:<verbosity> - q[uiet], m[inimal], n[ormal], d[etailed], diag[nostic]
set VERBOSE=m

set SOLUTION=wmf.sln
rem CWD - Current working directory, should be the solution directory when publishing, but can be other when deploying
set CWD=%CD%
rem CPU_CORE_COUNT - Number of CPU cores for faster builds
set CPU_CORE_COUNT=2

:validation
if %CWD%=="" goto error-cwd-missing
if not exist %MSBUILD% goto error-msbuild-missing
if not exist %SOLUTION% goto error-solution-missing
if not "%1"=="" goto %1

:default
:build
:rebuild
echo This is a 'make' script for '%NAME%'
call %0 library
call %0 sample
goto end

:clean
if not exist %SOLUTION% goto error-solution-missing
echo Cleaning...
rem %MSBUILD% %SOLUTION% /t:Clean /p:WarningLevel=1;Configuration=%CONFIG%;Platform=%PLATFORM% /v:%VERBOSE% /m:%CPU_CORE_COUNT% /nologo
rmdir /S /Q sample\bin
rmdir /S /Q sample\obj
rmdir /S /Q library\bin
rmdir /S /Q library\obj
attrib -h /S *.user
attrib -h /S *.suo
del /S /Q *.user
del /S /Q *.suo
goto end

:library
if not exist %SOLUTION% goto error-solution-missing
echo --------------------------------------------------------------------------------
echo Building 'library' (%CONFIG%^|%PLATFORM%)...

set CSPROJ_PATH=library\Library.csproj
set CSPROJ_REL=..\
set OUT_DIR=bin\library

if exist "%OUT_DIR%" rmdir /S /Q "%OUT_DIR%"
%MSBUILD% %CSPROJ_PATH% /t:Rebuild /p:OutDir=%CSPROJ_REL%%OUT_DIR%\ /p:WarningLevel=1;Configuration=%CONFIG%;Platform=%PLATFORM% /v:%VERBOSE% /m:%CPU_CORE_COUNT% /nologo
rem /t:<project>:<target>
rem /p:<property1>;<property2>;WarningLevel=1;Configuration=%CONFIG%;OutputDir=bin\Debug
rem /v:<verbosity>     - q[uiet], m[inimal], n[ormal], d[etailed], diag[nostic]
rem /m:2               - number of msbuild.exe processes, 2 for dual core
rem /fl                - file logger, create msbuild.log
rem /nologo            - hides Microsoft (c) ... message
rem /fileLoggerParameters:LogFile=MyLog.log;Append;Verbosity=diagnostic;Encoding=UTF-8
rem /noconsolelogger   - if running from background script

goto end

:sample
if not exist %SOLUTION% goto error-solution-missing
set PLATFORM=x86
echo --------------------------------------------------------------------------------
echo Building 'sample' (%CONFIG%^|%PLATFORM%)...

set CSPROJ_PATH=sample\Console.csproj
set CSPROJ_REL=..\
set OUT_DIR=bin\sample
set OUT_EXE_MERGED=%OUT_DIR%\wmf.exe

if exist "%OUT_DIR%" rmdir /S /Q "%OUT_DIR%"
%MSBUILD% %SOLUTION% /t:Rebuild /p:OutDir=%CSPROJ_REL%%OUT_DIR%\ /p:WarningLevel=1;Configuration=%CONFIG%;Platform=%PLATFORM% /v:%VERBOSE% /m:%CPU_CORE_COUNT% /nologo
rem %MSBUILD% %CSPROJ_PATH% /t:Rebuild /p:OutDir=%CSPROJ_REL%%OUT_DIR%\ /p:WarningLevel=1;Configuration=%CONFIG%;Platform=%PLATFORM% /v:%VERBOSE% /m:%CPU_CORE_COUNT% /nologo
rem /t:<project>:<target>
rem /p:<property1>;<property2>;WarningLevel=1;Configuration=%CONFIG%;OutputDir=bin\Debug
rem /v:<verbosity>     - q[uiet], m[inimal], n[ormal], d[etailed], diag[nostic]
rem /m:2               - number of msbuild.exe processes, 2 for dual core
rem /fl                - file logger, create msbuild.log
rem /nologo            - hides Microsoft (c) ... message
rem /fileLoggerParameters:LogFile=MyLog.log;Append;Verbosity=diagnostic;Encoding=UTF-8
rem /noconsolelogger   - if running from background script

del /S /F /Q %OUT_DIR%\*.pdb

echo Merging to '%OUT_EXE_MERGED%'...
move %OUT_DIR%\wmf.exe %OUT_DIR%\tmp.exe
if exist %ILMERGE% (
	%ILMERGE% /target:exe /out:%OUT_EXE_MERGED% %OUT_DIR%\tmp.exe %OUT_DIR%\Oxage.Wmf.dll
	del /S /F /Q %OUT_DIR%\*.dll
	del /S /F /Q %OUT_DIR%\*.pdb
	del /S /F /Q %OUT_DIR%\*.xml
)
del /F /Q %OUT_DIR%\tmp.exe

goto end

:error-solution-missing
echo Error: Cannot find %SOLUTION%
echo Search location: %CD%
echo Hint: copy this script to the solution directory
goto end

:error-msbuild-missing
echo Error: Cannot find MSBuild.exe
echo Search location: %MSBUILD%
goto end

:error-cwd-missing
echo Error: cannot get current working directory
goto end

:end-error
echo Error!
goto end

:end