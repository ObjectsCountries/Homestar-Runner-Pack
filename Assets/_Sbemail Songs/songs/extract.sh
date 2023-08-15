#!/bin/bash

sbemails=(6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 28 29 30 31 34 35 37 38 41 42 47 48 49 51 52 53 54 56 57 61 62 64 65 67 68 72 73 76 78 81 83 84 85 88 91 92 93 94 95 101 103 105 107 110 115 117 119 123 142 149 166 170 172 181 189 'twohundred' 202)

total_time=0
total_parts=0

for i in ${sbemails[@]}; do
  start_time=$(date +%s.%N)
  wget "https://old.homestarrunner.com/sbemail$i.swf"
  echo "Downloading sbemail $i…"
  swfextract -m -o AUDIO_ss_$i.mp3 sbemail$i.swf
  echo "Extracting audio…"
  rm sbemail$i.swf
  ffmpeg -i AUDIO_ss_$i.mp3 -c:a libvorbis -q:a 4 -t 5 AUDIO_ss_$i.ogg
  echo "Converting to ogg…"
  end_time=$(date +%s.%N)
  elapsed_time=$(echo "$end_time - $start_time" | bc)
  total_time=$(echo "$total_time + $elapsed_time" | bc)
  total_parts=$((total_parts + 1))
done

start_time=$(date +%s.%N)
wget "https://old.homestarrunner.com/hremail3184.swf"
echo "Downloading sbemail 201…"
swfextract -m -o AUDIO_ss_201.mp3 hremail3184.swf
echo "Extracting audio…"
rm hremail3184.swf
ffmpeg -i AUDIO_ss_201.mp3 -c:a libvorbis -q:a 4 -t 5 AUDIO_ss_201.ogg
echo "Converting to ogg…"
end_time=$(date +%s.%N)
elapsed_time=$(echo "$end_time - $start_time" | bc)
total_time=$(echo "$total_time + $elapsed_time" | bc)
total_parts=$((total_parts + 1))

average_time=$(echo "scale=2; $total_time / $total_parts" | bc)
echo "Average time per part: $average_time seconds"
