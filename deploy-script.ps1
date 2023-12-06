$sourceDirectory = "${{ github.workspace }}/publish"
$destinationDirectory = "C:\inetpub\wwwroot\leave-day-services"
$appPoolName = "leave-day-services"

Copy-Item -Path $sourceDirectory -Destination $destinationDirectory -Recurse -Force

Restart-WebAppPool -Name $appPoolName
