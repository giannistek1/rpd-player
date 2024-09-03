import os
import re

folder_path = "D:\\Projects MAUI\\rpd-albums\\"
github_url = "https://github.com/giannistek1/rpd-albums/blob/main/"

# Clears file
open(f'{folder_path}albums.txt').close()

with open(f'{folder_path}albums.txt', 'w') as f:
    for filename in os.listdir(folder_path):
        filename, file_extension = os.path.splitext(filename)
        
        if "[" in filename:
            print(filename)
            matches = re.findall(r'\[.*?\]', filename)
            artist = matches[0].replace("[", "").replace("]", "") # Artist
            album = matches[1].replace("[", "").replace("]", "")  # Album
            release_date = matches[2].replace("[", "").replace("]", "")  # Release date
            language = matches[3].replace("[", "").replace("]", "")  # Language
            imageUrl = f"{github_url}{filename}{file_extension}?raw=true"
            #print(f'{{{artist}}}{{{album}}}{{{release_date}}}{{{language}}}{{{imageUrl}}}')
            print(f'{{{artist}}}{{{album}}}{{{release_date}}}{{{language}}}{{{imageUrl}}}', file=f)
