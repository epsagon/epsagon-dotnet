#!/bin/bash

version=$1
root_dir=`git rev-parse --show-toplevel`

# clean current files
sh $root_dir/scripts/clean.sh

# version bump all projects
dotnet sln list \
    | grep .csproj \
    | xargs -n 1 python3 $root_dir/scripts/version-bump.py --set-version "$version" --proj

# package all projects
dotnet pack $root_dir/Epsagon.Dotnet.sln -c Release
