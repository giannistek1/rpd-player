<img src="https://github.com/user-attachments/assets/2b9a0be7-818d-4201-95f9-0fdc7abb42f1" width="210"/>
<img src="https://github.com/user-attachments/assets/ab4721f0-c7d7-4a57-80ee-f8cd9a714040" width="210"/>
<img src="https://github.com/user-attachments/assets/088d3ed7-9880-4d34-8bdb-f90fe6d25aa0" width="210"/>
<img src="https://github.com/user-attachments/assets/3b5510c5-38aa-44b0-bde0-09b71e46e7d3" width="210"/>
<img src="https://github.com/user-attachments/assets/bcb09cc5-0b41-48cc-930f-caa1bc79f743" width="210"/>
<img src="https://github.com/user-attachments/assets/1d822825-acfb-47ff-944b-134a84cfb02f" width="210"/>



# Disclaimer
I do not own any rights to any of these songs, album images or dance videos. 
The songs were written and produced by their respective artists. 
The album images also belong to their respective artists.
The dance videos belong to their respective channel owners on YouTube or original agency of the artist.
I am simply cutting parts out of the songs to use in this free-to-use app.

# Rpd-player
This is a hobby/educational project I work on in my free time and hopefully helps people create random play dances. 

# Preconditions development
- Syncfusion version 28.1.33
- .NET 8 + .NET Maui (XAML) workload
- When you have a longpath: path > 260 characters exception, change your path to be shorter.

## Overview
- Context
- Scope
- Problems Solved
- Summary
- Home screen
- Search/library screen
- Playlists screen
- Current playlist

## Context
RPD stands for Random Play Dance. <br/>
A RPD is a social, international, dance event where people stand around a marked area (usually a large rectangle), where they should dance when they know the dance of the song that is playing. <br/>
A short countdown is played between each song so people have enough time to leave the area (so it is empty again) and prepare for the next song to dance to. Otherwise the transition would be very chaotic. <br/>

### More in-depth context
Tyically in South Korea they last an hour <br/>
However in other countries they may be for the whole afternoon from 12:00-18:00 with several short breaks and several different playlists of each 1-2 hours with different twists, other dancing minigames and/or different dance crews/groups covering mashups or full songs.

## Scope
- Platforms: Android, iOS and hopefully Windows but not looking promising so far.
- Music player only plays mp3, because it is high quality and compressed.
- Dance practice videos are mirrored in 480p (or 360p), so the videos will be max 15mb per video.
- Playlists are saved locally and online.
- Mostly Korean, some Chinese-pop, some Japanese-pop, English and Thai-pop songs for now.
- Todo: Dance practices for most songs included with priority for more popular songs.
- Future: May make an account system to save and load your own playlists online.
- Future: May add bpm per song to add countings and make animation based on bpm.

## Benefits:
- Time save: Hours of searching and editing music/videos -> Mere minutes of making a RPD playlist by importing your songlist or searching one by one.
- Save space: Storage space saved on your phone by streaming the songs.
- Library of playlists: You can have countless playlists, see their duration and they are easily editable.
- New songs every week.
- Favorite choreos.
- Mirrored dance practices in one place: Easy way to see the dance practice in ONE place and slow it down.
- Generate your own unique random play dance of 1-3 hours with certain settings, instead of going to the same old youtube videos.
- Random play dance by generation (sometimes people prefer certain kpop generations, because that is when they mostly listened to kpop).
- Random play dance by k-pop year.
- Random play dance by company, for example someone may be a SM stan and mostly listen to SM groups, e.g. NCT, aespa, Red Velvet, etc.
- Easy way to share your playlist with other people when they have the app or send them a textual list.
- Todo expand: Easy way to randomize your playlist with good variety.
- Todo: Language variety (Kpop, cpop, jpop, tpop)
- Songs are ~30% louder than usual, which is ideal for people with smaller speakers.
- No double song segments any more in your playlist.

# Feature summary
- Music Player app for (kpop) random play dances. Includes playing, pausing and moving the audio progress slider.
- Catalog of ~2000 songs and ~210 artists (1450 songs and 210 artists right now)
   -  Mostly choruses and pre-choruses, but also dance breaks/bridges and tiktok versions
   -  In-app song request function
- Create your own playlist, save it locally or online.
- Balanced randomizer for any playlist which mixes grouptypes and TODO: artists.
- Themes! Thanks to Daniel Hindrik's tutorial.
- Generate your own random play dance playlist based on a few settings
- Audio clips in between songs (3,2,1, todo: dance break)
- Multiple different filters/grouping: artist, company, clip length, generation, group type, language, song title
- Video player to learn dances and adjust video speed.

## Libraries
- CommunityToolkit by Microsoft -> Toasts
- CommunityToolkit.Maui.MediaElement by Microsoft -> Audio and video processing
- Dropbox API by Dropbox -> Online playlist storage
- Syncfusion by Syncfusion -> Better ListView, slider, tabview, expander and Windows support (however bottomsheet is sadly not a Windows feature)
- The49 BottomSheet -> For the bottomsheet
- Sentry by Sentry -> Crashlytics
- https://github.com/dhindrik/MauiThemes -> For themes

## Inspiration 
- Spotify
- Nintendo Music
- Kpop random play dance + video editors

## Rival apps?
- STEPIN - KPOP DANCE (100k users)
- Sparky: Learn Kpop Dance (50k users)

## Backlog dump 2025-11-10
iOS flaws:
Icon?
Splash?

Existing songs 
Louder:

Work it louder
Into the island louder
What type of x louder
Wakey wakey louder 
Highway to heaven louder
Moviestar louder
Pink venom less loud/new song
Boom boom louder
Billy poco louder 
Resolver louder?
Lalalay louder

Audio new
Chung ha love u
Gleam new audio (live)
Crazy new audio
Senorita new audio
The real heung new audio
Gotta go chung ha new music (live)
Oneus Luna new music (already have new)
Crazy over you bp new music/louder
Rock ur body new music, sounds old
Sneakers new music
Exo power new music (live)
You calling my name new music
Napal baji new music (live?)
Retro future new music
Touch my body live
Feelin like new music
Tomboy new music (inside)?
Txt crown new music
Kard tell my momma (live)
Dice
Beast - Good luck 

Videos
Bomb bomb kard
Lifes too short new video
Dazzling light new dance video (cant see all)

-----------------
New songs
Aespa - Rich man, Dirty Work
Ateez - hush Hush 
BSS (Seventeen) -
Chaeyoung (twice) - solo song
Chaeryeong (itzy) solo
Irene & Seulgi (Red velvet) - Tilt
Jennie - Like Jennie
Hearts2hearts - 
Katseye - Gabriela, Gnarly, Gameboy
Loona - Star, Flip that, Favorite? 365?
Oneus - Lit dance break, no diggity dance break
Seventeen - Echo, Super, Thunder, God of music
Stray Kids - In the water, I like it, Giant, booster
Twice - This is for
Twice - 123, fanfare
!!!Twice doughnut
Twice - Hare hare
Twice - One more time
Twice - wake me up
Twice - dive jp, celebrate jp, candy pop jp
Rose - apt?
Itzy - chillin 2x
Txt - new cb
Ten - nightwalker and more
Gdragon - heartbreak
Nmixx - 
Tbz comeback
!!!!!Bad girl snsd jp

82Majo
4min
AlexA
ARTMS
BTBT
BDU
Bibi
Billie
Bugaboo
Cignature
Craxy
Cross Gene
Diva
Drippin
Evnne
Henry
Ilso
Izna
Jewelry
Jo1
Jo Yuri
NiziU
Playback
Rescene
Rowoon
Soz
T-ara
TNX (THE NEW SIX)
TripleS
Verivery
WOOAH songs
Ini 
Loossemble
Yedam
Lovelyz (Woolim)
Rocket punch (Woolim)
Infinite songs (woolim)
Golden Child songs (woolim)
Eun-bi (woolim
ATBO
Exo first snow
Key songs
Refund sisters - dont touch me
Power 
Enhypen
Kard icky
Kick it 4 now Just B
TXT song
Txt open always wins
Super by seventeen
Hyolyn wait
Papaya
Secret
After school
btbt of bi
Nowadays 
Lun8
Lightsum
Me:i
Tribe
Old boygroups (teen top, big bang, suju, bap, etc)
Beast bad girl has a dancebreak but live and mv are different 
More Source artists: Eden, MIO,  Kan Miyoun, 8Eight, GLAM
Dutch artists: Chipz, k3, jekyl and hyde freefall

Features

General

Offline mode list
- Import fails as song request and save spelling mistakes (use for import) and then automate file name generation with chatgpt. 
Or even more ideal: search mirrored practice videos on youtube, and downloads them for you

Import songs to playlist (maybe add modes)
Advanced mode / simple mode (oldest, old kpop / new kpop)
Onboarding

Bug: Toolbar breaks after leaving app
!!!! Sentry on with new mediaelement?
Analytics?
Advanced fuzzy searching empty: did you mean: something similar
!!!! Trailmiddle bij items searchsongpart en currentplaylist 
Textlist version listview
Expand to K-rnb, k-rock etc
Marquee (scrolling) labels? 
User feedback page or request song with checkbox include dance practice.
IsInCurrentPlaylist
Icon toggle: outline, grayed and red, and one is full and black green
Spanish words filter (te quiero, Mamacita, senorita, etc)?
Skz2020 albums maken
!!!!hellevator = 6 october 2017
Disable buttons or actions when does nothing.
Artist ages/birthdates
Change font
Titleok
Announcements: Dance break, chorus X, verse, tiktok
Lightstick in detail screen
Stats with icons.

Notification channel 
Android auto
Settings:
----Non-choreo songs (for day6, the rose, rolling Quartz, etc)

User
Generate playlist eventually add details (bg/gg ratio, gens).

Configurable filters

Preconfigured playlists from dropbox (new/recent, popular, oldschool, etc)
Popularity list (group per year, song per year)
Generate mp3 with timestamps that can be read by the app with its own view.

SearchSongPart
CRASH: Scrollling fast, may be fixed?
Show group type: inotify
Advanced search: artist: aaa, title: sjdhd
Notification event property on IsPlaying
Turn on/off grouping
Bug: Searchfilter removes orderby?
If sorted by... Change search functionality to search group keys

Categories: like home screen
Grouping artist with colors (kinda done?)
Lazy loading?

Currentplaylist
Improve mix by calculating scale first by dividing bigger with smaller. If more than 2 then use 2 each for the biggest group.

Tabitem - Badge with current item count or in text. 
-----Library -> playlist name
Expander for advanced options (voices, countdown)
Playlists subtabview (Local list, online list, currentplaylist)
Detailed stats button
        Artist counts
        Chorus times/counts 
        Dancebreak counts 
Edit button
       Opens screen for removing certain songparts, 
     End with ending song
Artist balance shuffle
Flag: HasFadeOut
Int: CountdownMode
Flag: DoesLoop
Added date
Export/share:
   List
   Mp3
   Grouped by artist in alphabetical order

VideoPage
Landscape mode
Random / Autoplay
8 Counts? -> too hard

Songdetailpage
Video button
Add playlist/queue button
Swipe for next/previous song (make bottom sheet modal?)
Playing animation (like when you choose Music on Instagram)
Make album bounce (scale up and down)

Playlists
Timestamp sharing
Last  modified date
DataGrid?

Queue page?


Future:
Silent RPD/Start a jam in Spotify 
     Connect to main server that streams     music.
Better swipe function? 
!!!!Swipe mode: swipeended/changed/execute and autoclose

Or tap to add (quick add) mode

website previews all songs
