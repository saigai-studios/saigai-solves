#!/usr/bin/env python 
# Project: Saigai Solves
# Script: build.py
#
# Builds the game for your preferred platform using the Unity editor.

import subprocess, sys

UNITY_PATH = sys.argv[1]
TARGET = sys.argv[2]

child = subprocess.Popen(['cargo', 'run', '--release'], cwd='Rust')
child.wait()
child = subprocess.Popen(['python', './.github/workflows/store-dll.py'])
child.wait()
child = subprocess.Popen([UNITY_PATH, '-projectPath', '.', '-batchmode', '-buildTarget', TARGET, '-logFile' '-'])
child.wait()