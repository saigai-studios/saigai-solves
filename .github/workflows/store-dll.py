# Project: Saigai Solver
# Script: store-dll.py
#
# This script is used for CI/CD to automatically place the generated C# FFI and
# dynamic C library in the unity project before building the final artifact.


import glob, os, shutil
import platform

PLATFORM = platform.platform().upper().split('-')[0]

DLL_NAME = 'saigai'

DLL_SRC = 'Rust/target/release'
DLL_DEST = 'Assets/Plugins'

# Use this to specify the source of where the C# bindings are generated
# otherwise the files may already exist at the destination
CSHARP_SRC = 'Rust/bindings/csharp'
CSHARP_DEST = 'Assets/Scripts'

dll_prefix = ''
dll_suffix = ''

# Determine the correct file names
if PLATFORM == 'MACOS':
    dll_prefix = 'lib'
    dll_suffix = '.dylib'
elif PLATFORM == 'WINDOWS':
    dll_prefix = ''
    dll_suffix = '.dll'
elif PLATFORM == 'LINUX':
    dll_prefix = 'lib'
    dll_suffix = '.so'
else:
    print("error: unknown platform:", PLATFORM)
    exit(101)

# Copy the DLL to the correct Unity folder
DLL_FILENAME = dll_prefix + DLL_NAME + dll_suffix
dll_src_path = os.path.join(DLL_SRC, DLL_FILENAME)
dll_dest_path = os.path.join(DLL_DEST, DLL_FILENAME)
print('info: copying DLL', dll_src_path, 'to', dll_dest_path, '...')
shutil.copy(dll_src_path, dll_dest_path)

# Copy the csharp files to the correct Unity folder (if they exist)
cs_files = glob.glob(CSHARP_SRC + '/*.cs')
for cs in cs_files:
    filename = os.path.basename(cs)
    dest = os.path.join(CSHARP_DEST, filename)
    print('info: copying C#', cs, 'to', dest, '...')
    shutil.copy(cs, dest)
    pass
