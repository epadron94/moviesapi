accountName="moviesapp"
databaseName="moviesdb"
resourceGroup="MoviesApp"



# create model containers


#######################USER##########################

az cosmosdb sql container create \
    --account-name $accountName \
    --database-name $databaseName \
    --resource-group $resourceGroup \
    --name User \
    --partition-key-path "/userId" \
    --idx @moviesapi/scripting/Containers/UserCompositeIndex.json

#######################MOVIES##########################



az cosmosdb sql container create \
    --account-name $accountName \
    --database-name $databaseName \
    --resource-group $resourceGroup \
    --name Movie \
    --partition-key-path "/movieId" \
    --idx @moviesapi/scripting/Containers/MovieCompositeIndex.json




#######################REVIEW##########################

az cosmosdb sql container create \
    --account-name $accountName \
    --database-name $databaseName \
    --resource-group $resourceGroup \
    --name Review \
    --partition-key-path "/movieId" \
    --idx @moviesapi/scripting/Containers/ReviewCompositeIndex.json