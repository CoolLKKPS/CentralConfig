using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using HarmonyLib;
using LethalLevelLoader;
using LethalLevelLoader.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;

namespace CentralConfig
{
    [HarmonyPatch(typeof(HangarShipDoor), "Start")]
    [HarmonyPriority(666)]
    public class WaitForMoonsToRegister
    {
        public static CreateMoonConfig Config;

        public static SpawnableMapObject turretContainerObjectReference = null;
        public static SpawnableMapObject landmineObjectReference = null;
        public static SpawnableMapObject spikeRoofTrapHazardObjectReference = null;

        public static List<SpawnableEnemyWithRarity> IEnemies = new List<SpawnableEnemyWithRarity>();
        public static List<SpawnableEnemyWithRarity> DEnemies = new List<SpawnableEnemyWithRarity>();
        public static List<SpawnableEnemyWithRarity> NEnemies = new List<SpawnableEnemyWithRarity>();
        public static List<SpawnableItemWithRarity> Scrap = new List<SpawnableItemWithRarity>();

        [DataContract]
        public class CreateMoonConfig : ConfigTemplate
        {
            public static ConfigFile _cfg;
            // Declare config entries tied to the dictionary

            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> RoutePriceOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<string>> RiskLevelOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<string>> DescriptionOverride;

            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> MinScrapOverrides;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> MaxScrapOverrides;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<float>> ScrapValueMultiplier;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<string>> ScrapListOverrides;

            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> InteriorEnemyPowerCountOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<string>> InteriorEnemyOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> DaytimeEnemyPowerCountOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<string>> DaytimeEnemyOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> NighttimeEnemyPowerCountOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<string>> NighttimeEnemyOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<float>> SpawnSpeedScaler;

            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> MinTurretOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> MaxTurretOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> MinMineOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> MaxMineOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> MinSpikeTrapOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<int>> MaxSpikeTrapOverride;

            // [DataMember] public static Dictionary<string, SyncedEntry<string>> WeatherTypeOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<string>> AddTags;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<bool>> VisibleOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<bool>> LockedOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<bool>> TimeOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<float>> TimeMultiplierOverride;
            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<bool>> WatiForShipToLandBeforeTimeMoves;

            [DataMember] public static Dictionary<ExtendedLevel, SyncedEntry<float>> FaciltySizeOverride;

            public static Dictionary<ExtendedLevel, float> MoonsNewScrapMultiplier = new Dictionary<ExtendedLevel, float>();

            [DataMember] public static SyncedEntry<string> BigInteriorList { get; set; }
            [DataMember] public static SyncedEntry<string> BigDayTimeList { get; set; }
            [DataMember] public static SyncedEntry<string> BigNightTimeList { get; set; }

            [DataMember] public static SyncedEntry<string> AddIndoorEnemiesToAllMoons { get; set; }
            [DataMember] public static SyncedEntry<string> AddDayEnemiesToAllMoons { get; set; }
            [DataMember] public static SyncedEntry<string> AddNightEnemiesToAllMoons { get; set; }
            [DataMember] public static SyncedEntry<string> AddScrapToAllMoons { get; set; }
            [DataMember] public static SyncedEntry<string> ReplaceIndoorEnemiesOnAllMoons { get; set; }
            [DataMember] public static SyncedEntry<string> ReplaceDayEnemiesOnAllMoons { get; set; }
            [DataMember] public static SyncedEntry<string> ReplaceNightEnemiesOnAllMoons { get; set; }
            [DataMember] public static SyncedEntry<string> MultiplyIndoorEnemiesOnAllMoons { get; set; }
            [DataMember] public static SyncedEntry<string> MultiplyDayEnemiesOnAllMoons { get; set; }
            [DataMember] public static SyncedEntry<string> MultiplyNightEnemiesOnAllMoons { get; set; }
            public CreateMoonConfig(ConfigFile cfg) : base(cfg, "CentralConfig", 0)
            {
                _cfg = cfg;
                // Intialize config entries tied to the dictionary

                RoutePriceOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                RiskLevelOverride = new Dictionary<ExtendedLevel, SyncedEntry<string>>();
                DescriptionOverride = new Dictionary<ExtendedLevel, SyncedEntry<string>>();

                MinScrapOverrides = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                MaxScrapOverrides = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                ScrapValueMultiplier = new Dictionary<ExtendedLevel, SyncedEntry<float>>();
                ScrapListOverrides = new Dictionary<ExtendedLevel, SyncedEntry<string>>();

                InteriorEnemyPowerCountOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                InteriorEnemyOverride = new Dictionary<ExtendedLevel, SyncedEntry<string>>();
                DaytimeEnemyPowerCountOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                DaytimeEnemyOverride = new Dictionary<ExtendedLevel, SyncedEntry<string>>();
                NighttimeEnemyPowerCountOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                NighttimeEnemyOverride = new Dictionary<ExtendedLevel, SyncedEntry<string>>();
                SpawnSpeedScaler = new Dictionary<ExtendedLevel, SyncedEntry<float>>();

                MinTurretOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                MaxTurretOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                MinMineOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                MaxMineOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                MinSpikeTrapOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();
                MaxSpikeTrapOverride = new Dictionary<ExtendedLevel, SyncedEntry<int>>();

                // WeatherTypeOverride = new Dictionary<string, SyncedEntry<string>>();
                AddTags = new Dictionary<ExtendedLevel, SyncedEntry<string>>();

                VisibleOverride = new Dictionary<ExtendedLevel, SyncedEntry<bool>>();
                LockedOverride = new Dictionary<ExtendedLevel, SyncedEntry<bool>>();
                TimeOverride = new Dictionary<ExtendedLevel, SyncedEntry<bool>>();
                TimeMultiplierOverride = new Dictionary<ExtendedLevel, SyncedEntry<float>>();
                WatiForShipToLandBeforeTimeMoves = new Dictionary<ExtendedLevel, SyncedEntry<bool>>();

                FaciltySizeOverride = new Dictionary<ExtendedLevel, SyncedEntry<float>>();

                if (CentralConfig.HarmonyTouch7)
                {
                    if (NetworkManager.Singleton.IsHost)
                    {
                        ResetChanger.SavePlanetData();
                    }
                }
                CentralConfig.HarmonyTouch7 = true;

                if (CentralConfig.HarmonyTouch && MiscConfig.CreateMiscConfig.ShuffleSave != null)
                {
                    if (NetworkManager.Singleton.IsHost && MiscConfig.CreateMiscConfig.ShuffleSave) // sets the string versions of the dicts to the saved ones
                    {
                        if (CentralConfig.SyncConfig.ScrapShuffle && ES3.KeyExists("ScrapAppearanceString", GameNetworkManager.Instance.currentSaveFileName))
                        {
                            ShuffleSaver.ScrapAppearanceString = ES3.Load<Dictionary<string, int>>("ScrapAppearanceString", GameNetworkManager.Instance.currentSaveFileName);
                            CentralConfig.instance.mls.LogInfo("Loaded Scrap Shuffle Data");
                        }
                        if (CentralConfig.SyncConfig.EnemyShuffle && ES3.KeyExists("EnemyAppearanceString", GameNetworkManager.Instance.currentSaveFileName))
                        {
                            ShuffleSaver.EnemyAppearanceString = ES3.Load<Dictionary<string, int>>("EnemyAppearanceString", GameNetworkManager.Instance.currentSaveFileName);
                            CentralConfig.instance.mls.LogInfo("Loaded Enemy Shuffle Data");
                        }
                        if (CentralConfig.SyncConfig.DungeonShuffler && ES3.KeyExists("DungeonAppearanceString", GameNetworkManager.Instance.currentSaveFileName))
                        {
                            ShuffleSaver.DungeonAppearanceString = ES3.Load<Dictionary<string, int>>("DungeonAppearanceString", GameNetworkManager.Instance.currentSaveFileName);
                            CentralConfig.instance.mls.LogInfo("Loaded Dungeon Shuffle Data");
                        }
                    }
                }
                CentralConfig.HarmonyTouch = true;

                List<ExtendedLevel> allExtendedLevels;
                List<string> ignoreListEntries = ConfigAider.SplitStringsByDaComma(CentralConfig.SyncConfig.BlacklistMoons.Value).Select(entry => ConfigAider.CauterizeString(entry)).ToList();

                if (CentralConfig.SyncConfig.IsWhiteList)
                {
                    allExtendedLevels = PatchedContent.ExtendedLevels.Where(level => ignoreListEntries.Any(b => ConfigAider.CauterizeString(level.NumberlessPlanetName).Equals(b))).ToList();
                }
                else
                {
                    allExtendedLevels = PatchedContent.ExtendedLevels.Where(level => !ignoreListEntries.Any(b => ConfigAider.CauterizeString(level.NumberlessPlanetName).Equals(b))).ToList();
                }

                string rebalancedmoons = "Assurаncе,Mаrch,Offеnsе,Adаmance,Dіne,Tіtan,Embrіon,Rеnd,Vоw";
                List<string> rmlist = ConfigAider.SplitStringsByDaComma(rebalancedmoons).Select(entry => ConfigAider.CauterizeString(entry)).ToList();
                allExtendedLevels = allExtendedLevels.Where(level => !rmlist.Any(b => ConfigAider.CauterizeString(level.NumberlessPlanetName).Equals(b))).ToList();

                foreach (ExtendedLevel level in allExtendedLevels)
                {
                    string PlanetName = level.NumberlessPlanetName;

                    // General

                    if (CentralConfig.SyncConfig.DoGenOverrides)
                    {
                        RoutePriceOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName, // Assigns the config with the dictionary so that it is unique to the level/moon/planet
                            PlanetName + " - Route Price",
                            level.RoutePrice,
                            "Sets the cost of routing to the moon.");

                        RiskLevelOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Risk Level",
                            level.SelectableLevel.riskLevel,
                            "Sets the risk level of the moon (only visual).");

                        DescriptionOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Description",
                            level.SelectableLevel.LevelDescription,
                            "Sets the description of the moon, \\n is used to go to the next line (basically the enter key) (only visual).");
                    }

                    // Scrap

                    if (CentralConfig.SyncConfig.DoScrapOverrides)
                    {
                        MinScrapOverrides[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Min Scrap",
                            level.SelectableLevel.minScrap,
                            "Sets the minimum amount of scrap to generate on the moon.");

                        MaxScrapOverrides[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Max Scrap",
                            level.SelectableLevel.maxScrap,
                            "Sets the maximum amount of scrap to generate on the moon.");

                        ScrapValueMultiplier[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " -  Scrap Value Multiplier",
                            1f,
                            "Each scrap object on this moon will have its personal min/max values multiplied by this amount.");
                    }
                    if (CentralConfig.SyncConfig.DoScraplistOverrides)
                    {
                        string ScrapList = ConfigAider.ConvertItemListToString(level.SelectableLevel.spawnableScrap); // Method turns the scrap list into string (check postfix)

                        ScrapListOverrides[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Scrap List",
                            ScrapList,
                            "Sets the list of scrap with attached rarities to generate on the moon.");
                    }

                    // Enemies

                    if (CentralConfig.SyncConfig.DoEnemyOverrides)
                    {
                        InteriorEnemyPowerCountOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Interior Enemy Power",
                            level.SelectableLevel.maxEnemyPowerCount,
                            "Sets the power available for interior enemies on the moon.");

                        string InteriorEnemyList = ConfigAider.ConvertEnemyListToString(level.SelectableLevel.Enemies); // As above with these lists

                        InteriorEnemyOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Interior Enemy List",
                            InteriorEnemyList,
                            "Sets the spawn weights for interior enemies on the moon.");

                        DaytimeEnemyPowerCountOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Daytime Enemy Power",
                            level.SelectableLevel.maxDaytimeEnemyPowerCount,
                            "Sets the power available for daytime enemies on the moon.");

                        string DaytimeEnemyList = ConfigAider.ConvertEnemyListToString(level.SelectableLevel.DaytimeEnemies);

                        DaytimeEnemyOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Daytime Enemy List",
                            DaytimeEnemyList,
                            "Sets the spawn weights for daytime enemies on the moon.");

                        NighttimeEnemyPowerCountOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Nighttime Enemy Power",
                            level.SelectableLevel.maxOutsideEnemyPowerCount,
                            "Sets the power available for nighttime enemies on the moon.");

                        string NighttimeEnemyList = ConfigAider.ConvertEnemyListToString(level.SelectableLevel.OutsideEnemies);

                        NighttimeEnemyOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Nighttime Enemy List",
                            NighttimeEnemyList,
                            "Sets the spawn weights for nighttime enemies on the moon.");

                    }
                    if (CentralConfig.SyncConfig.EnemySpawnTimes)
                    {
                        SpawnSpeedScaler[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Spawn Speed Scaler",
                            1f,
                            "This value determines how fast (or slow) it takes for the enemy spawn curves to advance. If it is 2x, you can expect to see night enemies spawn in twice as early. If it is 0.25x, enemies will arrive slower and be minimal.\nThis affects the indoor, daytime, and nighttime enemy spawn curves.");
                    }

                    // Traps

                    if (CentralConfig.SyncConfig.DoTrapOverrides)
                    {
                        List<string> mapObjectNames = level.SelectableLevel.spawnableMapObjects.Select(mapObject => mapObject.prefabToSpawn.name).ToList();

                        var turretContainerObject = level.SelectableLevel.spawnableMapObjects.FirstOrDefault(mo => mo.prefabToSpawn.name == "TurretContainer");
                        if (turretContainerObject != null)
                        {
                            AnimationCurve numberToSpawnCurve = turretContainerObject.numberToSpawn;
                            int leftMost = (int)Math.Round(numberToSpawnCurve.Evaluate(0));
                            int rightMost = (int)Math.Round(numberToSpawnCurve.Evaluate(numberToSpawnCurve.keys[numberToSpawnCurve.length - 1].time));

                            MinTurretOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Min Turrets",
                                leftMost,
                                "Sets the minimum number of turrets to spawn on the moon.");

                            MaxTurretOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Max Turrets",
                                rightMost,
                                "Sets the maximum number of turrets to spawn on the moon.");

                            if (turretContainerObjectReference == null)
                            {
                                turretContainerObjectReference = Array.Find(level.SelectableLevel.spawnableMapObjects, obj => obj.prefabToSpawn != null && obj.prefabToSpawn.name == "TurretContainer");

                                if (turretContainerObjectReference != null)
                                {
                                    // CentralConfig.instance.mls.LogInfo("TurretContainer reference saved from " + PlanetName);
                                }
                            }
                        }
                        else
                        {
                            MinTurretOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Min Turrets",
                                0,
                                "Sets the minimum number of turrets to spawn on the moon.");

                            MaxTurretOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Max Turrets",
                                0,
                                "Sets the maximum number of turrets to spawn on the moon.");

                            if (turretContainerObjectReference != null)
                            {
                                SpawnableMapObject newObject = new SpawnableMapObject
                                {
                                    prefabToSpawn = turretContainerObjectReference.prefabToSpawn,
                                    spawnFacingAwayFromWall = true,
                                    spawnFacingWall = false,
                                    spawnWithBackToWall = false,
                                    spawnWithBackFlushAgainstWall = false,
                                    requireDistanceBetweenSpawns = false,
                                    disallowSpawningNearEntrances = false
                                };
                                SpawnableMapObject[] newArray = new SpawnableMapObject[level.SelectableLevel.spawnableMapObjects.Length + 1];
                                Array.Copy(level.SelectableLevel.spawnableMapObjects, newArray, level.SelectableLevel.spawnableMapObjects.Length);
                                newArray[^1] = newObject;
                                level.SelectableLevel.spawnableMapObjects = newArray;

                                // CentralConfig.instance.mls.LogInfo("Successfully accessed stored TurretContainer reference to use for " + PlanetName);
                            }
                            else
                            {
                                // CentralConfig.instance.mls.LogInfo("Tried to access unstored TurretContainer reference to use for " + PlanetName);
                            }
                        }
                        var landmineObject = level.SelectableLevel.spawnableMapObjects.FirstOrDefault(mo => mo.prefabToSpawn.name == "Landmine");
                        if (landmineObject != null)
                        {
                            AnimationCurve numberToSpawnCurve = landmineObject.numberToSpawn;
                            int leftMost = (int)Math.Round(numberToSpawnCurve.Evaluate(0));
                            int rightMost = (int)Math.Round(numberToSpawnCurve.Evaluate(numberToSpawnCurve.keys[numberToSpawnCurve.length - 1].time));

                            MinMineOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Min Mines",
                                leftMost,
                                "Sets the minimum number of mines to spawn on the moon.");

                            MaxMineOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Max Mines",
                                rightMost,
                                "Sets the maximum number of mines to spawn on the moon.");

                            if (landmineObjectReference == null)
                            {
                                landmineObjectReference = Array.Find(level.SelectableLevel.spawnableMapObjects, obj => obj.prefabToSpawn != null && obj.prefabToSpawn.name == "Landmine");

                                if (landmineObjectReference != null)
                                {
                                    // CentralConfig.instance.mls.LogInfo("Landmine reference saved from " + PlanetName);
                                }
                            }
                        }
                        else
                        {
                            MinMineOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Min Mines",
                                0,
                                "Sets the minimum number of mines to spawn on the moon.");

                            MaxMineOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Max Mines",
                                0,
                                "Sets the maximum number of mines to spawn on the moon.");

                            if (landmineObjectReference != null)
                            {
                                SpawnableMapObject newObject = new SpawnableMapObject
                                {
                                    prefabToSpawn = landmineObjectReference.prefabToSpawn,
                                    spawnFacingAwayFromWall = false,
                                    spawnFacingWall = false,
                                    spawnWithBackToWall = false,
                                    spawnWithBackFlushAgainstWall = false,
                                    requireDistanceBetweenSpawns = false,
                                    disallowSpawningNearEntrances = false
                                };
                                SpawnableMapObject[] newArray = new SpawnableMapObject[level.SelectableLevel.spawnableMapObjects.Length + 1];
                                Array.Copy(level.SelectableLevel.spawnableMapObjects, newArray, level.SelectableLevel.spawnableMapObjects.Length);
                                newArray[^1] = newObject;
                                level.SelectableLevel.spawnableMapObjects = newArray;

                                // CentralConfig.instance.mls.LogInfo("Successfully accessed stored Landmine reference to use for " + PlanetName);
                            }
                            else
                            {
                                // CentralConfig.instance.mls.LogInfo("Tried to access unstored Landmine reference to use for " + PlanetName);
                            }
                        }
                        var spikeRoofTrapHazardObject = level.SelectableLevel.spawnableMapObjects.FirstOrDefault(mo => mo.prefabToSpawn.name == "SpikeRoofTrapHazard");
                        if (spikeRoofTrapHazardObject != null)
                        {
                            AnimationCurve numberToSpawnCurve = spikeRoofTrapHazardObject.numberToSpawn;
                            int leftMost = (int)Math.Round(numberToSpawnCurve.Evaluate(0));
                            int rightMost = (int)Math.Round(numberToSpawnCurve.Evaluate(numberToSpawnCurve.keys[numberToSpawnCurve.length - 1].time));

                            MinSpikeTrapOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Min Spike Traps",
                                leftMost,
                                "Sets the minimum number of spike traps to spawn on the moon.");

                            MaxSpikeTrapOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Max Spike Traps",
                                rightMost,
                                "Sets the maximum number of spike traps to spawn on the moon.");

                            if (spikeRoofTrapHazardObjectReference == null)
                            {
                                spikeRoofTrapHazardObjectReference = Array.Find(level.SelectableLevel.spawnableMapObjects, obj => obj.prefabToSpawn != null && obj.prefabToSpawn.name == "SpikeRoofTrapHazard");

                                if (spikeRoofTrapHazardObjectReference != null)
                                {
                                    // CentralConfig.instance.mls.LogInfo("SpikeRoofTrapHazard reference saved from " + PlanetName);
                                }
                            }
                        }
                        else
                        {
                            MinSpikeTrapOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Min Spike Traps",
                                0,
                                "Sets the minimum number of spike traps to spawn on the moon.");

                            MaxSpikeTrapOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                                PlanetName + " - Max Spike Traps",
                                0,
                                "Sets the maximum number of spike traps to spawn on the moon.");

                            if (spikeRoofTrapHazardObjectReference != null)
                            {
                                SpawnableMapObject newObject = new SpawnableMapObject
                                {
                                    prefabToSpawn = spikeRoofTrapHazardObjectReference.prefabToSpawn,
                                    spawnFacingAwayFromWall = false,
                                    spawnFacingWall = true,
                                    spawnWithBackToWall = true,
                                    spawnWithBackFlushAgainstWall = true,
                                    requireDistanceBetweenSpawns = true,
                                    disallowSpawningNearEntrances = false
                                };
                                SpawnableMapObject[] newArray = new SpawnableMapObject[level.SelectableLevel.spawnableMapObjects.Length + 1];
                                Array.Copy(level.SelectableLevel.spawnableMapObjects, newArray, level.SelectableLevel.spawnableMapObjects.Length);
                                newArray[^1] = newObject;
                                level.SelectableLevel.spawnableMapObjects = newArray;

                                // CentralConfig.instance.mls.LogInfo("Successfully accessed stored SpikeRoofTrapHazard reference to use for " + PlanetName);
                            }
                            else
                            {
                                // CentralConfig.instance.mls.LogInfo("Tried to access unstored SpikeRoofTrapHazard reference to use for " + PlanetName);
                            }
                        }
                    }

                    // Weather

                    /*if (CentralConfig.SyncConfig.DoMoonWeatherOverrides)
                    {
                        string PossibleWeatherArray = ConfigAider.ConvertWeatherArrayToString(level.SelectableLevel.randomWeathers);

                        WeatherTypeOverride[PlanetName] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Possible Weathers",
                            PossibleWeatherArray,
                            "Sets the possible weathers that can occur on the moon");
                    }*/

                    // Tags

                    if (CentralConfig.SyncConfig.DoEnemyTagInjections || CentralConfig.SyncConfig.DoScrapTagInjections)
                    {
                        string ContentTags = ConfigAider.ConvertTagsToString(level.ContentTags);

                        AddTags[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Content Tags",
                            ContentTags,
                            "Add new content tags to the moon (The tags shown in the default value cannot be removed).");
                    }

                    // Misc

                    if (CentralConfig.SyncConfig.DoDangerBools)
                    {
                        VisibleOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Should The Moon Be Hidden?",
                            level.IsRouteHidden,
                        "Set to true to hide the moon in the terminal.");

                        LockedOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Should The Moon Be Locked?",
                            level.IsRouteLocked,
                            "Set to true to prevent visiting the moon.");
                    }
                    if (CentralConfig.SyncConfig.TimeSettings)
                    {
                        TimeOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Should The Moon Have Time?",
                            level.SelectableLevel.planetHasTime,
                            "Set to true to enable time progression. Set to false for no time progression.");

                        TimeMultiplierOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Day Speed Multiplier",
                            1.4f,
                            "Adjusts the speed of day progression. For example, 2.8 will be twice as fast as vanilla.");

                        WatiForShipToLandBeforeTimeMoves[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Should Time Wait on the Ship?",
                            true,
                            "Set to true to make time only progress AFTER the ship has landed.");
                    }

                    // Dungeon Size

                    if (CentralConfig.SyncConfig.DoDunSizeOverrides)
                    {
                        FaciltySizeOverride[level] = cfg.BindSyncedEntry("Moon: " + PlanetName,
                            PlanetName + " - Dungeon Size",
                            level.SelectableLevel.factorySizeMultiplier,
                            "Sets the dungeon size multiplier granted by this moon.");
                    }
                }

                if (CentralConfig.SyncConfig.GlobalEnemyAndScrap)
                {
                    AddIndoorEnemiesToAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Add Indoor Enemies To All Moons",
                        "Default Values Were Empty",
                        "Enemies in the 'EnemyName:Rarity' format will be added to the indoor enemy pool on every moon (Before manipulation by tags, current weather, and current dungeon).");

                    ReplaceIndoorEnemiesOnAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Replace Indoor Enemies On All Moons",
                        "Default Values Were Empty",
                        "In the example, \"Flowerman:Plantman,Crawler:Mauler\",\nOn all moons, Brackens will be replaced with hypothetical Plantmen, and Crawlers with hypothetical Maulers.\nYou could also use inputs such as \"Flowerman-15:Plantman~50\", this will give the Plantman a rarity of 15 instead of using the Bracken's and it will only have a 50% chance to replace.\nThis is done before enemies are added by the setting above and before manipulation by tags, current weather, and current dungeon.");

                    MultiplyIndoorEnemiesOnAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Multiply Interior Enemies On All Moons",
                        "Default Values Were Empty",
                        "Enemies listed here will be multiplied by the assigned value on all moons. \"Maneater:1.7,Jester:0.4\" will multiply the Maneater's rarity by 1.7 and the Jester's rarity by 0.4 on all moons.");

                    AddDayEnemiesToAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Add Day Enemies To All Moons",
                        "Default Values Were Empty",
                        "Enemies in the 'EnemyName:Rarity' format will be added to the daytime enemy pool on every moon (Before manipulation by tags, current weather, and current dungeon).");

                    ReplaceDayEnemiesOnAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Replace Day Enemies On All Moons",
                        "Default Values Were Empty",
                        "In the example, \"Manticoil:Mantisoil,Docile Locust Bees:Angry Moth Wasps\",\nOn all moons, Manticoils will be replaced with hypothetical Mantisoils, and docile locust bees with hypothetical angry moth wasps.\nYou could also use inputs such as \"Manticoil-90:Mantisoil\", this will give the Mantisoil a rarity of 90 instead of using the Manticoil's and it will still have a 100% chance to replace.\nThis is done before enemies are added by the setting above and before manipulation by tags, current weather, and current dungeon.");

                    MultiplyDayEnemiesOnAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Multiply Day Enemies On All Moons",
                        "Default Values Were Empty",
                        "Enemies listed here will be multiplied by the assigned value on all moons. \"Red Locust Bees:2.4,Docile Locust Bees:0.8\" will multiply the Bee's rarity by 2.4 and the locust's rarity by 0.8 on all moons.");

                    AddNightEnemiesToAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Add Night Enemies To All Moons",
                        "Default Values Were Empty",
                        "Enemies in the 'EnemyName:Rarity' format will be added to the night enemy pool on every moon (Before manipulation by tags, current weather, and current dungeon).");

                    ReplaceNightEnemiesOnAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Replace Night Enemies On All Moons",
                        "Default Values Were Empty",
                        "In the example, \"MouthDog:OceanDog,ForestGiant:FireGiant\",\nOn all moons, Mouthdogs will be replaced with hypothetical Oceandogs, and Forest giants with hypothetical Fire giants.\nYou could also use inputs such as \"MouthDog:OceanDog~75\", the OceanDog will still inherit the rarity from the MouthDog but it will only have a 75% chance to replace.\nThis is done before enemies are added by the setting above and before manipulation by tags, current weather, and current dungeon.");

                    MultiplyNightEnemiesOnAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Multiply Night Enemies On All Moons",
                        "Default Values Were Empty",
                        "Enemies listed here will be multiplied by the assigned value on all moons. \"MouthDog:0.33,ForestGiant:1.1\" will multiply the Dog's rarity by 0.33 and the giant's rarity by 1.1 on all moons.");

                    AddScrapToAllMoons = cfg.BindSyncedEntry("~Global~",
                        "Add Scrap To All Moons",
                        "Default Values Were Empty",
                        "Scrap in the 'ScrapName:Rarity; format will be added to the scrap pool on every moon.");
                }
                ConfigAider.Instance.CleanConfig(cfg); // Cleans out orphaned config entries (ones that you don't want to use anymore)
                if (CentralConfig.HarmonyTouch6 && NetworkManager.Singleton.IsHost && (CentralConfig.SyncConfig.DoGenOverrides || CentralConfig.SyncConfig.DoScrapOverrides || CentralConfig.SyncConfig.DoScraplistOverrides || CentralConfig.SyncConfig.DoEnemyOverrides || CentralConfig.SyncConfig.DoTrapOverrides || CentralConfig.SyncConfig.DoDangerBools || CentralConfig.SyncConfig.TimeSettings))
                {
                    CentralConfig.instance.mls.LogInfo("Moon config has been registered.");
                }
                CentralConfig.HarmonyTouch6 = true;
            }
        }
        static void Prefix()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                int seed;
                if (CentralConfig.SyncConfig.RandomSeed < 0)
                {
                    seed = UnityEngine.Random.Range(1, 100000000);
                }
                else
                {
                    seed = CentralConfig.SyncConfig.RandomSeed;
                }
                if (StartOfRound.Instance.randomMapSeed <= 0)
                {
                    StartOfRound.Instance.randomMapSeed = seed;
                }

                StartOfRound.Instance.SetPlanetsWeather();
                StartOfRound.Instance.SetMapScreenInfoToCurrentLevel();

            }
            CentralConfig.ConfigFile = new CreateMoonConfig(CentralConfig.instance.Config); // Moon config is created when you join a lobby (So every other config is already applied)
        }
    }
    public class ApplyMoonConfig
    {
        public static bool Ready = false;
        public void UpdateMoonValues() // This is called on as a postfix on the same method as creating the config stuff so it gets applied here right after the config is intialized
        {
            List<ExtendedLevel> allExtendedLevels;
            List<string> ignoreListEntries = ConfigAider.SplitStringsByDaComma(CentralConfig.SyncConfig.BlacklistMoons.Value).Select(entry => ConfigAider.CauterizeString(entry)).ToList();

            if (CentralConfig.SyncConfig.IsWhiteList)
            {
                allExtendedLevels = PatchedContent.ExtendedLevels.Where(level => ignoreListEntries.Any(b => ConfigAider.CauterizeString(level.NumberlessPlanetName).Equals(b))).ToList();
            }
            else
            {
                allExtendedLevels = PatchedContent.ExtendedLevels.Where(level => !ignoreListEntries.Any(b => ConfigAider.CauterizeString(level.NumberlessPlanetName).Equals(b))).ToList();
            }

            string rebalancedmoons = "Assurаncе,Mаrch,Offеnsе,Adаmance,Dіne,Tіtan,Embrіon,Rеnd,Vоw";
            List<string> rmlist = ConfigAider.SplitStringsByDaComma(rebalancedmoons).Select(entry => ConfigAider.CauterizeString(entry)).ToList();
            allExtendedLevels = allExtendedLevels.Where(level => !rmlist.Any(b => ConfigAider.CauterizeString(level.NumberlessPlanetName).Equals(b))).ToList();

            foreach (ExtendedLevel level in allExtendedLevels)
            {

                // General

                if (CentralConfig.SyncConfig.DoGenOverrides)
                {
                    level.RoutePrice = WaitForMoonsToRegister.CreateMoonConfig.RoutePriceOverride[level]; // These are just easy variables

                    level.SelectableLevel.riskLevel = WaitForMoonsToRegister.CreateMoonConfig.RiskLevelOverride[level];

                    level.SelectableLevel.LevelDescription = WaitForMoonsToRegister.CreateMoonConfig.DescriptionOverride[level];
                    level.OverrideRouteNodeDescription = WaitForMoonsToRegister.CreateMoonConfig.DescriptionOverride[level];
                    level.OverrideRouteConfirmNodeDescription = WaitForMoonsToRegister.CreateMoonConfig.DescriptionOverride[level];
                    level.OverrideInfoNodeDescription = WaitForMoonsToRegister.CreateMoonConfig.DescriptionOverride[level];
                }

                // Scrap

                if (CentralConfig.SyncConfig.DoScrapOverrides && NetworkManager.Singleton.IsHost)
                {
                    level.SelectableLevel.minScrap = WaitForMoonsToRegister.CreateMoonConfig.MinScrapOverrides[level];
                    level.SelectableLevel.maxScrap = WaitForMoonsToRegister.CreateMoonConfig.MaxScrapOverrides[level];
                }
                // ScrapList
                if (CentralConfig.SyncConfig.DoScraplistOverrides && NetworkManager.Singleton.IsHost)
                {
                    string scrapStr = WaitForMoonsToRegister.CreateMoonConfig.ScrapListOverrides[level]; // Ok so the lists kinda suck
                    Vector2 clamprarity = new Vector2(-99999, 99999);
                    List<SpawnableItemWithRarity> scrap = ConfigAider.ConvertStringToItemList(scrapStr, clamprarity); // This method turns the string back into a list
                    level.SelectableLevel.spawnableScrap = scrap;
                }

                // Enemies

                if (CentralConfig.SyncConfig.DoEnemyOverrides && NetworkManager.Singleton.IsHost)
                {
                    if (CentralConfig.SyncConfig.ScaleEnemySpawnRate)
                    {
                        if (level.SelectableLevel.maxEnemyPowerCount != 0)
                        {
                            float Intmultiplier = WaitForMoonsToRegister.CreateMoonConfig.InteriorEnemyPowerCountOverride[level] / level.SelectableLevel.maxEnemyPowerCount;
                            if (Intmultiplier != 1)
                            {
                                AnimationCurve IntCurve = ConfigAider.MultiplyYValues(level.SelectableLevel.enemySpawnChanceThroughoutDay, Intmultiplier, level.NumberlessPlanetName, "Interior Curve");
                                if (IntCurve != null)
                                {
                                    level.SelectableLevel.enemySpawnChanceThroughoutDay = IntCurve;
                                }
                            }
                        }
                        if (level.SelectableLevel.maxDaytimeEnemyPowerCount != 0)
                        {
                            float Daymultiplier = WaitForMoonsToRegister.CreateMoonConfig.DaytimeEnemyPowerCountOverride[level] / level.SelectableLevel.maxDaytimeEnemyPowerCount;
                            if (Daymultiplier != 1)
                            {
                                AnimationCurve DayCurve = ConfigAider.MultiplyYValues(level.SelectableLevel.daytimeEnemySpawnChanceThroughDay, Daymultiplier, level.NumberlessPlanetName, "Daytime Curve");
                                if (DayCurve != null)
                                {
                                    level.SelectableLevel.daytimeEnemySpawnChanceThroughDay = DayCurve;
                                }
                            }
                        }
                        if (level.SelectableLevel.maxOutsideEnemyPowerCount != 0)
                        {
                            float Noxmultiplier = WaitForMoonsToRegister.CreateMoonConfig.NighttimeEnemyPowerCountOverride[level] / level.SelectableLevel.maxOutsideEnemyPowerCount;
                            if (Noxmultiplier != 1)
                            {
                                AnimationCurve NoxCurve = ConfigAider.MultiplyYValues(level.SelectableLevel.outsideEnemySpawnChanceThroughDay, Noxmultiplier, level.NumberlessPlanetName, "Nighttime Curve");
                                if (NoxCurve != null)
                                {
                                    level.SelectableLevel.outsideEnemySpawnChanceThroughDay = NoxCurve;
                                }
                            }
                        }
                    }
                    level.SelectableLevel.maxEnemyPowerCount = WaitForMoonsToRegister.CreateMoonConfig.InteriorEnemyPowerCountOverride[level]; // Same as the scrap list but I had to explicitly exclude Lasso since he will fuck up the stuff (pls for the love of god if you bring back Lasso don't make its enemyName = "Lasso" I will cry) ((This mod will ignore it))
                    // InteriorEnemyList
                    string IntEneStr = WaitForMoonsToRegister.CreateMoonConfig.InteriorEnemyOverride[level];
                    Vector2 clampIntRarity = new Vector2(-99999, 99999);
                    List<SpawnableEnemyWithRarity> IntEnemies = ConfigAider.ConvertStringToEnemyList(IntEneStr, clampIntRarity);
                    if (IntEnemies != level.SelectableLevel.Enemies)
                    {
                        level.SelectableLevel.Enemies = IntEnemies;
                    }

                    level.SelectableLevel.maxDaytimeEnemyPowerCount = WaitForMoonsToRegister.CreateMoonConfig.DaytimeEnemyPowerCountOverride[level];
                    // DaytimeEnemyList
                    string DayEneStr = WaitForMoonsToRegister.CreateMoonConfig.DaytimeEnemyOverride[level];
                    Vector2 clampDayRarity = new Vector2(-99999, 99999);
                    List<SpawnableEnemyWithRarity> DayEnemies = ConfigAider.ConvertStringToEnemyList(DayEneStr, clampDayRarity);
                    if (DayEnemies != level.SelectableLevel.DaytimeEnemies)
                    {
                        level.SelectableLevel.DaytimeEnemies = DayEnemies;
                    }

                    level.SelectableLevel.maxOutsideEnemyPowerCount = WaitForMoonsToRegister.CreateMoonConfig.NighttimeEnemyPowerCountOverride[level];
                    // NighttimeEnemyList
                    string NightEneStr = WaitForMoonsToRegister.CreateMoonConfig.NighttimeEnemyOverride[level];
                    Vector2 clampNightRarity = new Vector2(-99999, 99999);
                    List<SpawnableEnemyWithRarity> NightEnemies = ConfigAider.ConvertStringToEnemyList(NightEneStr, clampNightRarity);
                    if (NightEnemies != level.SelectableLevel.OutsideEnemies)
                    {
                        level.SelectableLevel.OutsideEnemies = NightEnemies;
                    }
                }
                if (CentralConfig.SyncConfig.GlobalEnemyAndScrap && NetworkManager.Singleton.IsHost)
                {
                    string IntEneStr = WaitForMoonsToRegister.CreateMoonConfig.AddIndoorEnemiesToAllMoons;
                    Vector2 clampIntRarity = new Vector2(-99999, 99999);
                    List<SpawnableEnemyWithRarity> interiorenemyList = ConfigAider.ConvertStringToEnemyList(IntEneStr, clampIntRarity);
                    WaitForMoonsToRegister.IEnemies = interiorenemyList;

                    string DayEneStr = WaitForMoonsToRegister.CreateMoonConfig.AddDayEnemiesToAllMoons;
                    Vector2 clampDayRarity = new Vector2(-99999, 99999);
                    List<SpawnableEnemyWithRarity> dayenemyList = ConfigAider.ConvertStringToEnemyList(DayEneStr, clampDayRarity);
                    WaitForMoonsToRegister.DEnemies = dayenemyList;

                    string NightEneStr = WaitForMoonsToRegister.CreateMoonConfig.AddNightEnemiesToAllMoons;
                    Vector2 clampNightRarity = new Vector2(-99999, 99999);
                    List<SpawnableEnemyWithRarity> nightenemyList = ConfigAider.ConvertStringToEnemyList(NightEneStr, clampNightRarity);
                    WaitForMoonsToRegister.NEnemies = nightenemyList;

                    string ScrStr = WaitForMoonsToRegister.CreateMoonConfig.AddScrapToAllMoons;
                    Vector2 clampScrRarity = new Vector2(-99999, 99999);
                    List<SpawnableItemWithRarity> scraplist = ConfigAider.ConvertStringToItemList(ScrStr, clampScrRarity);
                    WaitForMoonsToRegister.Scrap = scraplist;

                    string OoO = CentralConfig.SyncConfig.OoO;
                    var pairs = OoO.Split(',');

                    foreach (var pair in pairs)
                    {
                        if (ConfigAider.CauterizeString(pair) == "add")
                        {
                            if (WaitForMoonsToRegister.IEnemies.Count > 0)
                            {
                                level.SelectableLevel.Enemies = level.SelectableLevel.Enemies.Concat(WaitForMoonsToRegister.IEnemies).ToList();
                            }
                            if (WaitForMoonsToRegister.DEnemies.Count > 0)
                            {
                                level.SelectableLevel.DaytimeEnemies = level.SelectableLevel.DaytimeEnemies.Concat(WaitForMoonsToRegister.DEnemies).ToList();
                            }
                            if (WaitForMoonsToRegister.NEnemies.Count > 0)
                            {
                                level.SelectableLevel.OutsideEnemies = level.SelectableLevel.OutsideEnemies.Concat(WaitForMoonsToRegister.NEnemies).ToList();
                            }
                            if (WaitForMoonsToRegister.Scrap.Count > 0)
                            {
                                level.SelectableLevel.spawnableScrap = level.SelectableLevel.spawnableScrap.Concat(WaitForMoonsToRegister.Scrap).ToList();
                            }
                        }
                        else if (ConfigAider.CauterizeString(pair) == "multiply")
                        {
                            level.SelectableLevel.Enemies = ConfigAider.MultiplyEnemyRarities(level.SelectableLevel.Enemies, WaitForMoonsToRegister.CreateMoonConfig.MultiplyIndoorEnemiesOnAllMoons);
                            level.SelectableLevel.DaytimeEnemies = ConfigAider.MultiplyEnemyRarities(level.SelectableLevel.DaytimeEnemies, WaitForMoonsToRegister.CreateMoonConfig.MultiplyDayEnemiesOnAllMoons);
                            level.SelectableLevel.OutsideEnemies = ConfigAider.MultiplyEnemyRarities(level.SelectableLevel.OutsideEnemies, WaitForMoonsToRegister.CreateMoonConfig.MultiplyNightEnemiesOnAllMoons);
                        }
                        else if (ConfigAider.CauterizeString(pair) == "replace")
                        {
                            level.SelectableLevel.Enemies = ConfigAider.ReplaceEnemies(level.SelectableLevel.Enemies, WaitForMoonsToRegister.CreateMoonConfig.ReplaceIndoorEnemiesOnAllMoons);
                            level.SelectableLevel.DaytimeEnemies = ConfigAider.ReplaceEnemies(level.SelectableLevel.DaytimeEnemies, WaitForMoonsToRegister.CreateMoonConfig.ReplaceDayEnemiesOnAllMoons);
                            level.SelectableLevel.OutsideEnemies = ConfigAider.ReplaceEnemies(level.SelectableLevel.OutsideEnemies, WaitForMoonsToRegister.CreateMoonConfig.ReplaceNightEnemiesOnAllMoons);
                        }
                        else
                        {
                            CentralConfig.instance.mls.LogInfo($"Order of Operation: {pair} cannot be understood");
                        }
                    }
                }
                if (CentralConfig.SyncConfig.EnemySpawnTimes && NetworkManager.Singleton.IsHost)
                {
                    float scaleFactor = WaitForMoonsToRegister.CreateMoonConfig.SpawnSpeedScaler[level];
                    if (scaleFactor != 1)
                    {
                        if (level.SelectableLevel.maxEnemyPowerCount != 0)
                        {
                            AnimationCurve IntCurve = ConfigAider.ScaleXValues(level.SelectableLevel.enemySpawnChanceThroughoutDay, scaleFactor, level.NumberlessPlanetName, "Interior Curve");
                            if (IntCurve != null)
                            {
                                level.SelectableLevel.enemySpawnChanceThroughoutDay = IntCurve;
                            }
                        }
                        if (level.SelectableLevel.maxDaytimeEnemyPowerCount != 0)
                        {
                            AnimationCurve DayCurve = ConfigAider.ScaleXValues(level.SelectableLevel.daytimeEnemySpawnChanceThroughDay, scaleFactor, level.NumberlessPlanetName, "Daytime Curve");
                            if (DayCurve != null)
                            {
                                level.SelectableLevel.daytimeEnemySpawnChanceThroughDay = DayCurve;
                            }
                        }
                        if (level.SelectableLevel.maxOutsideEnemyPowerCount != 0)
                        {
                            AnimationCurve NoxCurve = ConfigAider.ScaleXValues(level.SelectableLevel.outsideEnemySpawnChanceThroughDay, scaleFactor, level.NumberlessPlanetName, "Nighttime Curve");
                            if (NoxCurve != null)
                            {
                                level.SelectableLevel.outsideEnemySpawnChanceThroughDay = NoxCurve;
                            }
                        }
                    }
                }

                // Traps

                if (CentralConfig.SyncConfig.DoTrapOverrides && NetworkManager.Singleton.IsHost)
                {
                    List<string> mapObjectNames = level.SelectableLevel.spawnableMapObjects.Select(mapObject => mapObject.prefabToSpawn.name).ToList();

                    var turretContainerObject = level.SelectableLevel.spawnableMapObjects.FirstOrDefault(mo => mo.prefabToSpawn.name == "TurretContainer");
                    if (turretContainerObject != null)
                    {
                        int minTurrets = WaitForMoonsToRegister.CreateMoonConfig.MinTurretOverride[level];
                        int maxTurrets = WaitForMoonsToRegister.CreateMoonConfig.MaxTurretOverride[level];

                        Keyframe key1 = new Keyframe(0, minTurrets);
                        Keyframe key2 = new Keyframe(1, maxTurrets);
                        AnimationCurve newCurve = new AnimationCurve(key1, key2);
                        turretContainerObject.numberToSpawn = newCurve;
                    }
                    var landmineObject = level.SelectableLevel.spawnableMapObjects.FirstOrDefault(mo => mo.prefabToSpawn.name == "Landmine");
                    if (landmineObject != null)
                    {
                        int minMines = WaitForMoonsToRegister.CreateMoonConfig.MinMineOverride[level];
                        int maxMines = WaitForMoonsToRegister.CreateMoonConfig.MaxMineOverride[level];

                        Keyframe key1 = new Keyframe(0, minMines);
                        Keyframe key2 = new Keyframe(1, maxMines);
                        AnimationCurve newCurve = new AnimationCurve(key1, key2);
                        landmineObject.numberToSpawn = newCurve;
                    }
                    var spikeRoofTrapHazardObject = level.SelectableLevel.spawnableMapObjects.FirstOrDefault(mo => mo.prefabToSpawn.name == "SpikeRoofTrapHazard");
                    if (spikeRoofTrapHazardObject != null)
                    {
                        int minSpikes = WaitForMoonsToRegister.CreateMoonConfig.MinSpikeTrapOverride[level];
                        int maxSpikes = WaitForMoonsToRegister.CreateMoonConfig.MaxSpikeTrapOverride[level];

                        Keyframe key1 = new Keyframe(0, minSpikes);
                        Keyframe key2 = new Keyframe(1, maxSpikes);
                        AnimationCurve newCurve = new AnimationCurve(key1, key2);
                        spikeRoofTrapHazardObject.numberToSpawn = newCurve;
                    }
                }

                // Weather

                /*if (CentralConfig.SyncConfig.DoMoonWeatherOverrides)
                {
                    string WeatherStr = WaitForMoonsToRegister.CreateMoonConfig.WeatherTypeOverride[PlanetName];
                    RandomWeatherWithVariables[] PossibleWeathers = ConfigAider.ConvertStringToWeatherArray(WeatherStr);
                    level.SelectableLevel.randomWeathers = PossibleWeathers;
                    level.SelectableLevel.overrideWeather = false;
                    string PossibleWeatherArray = ConfigAider.ConvertWeatherArrayToString(level.SelectableLevel.randomWeathers);

                    Ororo Ororo = new Ororo();
                    Ororo.SetSinglePlanetWeather(level);
                }*/

                // Misc

                if (CentralConfig.SyncConfig.DoDangerBools)
                {
                    level.IsRouteHidden = WaitForMoonsToRegister.CreateMoonConfig.VisibleOverride[level];
                    level.IsRouteLocked = WaitForMoonsToRegister.CreateMoonConfig.LockedOverride[level];
                }
                if (CentralConfig.SyncConfig.TimeSettings)
                {
                    level.SelectableLevel.planetHasTime = WaitForMoonsToRegister.CreateMoonConfig.TimeOverride[level];
                }

                // Dungeon Size

                if (CentralConfig.SyncConfig.DoDunSizeOverrides)
                {
                    level.SelectableLevel.factorySizeMultiplier = WaitForMoonsToRegister.CreateMoonConfig.FaciltySizeOverride[level];
                }

                // tags

                if ((CentralConfig.SyncConfig.DoEnemyTagInjections || CentralConfig.SyncConfig.DoScrapTagInjections) && NetworkManager.Singleton.IsHost)
                {
                    string TagStr = WaitForMoonsToRegister.CreateMoonConfig.AddTags[level];
                    List<ContentTag> MoonTags = ConfigAider.ConvertStringToTagList(TagStr);
                    if (MoonTags.Count > 0)
                    {
                        level.ContentTags.AddRange(MoonTags);
                    }
                }
            }
            if (CentralConfig.SyncConfig.BigEnemyList && NetworkManager.Singleton.IsHost)
            {
                string BigInteriorList = ConfigAider.GetBigList(0);
                string BigDayTimeList = ConfigAider.GetBigList(1);
                string BigNightTimeList = ConfigAider.GetBigList(2);

                WaitForMoonsToRegister.CreateMoonConfig.BigInteriorList = WaitForMoonsToRegister.CreateMoonConfig._cfg.BindSyncedEntry("~Big Lists~",
                    "Big Interior List",
                    BigInteriorList,
                    "Sets the Interior Enemy Lists for every moon.");

                WaitForMoonsToRegister.CreateMoonConfig.BigDayTimeList = WaitForMoonsToRegister.CreateMoonConfig._cfg.BindSyncedEntry("~Big Lists~",
                    "Big Day List",
                    BigDayTimeList,
                    "Sets the Day Enemy Lists for every moon.");

                WaitForMoonsToRegister.CreateMoonConfig.BigNightTimeList = WaitForMoonsToRegister.CreateMoonConfig._cfg.BindSyncedEntry("~Big Lists~",
                    "Big Night List",
                    BigNightTimeList,
                    "Sets the Night Enemy Lists for every moon.");

                ConfigAider.SetBigList(0, WaitForMoonsToRegister.CreateMoonConfig.BigInteriorList);
                ConfigAider.SetBigList(1, WaitForMoonsToRegister.CreateMoonConfig.BigDayTimeList);
                ConfigAider.SetBigList(2, WaitForMoonsToRegister.CreateMoonConfig.BigNightTimeList);
            }
            if (NetworkManager.Singleton.IsHost && (CentralConfig.SyncConfig.DoGenOverrides || CentralConfig.SyncConfig.DoScrapOverrides || CentralConfig.SyncConfig.DoScraplistOverrides || CentralConfig.SyncConfig.DoEnemyOverrides || CentralConfig.SyncConfig.DoTrapOverrides || CentralConfig.SyncConfig.DoDangerBools || CentralConfig.SyncConfig.TimeSettings))
            {
                CentralConfig.instance.mls.LogInfo("Moon config Values Applied.");
            }
            ConfigAider.Instance.StartCoroutine(LogSeed());
            foreach (ExtendedLevel level in PatchedContent.ExtendedLevels)
            {
                ResetChanger.ResetOnDisconnect.AllLevels.Add(level);
            }
            Ready = true;
        }
        static IEnumerator LogSeed()
        {
            yield return new WaitForSeconds(10);

            CentralConfig.instance.mls.LogInfo($"Starting Seed: {StartOfRound.Instance.randomMapSeed}");
        }
    }
    [HarmonyPatch(typeof(HangarShipDoor), "Start")]
    public class FrApplyMoon
    {
        static void Postfix()
        {
            ApplyMoonConfig applyConfig = new ApplyMoonConfig();
            applyConfig.UpdateMoonValues();
        }
    }
    [HarmonyPatch(typeof(RoundManager), "SpawnDaytimeEnemiesOutside")]
    public class CountTraps
    {
        static void Postfix(RoundManager __instance)
        {
            var landmines = UnityEngine.Object.FindObjectsByType<Landmine>(FindObjectsSortMode.None);

            CentralConfig.instance.mls.LogInfo("Number of landmines in the level: " + landmines.Length);

            var turrets = UnityEngine.Object.FindObjectsByType<Turret>(FindObjectsSortMode.None);

            CentralConfig.instance.mls.LogInfo("Number of turrets in the level: " + turrets.Length);

            var spikes = UnityEngine.Object.FindObjectsByType<SpikeRoofTrap>(FindObjectsSortMode.None);

            CentralConfig.instance.mls.LogInfo("Number of spike traps in the level: " + spikes.Length);

            MoldSpreadManager moldManager = UnityEngine.Object.FindFirstObjectByType<MoldSpreadManager>();
            CentralConfig.instance.mls.LogInfo("Generated Mold Count: " + moldManager.generatedMold.Count);
        }
    }
    [HarmonyPatch(typeof(RoundManager), "SpawnScrapInLevel")]
    public class FreeEnemies
    {
        static void Prefix(RoundManager __instance)
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                return;
            }

            if (CentralConfig.SyncConfig.FreeEnemies)
            {
                __instance.hourTimeBetweenEnemySpawnBatches = 1;
            }
        }
    }
    [HarmonyPatch(typeof(TimeOfDay), "MoveGlobalTime")]
    public static class TimeFix
    {
        static bool Prefix(TimeOfDay __instance)
        {
            if (!CentralConfig.SyncConfig.TimeSettings)
            {
                return true;
            }
            if (ImperiumCompatibility.enabled)
                if (ImperiumCompatibility.ImperiumTimePaused())
                {
                    return false;
                }

            StartOfRound startOfRound = StartOfRound.Instance;

            if (WaitForMoonsToRegister.CreateMoonConfig.WatiForShipToLandBeforeTimeMoves.ContainsKey(LevelManager.CurrentExtendedLevel))
            {
                if (WaitForMoonsToRegister.CreateMoonConfig.WatiForShipToLandBeforeTimeMoves[LevelManager.CurrentExtendedLevel].Value)
                {
                    if (!startOfRound.shipHasLanded && !__instance.shipLeavingAlertCalled)
                    {
                        return false;
                    }
                }
                __instance.globalTimeSpeedMultiplier = WaitForMoonsToRegister.CreateMoonConfig.TimeMultiplierOverride[LevelManager.CurrentExtendedLevel].Value;
            }

            float num = __instance.globalTime;
            __instance.globalTime = Mathf.Clamp(__instance.globalTime + (Time.deltaTime * __instance.globalTimeSpeedMultiplier), 0f, __instance.globalTimeAtEndOfDay);
            num = __instance.globalTime - num;
            __instance.timeUntilDeadline -= num;
            CentralConfig.shid += num;
            // CentralConfig.instance.mls.LogInfo("shid is now : " + CentralConfig.shid);

            return false;
        }
    }
    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    public static class FixTimeUntilDeadlineonDC
    {
        static void Postfix()
        {
            TimeOfDay timeOfDay = UnityEngine.Object.FindFirstObjectByType<TimeOfDay>();
            if (timeOfDay != null)
            {
                timeOfDay.timeUntilDeadline += CentralConfig.shid;
            }
            CentralConfig.shid = 0;
        }
    }
    [HarmonyPatch(typeof(TimeOfDay), "MoveGlobalTime")]
    public static class UpdateTimeFaster
    {
        static void Postfix()
        {
            if (!CentralConfig.SyncConfig.UpdateTimeFaster)
            {
                return;
            }
            float ToD = TimeOfDay.Instance.currentDayTime / TimeOfDay.Instance.totalTime;
            HUDManager.Instance.SetClock(ToD, TimeOfDay.Instance.numberOfHours);
        }
    }

    [HarmonyPatch(typeof(System.Random), "Next", new[] { typeof(int), typeof(int) })]
    public static class RandomNextPatch
    {
        static void Prefix(ref int minValue, ref int maxValue)
        {
            if (minValue > maxValue)
            {
                minValue = maxValue;
            }
        }
    }
    [HarmonyPatch(typeof(StartOfRound), "PassTimeToNextDay")]
    public static class DayTimePassFix
    {
        static bool Prefix(StartOfRound __instance, int connectedPlayersOnServer = 0)
        {
            if (!CentralConfig.SyncConfig.TimeSettings)
            {
                return true;
            }
            if (__instance.isChallengeFile)
            {
                TimeOfDay.Instance.globalTime = 100f;
                __instance.SetMapScreenInfoToCurrentLevel();
                return false;
            }
            float num = 980 - CentralConfig.shid;
            if (LevelManager.CurrentExtendedLevel.NumberlessPlanetName != "Gordion" || TimeOfDay.Instance.daysUntilDeadline <= 0)
            {
                TimeOfDay.Instance.timeUntilDeadline -= num;
                // CentralConfig.instance.mls.LogInfo("timeUntilDeadline adjustment was: " + num);
                TimeOfDay.Instance.OnDayChanged();
            }
            TimeOfDay.Instance.globalTime = 100f;
            TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
            if (LevelManager.CurrentExtendedLevel.NumberlessPlanetName != "Gordion" || TimeOfDay.Instance.daysUntilDeadline <= 0)
            {
                HUDManager.Instance.DisplayDaysLeft((int)Mathf.Floor(TimeOfDay.Instance.timeUntilDeadline / TimeOfDay.Instance.totalTime));
            }
            UnityEngine.Object.FindFirstObjectByType<Terminal>().SetItemSales();
            __instance.SetMapScreenInfoToCurrentLevel();
            if (TimeOfDay.Instance.timeUntilDeadline > 0f && TimeOfDay.Instance.daysUntilDeadline <= 0 && TimeOfDay.Instance.timesFulfilledQuota <= 0)
            {
                __instance.StartCoroutine(playDaysLeftAlertSFXDelayed());
            }
            CentralConfig.shid = 0;
            return false;
        }
        public static IEnumerator playDaysLeftAlertSFXDelayed()
        {
            yield return new WaitForSeconds(3f);
            StartOfRound.Instance.speakerAudioSource.PlayOneShot(StartOfRound.Instance.zeroDaysLeftAlertSFX);
        }
    }
    [HarmonyPatch(typeof(RoundManager), "PlotOutEnemiesForNextHour")]
    public static class ShowIntEnemyCount
    {
        public static int IntEnemiesSpawned = 0;
        static void Postfix(RoundManager __instance)
        {
            float fakenum = __instance.currentLevel.enemySpawnChanceThroughoutDay.Evaluate(__instance.timeScript.currentDayTime / __instance.timeScript.totalTime);
            float fakenum2 = fakenum + ((float)Mathf.Abs(TimeOfDay.Instance.daysUntilDeadline - 3) / 1.6f);
            int fakevalue = Mathf.Clamp(__instance.AnomalyRandom.Next((int)(fakenum2 - __instance.currentLevel.spawnProbabilityRange), (int)(fakenum + __instance.currentLevel.spawnProbabilityRange)), __instance.minEnemiesToSpawn, 20);
            fakevalue = Mathf.Clamp(fakevalue, 0, __instance.allEnemyVents.Length);

            IntEnemiesSpawned += fakevalue;
            CentralConfig.instance.mls.LogInfo("There are now " + IntEnemiesSpawned + " interior enemies");
        }
    }
    [HarmonyPatch(typeof(RoundManager), "PlotOutEnemiesForNextHour")]
    public static class MoarEnemies1
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var inst in instructions)
            {
                if (inst.opcode == OpCodes.Ldc_I4_S && (sbyte)inst.operand == 20)
                {
                    if (CentralConfig.SyncConfig.FreeEnemies)
                    {
                        inst.operand = (sbyte)127;
                    }
                }
                yield return inst;
            }
        }
    }
    [HarmonyPatch(typeof(RoundManager), "SpawnDaytimeEnemiesOutside")]
    public static class MoarEnemies2
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var inst in instructions)
            {
                if (inst.opcode == OpCodes.Ldc_I4_S && (sbyte)inst.operand == 20)
                {
                    if (CentralConfig.SyncConfig.FreeEnemies)
                    {
                        inst.operand = (sbyte)127;
                    }
                }
                yield return inst;
            }
        }
    }
    [HarmonyPatch(typeof(RoundManager), "SpawnEnemiesOutside")]
    public static class MoarEnemies3
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var inst in instructions)
            {
                if (inst.opcode == OpCodes.Ldc_I4_S && (sbyte)inst.operand == 20)
                {
                    if (CentralConfig.SyncConfig.FreeEnemies)
                    {
                        inst.operand = (sbyte)127;
                    }
                }
                yield return inst;
            }
        }
    }
    [HarmonyPatch(typeof(ExtendedLevel), "GetNumberlessPlanetName")]
    [HarmonyPriority(1000)]
    public class RenameCelest
    {
        public static string planetName = "";
        static bool Prefix(ref SelectableLevel selectableLevel, ref string __result)
        {
            if (selectableLevel != null)
            {
                planetName = new string(selectableLevel.PlanetName.Where(c => c != '\'').ToArray());
                planetName = new string(planetName.SkipWhile(c => !char.IsLetter(c)).ToArray());
                if (planetName == "Celest" && CentralConfig.SyncConfig.RenameCelest)
                {
                    __result = "Celeste";
                    return false;
                }
            }
            __result = planetName;
            return false;
        }
    }
}