@echo off
SETLOCAL ENABLEDELAYEDEXPANSION
set /p version=<VERSION.txt
mkdir tmp
cd tmp
mkdir RandomTweaksFontModule
cp ../Info_Font.json RandomTweaksFontModule/Info.json
cp ../FontModule/bin/Release/RandomTweaksFontModule.dll RandomTweaksFontModule

mkdir RandomTweaksMiscModule
cp ../Info_Misc.json RandomTweaksMiscModule/Info.json
cp ../MiscModule/bin/Release/RandomTweaksMiscModule.dll RandomTweaksMiscModule

mkdir RandomTweaksPlayingModule
cp ../Info_Playing.json RandomTweaksPlayingModule/Info.json
cp ../PlayingModule/bin/Release/RandomTweaksPlayingModule.dll RandomTweaksPlayingModule

cd RandomTweaksFontModule
for /f "delims=" %%a in (Info.json) do (
    SET s=%%a
    SET s=!s:$VERSION=%version%!
    echo !s!
) >>"InfoChanged.json"
rm Info.json
mv InfoChanged.json Info.json
cd ..

cd RandomTweaksMiscModule
for /f "delims=" %%a in (Info.json) do (
    SET s=%%a
    SET s=!s:$VERSION=%version%!
    echo !s!
) >>"InfoChanged.json"
rm Info.json
mv InfoChanged.json Info.json
cd ..

cd RandomTweaksPlayingModule
for /f "delims=" %%a in (Info.json) do (
    SET s=%%a
    SET s=!s:$VERSION=%version%!
    echo !s!
) >>"InfoChanged.json"
rm Info.json
mv InfoChanged.json Info.json
cd ..

tar -a -c -f RandomTweaks-%version%.zip RandomTweaksFontModule RandomTweaksMiscModule RandomTweaksPlayingModule
mv RandomTweaks-%version%.zip ..
cd ..
rm -rf tmp
pause