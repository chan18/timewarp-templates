if (!$ApplicationNameSpace) { throw "ApplicationNameSpace is not set"}
if (!$RegistryHost) { throw "RegistryHost is not set"}

$global:ApplicationName = "api-server"
$ApplicationImageTag = "1.0.0"
$global:ApplicationImage = "$RegistryHost/$($ApplicationName):$ApplicationImageTag"

Push-Location $PSScriptRoot
try { 
  Apply-Manifest ./api_server-deployment.yaml
}
finally {
  Pop-Location
}
