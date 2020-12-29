@echo off
set configuration=Debug
clean ^
  && dotnet test test\Aqua.AccessControl.Tests                  --configuration %configuration% ^
  && dotnet test test\Aqua.AccessControl.Tests.SQLite.EF6       --configuration %configuration% ^
  && dotnet test test\Aqua.AccessControl.Tests.SQLite.EFCore    --configuration %configuration% ^
  && dotnet test test\Aqua.AccessControl.Tests.SqlServer.EFCore --configuration %configuration% ^
  && dotnet pack src\Aqua.AccessControl                         --configuration %configuration%