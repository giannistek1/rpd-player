# Rpd-player
Currently this hobby project is in a very WIP state.

## Overview
- Context
- Supported platforms
- Problems Solved
- Summary
- Home screen
- Search/library screen
- Playlists screen
- Current playlist

## Context
RPD stands for Random Play Dance. <br/>
These are socia, international, dance events where a group of people stand around in a rectangle and dance in the rectangle when they hear a song they know the dance of. <br/>
This is paired with a short countdown between each song so people have enough time to leave the area (so it is empty again) and prepare for the next song to dance to. Otherwise it would be very chaotic. <br/>
Tyically in South Korea they last an hour. <br/>
However in other countries they may be scheduled from 12:00-18:00 with several breaks and several different playlists of each 1-2 hours.

## Supported platforms and scope
- Android, iOS (Todo), Windows
- Music player will only play mp3, because it is high quality and compressed.
- Playlists will be saved locally and online.
- Only Korean, Chinese, Japanese, English and Thai songs for now.
- Dance practices for most songs included with priority for more popular songs.
- May make an account system to save and load your own playlists online.

## Problems solved:
- Todo: Hours of searching and editing music/videos -> Mere minutes of making a RPD playlist by importing your songlist or searching one by one.
- Storage space saved on your phone by streaming the songs.
- Save countless playlists, see how long they are and are easily editable.
- Todo: Easy way to search up the dance practice in one place and slow it down.
- Todo: Generate your own random play dance of X hours with certain settings, instead of going to the same old youtube videos.
- Random play dance by generation (sometimes people prefer certain kpop generations, because that is when they mostly listened to kpop).
- Random play dance by company, for example someone may be a SM stan and mostly listen to SM groups, e.g. NCT, aespa, Red Velvet, etc.
- Todo: Easy way to share your playlist with other people when they have the app or send them a textual list.
- Todo: Easy way to randomize your playlist with good variety.
- Todo: Language variety (Kpop, cpop, jpop, tpop)

# Feature summary
- Music Player app for (kpop) random play dances. Includes playing, pausing and moving the audio progress slider.
- Catalog of ~1500 songs and ~210 artists (1000 songs and 170 artists right now)
   -  Mostly choruses and pre-choruses, but also dance breaks/bridges and tiktok versions
- Create your own playlist, save it locally or online (todo: shareable)
- Balanced randomizer for any playlist which mixes grouptypes and TODO: artists.
- Todo: Generate your own random play dance playlist based on a few settings
- Todo: Voice clips in between songs (3,2,1, dance break)
- Multiple different filters/grouping: artist, company, clip length, generation, group type, language, song title
- Todo: Video player to learn dances with speed adjustment (0.5, 0.75 speed)

## Home 
<p float="left">
   <img src="https://github.com/giannistek1/rpd-images/blob/main/examples/home-1.jpg?raw=true" width="280">
   ----
   <img src="https://github.com/giannistek1/rpd-images/blob/main/examples/home-2.jpg?raw=true" width="280">
</p>

## Search library
<img src="https://github.com/giannistek1/rpd-images/blob/main/examples/search-song.jpg?raw=true" width="280">

## Sorting
<img src="https://github.com/giannistek1/rpd-images/blob/main/examples/order-by.jpg?raw=true" width="280">

## Playlists
<img src="https://github.com/giannistek1/rpd-images/blob/main/examples/playlists.jpg?raw=true" width="280">

## Current playlist
<img src="https://github.com/giannistek1/rpd-images/blob/main/examples/current-playlist.jpg?raw=true" width="280">

## Libraries
- CommunityToolkit.Maui.MediaElement by Microsoft -> audio and video processing
- Dropbox API by Dropbox -> online playlist storage
- Syncfusion by Syncfusion -> Better advanced ListView
- UraniumUI by Enisn -> For the tabview, bottomsheet and icons
- Sentry by Sentry -> Crashlytics/analytics

## Backlog dump 2024-09-03
- Existing songs 
- Louder:
- Lion louder
- Wife louder
- Run2u louder 
- Favorite louder?
- Illusion ateez louder
- Dazzling light louder
- Precious louder
- Clc helicopter louder

- Work it louder
- Limitles louder 
- Giants louder
- What type of x louder
- Highway to heaven louder
- 4 walls louder 

- Audio new:
- Chung ha love u
- Gleam new audio (live)
- Crazy new audio
- Senorita new audio
- The real heung new audio
- 2 baddies new music
- Gotta go chung ha new music (live)
- Oneus Luna new music
- Crazy over you bp new music
- Rock ur body new music, sounds old
- Sneakers new music
- Exo power new music (live) 
-----------------
- New songs
- Lit dance break
- No diggity db 

- Nct dream songs
- !!!Twice breakthrough, doughnut
- Bad girl snsd jp
- NiziU
- Xikers
- Henry
- AlexA
- Cignature
- Skz booster
- Sober dkb
- Playback
- Craxy
- Rowoon
- Ini
- Bibi 
- Drippin
- Evnne
- Verivery
- Water Ten
- Nu'est face
- Knock lee chae yeon
- Hare hare twice
- 4min
- T-ara
- Yedam
- Cross Gene
- Lovelyz (Woolim)
- Rocket punch (Woolim)
- Infinite songs (woolim)
- Golden Child songs (woolim)
- Eun-bi (woolim
- Chaeryeong
- Billie
- Chung ha
- Piwon
- 82Majo
- Izna
- ATBO
- Key songs
- Power 
- Lisa Rockstar, new woman
- Crazy - lsf
- StayC
- Enhypen
- Kard icky
- Kick it 4 now Just B
- TXT song
- Baggyubin satellite
- Ateez hush Hush 
- Txt open always wins
- Somi ice cream
- ARTMS
- Ilso
- Right now new jeans
- Lucas Renegade 
- Gidle my bag
- Hyolyn wait
- Papaya
- Diva
- Jewelry
- Secret
- After school
- Nowadays 
- Lun8
- Lightsum
- Evnne
- Me:i
- Jo Yuri
- Izna
- Old boygroups (teen top, big bang, suju, bap, etc)

- Features

- General
- Icon toggle: outline, grayed and red, and one is full and black green
- Rbw kara
- Mamamoo has mamamoo+?!
- Go to bottom button 
- Go to top button
- More Source artists: Eden, MIO,  Kan Miyoun, 8Eight, GLAM
- Grouped by "chorus" or "pre-chorus" or "dancebreak"
- Change font
- Song detail page
- Looping
- Titleok
- Shortlived token ophalen via textfile
- Animate three rectangles on listitem when playing(visible true?) 
- Next song by swipe gesture left 
- Announcements: Dance break, chorus X, verse, tiktok

- Notification channel 
- Android auto

- SearchSongPart
- Double tap video = single tap with video icon (grayed out or not)
- Order by release date: order by date within group
- Turn on/off grouping
- Bug: Searchfilter removes orderby?
- Bug: Orderby ignores searchfilter 
- AddAllResults update playlistscreen
- Looping

- Categories: like home screen
- Viewmode: treenode or listview
- Lazy loading?

- Home
- Settings button (for day6, the rose, etc), master volume
- User
- Expandable stats.
- Press filter to go to searchsong tab.
- Generate random playlist of half an hour, an hour, 1.5 hours etc. Eventually add details (bg/gg ratio, gens).
- Years category?

- Configurable filters
- Recent years: 2020, 2021, 2022, 2023, 2024

- Preconfigured playlists from dropbox (new/recent, popular, oldschool, etc)

- Currentplaylist
- Vertical bar at the start of the item with color of grouptype, maybe look into color uneven rows
- Checkbox -> cloud symbol
- Playlists subtabview (Local list, online list, currentplaylist)
- group balance shuffle, 
- Checkbox: add fade out/fade in between songs
- The 321 in between
- Looping

- VideoPage
- Mirrored label 
- Speed adjustment
- Details
- Landscape mode
- 8 Counts

- Playlists
- DataGrid

- Queue page or tabpage? Or mainpage attachment?


- Future: 
- What if... You could swipe a whole group so it gets added to the playlist?!
- Or tap to add (quick add) mode

- Website previews all songs
