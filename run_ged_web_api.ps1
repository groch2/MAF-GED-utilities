$ged_api_url = "http://localhost:44363/"
"lauching GED API at:", $ged_api_url | %{ Write-Host $_ }
dotnet watch --project "C:\TeamProjects\GED API\MAF.GED.API.Host\MAF.GED.API.Host.csproj" --urls=$ged_api_url
