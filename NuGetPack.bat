@echo off
set /p version="Version: "
msbuild HubConnectionManager\HubConnectionManager.csproj /P:Configuration=Release
rmdir /S /Q nuget-pack\lib
xcopy HubConnectionManager\bin\Release\HubConnectionManager.Lib.dll nuget-pack\lib\net40\ /Y
.nuget\nuget pack nuget-pack\HubConnectionManager.nuspec -Version %version%
pause