Foreword
===

If you already know C# and coded a bit in Unity you should be finde. If not, this could be a furstrating place to start learning.

What technologies are used
===
Skul itself is made with Unity. So most of the Unity C# API is also available to the modder: [Unity Monobehaviour](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html)

The Mod uses [BepInEx](https://github.com/BepInEx/BepInEx) which is build on top of [Harmony](https://github.com/pardeike/Harmony). They have a lot of documentation on how the stuff works and how it can be used.

How does the build work
===

The build requires 2 special things:
* Skul game files
* BepInEx dependencies 

### Skul game files

The `Skul.Mod.csproj` has a `SkulDirectory` entry.
The required gamefiles will be obtained from there and the build output of the mod will be copied there.

If you change the entry, you may have to restart your IDE

### BepInEx dependencies

The required NuGets are inside the "Package"-directory of this repository. The original source for these NuGets is: https://nuget.bepinex.dev/v3/index.json

The Build tries to get the NuGets from the "Package"-directory if possible, so you don't have to add the NuGets source manually.

Where to get help
===
First, consider the docs for [Unity](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html), [BepInEx](https://docs.bepinex.dev/master/) and [Harmony](https://harmony.pardeike.net/).

If that is not enough:
This mod was only possible with help from the smart and motivated people from the BepInEx Discord. If you need help on Unity-Modding, that would be a good place to start.