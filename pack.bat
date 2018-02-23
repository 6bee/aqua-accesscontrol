@echo off
set configuration=Debug
set version-suffix="alpha-002"
clean ^
  && dotnet restore ^
  && dotnet test test\Aqua.AccessControl.Tests\Aqua.AccessControl.Tests.csproj --configuration %configuration% ^
  && dotnet test test\Aqua.AccessControl.Tests.SQLite.EF6\Aqua.AccessControl.Tests.SQLite.EF6.csproj --configuration %configuration% ^
  && dotnet test test\Aqua.AccessControl.Tests.SQLite.EFCore\Aqua.AccessControl.Tests.SQLite.EFCore.csproj --configuration %configuration% ^
  && dotnet test test\Aqua.AccessControl.Tests.SqlServer.EFCore\Aqua.AccessControl.Tests.SqlServer.EFCore.csproj --configuration %configuration% ^
  && dotnet pack src\Aqua.AccessControl\Aqua.AccessControl.csproj --output "..\..\artifacts" --configuration %configuration% --include-symbols --version-suffix "%version-suffix%"