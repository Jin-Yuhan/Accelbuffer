call dotnet restore  /p:UseSharedCompilation=false /p:BuildInParallel=false /m:1 -r win-x86
call dotnet build -c Release  /p:UseSharedCompilation=false /p:BuildInParallel=false /m:1 -r win-x86
call dotnet publish -c Release  /p:UseSharedCompilation=false /p:BuildInParallel=false /m:1 -r win-x86
