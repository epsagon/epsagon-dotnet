#!/usr/bin/env python3

from argparse import ArgumentParser
import xml.etree.ElementTree as ET

parser = ArgumentParser()
parser.add_argument('--type', help="type of bump (major, minor, patch)")
parser.add_argument('--proj', help="path to the .csproj file to update")
args = parser.parse_args()

tree = ET.parse(args.proj)
root = tree.getroot()
version = root.findall('./PropertyGroup/Version')[0]
major, minor, patch = [int(x) for x in version.text.split('.')]

if args.type == 'major': major += 1
elif args.type == 'minor': minor += 1
elif args.type == 'patch': patch += 1

version.text = '.'.join(str(x) for x in (major, minor, patch))
tree.write(args.proj)

print(f"(version-bump): {args.proj} -> {version.text}")
