#!/bin/bash

version_type=$1
root_dir=`git rev-parse --show-toplevel`

# clean current files
sh $root_dir/scripts/clean.sh

# version bump all projects
dotnet sln list \
    | grep .csproj \
    | xargs -n 1 python3 $root_dir/scripts/version-bump.py --type "$version_type" --proj

# package all projects
dotnet pack $root_dir/Epsagon.Dotnet.sln -c Release

# publish
find $root_dir -type f -name "*.nupkg" -exec \
    dotnet nuget push {} -k oy2kiexdmmonahnlwo3apjpavd7oexlufkmzpksc6hw3uu -s https://api.nuget.org/v3/index.json \;
