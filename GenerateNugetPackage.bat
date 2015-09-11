@echo off

echo Welcome, let's create a new NuGet package for QueryString.NET!
echo.

set /p version="Enter Version Number (ex. 1.0.0): "

if not exist "NugetPackages" mkdir "NugetPackages"

"C:\Program Files (x86)\NuGet\nuget.exe" pack -Version %version% -OutputDirectory "NugetPackages"

PAUSE

explorer NugetPackages