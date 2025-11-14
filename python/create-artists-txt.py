import os
import re

# Forbidden symbols: :, apostrophes, the long dash, accent letters or language specific letters (ae, polish l etc)

folder_path = "D:\\Projects MAUI\\rpd-artists\\"
github_url = "https://github.com/giannistek1/rpd-artists/blob/main/"

open(f'{folder_path}artists.txt').close()

with open(f'{folder_path}artists.txt', 'w') as f:
    for filename in os.listdir(folder_path):
        if "[" in filename:
            print(filename)
            matches = re.findall(r'\[.*?\]', filename)
            artist = matches[0].replace("[", "").replace("]", "") # Artist
            artist_alt = matches[1].replace("[", "").replace("]", "")  # Artist alt
            debut_date = matches[2].replace("[", "").replace("]", "")  # Debut date
            grouptype = matches[3].replace("[", "").replace("]", "")  # Grouptype
            members = matches[4].replace("[", "").replace("]", "")  # Membercount
            company = matches[5].replace("[", "").replace("]", "")  # Company
            url = filename.replace("[", "%5B").replace("]","%5D") # Since url changes on 26/09/2024
            imageUrl = f"{github_url}{url}?raw=true"
            #print(f'{{{artist}}}{{{artist_alt}}}{{{debut_date}}}{{{grouptype}}}{{{members}}}{{{company}}}{{{imageUrl}}}')
            print(f'{{{artist}}}{{{artist_alt}}}{{{debut_date}}}{{{grouptype}}}{{{members}}}{{{company}}}{{{imageUrl}}}', file=f)
