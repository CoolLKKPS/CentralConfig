using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using WeatherRegistry;

namespace CentralConfig
{
    public static class WRCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("mrov.WeatherRegistry");
                return (bool)_enabled;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static List<string> GetAllWeathersWithWR()
        {
            return WeatherManager.Weathers.Select(weather => weather.Name).ToList();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static float GetWRWeatherMultiplier()
        {
            return WeatherManager.GetCurrentLevelWeather().ScrapAmountMultiplier;
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void RemoveWRScrapMultiplierHardSet()
        {
            Settings.ScrapMultipliers = false;
        }
    }
    public static class ImperiumCompatibility
    {
        private static bool? _enabled;
        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("giosuel.Imperium");
                return (bool)_enabled;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool ImperiumTimePaused()
        {
            return Imperium.Imperium.MoonManager.TimeIsPaused.Value;
        }
    }
    public static class NPLCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("LCNoPropsLost");
                return (bool)_enabled;
            }
        }
    }
    public static class KirSSIODCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Kirpichyov.SaveShipItemsOnDeath");
                return (bool)_enabled;
            }
        }
    }
    public static class BobKSCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Bob123.LCM_KeepScrap");
                return (bool)_enabled;
            }
        }
    }
    public static class NDDCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("NoDeathDespawn");
                return (bool)_enabled;
            }
        }
    }
    public static class KSCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("KeepScrap");
                return (bool)_enabled;
            }
        }
    }
    public static class LUCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Kyxino.LethalUtils");
                return (bool)_enabled;
            }
        }
        public static bool ReturnLUScrapKeeper()
        {
            if (LethalUtilities.RoundSettings.LoseUponDeath.Value == LethalUtilities.UponDeathShipLoses.Nothing)
                return true;
            else
                return false;
        }
    }
    public static class FMCompatibility
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("me.loaforc.facilitymeltdown");
                return (bool)_enabled;
            }
        }
    }
    public static class LBCompatability
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Xilef.LethalBestiary");
                return (bool)_enabled;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static List<EnemyType> AddLBEnemies(List<EnemyType> enemies)
        {
            foreach (LethalBestiary.Modules.Enemies.SpawnableEnemy spawnableEnemy in LethalBestiary.Modules.Enemies.spawnableEnemies)
            {
                if (!enemies.Contains(spawnableEnemy.enemy))
                {
                    enemies.Add(spawnableEnemy.enemy);
                    // CentralConfig.instance.mls.LogMessage($"Added enemy: {spawnableEnemy.enemy.enemyName} from LethalBestiary");
                }
            }
            return enemies;
        }
    }
    public static class DiversityCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Chaos.Diversity");
                return (bool)_enabled;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static List<EnemyType> AddWalker(List<EnemyType> enemies)
        {
            EnemyType Walker = DiversityRemastered.ContentLoader.MainAssets.LoadAsset<EnemyType>("Assets/custom/diversity/walker/WalkerType.asset");
            if (!enemies.Contains(Walker))
            {
                enemies.Add(Walker);
                // CentralConfig.instance.mls.LogMessage($"Added enemy: {Walker.enemyName} from Diversity");
            }
            return enemies;
        }
    }
    public static class PintoBoyCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Pinta.PintoBoy");
                return (bool)_enabled;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void RenamePintoBois()
        {
            PintoMod.Pinto_ModBase.itemPintoBoyLJ.itemName = "PintoBoyLJ";
            PintoMod.Pinto_ModBase.itemPintoBoyFD.itemName = "PintoBoyFD";
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static string PBSIWRTS(SpawnableItemWithRarity item)
        {
            string returnString = string.Empty;
            if (item.spawnableItem == PintoMod.Pinto_ModBase.itemPintoBoyLJ && item.rarity > 0)
            {
                returnString = "PintoBoyLJ:" + item.rarity.ToString() + ",";
            }
            else if (item.spawnableItem == PintoMod.Pinto_ModBase.itemPintoBoyFD && item.rarity > 0)
            {
                returnString = "PintoBoyFD:" + item.rarity.ToString() + ",";
            }
            else
            {
                if (item.rarity > 0)
                    returnString = ConfigAider.LightlyToastString(item.spawnableItem.itemName) + ":" + item.rarity.ToString() + ",";
            }
            return returnString;
        }
    }
    public static class FootBallCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Kittenji.FootballEntity");
                return (bool)_enabled;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static List<EnemyType> AddFootball(List<EnemyType> enemies)
        {
            EnemyType Football = Kittenji.FootballEntity.Plugin.EnemyDef;
            if (!enemies.Contains(Football))
            {
                enemies.Add(Football);
                // CentralConfig.instance.mls.LogMessage($"Added enemy: {Football.enemyName} from FootBall");
            }
            return enemies;
        }
    }
    public static class PeeperCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("x753.Peepers");
                return (bool)_enabled;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static List<EnemyType> AddPeeper(List<EnemyType> enemies)
        {
            EnemyType Peeper = LCPeeper.Peeper.PeeperType;
            if (!enemies.Contains(Peeper))
            {
                enemies.Add(Peeper);
                // CentralConfig.instance.mls.LogMessage($"Added enemy: {Peeper.enemyName} from Peepers");
            }
            return enemies;
        }
    }
    public static class RollingGiantCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("nomnomab.rollinggiant");
                return (bool)_enabled;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static List<EnemyType> AddRollingGiant(List<EnemyType> enemies)
        {
            EnemyType RollingGiantInside = RollingGiant.Plugin.EnemyTypeInside;
            RollingGiantInside.enemyName = "Rolling Giant Inside";
            EnemyType RollingGiantDay = RollingGiant.Plugin.EnemyTypeOutsideDaytime;
            RollingGiantDay.enemyName = "Rolling Giant Day";
            EnemyType RollingGiantNight = RollingGiant.Plugin.EnemyTypeOutside;
            RollingGiantNight.enemyName = "Rolling Giant Night";
            if (!enemies.Contains(RollingGiantInside))
            {
                enemies.Add(RollingGiantInside);
                // CentralConfig.instance.mls.LogMessage($"Added enemy: {RollingGiantInside.enemyName} from RollingGiant");
            }
            if (!enemies.Contains(RollingGiantDay))
            {
                enemies.Add(RollingGiantDay);
                // CentralConfig.instance.mls.LogMessage($"Added enemy: {RollingGiantDay.enemyName} from RollingGiant");
            }
            if (!enemies.Contains(RollingGiantNight))
            {
                enemies.Add(RollingGiantNight);
                // CentralConfig.instance.mls.LogMessage($"Added enemy: {RollingGiantNight.enemyName} from RollingGiant");
            }
            return enemies;
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static string RGSEWRTS(SpawnableEnemyWithRarity enemy)
        {
            string returnString = string.Empty;
            if (enemy.enemyType == RollingGiant.Plugin.EnemyTypeInside && enemy.rarity > 0)
            {
                returnString = "Rolling Giant Inside:" + enemy.rarity.ToString() + ",";
            }
            else if (enemy.enemyType == RollingGiant.Plugin.EnemyTypeOutsideDaytime && enemy.rarity > 0)
            {
                returnString = "Rolling Giant Day:" + enemy.rarity.ToString() + ",";
            }
            else if (enemy.enemyType == RollingGiant.Plugin.EnemyTypeOutside && enemy.rarity > 0)
            {
                returnString = "Rolling Giant Night:" + enemy.rarity.ToString() + ",";
            }
            else
            {
                if (enemy.enemyType.enemyName != "Lasso" && enemy.rarity > 0)
                {
                    returnString = ConfigAider.LightlyToastString(enemy.enemyType.enemyName) + ":" + enemy.rarity.ToString() + ",";
                }
            }
            return returnString;
        }
    }
    [HarmonyPatch(typeof(StartOfRound), "ShipLeave")]
    public class ShipleaveCalc
    {
        public static int RemainingScrapInLevel;
        [HarmonyPostfix]
        public static void Postfix()
        {
            RemainingScrapInLevel = CalculateRemainingScrapInLevel();
        }

        public static int CalculateRemainingScrapInLevel()
        {
            GrabbableObject[] array = UnityEngine.Object.FindObjectsByType<GrabbableObject>(UnityEngine.FindObjectsSortMode.None);
            int num = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].itemProperties.isScrap && !array[i].isInShipRoom && !array[i].isInElevator && !array[i].scrapPersistedThroughRounds)
                {
                    num += array[i].scrapValue;
                }
            }
            return num;
        }
    }
    [HarmonyPatch(typeof(HUDManager), "FillEndGameStats")]
    public class HUDManagerPatch
    {
        [HarmonyPostfix]
        public static void FillEndGameStatsPostfix(HUDManager __instance, int scrapCollected)
        {
            float num = scrapCollected + ShipleaveCalc.RemainingScrapInLevel;
            __instance.statsUIElements.quotaDenominator.text = num.ToString();
        }
        [HarmonyPrefix]
        public static void UpdateTotalScrap(HUDManager __instance, int scrapCollected)
        {
            float num = scrapCollected + ShipleaveCalc.RemainingScrapInLevel;
            RoundManager.Instance.totalScrapValueInLevel = num;
        }
    }
}