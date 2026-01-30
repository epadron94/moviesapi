resourcegroupname=$(az group list --query "[].{name:name}" -o tsv) 
az appservice plan create --name moviesappplan --resource-group $resourcegroupname --is-linux --location "South Central US" --sku F1
planname="moviesappplan"
az webapp create --name movieswebapi  --plan $planname --resource-group $resourcegroupname --https-only true --runtime "DOTNETCORE:10.0"


keyvaulturi=$(az keyvault show --name moviesappvault --query properties.vaultUri -o tsv)
certuri=$(az keyvault certificate list --id $keyvaulturi --query "[].{id:id}" -o tsv)
appsettingvalue="movieswebapicert=@Microsoft.KeyVault(CertificateUri=$certuri)"
az webapp config appsettings set --name movieswebapi --resource-group $resourcegroupname --settings $appsettingvalue
