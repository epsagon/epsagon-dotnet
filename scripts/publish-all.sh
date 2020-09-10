#!/bin/bash

version=$1
update_all=$2
root_dir=`git rev-parse --show-toplevel`

# load env vars
. $root_dir/.env

# clean current files
sh $root_dir/scripts/clean.sh

# update all dependencies
# dotnet sln list \
#     | grep .csproj \
#     | xargs -n 1 python3 $root_dir/scripts/update-all-packages.py --proj

# version bump all projects
dotnet sln list \
    | grep .csproj \
    | xargs -n 1 python3 $root_dir/scripts/version-bump.py --set-version "$version" --proj

# package all projects
dotnet pack $root_dir/Epsagon.Dotnet.sln -c Release

# publish
find $root_dir -type f -name "*.nupkg" -exec \
    dotnet nuget push {} -k $NUGET_TOKEN -s https://api.nuget.org/v3/index.json \;
