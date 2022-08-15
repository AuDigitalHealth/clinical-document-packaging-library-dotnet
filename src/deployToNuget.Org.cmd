del *.nupkg

msbuild /p:Configuration=Release

REM Use dotnet for packaing now
REM NuGet.exe pack CDAPackage/CDAPackage.csproj -Properties Configuration=Release
dotnet pack .\CDAPackage\CDAPackage.csproj -c Release -o .

forfiles /m *.nupkg /c "cmd /c NuGet.exe push @FILE -Source https://www.nuget.org/api/v2/package"
