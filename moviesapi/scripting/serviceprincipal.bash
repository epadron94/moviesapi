#openssl req -x509 -newkey rsa:2048 -keyout moviesapikey.pem -out moviesapicert.pem -days 365 -nodes -subj "/CN=moviesapi"

#az ad sp create-for-rbac --name "moviesapi" --cert @moviesapicert.pem

#moviesapisp=$(az ad sp  list --query "[?displayName=='moviesapi'].{appId:appId}" --output tsv)
#keyvaultscope=$(az keyvault show --name moviesappvault --query id --output tsv)
#az role assignment create --assignee $moviesapisp --role "Key Vault Secrets User" --scope $keyvaultscope

userid=$(az ad signed-in-user show --query "id" --output tsv)
keyvaultscope=$(az keyvault show --name moviesappvault --query id --output tsv)
az role assignment create --assignee $userid --role "Key Vault Certificates Officer" --scope $keyvaultscope
az ad sp create-for-rbac --keyvault moviesappvault --cert moviesapicert --create-cert --name moviesapi --create-password
az role assignment create --assignee $moviesapisp --role "Key Vault Secrets User" --scope $keyvaultscope