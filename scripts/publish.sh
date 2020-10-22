#!/bin/bash

root_dir=`git rev-parse --show-toplevel`

# publish
find $root_dir -type f -name "*.nupkg" -exec \
    dotnet nuget push {} -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json \;

