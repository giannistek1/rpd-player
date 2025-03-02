<img src="https://github.com/user-attachments/assets/48cbe87d-76e7-432b-8391-92054100d510" width="210"/>
<img src="https://github.com/user-attachments/assets/9311c7b9-dbf0-4167-a1ee-25e260732d12" width="210"/>
<img src="https://github.com/user-attachments/assets/bb066721-d1e1-40ee-9079-27be663bfbe4" width="210"/>
<img src="https://github.com/user-attachments/assets/4f8a94fc-e485-40bf-9e22-a397ba5574c0" width="210"/>

# Disclaimer
I do not own any rights to any of these songs, album images or dance videos. 
The songs were written and produced by their respective artists. 
The album images also belong to their respective artists.
The dance videos belong to their respective channel owners on YouTube.
I am simply cutting parts out of the songs to use in this free-to-use app.

# Rpd-player
This is a hobby project I work on in my free time. 

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
However in other countries they may be for the whole afternoon from 12:00-18:00 with several short breaks and several different playlists of each 1-2 hours with different twists, other dancing minigames and/or different dancecrews covering mashups or full songs.

## Scope
- Platforms: Android, iOS and hopefully Windows but not looking promising so far.
- Music player will only play mp3, because it is high quality and compressed.
- Playlists will be saved locally and online.
- Only Korean, Chinese-pop, Japanese-pop, English and Thai-pop songs for now.
- Todo: Dance practices for most songs included with priority for more popular songs.
- Future: May make an account system to save and load your own playlists online.
- Future: May add bpm per song to add countings and make animation based on bpm.

## Problems solved and benefits:
- Todo: Hours of searching and editing music/videos -> Mere minutes of making a RPD playlist by importing your songlist or searching one by one.
- Storage space saved on your phone by streaming the songs.
- Save countless playlists, see how long they are and are easily editable.
- Updating over time: Easy way to search up the dance practice in one place and slow it down.
- Todo: Generate your own random play dance of X hours with certain settings, instead of going to the same old youtube videos.
- Random play dance by generation (sometimes people prefer certain kpop generations, because that is when they mostly listened to kpop).
- Random play dance by k-pop year.
- Random play dance by company, for example someone may be a SM stan and mostly listen to SM groups, e.g. NCT, aespa, Red Velvet, etc.
- Todo: Easy way to share your playlist with other people when they have the app or send them a textual list.
- Todo: Easy way to randomize your playlist with good variety.
- Todo: Language variety (Kpop, cpop, jpop, tpop)
- Songs are ~30% louder than usual, which is ideal for people with smaller speakers.
- No double songparts any more in your playlist

# Feature summary
- Music Player app for (kpop) random play dances. Includes playing, pausing and moving the audio progress slider.
- Catalog of ~2000 songs and ~210 artists (1400 songs and 210 artists right now)
   -  Mostly choruses and pre-choruses, but also dance breaks/bridges and tiktok versions
   -  In-app song request function
- Create your own playlist, save it locally or online (todo: shareable)
- Balanced randomizer for any playlist which mixes grouptypes and TODO: artists.
- Themes! Thanks to Daniel Hindrik's tutorial.
- Todo: Generate your own random play dance playlist based on a few settings
- Todo: Voice clips in between songs (3,2,1, dance break)
- Multiple different filters/grouping: artist, company, clip length, generation, group type, language, song title
- Video player to learn dances with speed adjustment (0.5, 0.75 speed)

## Home 
<p float="left">
   <img src="https://github.com/user-attachments/assets/48cbe87d-76e7-432b-8391-92054100d510" width="280">
   ----
   <img src="https://github.com/user-attachments/assets/1036d522-289a-4e0f-a445-c4b903b5f531" width="280">
</p>

## Search library
<img src="https://github.com/user-attachments/assets/0809046d-cb0b-4b87-8cc0-c99c7c064904" width="280">

## Sorting
<img src="https://github.com/user-attachments/assets/90b22846-f75e-4efa-a80f-81b98307ae69" width="280">

## Playlists
<img src="https://github.com/user-attachments/assets/2dc8cf4c-37a3-4701-a3dd-892da8b6a1cc" width="280">

## Current playlist
<img src="https://github.com/user-attachments/assets/22d41c1f-2a3f-4a48-8507-712ac0b17d1f" width="280">

## Libraries
- CommunityToolkit by Microsoft -> Toasts
- CommunityToolkit.Maui.MediaElement by Microsoft -> audio and video processing
- Dropbox API by Dropbox -> Online playlist storage
- Syncfusion by Syncfusion -> Better ListView, slider, tabview, expander and Windows support (however bottomsheet is sadly not a Windows feature)
- The49 BottomSheet -> For the bottomsheet
- Sentry by Sentry -> Crashlytics
- https://github.com/dhindrik/MauiThemes -> For themes

## Inspiration 
- Spotify
- Nintendo Music

## Rival apps?
- STEPIN - KPOP DANCE (100k users)
- Sparky: Learn Kpop Dance (50k users)

## Backlog dump 2025-02-03
iOS flaws:
Icon?
Splash?

Windows
Medialement crash on song end?

Existing songs 
Louder:
Pink venom less loud/new song
Resolver louder?
Fake and true
Trouble maker 
Red light fx
Popin stat

Audio new
Oneus Luna new music (already have new)
Crazy over you bp new music/louder - hard to find
Check giants
Rock ur body new music, sounds old
You calling my name new music
No Celestial new music!!
Antifragile new music!!!
Blue orangeade
Kard tell my momma (live)
Dice
IVE - Love Dice (distorted)
Taemin - Idea (distorted)
XG - Shooting star (distorted?)
Beast - Good luck 
Napal baji new music (live?)
Gotta go chung ha new music (live)?
TT new music?
Retro future new music
Tomboy new music (inside)?
Txt crown new music?
Chung ha love u?

Videos
Bomb bomb kard
Lifes too short new video
Dazzling light new dance video (cant see all)

-----------------
New songs
Twice - dive jp, celebrate jp, candy pop jp,
!!!Twice doughnut
Twice 123, fanfare
Twice Hare hare, one more time, wake me up
Rose - drinks or coffee, apt?
Txt - open always wins, tinnitus
Refund sisters - dont touch me
Nct wish - steady
Apink - I dont know 
Tbz - Nectar, roar
Babymonster - really like you, billionaire
IVE - attitude
Hyunjin -  Good
Lee know - Youth
I.N. - hallucination
Viviz kep1er - purr
Lisa - moonlit floor
TripleS - 24, complexity
April - more songs
Jaehyun - Smoke
Skz - In the water, I like it, booster, giant, jjam
KIOF - Get loud
Jennie - ExtraL
Pixy - more songs
Wonho - With You, somebody
ATBO - Attitude
Kiikii - I do me
Bambam - Last Parade
Nct u - raise the roof, round and round
Hyolyn - wait
Ten - birthday, nightwalker and more songs
Seventeen - Super, hot
BoA - one shot two shot
Exo - first snow (very short TikTok)
Puárple kiss - Sweet juice and more
P1h - back down, sad song, jump
TNX (The new six) - fuego, kick it 4 now
Class:y - Shut down
Youha - Last dance
Blitzers - superpower, macarena
Youha - Last dance
Trendz - Go up
VCHA - Girls of the year
BI - Keep me up, tasty
8Turn - Excel
Boynextdoor - Nice Guy, Dangerous, luv you, I like you
Key - helium, more songs
Kickflip - umm great, mama said
Itzy solos 
Key songs
BTBT
JYP - groove back, like magic
BDU
Chaeryeong solos
Yedam - o-he, only one
Bugaboo
NiziU
Henry
Soz
Jo1
More WOOAH songs
Key - Tongue Tied jp
AlexA
TTS - Adrenaline 
Shinee body rhythm
Kard icky
birthday party
Cignature
Playback more songs
Wheein Easy and cocowater
Craxy
Rowoon
Ini
Bibi 
Drippin
Evnne
Verivery
nu'est more songs?
Knock lee chae yeon
Gen 2:
4min
Block B more songs
T-ara
More Billie songs 
Cross Gene
Lovelyz (Woolim)
Rocket punch (Woolim)
Infinite songs (woolim)
Golden Child songs (woolim)
Eun-bi (woolim
82Majo
Izna
ATBO
Power 
Enhypen
Minnie solo
Ilso
Papaya
Diva
Jewelry
Secret
After school
btbt of bi
Nowadays 
Lun8
Lightsum
Limelight
Evnne
Me:i
Nct wish (nct new team)
Tri.be more songs
Old boygroups (teen top, big bang, suju, bap, etc)
Beast bad girl has a dancebreak but live and mv are different 
Kpop songs with! Can be with ! Like Go Crazy 2PM. 
More Source artists: Eden, MIO,  Kan Miyoun, 8Eight, GLAM
Dutch artists: Chipz, k3, jekyl and hyde 
freefall

Features

Onboarding screen
   Genres?
   Companies? (Highlight preferences?)
   Boy, girl or both? (Change homeview categories based on this?)
   Artists? (Highlight in artistcollectionview?)
   Username

Store setup
URL nodig 
Music app onderzoek qua plaatjes en copyright 

General
High 4: Save rpd settings
Idk: Rpdsettings in expander
High 2: IsInCurrentPlaylist
High 4: User feedback page or request song with checkbox include dance practice.
Med 7: Two mediaelements (experimental) for seamless loading
High 5: Generate random playlist x hours and eventually add details (bg/gg ratio, gens).
Bug: Toolbar breaks after a while of not using the app
Medium 3: Shuffle mode decides beforehand, so next song can be filled
Med 2: Add icons to headers
Med 4: Injection for singleton managers
High: Home revamp:
   Tabs: Recent/explore playlists
    Titletracks?
    Show artistcollection
    Scales songparts
    Scales male/female
    Adv: Scales artist
     Mix male/female
   
Med 3: Expand to k-rock etc
Med 3: Handle Unhandled exceptions.
Med 3: Spanish words searchfilter (te quiero, Mamacita, senorita, lo siento, etc)?
Med 2: Skz2020 albums maken, hellevator = 2017
Med 4: Know the dance 
Med 6: Disable buttons or actions when does nothing.
Low 5: Homeview 3 pages?
Low 5: Artist ages/birthdates
Low 4: Change font
Med: Difficulty user based
Low 4: Textlist version listview
Low 4: Expand buttonimages and add paddings
Low ??: Titleok
Shortlived token ophalen via textfile
Announcements: Dance break X, chorus X, verse, tiktok
Low 6: Lightstick in detail screen
Low 6: Rating system (popularity 
Low 4: fuzzy searching empty: did you mean: something similar 3 actions, add, replace, remove. 
Med 7: Hard Notification channel 
Low 7: Hard but doable: Android auto
Settings:
----Non-choreo songs: Day6, the rose, rolling Quartz, etc

User

Configurable filters

Preconfigured playlists from dropbox (new/recent, popular, oldschool, etc)
Popularity list (group per year, song per year)
Generate mp3 with timestamps that can be read by the app with its own view.

SearchSongPart
High 2: Releasedate/yearlydate device culture dependent
Med 2: Clear category filter if category applied
Med 2: Press random with playlist mode = clear (playlist)queue
Low 2: Sorting turn off grouptype colors/INotify on grouptype
CRASH: Scrollling fast, may be fixed?
Notification property on IsPlaying
Bug: Searchfilter removes orderby?
If sorted by... Change search functionality to search group keys or add search keys

Low 6: Categories: like home screen
Low 5: Grouping artist with colors (kinda done?)
Low 6: Lazy loading?

Low 8: Future: Match lyrics with song for chorus generation + check voor drop (geen geluid)
Low 6: Advanced search: artist: aaa, title: sjdhd

Currentplaylist
High 2: Lock button
High 2: Remove confirmation
High 1: Added date
High 3: Countdown field
High 5: Artist balance shuffle
High 5: Playlists subtabview (Local list, online list, currentplaylist)
High 8: Export/share:
   List
   Mp3
   Grouped by artist in alphabetical order
Med 2: Tabitem - Badge with current item count. 
-----Library -> playlist name
Expander for advanced options (voices, countdown)
Med 2: Flag: DoesLoop
Med 3: Detailed stats button
        Artist counts
        Chorus times/counts 
        Dancebreak counts 
Med 4: Undo button 
Low 4: End with ending song
Low 3: Flag: HasFadeOut

VideoPage
Play pause button
Landscape mode
Random mode
Autoplay
8 Counts? -> too hard

Songdetailpage
High 5: Add to playlist/queue button
High 3: Lock button to not accidentally go next, mute or change settings 
Med 2: Extra data as CSV?

Med 3: Video button
Med 3: Replay 5, Forward 5
Med 2: Restart song button
press next/previous or FF/Reverse.
Swipe for next/previous song (make bottom sheet modal?)
Drag for next?
Playing animation (like when you choose Music on Instagram)
Low 3: Make album bounce (scale up and down)

Playlists
High 4: DataGrid
High 2: Create date
Med 2: Last modified date

Med: Queue page
Med: History page


Future
Low: Silent RPD/Start a jam in Spotify 
     Connect to main server that streams     music.
Low: Chorus Lyrics 

Or tap to add (quick add) mode

website previews all songs



Pricing
4 yes question
3 day trial basic/pro version
Trial over? Buy license (add mac-address)
Stripe/Paypal/Bank Transfer
Basic - basic functionality
Pro - extra functionality
   Extra themes (more colors)
   Custom tune
   Super seamless rpd (two mediaelements)
   Extra detail functionality (5 sec forward/backward)
   Features
   Songs
Ipa via mac

First name, last name or username
Send me your phone's mac address and you can use the app. Pair address with trial, basic or pro functionality
Phone checks mac address and date and you can use the app.
- 
- Future: 
- What if... You could swipe a whole group so it gets added to the playlist?!
- Or tap to add (quick add) mode
- 
- website previews all songs
