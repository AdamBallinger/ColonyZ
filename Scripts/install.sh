#! /bin/sh
echo 'Downloading from http://beta.unity3d.com/download/f2970305fe1c/MacEditorInstaller/Unity-2019.1.6f1.pkg'
curl -o Unity.pkg http://beta.unity3d.com/download/f2970305fe1c/MacEditorInstaller/Unity-2019.1.6f1.pkg

echo 'Installing Unity.'
sudo installer -dumplog -package Unity.pkg -target /