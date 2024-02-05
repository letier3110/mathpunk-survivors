#!/bin/bash

# Check that at least one image path was provided
if [ $# -eq 0 ]; then
  echo "Usage: $0 <path_to_images>"
  exit 1
fi

NUM_IMAGES=10
COLS=3
ROWS=$(expr $NUM_IMAGES / $COLS + 1)
CANVAS=$(convert -size 0x0 xc:transparent PNG32:output.png)

# 1. get all images from the provided path (directory)
# 2. output them as a grid of images, each image resized to 100x100
# 3. remove temp files

echo "output.png created"
find $1 -type f \( -iname "*.jpg" -o -iname "*.jpeg" -o -iname "*.png" -o -iname "*.gif" -o -iname "*.bmp" -o -iname "*.tiff" \) -print0 | while read -d $'\0' file
do
  convert "$file" -resize 100x100^ -gravity center -background none -extent 100x100 PNG32:output.png +append -background none PNG32:output.png
done
mv output.png ../../atlas/output_tile.png
echo "output.png updated"
