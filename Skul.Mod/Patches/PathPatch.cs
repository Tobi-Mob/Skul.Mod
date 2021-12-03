using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using Level;
using Services;
using Singletons;

namespace Skul.Mod
{
    public static class PathPatch
    { 
        private static ManualLogSource Logger => Helper.Logger;
        private static LevelManager levelManager => Helper.LevelManager;
        private static FloatingTextSpawner textSpawner => Helper.TextSpawner;
        
        #region GeneratePath
        [HarmonyPatch(typeof(StageInfo), "GeneratePath")]
        [HarmonyPostfix]
        static void GeneratePath(
            StageInfo __instance,
            (PathNode type1, PathNode type2)[] ____path)
        {
            Logger.LogInfo("GeneratePath invoked");
            
            Logger.LogInfo($"Got {____path.Length} paths");
            
            // Check all Paths that were generated by the base logic
            for (int i = 0; i < ____path.Length; i++)
            {
                var pair = ____path[i];

                try
                {
                    Logger.LogInfo($"[Path {i}.0] {pair.type1.reference?.path}|{pair.type1.reference?.type}|{pair.type1.gate}|{pair.type1.reward}");
                    Logger.LogInfo($"[Path {i}.1] {pair.type2.reference?.path}|{pair.type2.reference?.type}|{pair.type2.gate}|{pair.type2.reward}");

                    // If we got a normal map segment at this path
                    // we can change one path to a Item and one path to a Grave
                    if (pair.type1.reference?.type == Map.Type.Normal)
                    {
                        pair.type1.gate = Gate.Type.Grave;
                        pair.type1.reward = MapReward.Type.Head;
                    }

                    if (pair.type2.reference?.type == Map.Type.Normal)
                    {
                        pair.type2.gate = Gate.Type.Chest;
                        pair.type2.reward = MapReward.Type.Item;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                }
            }
        }
        #endregion
    }
}