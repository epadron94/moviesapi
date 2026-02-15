resourceGroup="MoviesApp"
accountName="moviesapp"
databaseName="moviesdb"
#az cosmosdb create \
#    --name "moviesapp" \
#    --resource-group $resourceGroup \
#    --kind GlobalDocumentDB \
#    --locations regionName="South Central US" \
#    --default-consistency-level Session \
#    --enable-free-tier true

az cosmosdb sql database create \
    --account-name $accountName \
    --name $databaseName \
    --resource-group $resourceGroup

 #create model containers
#######################MOVIES##########################



az cosmosdb sql container create \
    --account-name $accountName \
    --database-name $databaseName \
    --resource-group $resourceGroup \
    --name Movie \
    --partition-key-path "/id" \
    --idx @moviesapi/scripting/Containers/MovieCompositeIndex.json

az cosmosdb sql container create \
    --account-name $accountName \
    --database-name $databaseName \
    --resource-group $resourceGroup \
    --name MovieGenre \
    --partition-key-path "/movieId" \



#######################USER##########################

az cosmosdb sql container create \
    --account-name $accountName \
    --database-name $databaseName \
    --resource-group $resourceGroup \
    --name User \
    --partition-key-path "/id"


#######################REVIEW##########################

az cosmosdb sql container create \
    --account-name $accountName \
    --database-name $databaseName \
    --resource-group $resourceGroup \
    --name Review \
    --partition-key-path "/id" \
    --idx @moviesapi/scripting/Containers/ReviewCompositeIndex.json

az cosmosdb sql container create \
    --account-name $accountName \
    --database-name $databaseName \
    --resource-group $resourceGroup \
    --name UserReview \
    --partition-key-path "/userId" 

az cosmosdb sql container create \
    --account-name $accountName \
    --database-name $databaseName \
    --resource-group $resourceGroup \
    --name MovieReview \
    --partition-key-path "/movieId"