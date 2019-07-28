#!/bin/bash

root_dir=`git rev-parse --show-toplevel`
find $root_dir -type f -name "*.nupkg" -exec rm -rf {} \;
