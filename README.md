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

## Supported platforms
Android, iOS (Todo), Windows

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
