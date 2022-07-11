## Full Installation Guide

This is a more detailed description of things I did to get the mod working. If you just want to play with the mod, you don't have to read this.


## Install BepInEx in your Skul Root folder
Long general installation guide for BepInEx can be found [here](https://docs.bepinex.dev/master/articles/user_guide/installation/index.html).  
Shorter description for Skul below:

At the time of writing, BepInEx 6 has no stable release.  
The current builds can be found here: [BepisBuilds](https://builds.bepis.io/projects/bepinex_be).  

For Windows use: BepInEx_UnityMono_x64  
For Linux use: BepInEx_UnityMono_unix  

Extract the content of the archive into the Root folder of your Skul Installation. So that the Root folder of the game contains a "BepInEx" folder

For Windows usually: C:\Program Files\Steam\steamapps\common\Skul\
For Linux usually: /home/MyUserName/.steam/debian-installation/steamapps/common/Skul/

Linux Only:  
- modify `run_bepinex.sh` to set the `executable_name` on line 15 like this:  `executable_name="Skul.x86_64"`
- make `run_bepinex.sh` executable. To do that, run in terminal: `chmod u+x run_bepinex.sh`
- In Steam, under properties for Skul, set the Launch Options to `./run_bepinex.sh %command%`

## Build the Mod/PlugIn

Open the `Skul.Mod.csproj` and modify `<SkulDirectory>` to point to your Skul folder.  
Building the .net project will then build the .dll to `$(SkulDirectory)\BepInEx\plugins` where it will be automatically loaded by BepInEx

## Unstrip Unity dlls
Skul ships with stripped assemblies. ([for further reading](https://github.com/NeighTools/UnityDoorstop/issues/10#issuecomment-776921796))  
Downloading the unstripped assemblies and dropping them into `$(SkulDirectory)\Skul_Data\Managed\` fixes that (overwriting existing files as needed).  
The `Managed` folder can be backed up in case something goes wrong. Alternatively Steams "Verify integrity of game files" can be used to restore the modified files.

Long Description: [Guide by ghorsington](https://hackmd.io/@ghorsington/rJuLdZTzK)

Short Description:
- download https://unity.bepinex.dev/libraries/2020.3.22.zip and extract the content into `$(SkulDirectory)\Skul_Data\Managed\`
- go to the [Unity Download Archives](https://unity3d.com/get-unity/download/archive) and download the Unity [UnitySetup64-2020.3.22f1.exe](https://download.unity3d.com/download_unity/e1a7f79fd887/Windows64EditorInstaller/UnitySetup64-2020.3.22f1.exe) (Linux users should download the Windows Version)
- Install the Unity Editor and copy `$(UnityDirectory)/Editor/Data/MonoBleedingEdge/lib/mono/4.5` into `$(SkulDirectory)\Skul_Data\Managed\`.

## Run the game

If the mod is running correclty, pressing F1 while in the game will display a message above the player character.