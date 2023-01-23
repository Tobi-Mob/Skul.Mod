using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using GameResources;
using HarmonyLib;
using Level;
using Services;
using Singletons;

namespace Skul.Mod
{
    public static class DropRatePatch
    {
        public static bool Enabled { get; private set; } = true;
        private static ManualLogSource Logger => Helper.Logger;
        private static LevelManager levelManager => Helper.LevelManager;
        private static FloatingTextSpawner textSpawner => Helper.TextSpawner;
        
        /// <summary>
        /// If true, allows to drop Items which are already equipped
        /// </summary>
        public static bool AllowDuplicateItems = false;
        
        /// <summary>
        /// If true, allows to drop skulls which are already equipped
        /// </summary>
        public static bool AllowDuplicateSkulls = false;
        
        /// <summary>
        /// If true, scale dropped skulls with the current chapter 
        /// </summary>
        public static bool SkullsScaleWithChapter = true;
        
        /// <summary>
        /// Every N Item-Drops the quality of the dropped item is increased
        /// by one rarity level.
        /// -1 to disable
        /// </summary>
        public static int ItemUpgradeEveryXDrops = 4;
        
        private static int _itemDropCount = 0;
        
        #region GetWeaponToTakePrefix
        [HarmonyPatch(typeof(GearManager), "GetWeaponToTake", new Type[]{typeof(Random), typeof(Rarity)})]
        [HarmonyPrefix]
        static bool GetWeaponToTakePrefix(
            ref WeaponReference __result, 
            GearManager __instance, 
            Random random,
            Rarity rarity, 
            EnumArray<Rarity, WeaponReference[]> ____weapons, 
            List<Characters.Gear.Gear> ____weaponInstances)
        {
            if (!Enabled)
                return true;
            
            var weapons = ____weapons;
            
            if (SkullsScaleWithChapter)
                rarity = GetChapterRarity();
            
            Logger.LogInfo("GetWeaponToTakePrefix for rarity " + rarity);
            
            // Get all weapons up to the requested rarity
            IEnumerable<WeaponReference> possibleDrops = weapons[Rarity.Common];

            if (rarity >= Rarity.Rare)
                possibleDrops = possibleDrops.Concat(weapons[Rarity.Rare]);
            
            if (rarity >= Rarity.Unique)
                possibleDrops = possibleDrops.Concat(weapons[Rarity.Unique]);
            
            if (rarity >= Rarity.Legendary)
                possibleDrops = possibleDrops.Concat(weapons[Rarity.Legendary]);
            
            possibleDrops = possibleDrops.Where(w => w.unlocked && w.obtainable);
            
            // If enabled, do not drop items that are already equipped
            if (!AllowDuplicateSkulls)
            {
                HashSet<string> equipped = new HashSet<string>(____weaponInstances.Select(g => g.name));
                possibleDrops = possibleDrops.Where(item => !equipped.Contains(item.name));
            }

            List<WeaponReference> list = possibleDrops.ToList();

            if (list.Count > 0)
            {
                // get a random entry from the possible drops
                __result = list[random.Next(0, list.Count)];

                while (__result.rarity < rarity)
                {
                    Logger.LogInfo("Rarity to low. Upgrading");

                    // Load the weapon to find out its upgrade.
                    // TODO: possible without the load?
                    var request = __result.LoadAsync();
                    request.WaitForCompletion();

                    var realWeapon = request.asset;

                    if (realWeapon.upgradable)
                    {
                        __result = realWeapon.nextLevelReference;
                        Logger.LogInfo($"Upgraded to: " + __result.name);
                    }
                    else
                    {
                        Logger.LogInfo("Unable to upgrade " + __result.name + " further. Finding a new weapon");
                        return GetWeaponToTakePrefix(ref __result, __instance, random, rarity, ____weapons, ____weaponInstances);
                    }
                }
                
                Logger.LogInfo("Found weapon: " + __result.name );
                return false;
            }
            else
            {
                // this did not work. Let the unmodified code return a weapon
                Logger.LogWarning("no weapon found. lowering rarity");
                return true;
            }
        }
        #endregion
        
        #region GetItemToTakePrefix
        [HarmonyPatch(typeof(GearManager), "GetItemToTake", new Type[]{typeof(Random), typeof(Rarity)})]
        [HarmonyPrefix]
        static bool GetItemToTakePrefix(
            ref ItemReference __result, 
            GearManager __instance, 
            Random random,
            Rarity rarity, 
            EnumArray<Rarity, ItemReference[]> ____items, 
            List<Characters.Gear.Gear> ____itemInstances)
        {
            if (!Enabled)
                return true;
            
            if (ItemUpgradeEveryXDrops != -1)
            {
                ++_itemDropCount;
                Logger.LogInfo("Item drop Nr. " + _itemDropCount);

                if (_itemDropCount % ItemUpgradeEveryXDrops == 0)
                {
                    rarity = rarity + 1;
                    if (rarity < (Rarity.Legendary) + 1)
                    {
                        Logger.LogInfo($"Upgrading Item Quality to: " + rarity);
                    }
                    else
                    {
                        // limit reached
                        rarity = Rarity.Legendary;
                    }
                }

            }
            else
            {
                // option disabled
            }
            
            
            Logger.LogInfo("GetItemToTakePrefix for rarity '" + rarity + "'");

            var allItems = ____items;
            
            IEnumerable<ItemReference> possibleDrops = allItems[rarity];

            // all unlocked/obtainable items
            possibleDrops = possibleDrops.Where(w => w.unlocked && w.obtainable);

            // If enabled, do not drop items that are already equipped
            if (!AllowDuplicateItems)
            {
                HashSet<string> equipped = new HashSet<string>(____itemInstances.Select(g => g.name));
                possibleDrops = possibleDrops.Where(item => !equipped.Contains(item.name));
            }

            List<ItemReference> itemInfos = possibleDrops.ToList();

            if (itemInfos.Count > 0)
            {
                // get a random entry from the possible drops
                __result = itemInfos[random.Next(0, itemInfos.Count)];
                Logger.LogInfo("Found item: " + __result.name );
                return false;
            }
            else
            {
                Logger.LogWarning("no item found. lowering rarity");
                return GetItemToTakePrefix(ref __result, __instance, random, rarity - 1, ____items, ____itemInstances);
            }
        }
        #endregion
        
        #region GetQuintessenceToTakePrefix
        /* TODO: Works, but does nothing useful
        [HarmonyPatch(typeof(GearManager), "GetQuintessenceToTake", new Type[]{typeof(Random), typeof(Rarity)})]
        [HarmonyPrefix]
        static bool GetQuintessenceToTakePrefix(
            ref Resource.QuintessenceInfo __result, 
            GearManager __instance, 
            Random random,
            Rarity rarity, 
            EnumArray<Rarity, Resource.QuintessenceInfo[]> ____quintessences,
            List<Characters.Gear.Gear> ____essenceInstances)
        {
            Logger.LogInfo($"GetQuintessenceToTakePrefix for rarity '{rarity}'");

            var items = ____quintessences;
            
            // get all of the requested rarity
            IEnumerable<Resource.QuintessenceInfo> possibleDrops = items[rarity];
            
            // only allow unlocked and obtainable
            possibleDrops = possibleDrops.Where(q => q.unlocked && q.obtainable);
            
            // do not drop one that we already carry
            possibleDrops = possibleDrops.Where(q => !____essenceInstances.Any(equipped => equipped.name == q.name));

            List<Resource.QuintessenceInfo> itemInfos = possibleDrops.ToList();

            if (itemInfos.Count > 0)
            {
                // get a random entry from the possible drops
                __result = itemInfos[random.Next(0, itemInfos.Count)];
                Logger.LogInfo($"Found Quintessence: '{__result.name}'");
                return false;
            }
            else
            {
                Logger.LogWarning("no Quintessence found. lowering rarity");
                return true;
            }
        }
        */
        #endregion

        #region GetChapterRarity
        /// <summary>
        /// Returns a rarity depending on the current chapter.
        /// Later Chapters have higher rarity
        /// </summary>
        static Rarity GetChapterRarity()
        {
            Chapter.Type chapterType = Singleton<Service>.Instance.levelManager.currentChapter.type;
            
            Rarity rarity = Rarity.Common;
        
            switch (chapterType)
            {
                case Chapter.Type.Chapter2:
                    rarity = Rarity.Rare;
                    break;
                case Chapter.Type.Chapter3:
                    rarity = Rarity.Unique;
                    break;
                case Chapter.Type.Chapter4:
                    rarity = Rarity.Legendary;
                    break;
                case Chapter.Type.Chapter5:
                    rarity = Rarity.Legendary;
                    break;
            }
            
            Logger.LogInfo("Chapter type '" + chapterType + "' to rarity '" + rarity + "'");

            return rarity;
        }
        #endregion
        
        #region ToggleEnabled
        public static void ToggleEnabled()
        {
            Enabled = !Enabled;
            
            string text = Enabled ? "enabled" : "disabled";

            Helper.TextSpawner.SpawnBuff($"Item drop modifications {text}", Helper.Player.transform.position);
        }
        #endregion
    }
}