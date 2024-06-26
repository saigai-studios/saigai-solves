#!/usr/bin/env python 
# Project: Saigai Solves
# Script: build.py
#
# Builds the game for your preferred platform using the Unity editor.

import subprocess, sys

UNITY_PATH = sys.argv[1]
TARGET = sys.argv[2]

if TARGET != "win64" and TARGET != "osxuniversal" and TARGET != "linux64":
    print("Error: Invalid build target (must be \"win64\", \"osxuniversal\", or \"linux64\")")
    quit()

build_path = "Builds/" + TARGET

print("***** BUILDING RUST DLL *****")
child = subprocess.Popen(['cargo', 'run', '--release'], cwd='Rust')
child.wait()
print("***** STORING RUST DLL *****")
child = subprocess.Popen(['python', './.github/workflows/store-dll.py'])
child.wait()
print("***** COMPILING UNITY *****")
child = subprocess.Popen([UNITY_PATH, '-projectPath', '.', '-batchmode', '-buildTarget', TARGET, '-logFile' '-', '-quit', '-nographics', '-customBuildPath', build_path, '-executeMethod', 'CLIBuild.Builder.BuildProjectParse'])
child.wait()