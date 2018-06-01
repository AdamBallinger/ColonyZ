echo 'Downloading from https://download.unity3d.com/download_unity/21ae32b5a9cb/MacEditorInstaller/Unity-2017.4.3f1.pkg'
curl -o Unity.pkg https://download.unity3d.com/download_unity/21ae32b5a9cb/MacEditorInstaller/Unity-2017.4.3f1.pkg

echo 'Installing Unity.pkg'
sudo installer -dumplog -package Unity.pkg -target /