@echo off
echo ===========================================
echo FanControl DLL Native Compiler
echo ===========================================

set CSC="C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
set NETSTANDARD="C:\Windows\Microsoft.NET\assembly\GAC_MSIL\netstandard\v4.0_2.0.0.0__cc7b13ffcd2ddd51\netstandard.dll"

if not exist %CSC% (
    echo ERROR: C# Compiler not found at %CSC%
    pause
    exit /b
)

if not exist "FanControl.Plugins.dll" (
    echo ERROR: FanControl.Plugins.dll must be in this folder to compile!
    pause
    exit /b
)

if not exist "..\FanControl.CustomLoop" (
    mkdir "..\FanControl.CustomLoop"
)

echo Compiling...
if exist %NETSTANDARD% (
    %CSC% /nologo /target:library /out:..\FanControl.CustomLoop\FanControl.CustomLoop.dll /reference:FanControl.Plugins.dll /reference:%NETSTANDARD% FanControl.CustomLoop.cs
) else (
    echo WARNING: netstandard.dll not found in the default Windows GAC path.
    echo Attempting to compile assuming it is in the current directory...
    %CSC% /nologo /target:library /out:..\FanControl.CustomLoop\FanControl.CustomLoop.dll /reference:FanControl.Plugins.dll /reference:netstandard.dll FanControl.CustomLoop.cs
)

if %ERRORLEVEL% EQU 0 (
    echo.
    echo SUCCESS! 
    echo File created: ..\FanControl.CustomLoop\FanControl.CustomLoop.dll
    echo Move this DLL to your FanControl\Plugins folder.
) else (
    echo.
    echo Compilation FAILED. Please check the errors above.
)
pause
