#!/bin/bash

set -e

sbemails=(24 34 41 56 78 88 110 119 'twohundred' 202)

for i in ${sbemails[@]}; do
  wget "https://old.homestarrunner.com/sbemail$i.swf"
  swfextract -m -o AUDIO_ss_$i.mp3 sbemail$i.swf
  ffmpeg -i AUDIO_ss_$i.mp3 -c:a libvorbis -q:a 4 AUDIO_ss_$i.ogg
  rm sbemail$i.swf AUDIO_ss_$i.mp3
done

wget 'https://old.homestarrunner.com/hremail3184.swf'
swfextract -m -o AUDIO_ss_201.mp3 hremail3184.swf
ffmpeg -i AUDIO_ss_201.mp3 -c:a libvorbis -q:a 4 AUDIO_ss_201.ogg
rm hremail3184.swf AUDIO_ss_201.mp3
