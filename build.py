#!/usr/bin/env python 
# Project: Saigai Solves
# Script: build.py
#
# Builds the game for your preferred platform using the Unity editor.

import subprocess, sys, platform

UNITY_PATH = sys.argv[1]
TARGET = sys.argv[2]
ISSC_PATH = ""

if len(sys.argv) >= 4:
    ISSC_PATH = sys.argv[3]

if TARGET != "win64" and TARGET != "osxuniversal" and TARGET != "linux64":
    print("Error: Invalid build target (must be \"win64\", \"osxuniversal\", or \"linux64\")")
    quit()

build_path = "Builds/" + TARGET

print("***** BUILDING RUST LIBRARY *****")
child = subprocess.Popen(['cargo', 'run', '--release'], cwd='Rust')
child.wait()
print("***** STORING RUST LIBRARY *****")
child = subprocess.Popen(['python', './.github/workflows/store-dll.py'])
child.wait()
print("***** COMPILING UNITY *****")
child = subprocess.Popen([UNITY_PATH, '-projectPath', '.', '-batchmode', '-buildTarget', TARGET, '-logFile' '-', '-quit', '-nographics', '-customBuildPath', build_path, '-executeMethod', 'CLIBuild.Builder.BuildProjectParse'])
child.wait()

if ISSC_PATH != "" and TARGET == "win64" and platform.system() == "Windows":
    print("***** GENERATING INSTALLER *****")
    child = subprocess.Popen([ISSC_PATH, 'installer.iss'])
    child.wait()