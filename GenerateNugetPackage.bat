@echo off

echo Welcome, let's create a new NuGet package for QueryString.NET!
echo.

set /p version="Enter Version Number (ex. 1.0.0): "

if not exist "NugetPackages" mkdir "NugetPackages"

"nuget.exe" pack -Version %version% -OutputDirectory "NugetPackages"

PAUSE

explorer NugetPackages