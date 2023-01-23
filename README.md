# Mod for Skul: The Hero Slayer

Some Modifications to make the game even more enjoyable for me.

Using [BepInEx](https://github.com/BepInEx/BepInEx) as modding framework

---

Features
===

- TurboButtonMode for basic attacks (F2 Key).
   - Holding down the attack button attacks as fast as possible.
- Skull drops scale with current chapter (F3 Key).
   - Later chapters always drop higher rarity skulls.
   - Awakened versions of skulls can drop.
- Modify gates (F4 Key)
   - Always spawn a Skull-Reward and a Item-Reward gate, if possible.
   - No broken gates will be spawned, if avoidable.
- Spawn DarkQuartz to unlock upgrades (F5 Key).
- Item drops configurable to drop a higher quality item every N drops.

Compability
===
- Skul Version 1.7.1
- Skul Unity Version 2020.3.34
- BepInEx Version 6.0.0.549

Installation
===

`$(SkulDirectory)` would be the folder where the Skul.exe is. 
For Windows it usually is: `C:\Program Files (x86)\Steam\steamapps\common\Skul` or something like that.

* Extract [Unstripped Unity files 2020.3.34](https://unity.bepinex.dev/libraries/2020.3.34.zip) into `$(SkulDirectory)\Skul_Data\Managed\`

* Extract [Unstripped corelibs 2020.3.34](https://unity.bepinex.dev/corlibs/2020.3.34.zip) into `$(SkulDirectory)\Skul_Data\Managed\`

* Extract [BepInEx 6.0.0.549](https://builds.bepinex.dev/projects/bepinex_be/549/BepInEx_UnityMono_x64_f2c0e0f_6.0.0-be.549.zip) into `$(SkulDirectory)`

* Extract [Skul.Mod.zip](https://github.com/Tobi-Mob/Skul.Mod/releases) into `$(SkulDirectory)`

* Run the game via Steam. If the mod is running correclty, pressing F1 while in the game will display a message above the player character.

There are more detailed Instructions available here: [Full Installation Guide](Docs/Full-Installation-Guide.md)

How to build the mod yourself
===

Before you continue here, do everything in the `Installation` section first.

With [git](https://github.com/git-guides/install-git) and [dotnet](https://dotnet.microsoft.com/en-us/download/dotnet) installed these commands should build the mod:

```
git clone https://github.com/Tobi-Mob/Skul.Mod.git
cd Skul.Mod
dotnet build
```

If no errors are thrown, the new build mod was installed into the SkulDirectory.


More infos on how this mod works and how it can be changed are here: [how to mod](Docs/How-To-Mod.md)

Future Ideas
===

- Start in a later chapter
   - Skip lower Chapters and jump right into higher action.
   - Give some start equipment matching the new start chapter.
- Configurable random seed
   - It should be possible to give a fixed seed to the used System.Random to replay a given seed.
- Configurable starting gear
- Configurable drops for a run
- Boss rush
   - Is could be enough to postfix `StageInfo.GeneratePath` and just remove all non boss segments.   
