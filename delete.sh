find . -name "*conflicted*" -print0 | xargs -0 -I {} rm {}
find . -name "*問題*" -print0 | xargs -0 -I {} rm {}
