
json=movies.json
secrets_created=();
jq -c '.[]' $json | while read -r movie; do
    releaseDate=$(echo "$movie" | jq -r '.releaseDate')
    releaseDateFormat=$(date -j -f "%m/%d/%Y" $releaseDate +"%Y-%m-%dT%000:%000:%000Z")
    #echo $releaseDateFormat
    movieId=$(echo "$movie" | jq -r '.id')
    validId=$(echo $movieId | grep -qE '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$' && echo $movieId || echo $(uuidgen | tr '[:upper:]' '[:lower:]'))
    echo "$movie" | jq --arg newDate "$releaseDateFormat" --arg guidValidId "$validId" '.releaseDate = $newDate | .id = $guidValidId | .movieId = $guidValidId'
    echo ","


done  > "moviesupdated.json"
echo "done"