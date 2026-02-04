keyvaultScope=$(az keyvault show --name moviesappvault --query id -o tsv)
movieswebapiScope=$(az webapp show --name movieswebapi --resource-group MoviesApp --query id -o tsv)