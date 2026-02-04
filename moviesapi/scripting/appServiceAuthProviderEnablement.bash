#from https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-provider-aad?tabs=workforce-configuration
#from https://learn.microsoft.com/en-us/azure/app-service/overview-authentication-authorization
# from https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-api-version

resourceGroup="MoviesApp"
appName="movieswebapi"
tenantId=$(az account show --query tenantId -o tsv)
clientId=$(az ad app list --query "[?displayName=='MoviesApp'].{appId:appId}" -o tsv)
clientSecret="dummySecretValue123!"  # Use a dummy value; the real secret is in Key Vault
issuerUrl="https://login.microsoftonline.com/$tenantId/v2.0"
appServiceHost=$(az webapp show --name movieswebapi -g MoviesApp --query defaultHostName -o tsv)
fullAppServiceUrl="https://$appServiceHost,api://$clientId"


az webapp auth microsoft update \
    --resource-group $resourceGroup \
    --name $appName \
    --client-id $clientId \
    --issuer $issuerUrl \
    --client-secret $clientSecret \
    --allowed-audiences $fullAppServiceUrl

az webapp auth update \
    --resource-group $resourceGroup \
    --name $appName \
    --enabled true \
    --unauthenticated-client-action Return401 \
    --set identityProviders.apple.enabled=false \
    --set identityProviders.facebook.enabled=false \
    --set identityProviders.github.enabled=false \
    --set identityProviders.google.enabled=false \
    --set identityProviders.legacyMicrosoftAccount.enabled=true \
    --set identityProviders.twitter.enabled=false