call dotnet restore  /p:UseSharedCompilation=false /p:BuildInParallel=false /m:1
call dotnet build -c Release  /p:UseSharedCompilation=false /p:BuildInParallel=false /m:1
