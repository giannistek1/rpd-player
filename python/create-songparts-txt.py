from email.mime import audio
import os
import re
from mutagen.mp3 import MP3

# Forbidden symbols: :, apostrophes, the long dash, accent letters or language specific letters (ae, polish l etc)

# after 26/09/2024: https://github.com/giannistek1/rpd-audio/blob/main/%5B(G)I-DLE%5D%5B2%5D%5BSuper%20Lady%5D%5BC%5D%5B1%5D.mp3 
# before 27/09/2024: https://github.com/giannistek1/rpd-audio/blob/main/[(G)I-DLE][2][Super Lady][C][1].mp3?raw=true 

audio_path = "D:\\Projects MAUI\\rpd-audio\\"
github_url = "https://github.com/giannistek1/rpd-audio/blob/main/"

open(f'{audio_path}songparts.txt').close()

with open(f'{audio_path}songparts.txt', 'w') as f:
    for filename in os.listdir(audio_path):
        if filename.endswith(".mp3") and "[" in filename:
            #print(filename)
            audio = MP3(f'{audio_path}\\{filename}')
            clipLength = audio.info.length
            clipLengthEuropean = str(clipLength).replace(".", ",")
            
            matches = re.findall(r'\[.*?\]', filename)
            artist = matches[0].replace("[", "").replace("]", "") # Artist
            album = matches[1].replace("[", "").replace("]", "")  # Album
            title = matches[2].replace("[", "").replace("]", "")  # Title
            part_name = matches[3].replace("[", "").replace("]", "")  # Song part
            part_number = matches[4].replace("[", "").replace("]", "")  # Song part number
            url = filename.replace("[", "%5B").replace("]","%5D") # Since url changes on 26/09/2024
            audioUrl = f"{github_url}{url}?raw=true"
            print(audioUrl)
            print(f'{{{artist}}}{{{album}}}{{{title}}}{{{part_name}}}{{{part_number}}}{{{clipLengthEuropean}}}{{{audioUrl}}}', file=f)
