import os
import re

# Forbidden symbols: :, apostrophes, the long dash, accent letters or language specific letters (ae, polish l etc)

video_path = "D:\\Projects MAUI\\rpd-videos\\"

open(f'{video_path}videos.txt').close()

with open(f'{video_path}videos.txt', 'w') as f:
    for filename in os.listdir(video_path):
        if filename.endswith(".mp4") and "[" in filename:
            #print(filename)       
            matches = re.findall(r'\[.*?\]', filename)
            artist = matches[0].replace("[", "").replace("]", "") # Artist
            album = matches[1].replace("[", "").replace("]", "")  # Album
            title = matches[2].replace("[", "").replace("]", "")  # Title
            part_name = matches[3].replace("[", "").replace("]", "")  # Song part
            part_number = matches[4].replace("[", "").replace("]", "")  # Song part number
            print(f'{{{artist}}}{{{album}}}{{{title}}}{{{part_name}}}{{{part_number}}}', file=f)
