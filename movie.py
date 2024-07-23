# Project: Saigai Solves
# Script: movie.py
#
# Converts .mp4 files to .webm files for cross-platform compatibility.

import glob
import subprocess


mp4s = glob.glob('./Assets/**/*.mp4', recursive=True)
# ffmpeg -i input-file.mp4 -c:v libvpx -crf 10 -b:v 1M -c:a libvorbis output-file.webm
def convert(input_path, output_path):
    child = subprocess.Popen(['./ffmpeg', '-i', input_path, '-c:v', 'libvpx', '-crf', '10', '-b:v', '1M', '-c:a', 'libvorbis', output_path])
    rc = child.wait()
    if rc != 0:
        exit(rc)
    pass

for mp4 in mp4s:
    input_path = str(mp4)
    output_path = input_path.replace('.mp4', '.webm')
    
    print("info: converting \""+str(input_path)+"\" ...")
    convert(input_path, output_path)
    print("info: saved to \""+str(output_path)+"\" ...")
    pass