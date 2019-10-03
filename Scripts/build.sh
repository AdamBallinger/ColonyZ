#! /bin/sh

executableName="ColonyZ-x64"

echo "Attempting to build $executableName for Windows"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/unity.log \
  -projectPath $(pwd) \
  -buildWindows64Player "$(pwd)/Build/windows/$executableName.exe" \
  -quit
  
echo "Build log."
cat $(pwd)/unity.log