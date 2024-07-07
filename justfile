# Project: Saigai Solves
# File: justfile
#
# This file presents aliases for common command sequences used during development.

_default:
    just recompile-dll

# Generates the DLL and the connecting C# FFI declarations used by the Unity project
recompile-dll:
    cd ./Rust; cargo r;
