@echo off
set configuration=Debug
set version-suffix="alpha-001"
clean ^
  && dotnet restore ^
  && dotnet build src\Aqua.AccessControl --configuration %configuration% ^
  && dotnet build test\Aqua.AccessControl.Tests --configuration %configuration% ^
  && dotnet test test\Aqua.AccessControl.Tests\Aqua.AccessControl.Tests.csproj --configuration %configuration% ^
  && dotnet pack src\Aqua.AccessControl\Aqua.AccessControl.csproj --output "..\..\artifacts" --configuration %configuration% --include-symbols --version-suffix "%version-suffix%"