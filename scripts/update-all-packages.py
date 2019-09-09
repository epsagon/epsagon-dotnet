#!/usr/bin/env python3

from argparse import ArgumentParser
import shutil
import subprocess
import xml.etree.ElementTree as ET

parser = ArgumentParser()
parser.add_argument('--proj', help="path to the .csproj file to update")
args = parser.parse_args()


def find_version(package_name):
    return subprocess.getoutput(f"dotnet search {package_name}"
                                f"| awk -F '[[:space:]][[:space:]]+' '$1 == \"{package_name}\" {{ print $4 }}'").strip()

tree = ET.parse(args.proj)
root = tree.getroot()
packages = root.findall('./ItemGroup/PackageReference')

for package in packages:
    name = package.attrib['Include']
    latest_version = find_version(name)
    print(f'package: {name} -> {latest_version}')
    package.attrib['Version'] = latest_version

tree.write(args.proj)
