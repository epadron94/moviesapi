# from https://learn.microsoft.com/en-us/azure/app-service/deploy-github-actions?tabs=openid%2Caspnetcore
subscriptionId=$(az account show --query id -o tsv)
appName="githubauth" 
movieswebapiscope=$(az webapp show --name movieswebapi --resource-group MoviesApp --query id -o tsv)
az ad app create --display-name $appName  
clientId=$(az ad app list --query "[?displayName=='$appName'].{appId:appId}" -o tsv)
objectId=$(az ad app list --query "[?displayName=='$appName'].{id:id}" -o tsv)
tenantId=$(az account show --query tenantId -o tsv)
az ad sp create --id $clientId
spObjectId=$( az ad sp list --query "[?displayName=='$appName'].{id:id}" -o tsv)

az role assignment create --role "Website Contributor" --subscription $subscriptionId --assigneeObjectId $spObjectId --scope $movieswebapiscope --assignee-principal-type ServicePrincipal 

az ad app federated-credential create --id $spObjectId --parameters @federatedcredential.json

gh secret set AZURE_CLIENT_ID --body $clientId --repo epadron94/moviesapi
gh secret set AZURE_TENANT_ID --body $tenantId --repo epadron94/moviesapi
gh secret set AZURE_SUBSCRIPTION_ID --body $subscriptionId --repo epadron94/moviesapi


