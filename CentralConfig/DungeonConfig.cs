using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using DunGen;
using HarmonyLib;
using LethalLevelLoader;
using LethalLevelLoader.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.Netcode;
using UnityEngine;

namespace CentralConfig
{
    [HarmonyPatch(typeof(HangarShipDoor), "Start")]
    public class WaitForDungeonsToRegister
    {
        public static CreateDungeonConfig Config;
        public static ExtendedDungeonFlow DefaultFacility;
        [DataContract]
        public class CreateDungeonConfig : ConfigTemplate
        {
            // Declare config entries tied to the dictionary

            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<float>> MinDungeonSize;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<float>> MaxDungeonSize;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<float>> DungeonSizeScaler;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<float>> MapTileSize;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<int>> RandomSizeMin;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<int>> RandomSizeMax;

            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> DungeonPlanetNameList;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> DungeonTagList;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> DungeonRoutePriceList;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> DungeonModNameList;

            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> InteriorEnemyByDungeon;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, List<SpawnableEnemyWithRarity>> InteriorEnemiesD;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> InteriorEnemyReplacementD;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> InteriorEnemyMultiplierD;

            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> DayTimeEnemyByDungeon;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, List<SpawnableEnemyWithRarity>> DayEnemiesD;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> DayEnemyReplacementD;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> DayEnemyMultiplierD;

            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> NightTimeEnemyByDungeon;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, List<SpawnableEnemyWithRarity>> NightEnemiesD;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> NightEnemyReplacementD;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> NightEnemyMultiplierD;


            [DataMember] public static Dictionary<ExtendedDungeonFlow, SyncedEntry<string>> ScrapByDungeon;
            [DataMember] public static Dictionary<ExtendedDungeonFlow, List<SpawnableItemWithRarity>> ScrapD;

            public CreateDungeonConfig(ConfigFile cfg) : base(cfg, "CentralConfig", 0)
            {
                // Intialize config entries tied to the dictionary

                MinDungeonSize = new Dictionary<ExtendedDungeonFlow, SyncedEntry<float>>();
                MaxDungeonSize = new Dictionary<ExtendedDungeonFlow, SyncedEntry<float>>();
                DungeonSizeScaler = new Dictionary<ExtendedDungeonFlow, SyncedEntry<float>>();
                MapTileSize = new Dictionary<ExtendedDungeonFlow, SyncedEntry<float>>();
                RandomSizeMin = new Dictionary<ExtendedDungeonFlow, SyncedEntry<int>>();
                RandomSizeMax = new Dictionary<ExtendedDungeonFlow, SyncedEntry<int>>();

                DungeonPlanetNameList = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                DungeonTagList = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                DungeonRoutePriceList = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                DungeonModNameList = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();

                InteriorEnemyByDungeon = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                InteriorEnemiesD = new Dictionary<ExtendedDungeonFlow, List<SpawnableEnemyWithRarity>>();
                InteriorEnemyReplacementD = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                InteriorEnemyMultiplierD = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();

                DayTimeEnemyByDungeon = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                DayEnemiesD = new Dictionary<ExtendedDungeonFlow, List<SpawnableEnemyWithRarity>>();
                DayEnemyReplacementD = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                DayEnemyMultiplierD = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();

                NightTimeEnemyByDungeon = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                NightEnemiesD = new Dictionary<ExtendedDungeonFlow, List<SpawnableEnemyWithRarity>>();
                NightEnemyReplacementD = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                NightEnemyMultiplierD = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();

                ScrapByDungeon = new Dictionary<ExtendedDungeonFlow, SyncedEntry<string>>();
                ScrapD = new Dictionary<ExtendedDungeonFlow, List<SpawnableItemWithRarity>>();

                List<ExtendedDungeonFlow> AllExtendedDungeons;
                List<string> ignoreListEntries = ConfigAider.SplitStringsByDaComma(CentralConfig.SyncConfig.BlackListDungeons.Value).Select(entry => ConfigAider.CauterizeString(entry)).ToList();

                if (CentralConfig.SyncConfig.IsDunWhiteList)
                {
                    AllExtendedDungeons = PatchedContent.ExtendedDungeonFlows.Where(dungeon => ignoreListEntries.Any(b => ConfigAider.CauterizeString(dungeon.DungeonName).Equals(b))).ToList();
                }
                else
                {
                    AllExtendedDungeons = PatchedContent.ExtendedDungeonFlows.Where(dungeon => !ignoreListEntries.Any(b => ConfigAider.CauterizeString(dungeon.DungeonName).Equals(b))).ToList();
                }
                foreach (ExtendedDungeonFlow dungeon in AllExtendedDungeons)
                {
                    string Dun = dungeon.DungeonName + " (" + dungeon.name + ")";
                    Dun = Dun.Replace("13Exits", "3Exits").Replace("1ExtraLarge", "ExtraLarge");
                    string DungeonName = Dun.Replace("ExtendedDungeonFlow", "").Replace("Level", "");

                    // CentralConfig.instance.mls.LogInfo(dungeon.DungeonName);
                    if (DungeonName == "Facility (1)")
                    {
                        DefaultFacility = dungeon;
                        CentralConfig.instance.mls.LogInfo("Saved Default dungeon" + DefaultFacility.DungeonName);
                    }

                    // Size

                    if (CentralConfig.SyncConfig.DoDunSizeOverrides)
                    {
                        float MinSize;
                        float MaxSize;

                        if (dungeon.DynamicDungeonSizeMinMax.x <= 0)
                        {
                            MinSize = 1;
                        }
                        else
                        {
                            MinSize = dungeon.DynamicDungeonSizeMinMax.x;
                        }
                        if (dungeon.DynamicDungeonSizeMinMax.y <= 0)
                        {
                            MaxSize = 2;
                        }
                        else
                        {
                            MaxSize = dungeon.DynamicDungeonSizeMinMax.y;
                        }
                        if (DungeonName == "Haunted Mansion (2)")
                        {
                            dungeon.MapTileSize = 1.5f;
                        }

                        MinDungeonSize[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName, // Assigns the config with the dictionary so that it is unique to the level/moon/planet
                            DungeonName + " - Minimum Size Multiplier",
                            MinSize,
                            "Sets the min size multiplier this dungeon can have.");

                        MaxDungeonSize[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Maximum Size Multiplier",
                            MaxSize,
                            "Sets the max size multiplier this dungeon can have.");

                        DungeonSizeScaler[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Dungeon Size Scaler",
                            100 - (dungeon.DynamicDungeonSizeLerpRate * 100),
                            "This setting controls the strictness of the clamp. At 0%, the clamp is inactive. At 100%, the clamp is fully enforced, pulling any out-of-bounds values back to the nearest boundary. For percentages in between, out-of-bounds values are partially pulled back towards the nearest boundary. For example given a value of 50%, a value exceeding the max would be adjusted halfway back to the max.");

                        // CentralConfig.instance.mls.LogInfo(DungeonName + " has a maptilesize of " + dungeon.MapTileSize);

                        MapTileSize[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Map Tile Size",
                            dungeon.MapTileSize,
                            "The size multiplier from the moon is divided by this value before clamps are applied. It ensures that interiors with different '1x' tile counts and room sizes are comparable in total size.\nThe Facility is 1x and the Mansion is 1.5x in Vanilla.");

                        RandomSizeMin[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Random Size Multiplier Min",
                            dungeon.DungeonFlow.Length.Min,
                            "The minimum random size multiplier applied to this dungeon's overall size AFTER all previous settings (inclusive).");

                        RandomSizeMax[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Random Size Multiplier Max",
                            dungeon.DungeonFlow.Length.Max,
                            "The maximum random size multiplier applied to this dungeon's overall size AFTER all previous settings (inclusive).");
                    }

                    // Selection

                    if (CentralConfig.SyncConfig.DoDungeonSelectionOverrides)
                    {
                        string DungeonPlanetList = ConfigAider.ConvertStringWithRarityToString(dungeon.LevelMatchingProperties.planetNames);

                        DungeonPlanetNameList[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Add Dungeon by Planet Name",
                            DungeonPlanetList,
                            "The dungeon will be added to any moons listed here. This must be the exact numberless name. \"Experimentatio\" =/= \"Experimentation\"");

                        string dungeonTagList = ConfigAider.ConvertStringWithRarityToString(dungeon.LevelMatchingProperties.levelTags);

                        DungeonTagList[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Add Dungeon by Planet Tags",
                            dungeonTagList,
                            "The dungeon will be added to all moons with a matching tag");

                        string DungeonRouteList = ConfigAider.ConvertVector2WithRaritiesToString(dungeon.LevelMatchingProperties.currentRoutePrice);

                        DungeonRoutePriceList[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Add Dungeon by Route Price",
                            DungeonRouteList,
                            "This dungeon will be added to all moons that have a route price between the first two values with a rarity of the final value.");

                        string DungeonModList = ConfigAider.ConvertStringWithRarityToString(dungeon.LevelMatchingProperties.modNames);

                        DungeonModNameList[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Add Dungeon By Mod Name",
                            DungeonModList,
                            "The dungeon will be added to all moons of any mod listed here. This doesn't have to be an exact match, \"Rosie\" works for \"Rosie's Moons\".");
                    }

                    // Injection

                    if (CentralConfig.SyncConfig.DoEnemyInjectionsByDungeon)
                    {
                        InteriorEnemyByDungeon[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Add Interior Enemies",
                            "Default Values Were Empty",
                            "Enemies listed here in the EnemyName:rarity,EnemyName:rarity format will be added to the interior enemy list on any moons currently featuring this dungeon.");

                        InteriorEnemyReplacementD[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Replace Interior Enemies",
                            "Default Values Were Empty",
                            "In the example, \"Flowerman:Plantman,Crawler:Mauler\",\nOn any moons currently featuring this dungeon, Brackens will be replaced with hypothetical Plantmen, and Crawlers with hypothetical Maulers.\nYou could also use inputs such as \"Flowerman-15:Plantman~50\", this will give the Plantman a rarity of 15 instead of using the Bracken's and it will only have a 50% chance to replace.\nThis runs before the above entry adds new enemies, and after the weather and tag adds enemies.");

                        InteriorEnemyMultiplierD[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Multiply Interior Enemies",
                            "Default Values Were Empty",
                            "Enemies listed here will be multiplied by the assigned value while this is the current dungeon. \"Maneater:1.7,Jester:0.4\" will multiply the Maneater's rarity by 1.7 and the Jester's rarity by 0.4 when this dungeon is selected.");

                        DayTimeEnemyByDungeon[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Add Day Enemies",
                            "Default Values Were Empty",
                            "Enemies listed here in the EnemyName:rarity,EnemyName:rarity format will be added to the day enemy list on any moons currently featuring this dungeon.");

                        DayEnemyReplacementD[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Replace Day Enemies",
                            "Default Values Were Empty",
                            "In the example, \"Manticoil:Mantisoil,Docile Locust Bees:Angry Moth Wasps\",\nOn any moons currently featuring this dungeon, Manticoils will be replaced with hypothetical Mantisoils, and docile locust bees with hypothetical angry moth wasps.\nYou could also use inputs such as \"Manticoil-90:Mantisoil\", this will give the Mantisoil a rarity of 90 instead of using the Manticoil's and it will still have a 100% chance to replace.\nThis runs before the above entry adds new enemies, and after the weather and tag adds enemies.");

                        DayEnemyMultiplierD[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Multiply Day Enemies",
                            "Default Values Were Empty",
                            "Enemies listed here will be multiplied by the assigned value while this is the current dungeon. \"Red Locust Bees:2.4,Docile Locust Bees:0.8\" will multiply the Bee's rarity by 2.4 and the locust's rarity by 0.8 when this dungeon is selected.");

                        NightTimeEnemyByDungeon[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Add Night Enemies",
                            "Default Values Were Empty",
                            "Enemies listed here in the EnemyName:rarity,EnemyName:rarity format will be added to the night enemy list on any moons currently featuring this dungeon.");

                        NightEnemyReplacementD[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Replace Night Enemies",
                            "Default Values Were Empty",
                            "In the example, \"MouthDog:OceanDog,ForestGiant:FireGiant\",\nOn any moons currently featuring this dungeon, Mouthdogs will be replaced with hypothetical Oceandogs, and Forest giants with hypothetical Fire giants.\nYou could also use inputs such as \"MouthDog:OceanDog~75\", the OceanDog will still inherit the rarity from the MouthDog but it will only have a 75% chance to replace.\nThis runs before the above entry adds new enemies, and after the weather and tag adds enemies.");

                        NightEnemyMultiplierD[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Multiply Night Enemies",
                            "Default Values Were Empty",
                            "Enemies listed here will be multiplied by the assigned value while this is the current dungeon. \"MouthDog:0.33,ForestGiant:1.1\" will multiply the Dog's rarity by 0.33 and the giant's rarity by 1.1 when this dungeon is selected.");
                    }
                    if (CentralConfig.SyncConfig.DoScrapInjectionsByDungeon)
                    {
                        ScrapByDungeon[dungeon] = cfg.BindSyncedEntry("Dungeon: " + DungeonName,
                            DungeonName + " - Add Scrap",
                            "Default Values Were Empty",
                            "Scrap listed here in the ScrapName:rarity,ScrapName,rarity format will be added to the scrap list any moons currently featuring this dungeon");
                    }
                }
                if (CentralConfig.HarmonyTouch2)
                {
                    if (NetworkManager.Singleton.IsHost && (CentralConfig.SyncConfig.DoScrapInjectionsByDungeon || CentralConfig.SyncConfig.DoEnemyInjectionsByDungeon || CentralConfig.SyncConfig.DoDungeonSelectionOverrides || CentralConfig.SyncConfig.DoDunSizeOverrides))
                    {
                        CentralConfig.instance.mls.LogInfo("Dungeon config has been registered.");
                    }
                }
                CentralConfig.HarmonyTouch2 = true;
            }
        }
        static void Prefix()
        {
            CentralConfig.ConfigFile2 = new CreateDungeonConfig(CentralConfig.instance.Config); // Dungeon config is created when you join a lobby (So every other config is already applied)
        }
    }
    public class ApplyDungeonConfig
    {
        public static bool Ready = false;
        public void UpdateDungeonValues() // This is called on as a postfix on the same method as creating the config stuff so it gets applied here right after the config is intialized
        {
            List<ExtendedDungeonFlow> AllExtendedDungeons;
            List<string> ignoreListEntries = ConfigAider.SplitStringsByDaComma(CentralConfig.SyncConfig.BlackListDungeons.Value).Select(entry => ConfigAider.CauterizeString(entry)).ToList();

            if (CentralConfig.SyncConfig.IsDunWhiteList)
            {
                AllExtendedDungeons = PatchedContent.ExtendedDungeonFlows.Where(dungeon => ignoreListEntries.Any(b => ConfigAider.CauterizeString(dungeon.DungeonName).Equals(b))).ToList();
            }
            else
            {
                AllExtendedDungeons = PatchedContent.ExtendedDungeonFlows.Where(dungeon => !ignoreListEntries.Any(b => ConfigAider.CauterizeString(dungeon.DungeonName).Equals(b))).ToList();
            }
            foreach (ExtendedDungeonFlow dungeon in AllExtendedDungeons)
            {

                string Dun = dungeon.DungeonName + " (" + dungeon.name + ")";
                Dun = Dun.Replace("13Exits", "3Exits").Replace("1ExtraLarge", "ExtraLarge");
                string DungeonName = Dun.Replace("ExtendedDungeonFlow", "").Replace("Level", "");


                // Size

                if (CentralConfig.SyncConfig.DoDunSizeOverrides)
                {
                    Vector2 newSize = dungeon.DynamicDungeonSizeMinMax;
                    newSize.x = WaitForDungeonsToRegister.CreateDungeonConfig.MinDungeonSize[dungeon];
                    newSize.y = WaitForDungeonsToRegister.CreateDungeonConfig.MaxDungeonSize[dungeon];
                    dungeon.DynamicDungeonSizeMinMax = newSize;

                    dungeon.DynamicDungeonSizeLerpRate = WaitForDungeonsToRegister.CreateDungeonConfig.DungeonSizeScaler[dungeon];

                    dungeon.DungeonFlow.Length.Min = WaitForDungeonsToRegister.CreateDungeonConfig.RandomSizeMin[dungeon];
                    dungeon.DungeonFlow.Length.Max = WaitForDungeonsToRegister.CreateDungeonConfig.RandomSizeMax[dungeon];
                }

                // Injection

                if (CentralConfig.SyncConfig.DoDungeonSelectionOverrides && NetworkManager.Singleton.IsHost)
                {
                    dungeon.LevelMatchingProperties.planetNames.Clear();
                    dungeon.LevelMatchingProperties.levelTags.Clear();
                    dungeon.LevelMatchingProperties.modNames.Clear();
                    dungeon.LevelMatchingProperties.currentRoutePrice.Clear();

                    // PlanetName
                    string PlanetNameStr = WaitForDungeonsToRegister.CreateDungeonConfig.DungeonPlanetNameList[dungeon];
                    Vector2 planetNameRarity = new Vector2(-99999, 99999);
                    List<StringWithRarity> InjectionPlanets = ConfigAider.ConvertPlanetNameStringToStringWithRarityList(PlanetNameStr, planetNameRarity);
                    if (InjectionPlanets.Count > 0)
                    {
                        dungeon.LevelMatchingProperties.planetNames = InjectionPlanets;
                    }

                    // Tags
                    string TagStr = WaitForDungeonsToRegister.CreateDungeonConfig.DungeonTagList[dungeon];
                    Vector2 tagRarity = new Vector2(-99999, 99999);
                    List<StringWithRarity> InjectionTags = ConfigAider.ConvertTagStringToStringWithRarityList(TagStr, tagRarity);
                    if (InjectionTags.Count > 0)
                    {
                        dungeon.LevelMatchingProperties.levelTags = InjectionTags;
                    }

                    // ModName
                    string ModNameStr = WaitForDungeonsToRegister.CreateDungeonConfig.DungeonModNameList[dungeon];
                    Vector2 modNameRarity = new Vector2(-99999, 99999);
                    List<StringWithRarity> InjectionMods = ConfigAider.ConvertModStringToStringWithRarityList(ModNameStr, modNameRarity);
                    if (InjectionMods.Count > 0)
                    {
                        dungeon.LevelMatchingProperties.modNames = InjectionMods;
                    }

                    // RoutePrice
                    string RoutePriceStr = WaitForDungeonsToRegister.CreateDungeonConfig.DungeonRoutePriceList[dungeon];
                    Vector2 RoutePriceRarity = new Vector2(-99999, 99999);
                    List<Vector2WithRarity> InjectionPrices = ConfigAider.ConvertStringToVector2WithRarityList(RoutePriceStr, RoutePriceRarity);
                    if (InjectionPrices.Count > 0)
                    {
                        dungeon.LevelMatchingProperties.currentRoutePrice = InjectionPrices;
                    }
                }

                if (CentralConfig.SyncConfig.DoEnemyInjectionsByDungeon && NetworkManager.Singleton.IsHost)
                {
                    if (WaitForDungeonsToRegister.CreateDungeonConfig.InteriorEnemyByDungeon.ContainsKey(dungeon))
                    {
                        string IntEneStr = WaitForDungeonsToRegister.CreateDungeonConfig.InteriorEnemyByDungeon[dungeon];
                        Vector2 clampIntRarity = new Vector2(-99999, 99999);
                        List<SpawnableEnemyWithRarity> interiorenemyList = ConfigAider.ConvertStringToEnemyList(IntEneStr, clampIntRarity);
                        WaitForDungeonsToRegister.CreateDungeonConfig.InteriorEnemiesD[dungeon] = interiorenemyList;

                        string DayEneStr = WaitForDungeonsToRegister.CreateDungeonConfig.DayTimeEnemyByDungeon[dungeon];
                        Vector2 clampDayRarity = new Vector2(-99999, 99999);
                        List<SpawnableEnemyWithRarity> dayenemyList = ConfigAider.ConvertStringToEnemyList(DayEneStr, clampDayRarity);
                        WaitForDungeonsToRegister.CreateDungeonConfig.DayEnemiesD[dungeon] = dayenemyList;

                        string NightEneStr = WaitForDungeonsToRegister.CreateDungeonConfig.NightTimeEnemyByDungeon[dungeon];
                        Vector2 clampNightRarity = new Vector2(-99999, 99999);
                        List<SpawnableEnemyWithRarity> nightenemyList = ConfigAider.ConvertStringToEnemyList(NightEneStr, clampNightRarity);
                        WaitForDungeonsToRegister.CreateDungeonConfig.NightEnemiesD[dungeon] = nightenemyList;
                    }
                }

                if (CentralConfig.SyncConfig.DoScrapInjectionsByDungeon && NetworkManager.Singleton.IsHost)
                {
                    if (WaitForDungeonsToRegister.CreateDungeonConfig.ScrapByDungeon.ContainsKey(dungeon))
                    {
                        string ScrStr = WaitForDungeonsToRegister.CreateDungeonConfig.ScrapByDungeon[dungeon];
                        Vector2 clampScrRarity = new Vector2(-99999, 99999);
                        List<SpawnableItemWithRarity> scraplist = ConfigAider.ConvertStringToItemList(ScrStr, clampScrRarity);
                        WaitForDungeonsToRegister.CreateDungeonConfig.ScrapD[dungeon] = scraplist;
                    }
                }
            }
            if (NetworkManager.Singleton.IsHost && (CentralConfig.SyncConfig.DoScrapInjectionsByDungeon || CentralConfig.SyncConfig.DoEnemyInjectionsByDungeon || CentralConfig.SyncConfig.DoDungeonSelectionOverrides || CentralConfig.SyncConfig.DoDunSizeOverrides))
            {
                CentralConfig.instance.mls.LogInfo("Dungeon config Values Applied.");
            }
            if (CentralConfig.SyncConfig.DungeonShuffler && NetworkManager.Singleton.IsHost)
            {
                int seed;
                if (ES3.KeyExists("LastGlorp", GameNetworkManager.Instance.currentSaveFileName))
                {
                    seed = ES3.Load<int>("LastGlorp", GameNetworkManager.Instance.currentSaveFileName);
                    ShuffleSaver.dungeonrandom = new System.Random(seed);
                }
                else
                {
                    seed = StartOfRound.Instance.randomMapSeed;
                    ShuffleSaver.dungeonrandom = new System.Random(StartOfRound.Instance.randomMapSeed);
                }

                foreach (ExtendedDungeonFlow flow in PatchedContent.ExtendedDungeonFlows)
                {
                    ResetChanger.ResetOnDisconnect.AllDungeons.Add(flow);
                    ShuffleSaver.DungeonMoonMatches[flow] = flow.LevelMatchingProperties.planetNames;
                    ShuffleSaver.DungeonModMatches[flow] = flow.LevelMatchingProperties.modNames;
                    ShuffleSaver.DungeonTagMatches[flow] = flow.LevelMatchingProperties.levelTags;
                    ShuffleSaver.DungeonRouteMatches[flow] = flow.LevelMatchingProperties.currentRoutePrice;

                    string gen = flow.DungeonName + " (" + flow.name + ")";
                    gen = gen.Replace("13Exits", "3Exits").Replace("1ExtraLarge", "ExtraLarge");
                    string FlowName = gen.Replace("ExtendedDungeonFlow", "").Replace("Level", "");

                    flow.LevelMatchingProperties.planetNames = ConfigAider.IncreaseDungeonRarities(flow.LevelMatchingProperties.planetNames, flow, FlowName);
                    flow.LevelMatchingProperties.modNames = ConfigAider.IncreaseDungeonRarities(flow.LevelMatchingProperties.modNames, flow, FlowName);
                    flow.LevelMatchingProperties.levelTags = ConfigAider.IncreaseDungeonRarities(flow.LevelMatchingProperties.levelTags, flow, FlowName);
                    flow.LevelMatchingProperties.currentRoutePrice = ConfigAider.IncreaseDungeonRaritiesVector2(flow.LevelMatchingProperties.currentRoutePrice, flow, FlowName);
                }
                ShuffleSaver.LastGlorp = seed;
            }
            Ready = true;
        }
    }
    [HarmonyPatch(typeof(HangarShipDoor), "Start")]
    public class FrApplyDungeon
    {
        static void Postfix()
        {
            ApplyDungeonConfig applyConfig = new ApplyDungeonConfig();
            applyConfig.UpdateDungeonValues();
        }
    }
    [HarmonyPatch(typeof(RoundManager), "GenerateNewFloor")]
    public static class NewDungeonGenerator
    {
        static float PreClampValue;
        static bool Prefix(RoundManager __instance)
        {
            if (LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes != null && LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes.Length != 0)
            {
                List<int> list = new List<int>();
                for (int i = 0; i < LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes.Length; i++)
                {
                    list.Add(LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[i].rarity);
                    // CentralConfig.instance.mls.LogInfo($"DungeonFlowType {i}: ID = {LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[i].id}, Rarity = {LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[i].rarity}");
                }

                System.Random seededRandom = new System.Random(StartOfRound.Instance.randomMapSeed);
                int DungeonID = __instance.GetRandomWeightedIndex(list.ToArray(), seededRandom);
                // CentralConfig.instance.mls.LogInfo($"Selected DungeonID: {DungeonID}");

                __instance.dungeonGenerator.Generator.DungeonFlow = __instance.dungeonFlowTypes[LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[DungeonID].id].dungeonFlow;
                __instance.currentDungeonType = LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[DungeonID].id;
                // CentralConfig.instance.mls.LogInfo($"Assigned DungeonFlow: {__instance.dungeonGenerator.Generator.DungeonFlow}, CurrentDungeonType: {__instance.currentDungeonType}");

                if (LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[DungeonID].overrideLevelAmbience != null)
                {
                    SoundManager.Instance.currentLevelAmbience = LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[DungeonID].overrideLevelAmbience;
                }
                else if (LevelManager.CurrentExtendedLevel.SelectableLevel.levelAmbienceClips != null)
                {
                    SoundManager.Instance.currentLevelAmbience = LevelManager.CurrentExtendedLevel.SelectableLevel.levelAmbienceClips;
                }
            }

            ExtendedDungeonFlow dungeon = DungeonManager.CurrentExtendedDungeonFlow;
            string Dun = dungeon.DungeonName + " (" + dungeon.name + ")";
            Dun = Dun.Replace("13Exits", "3Exits").Replace("1ExtraLarge", "ExtraLarge");
            string DungeonName = Dun.Replace("ExtendedDungeonFlow", "").Replace("Level", "");
            if (NetworkManager.Singleton.IsHost)
            {
                CentralConfig.instance.mls.LogInfo("Dungeon Selected: " + DungeonName);
            }

            if (CentralConfig.SyncConfig.DungeonShuffler && NetworkManager.Singleton.IsHost)
            {
                DungeonShuffler.lastpossibledungeons = DungeonManager.GetValidExtendedDungeonFlows(LevelManager.CurrentExtendedLevel, false);
                DungeonShuffler.lastdungeon = dungeon;
            }

            // __instance.dungeonGenerator.Generator.ShouldRandomizeSeed = false;
            // __instance.dungeonGenerator.Generator.Seed = StartOfRound.Instance.randomMapSeed;

            float NewMultiplier = LevelManager.CurrentExtendedLevel.SelectableLevel.factorySizeMultiplier;
            if (CentralConfig.SyncConfig.DoDunSizeOverrides)
            {
                if (WaitForDungeonsToRegister.CreateDungeonConfig.MapTileSize.ContainsKey(DungeonManager.CurrentExtendedDungeonFlow))
                {
                    NewMultiplier /= WaitForDungeonsToRegister.CreateDungeonConfig.MapTileSize[DungeonManager.CurrentExtendedDungeonFlow];
                    NewMultiplier *= __instance.mapSizeMultiplier;
                    NewMultiplier = (float)((double)Mathf.Round(NewMultiplier * 100f) / 100.0);

                    PreClampValue = NewMultiplier;
                    if (NewMultiplier < WaitForDungeonsToRegister.CreateDungeonConfig.MinDungeonSize[DungeonManager.CurrentExtendedDungeonFlow])
                    {
                        NewMultiplier = Mathf.Lerp(NewMultiplier, WaitForDungeonsToRegister.CreateDungeonConfig.MinDungeonSize[DungeonManager.CurrentExtendedDungeonFlow], WaitForDungeonsToRegister.CreateDungeonConfig.DungeonSizeScaler[DungeonManager.CurrentExtendedDungeonFlow] / 100);
                    }
                    else if (NewMultiplier > WaitForDungeonsToRegister.CreateDungeonConfig.MaxDungeonSize[DungeonManager.CurrentExtendedDungeonFlow])
                    {
                        NewMultiplier = Mathf.Lerp(NewMultiplier, WaitForDungeonsToRegister.CreateDungeonConfig.MaxDungeonSize[DungeonManager.CurrentExtendedDungeonFlow], WaitForDungeonsToRegister.CreateDungeonConfig.DungeonSizeScaler[DungeonManager.CurrentExtendedDungeonFlow] / 100);
                    }
                    NewMultiplier = (float)((double)Mathf.Round(NewMultiplier * 100f) / 100.0);
                    if (PreClampValue != NewMultiplier)
                    {
                        if (NetworkManager.Singleton.IsHost)
                        {
                            CentralConfig.instance.mls.LogInfo("Clamps for the dungeon have been applied. Original value: " + PreClampValue + " New value: " + NewMultiplier);
                        }
                    }
                    else
                    {
                        if (NetworkManager.Singleton.IsHost)
                        {
                            CentralConfig.instance.mls.LogInfo("The size was within the clamp range.");
                        }
                    }
                }
                else
                {
                    NewMultiplier /= DungeonManager.CurrentExtendedDungeonFlow.MapTileSize;
                    NewMultiplier *= __instance.mapSizeMultiplier;
                    NewMultiplier = (float)((double)Mathf.Round(NewMultiplier * 100f) / 100.0);

                    if (NetworkManager.Singleton.IsHost)
                    {
                        CentralConfig.instance.mls.LogInfo("The current dungeon is blacklisted. No clamping will be applied.");
                    }
                }
            }
            else
            {
                NewMultiplier /= DungeonManager.CurrentExtendedDungeonFlow.MapTileSize;
                NewMultiplier *= __instance.mapSizeMultiplier;
                NewMultiplier = (float)((double)Mathf.Round(NewMultiplier * 100f) / 100.0);
            }
            if (CentralConfig.SyncConfig.ScaleDungeonSizeByPlayers)
            {
                float PlayerDiff = StartOfRound.Instance.connectedPlayersAmount + 1 - (float)MiscConfig.CreateMiscConfig.SDSBPThreshold;
                float DSBP = Mathf.Clamp(Mathf.Pow(1f + (MiscConfig.CreateMiscConfig.SDSBPPercentIncrease + MiscConfig.CreateMiscConfig.SDSBPIncreaseChange * Mathf.Abs(PlayerDiff)) / 100f, PlayerDiff), MiscConfig.CreateMiscConfig.SDSBPMinIncrease, MiscConfig.CreateMiscConfig.SDSBPMaxIncrease);
                CentralConfig.instance.mls.LogInfo($"Dungeon Size Scaler by PlayerCount: {DSBP}x");
                NewMultiplier *= DSBP;
            }
            if (NewMultiplier < 0)
            {
                NewMultiplier = 1f;
                if (NetworkManager.Singleton.IsHost)
                {
                    CentralConfig.instance.mls.LogInfo("Dungeon size is in the negatives, go fix your size settings. It is now 1x");
                }
            }
            __instance.dungeonGenerator.Generator.LengthMultiplier = NewMultiplier;

            if (!CentralConfig.SyncConfig.UseNewGen)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    CentralConfig.instance.mls.LogInfo($"Generation safeguards are disabled, generating without them... The dungeon has a final size multiplier of {NewMultiplier}x.");
                }
                __instance.dungeonGenerator.Generate();
                /*for (int i = 0; i < 100; i++)
                {
                    List<int> list = new List<int>();
                    for (int f = 0; f < LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes.Length; f++)
                    {
                        list.Add(LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[f].rarity);
                        // CentralConfig.instance.mls.LogInfo($"DungeonFlowType {i}: ID = {LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[i].id}, Rarity = {LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[i].rarity}");
                    }

                    System.Random seededRandom = new System.Random();
                    int DungeonID = __instance.GetRandomWeightedIndex(list.ToArray(), seededRandom);
                    // CentralConfig.instance.mls.LogInfo($"Selected DungeonID: {DungeonID}");

                    __instance.dungeonGenerator.Generator.DungeonFlow = __instance.dungeonFlowTypes[LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[DungeonID].id].dungeonFlow;
                    CentralConfig.instance.mls.LogInfo("Size Multiplier: " + NewMultiplier);
                    __instance.dungeonGenerator.Generate();
                    TileCounter.CountTiles();
                    __instance.dungeonGenerator.Generator.Cancel();
                    __instance.dungeonGenerator.Generator.Seed = StartOfRound.Instance.randomMapSeed;
                    __instance.dungeonGenerator.Generator.LengthMultiplier = NewMultiplier;
                    CentralConfig.instance.mls.LogInfo("Attempt # " + TileCounter.CallNumber);
                }
                int countsum = TileCounter.TileCounts.Sum();
                float averagecount = (float)countsum / TileCounter.TileCounts.Count;
                averagecount = (float)((double)Mathf.Round(averagecount * 100f) / 100.0);

                float lengthsum = TileCounter.TileLengths.Sum();
                float averagelength = lengthsum / TileCounter.TileCounts.Count;
                float widthsum = TileCounter.TileWidths.Sum();
                float averagewidth = widthsum / TileCounter.TileWidths.Count;
                float heightsum = TileCounter.TileHeights.Sum();
                float averageheight = heightsum / TileCounter.TileHeights.Count;

                int min = TileCounter.TileCounts.Min();
                int max = TileCounter.TileCounts.Max();

                CentralConfig.instance.mls.LogInfo(TileCounter.BigLog);
                float FloorArea = averagelength * averagewidth;
                FloorArea = (float)((double)Mathf.Round(FloorArea * 100f) / 100.0);
                CentralConfig.instance.mls.LogInfo("Tests: " + TileCounter.TileCounts.Count + " sum: " + countsum + " average: " + averagecount + " min: " + min + " max: " + max + " Average Length: " + averagelength + " Average Height: " + averageheight + " Average Width: " + averagewidth + " Floor Area: " + FloorArea);*/
                return false;
            }

            if (DungeonName == "Black Mesa (Black Mesa)")
            {
                __instance.dungeonGenerator.Generator.GenerateAsynchronously = false;
            }
            else if (LoadstoneCompatibility.enabled)
            {
                if (LoadstoneCompatibility.IsLoadStoneAsyncing())
                    __instance.dungeonGenerator.Generator.GenerateAsynchronously = true;
            }
            else if (LoadstoneNCompatibility.enabled)
            {
                if (LoadstoneNCompatibility.IsLoadStoneNAsyncing())
                    __instance.dungeonGenerator.Generator.GenerateAsynchronously = true;
            }

            try
            {
                __instance.dungeonGenerator.Generate();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Dungeon Generation failed.")
                {
                    if (NetworkManager.Singleton.IsHost)
                    {
                        CentralConfig.instance.mls.LogInfo("Dungeon Generation has failed. Defaulting interior");
                    }
                    __instance.dungeonGenerator.Generator.DungeonFlow = WaitForDungeonsToRegister.DefaultFacility.DungeonFlow;
                    __instance.currentDungeonType = 0;

                    float factorySizeMultiplier;
                    if (LevelManager.CurrentExtendedLevel.SelectableLevel.factorySizeMultiplier >= 1)
                    {
                        factorySizeMultiplier = LevelManager.CurrentExtendedLevel.SelectableLevel.factorySizeMultiplier;
                    }
                    else
                    {
                        factorySizeMultiplier = 1f;
                    }
                    __instance.dungeonGenerator.Generator.LengthMultiplier = Mathf.Clamp(factorySizeMultiplier, 1f, 2.2f);

                    if (LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[0].overrideLevelAmbience != null)
                    {
                        SoundManager.Instance.currentLevelAmbience = LevelManager.CurrentExtendedLevel.SelectableLevel.dungeonFlowTypes[0].overrideLevelAmbience;
                    }
                    else if (LevelManager.CurrentExtendedLevel.SelectableLevel.levelAmbienceClips != null)
                    {
                        SoundManager.Instance.currentLevelAmbience = LevelManager.CurrentExtendedLevel.SelectableLevel.levelAmbienceClips;
                    }

                    __instance.dungeonGenerator.Generator.ShouldRandomizeSeed = false;
                    __instance.dungeonGenerator.Generator.Seed = StartOfRound.Instance.randomMapSeed - 420;
                    if (NetworkManager.Singleton.IsHost)
                    {
                        CentralConfig.instance.mls.LogInfo(DungeonManager.CurrentExtendedDungeonFlow.DungeonName);
                    }

                    InnerGenerateWithRetries.Defaulted = true;
                    __instance.dungeonGenerator.Generate();
                    if (NetworkManager.Singleton.IsHost)
                    {
                        CentralConfig.instance.mls.LogInfo("Dungeon has been loaded by Central Config using a safeguard dungeon.");
                    }
                }
            }
            if (!InnerGenerateWithRetries.Defaulted)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    CentralConfig.instance.mls.LogInfo("Dungeon has been loaded by Central Config. Final dungeon size multiplier is: " + InnerGenerateWithRetries.LengthMultiplier + "x after " + InnerGenerateWithRetries.RetryCounter + " attempts.");
                }
            }
            InnerGenerateWithRetries.RetryCounter = 0;
            InnerGenerateWithRetries.TryBig = false;
            InnerGenerateWithRetries.GenFailed = false;
            InnerGenerateWithRetries.Defaulted = false;

            InnerGenerateWithRetries.Catch20 = false;
            InnerGenerateWithRetries.Catch10 = false;
            InnerGenerateWithRetries.Catch5 = false;
            InnerGenerateWithRetries.Catch3 = false;
            InnerGenerateWithRetries.Catch2 = false;
            InnerGenerateWithRetries.Catch1 = false;

            return false;
        }
    }
    [HarmonyPatch(typeof(DungeonGenerator), "InnerGenerate")]
    public static class InnerGenerateWithRetries
    {
        public static int RetryCounter = 0;
        public static bool TryBig = false;
        public static bool GenFailed = false;
        public static bool Defaulted = false;
        public static float LengthMultiplier;

        public static bool Catch20 = false;
        public static bool Catch10 = false;
        public static bool Catch5 = false;
        public static bool Catch3 = false;
        public static bool Catch2 = false;
        public static bool Catch1 = false;
        static bool Prefix(DungeonGenerator __instance, ref bool isRetry)
        {
            if (!CentralConfig.SyncConfig.UseNewGen || Defaulted)
            {
                RetryCounter++;
                return true;
            }
            __instance.Seed = StartOfRound.Instance.randomMapSeed;
            // CentralConfig.instance.mls.LogInfo("Seed: " + __instance.Seed);

            if (RetryCounter >= CentralConfig.SyncConfig.UnShrankDungenTries)
            {
                if (__instance.LengthMultiplier > 20f)
                {
                    __instance.LengthMultiplier -= __instance.LengthMultiplier * 0.1f;
                }
                else if (__instance.LengthMultiplier > 10f)
                {
                    if (!Catch20)
                    {
                        __instance.LengthMultiplier = 20f;
                        Catch20 = true;
                    }
                    else
                    {
                        __instance.LengthMultiplier -= 10f / CentralConfig.SyncConfig.BracketTries;
                    }
                }
                else if (__instance.LengthMultiplier > 5f)
                {
                    if (!Catch10)
                    {
                        __instance.LengthMultiplier = 10f;
                        Catch10 = true;
                    }
                    else
                    {
                        __instance.LengthMultiplier -= 5f / CentralConfig.SyncConfig.BracketTries;
                    }
                }
                else if (__instance.LengthMultiplier > 3f)
                {
                    if (!Catch5)
                    {
                        __instance.LengthMultiplier = 5f;
                        Catch5 = true;
                    }
                    else
                    {
                        __instance.LengthMultiplier -= 3f / CentralConfig.SyncConfig.BracketTries;
                    }
                }
                else if (__instance.LengthMultiplier > 2f)
                {
                    if (!Catch3)
                    {
                        __instance.LengthMultiplier = 3f;
                        Catch3 = true;
                    }
                    else
                    {
                        __instance.LengthMultiplier -= 2f / CentralConfig.SyncConfig.BracketTries;
                    }
                }
                else if (__instance.LengthMultiplier > 1f)
                {
                    if (!Catch2)
                    {
                        __instance.LengthMultiplier = 2f;
                        Catch2 = true;
                    }
                    else
                    {
                        __instance.LengthMultiplier -= 1f / CentralConfig.SyncConfig.BracketTries;
                    }
                }
                else
                {
                    if (!Catch1)
                    {
                        __instance.LengthMultiplier = 1f;
                        Catch1 = true;
                    }
                    else
                    {
                        __instance.LengthMultiplier -= 0.02f;
                    }
                }
                __instance.LengthMultiplier = (float)Math.Round(__instance.LengthMultiplier, 2);
                if (NetworkManager.Singleton.IsHost)
                {
                    CentralConfig.instance.mls.LogInfo("Dungeon Length Multiplier reduced to: " + __instance.LengthMultiplier);
                }
            }
            else
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    CentralConfig.instance.mls.LogInfo("Retrying before reduction on attempt #" + (RetryCounter + 1));
                }
            }
            RetryCounter++;

            if (__instance.LengthMultiplier <= 0 && TryBig == true)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    CentralConfig.instance.mls.LogInfo("Dungeon Generation has failed " + RetryCounter + " times, this would indicate the dungeon refused to generate at any size. Please notify me if you see this.");
                }
                GenFailed = true;
                isRetry = false;
                __instance.Cancel();
                throw new Exception("Dungeon Generation failed.");
            }
            if (__instance.LengthMultiplier <= 0 && TryBig == false)
            {
                if (LevelManager.CurrentExtendedLevel.SelectableLevel.factorySizeMultiplier >= 0)
                {
                    __instance.LengthMultiplier = LevelManager.CurrentExtendedLevel.SelectableLevel.factorySizeMultiplier + 1f;
                }
                else
                {
                    __instance.LengthMultiplier = 1f;
                }
                TryBig = true;
                if (NetworkManager.Singleton.IsHost)
                {
                    CentralConfig.instance.mls.LogInfo("Trying to increase dungeon size in case it was too small.");
                }
            }
            isRetry = false;
            LengthMultiplier = __instance.LengthMultiplier;
            return true;
        }
    }
    [HarmonyPatch(typeof(DungeonGenerator), "InnerGenerate")]
    public static class LogFinalSize
    {
        static void Postfix(DungeonGenerator __instance)
        {
            int randomValue = __instance.DungeonFlow.Length.GetRandom(__instance.RandomStream);
            CentralConfig.instance.mls.LogInfo("Selected random value from IntRange: " + randomValue);

            float multipliedValue = (float)randomValue * __instance.LengthMultiplier;
            CentralConfig.instance.mls.LogInfo("Multiplied value: " + multipliedValue);

            int roundedValue = Mathf.RoundToInt(multipliedValue);
            CentralConfig.instance.mls.LogInfo("Rounded value: " + roundedValue);

            int finalTargetLength = Mathf.Max(roundedValue, 2);
            CentralConfig.instance.mls.LogInfo("Final target length: " + finalTargetLength);
        }
    }
    public static class TileCounter
    {
        public static string BigLog;
        public static List<int> TileCounts = new List<int>();
        public static List<float> TileLengths = new List<float>();
        public static List<float> TileWidths = new List<float>();
        public static List<float> TileHeights = new List<float>();
        public static Tile[] tiles;
        public static int CallNumber;
        public static void CountTiles()
        {
            tiles = new Tile[0];
            tiles = UnityEngine.Object.FindObjectsByType<Tile>(FindObjectsSortMode.None);
            TileCounts.Add(tiles.Length);
            BigLog += "\nThere are: " + tiles.Length + " tiles.";
            CallNumber++;
            float averageLength = GetAverageTileLength();
            TileLengths.Add(averageLength);
            float averageWidth = GetAverageTileWidth();
            TileWidths.Add(averageWidth);
            float averageHeight = GetAverageTileHeight();
            TileHeights.Add(averageHeight);
        }
        public static float GetAverageTileLength()
        {
            float totalLength = tiles.Sum(tile => tile.TileBoundsOverride.size.x);
            float averageLength = totalLength / tiles.Length;
            return averageLength;
        }

        public static float GetAverageTileWidth()
        {
            float totalWidth = tiles.Sum(tile => tile.TileBoundsOverride.size.z);
            float averageWidth = totalWidth / tiles.Length;
            return averageWidth;
        }
        public static float GetAverageTileHeight()
        {
            float totalHeight = tiles.Sum(tile => tile.TileBoundsOverride.size.y);
            float averageHeight = totalHeight / tiles.Length;
            return averageHeight;
        }
    }
    [HarmonyPatch(typeof(RoundManager), "GenerateNewFloor")]
    [HarmonyPriority(69)]
    public class EnactDungeonInjections
    {
        public static string EnemyTables;
        public static string ScrapSpawned;

        /*public static string AllEnemiesTable = "All Enemies Registered:";
        public static string AllItemsTable = "All Items Registered:";
        public static string PostableEnemies = "Postable Enemies:\n";
        public static string PostableScrap = "Postable Scrap:\n";
        public static string ScrapInLevels = "Scrap in levels:\n";
        public static string ScrapNotInLevels = "Scrap NOT in levels:\n";
        public static string ScrapInFewLevels = "Scrap in FEW levels:\n";
        public static Dictionary<Item, int> ItemAppearance = new Dictionary<Item, int>();
        public static Dictionary<ExtendedLevel, int> ScrapListLength = new Dictionary<ExtendedLevel, int>();
        public static int totalScrapTotal;
        public static int Moons;*/
        static void Postfix()
        {
            if (!NetworkManager.Singleton.IsHost)
            {
                return;
            }

            if (CentralConfig.SyncConfig.DoEnemyInjectionsByDungeon)
            {
                if (WaitForDungeonsToRegister.CreateDungeonConfig.InteriorEnemyByDungeon.ContainsKey(DungeonManager.CurrentExtendedDungeonFlow))
                {
                    string OoO = CentralConfig.SyncConfig.OoO;
                    var pairs = OoO.Split(',');

                    foreach (var pair in pairs)
                    {
                        if (ConfigAider.CauterizeString(pair) == "add")
                        {
                            if (WaitForDungeonsToRegister.CreateDungeonConfig.InteriorEnemiesD[DungeonManager.CurrentExtendedDungeonFlow].Count > 0)
                            {
                                LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies = LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies.Concat(WaitForDungeonsToRegister.CreateDungeonConfig.InteriorEnemiesD[DungeonManager.CurrentExtendedDungeonFlow]).ToList();
                            }
                            if (WaitForDungeonsToRegister.CreateDungeonConfig.DayEnemiesD[DungeonManager.CurrentExtendedDungeonFlow].Count > 0)
                            {
                                LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies = LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies.Concat(WaitForDungeonsToRegister.CreateDungeonConfig.DayEnemiesD[DungeonManager.CurrentExtendedDungeonFlow]).ToList();
                            }
                            if (WaitForDungeonsToRegister.CreateDungeonConfig.NightEnemiesD[DungeonManager.CurrentExtendedDungeonFlow].Count > 0)
                            {
                                LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies = LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies.Concat(WaitForDungeonsToRegister.CreateDungeonConfig.NightEnemiesD[DungeonManager.CurrentExtendedDungeonFlow]).ToList();
                            }
                        }
                        else if (ConfigAider.CauterizeString(pair) == "multiply")
                        {
                            LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies = ConfigAider.MultiplyEnemyRarities(LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies, WaitForDungeonsToRegister.CreateDungeonConfig.InteriorEnemyMultiplierD[DungeonManager.CurrentExtendedDungeonFlow]);
                            LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies = ConfigAider.MultiplyEnemyRarities(LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies, WaitForDungeonsToRegister.CreateDungeonConfig.DayEnemyMultiplierD[DungeonManager.CurrentExtendedDungeonFlow]);
                            LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies = ConfigAider.MultiplyEnemyRarities(LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies, WaitForDungeonsToRegister.CreateDungeonConfig.NightEnemyMultiplierD[DungeonManager.CurrentExtendedDungeonFlow]);
                        }
                        else if (ConfigAider.CauterizeString(pair) == "replace")
                        {
                            LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies = ConfigAider.ReplaceEnemies(LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies, WaitForDungeonsToRegister.CreateDungeonConfig.InteriorEnemyReplacementD[DungeonManager.CurrentExtendedDungeonFlow]);
                            LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies = ConfigAider.ReplaceEnemies(LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies, WaitForDungeonsToRegister.CreateDungeonConfig.DayEnemyReplacementD[DungeonManager.CurrentExtendedDungeonFlow]);
                            LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies = ConfigAider.ReplaceEnemies(LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies, WaitForDungeonsToRegister.CreateDungeonConfig.NightEnemyReplacementD[DungeonManager.CurrentExtendedDungeonFlow]);
                        }
                        else
                        {
                            CentralConfig.instance.mls.LogInfo($"Order of Operation: {pair} cannot be understood");
                        }
                    }
                }
            }

            if (CentralConfig.SyncConfig.DoScrapInjectionsByDungeon)
            {
                if (WaitForDungeonsToRegister.CreateDungeonConfig.ScrapD.ContainsKey(DungeonManager.CurrentExtendedDungeonFlow))
                {
                    if (WaitForDungeonsToRegister.CreateDungeonConfig.ScrapD[DungeonManager.CurrentExtendedDungeonFlow].Count > 0)
                    {
                        LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap = LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap.Concat(WaitForDungeonsToRegister.CreateDungeonConfig.ScrapD[DungeonManager.CurrentExtendedDungeonFlow]).ToList();
                        LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap = ConfigAider.RemoveLowerRarityDuplicateItems(LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap);
                    }
                }
            }

            if (CentralConfig.SyncConfig.DoEnemyInjectionsByDungeon || CentralConfig.SyncConfig.DoScrapInjectionsByDungeon)
            {
                CentralConfig.instance.mls.LogInfo("Dungeon Enemy/Scrap Injections Enacted.");
            }

            if (CentralConfig.SyncConfig.RemoveDuplicateEnemies)
            {
                LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies = ConfigAider.RemoveDuplicateEnemies(LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies);
                LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies = ConfigAider.RemoveDuplicateEnemies(LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies);
                LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies = ConfigAider.RemoveDuplicateEnemies(LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies);
                CentralConfig.instance.mls.LogInfo("Duplicate Enemies Removed.");
            }
            LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap = ConfigAider.RemoveLowerRarityDuplicateItems(LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap);

            if (MiscConfig.CreateMiscConfig.RemoveZeros != null)
                if (MiscConfig.CreateMiscConfig.RemoveZeros)
                {
                    LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies = ConfigAider.RemoveZeroRarityEnemies(LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies);
                    LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies = ConfigAider.RemoveZeroRarityEnemies(LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies);
                    LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies = ConfigAider.RemoveZeroRarityEnemies(LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies);
                    LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap = ConfigAider.RemoveZeroRarityItems(LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap);
                }
            if (CentralConfig.SyncConfig.ScrapShuffle)
            {
                ShuffleSaver.scraprandom = new System.Random(StartOfRound.Instance.randomMapSeed);
                LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap = ConfigAider.IncreaseScrapRarities(LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap);
            }
            if (MiscConfig.CreateMiscConfig.ShuffleFirst != null)
                if (CentralConfig.SyncConfig.EnemyShuffle && !MiscConfig.CreateMiscConfig.ShuffleFirst)
                {
                    ShuffleSaver.enemyrandom = new System.Random(StartOfRound.Instance.randomMapSeed);
                    LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies = ConfigAider.IncreaseEnemyRarities(LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies);
                    LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies = ConfigAider.IncreaseEnemyRarities(LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies);
                    LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies = ConfigAider.IncreaseEnemyRarities(LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies);
                }

            if (WRCompatibility.enabled)
                WRCompatibility.RemoveWRScrapMultiplierHardSet();
            RoundManager.Instance.scrapValueMultiplier = ShareScrapValue.Instance.CalculateScrapValueMultiplier();

            if (CentralConfig.SyncConfig.LogEnemies && NetworkManager.Singleton.IsHost)
            {
                ConfigAider.Instance.StartCoroutine(LogEnemyTables());
            }
        }
        static IEnumerator LogEnemyTables()
        {
            yield return new WaitForSeconds(10);

            EnemyTables = "";
            ScrapSpawned = "Scrap Spawned:";
            List<SpawnableEnemyWithRarity> InteriorEnemies = LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies;
            EnemyTables += "\nInterior Enemy List for current game:";
            foreach (SpawnableEnemyWithRarity enemy in InteriorEnemies)
            {
                EnemyTables += $"\n{enemy.enemyType.enemyName},{enemy.rarity}";
            }

            List<SpawnableEnemyWithRarity> DaytimeEnemies = LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies;
            EnemyTables += "\n\nDaytime Enemy List for current game:";
            foreach (SpawnableEnemyWithRarity enemy in DaytimeEnemies)
            {
                EnemyTables += $"\n{enemy.enemyType.enemyName},{enemy.rarity}";
            }

            List<SpawnableEnemyWithRarity> NightEnemies = LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies;
            EnemyTables += "\n\nNighttime Enemy List for current game:";
            foreach (SpawnableEnemyWithRarity enemy in NightEnemies)
            {
                EnemyTables += $"\n{enemy.enemyType.enemyName},{enemy.rarity}";
            }
            EnemyTables += "\n";

            CentralConfig.instance.mls.LogInfo(EnemyTables);

            List<SpawnableItemWithRarity> Scrap = LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap;
            foreach (SpawnableItemWithRarity scrap in Scrap)
            {
                ScrapSpawned += $"\n{scrap.spawnableItem.itemName},{scrap.rarity}";
            }

            CentralConfig.instance.mls.LogInfo(ScrapSpawned);

            /*List<EnemyType> AllEnemies = ConfigAider.GrabFullEnemyList();
            var sortedEnemiesList = AllEnemies.OrderBy(type => type.enemyName).ToList();
            foreach (EnemyType type in sortedEnemiesList)
            {
                AllEnemiesTable += $"\n{type.enemyName}";
                PostableEnemies += $"{type.enemyName}:1,";
            }
             CentralConfig.instance.mls.LogInfo(AllEnemiesTable);

            List<Item> AllItems = ConfigAider.GrabFullItemList();
            var sortedAllItemsList = AllItems.OrderBy(item => item.itemName).ToList();
            foreach (Item item in sortedAllItemsList)
            {
                AllItemsTable += $"\n{item.itemName}";
                if (item.isScrap && item.minValue > 0)
                {
                    PostableScrap += $"{item.itemName}:1,";
                }
            }
            PostableScrap += $"\nWhats on levels\n";
            foreach (ExtendedLevel level in PatchedContent.ExtendedLevels.Where(level => level.NumberlessPlanetName != "Gordion" && level.NumberlessPlanetName != "Liquidation"))
            {
                foreach (SpawnableItemWithRarity item in level.SelectableLevel.spawnableScrap)
                {
                    if (ItemAppearance.ContainsKey(item.spawnableItem))
                    {
                        ItemAppearance[item.spawnableItem] += 1;
                    }
                    else
                    {
                        ItemAppearance[item.spawnableItem] = 1;
                    }

                    if (ScrapListLength.ContainsKey(level))
                    {
                        ScrapListLength[level]++;
                    }
                    else
                    {
                        ScrapListLength[level] = 1;
                    }
                }
                totalScrapTotal += ScrapListLength[level];
                Moons += 1;
            }
            // CentralConfig.instance.mls.LogInfo($"TotalTotalScrap,{totalScrapTotal} Moons,{Moons} Math,{totalScrapTotal/Moons}");
            foreach (Item item in ConfigAider.GrabFullItemList())
            {
                if (ItemAppearance.ContainsKey(item))
                {
                    if (ItemAppearance[item] > 4)
                    {
                        ScrapInLevels += $"{item.itemName}:1,";
                    }
                    else
                    {
                        ScrapInFewLevels += $"{item.itemName}:1,";
                    }
                }
                else
                {
                    ScrapNotInLevels += $"{item.itemName}:1,";
                }
            }*/
            // CentralConfig.instance.mls.LogInfo(AllItemsTable);

            // CentralConfig.instance.mls.LogInfo(PostableEnemies);
            // CentralConfig.instance.mls.LogInfo(PostableScrap);
            // CentralConfig.instance.mls.LogInfo(ScrapInLevels);
            // CentralConfig.instance.mls.LogInfo(ScrapNotInLevels);
            // CentralConfig.instance.mls.LogInfo(ScrapInFewLevels);
        }
    }
    [HarmonyPatch(typeof(LungProp), "Start")]
    public static class IncreaseLungValue
    {
        static void Prefix(LungProp __instance)
        {
            ShareScrapValue.Instance.DetermineMultiplier((CurrentMultiplier) =>
            {
                if (!FMCompatibility.enabled)
                    __instance.scrapValue = Mathf.RoundToInt(80 * CurrentMultiplier * 2.5f);
                else if (FMCompatibility.enabled && WRCompatibility.enabled)
                    __instance.scrapValue = Mathf.RoundToInt(__instance.scrapValue * CurrentMultiplier * 2.5f / WRCompatibility.GetWRWeatherMultiplier());
                else if (FMCompatibility.enabled && !WRCompatibility.enabled)
                    __instance.scrapValue = Mathf.RoundToInt(__instance.scrapValue * CurrentMultiplier * 2.5f);
                ScanNodeProperties LungScanNode = __instance.gameObject.GetComponentInChildren<ScanNodeProperties>();
                LungScanNode.subText = $"Value: ${__instance.scrapValue}";
                LungScanNode.scrapValue = __instance.scrapValue;
            });
        }
    }
    [HarmonyPatch(typeof(RedLocustBees), "Start"), HarmonyPriority(Priority.Last)]
    public static class IncreaseHiveValue
    {
        public static int Counter = 0;
        public static Dictionary<RedLocustBees, int> BeeCount = new Dictionary<RedLocustBees, int>();
        static void Postfix(RedLocustBees __instance)
        {
            if (!BeeCount.ContainsKey(__instance))
                BeeCount.Add(__instance, Counter);
            __instance.StartCoroutine(WaitForHive(__instance));
            Counter++;
        }

        private static IEnumerator WaitForHive(RedLocustBees instance)
        {
            while (instance.hive == null)
            {
                yield return null;
            }
            yield return new WaitForSeconds(BeeCount[instance]);

            ShareScrapValue.Instance.DetermineMultiplier((CurrentMultiplier) =>
            {
                // CentralConfig.instance.mls.LogInfo($"Applying CurrentMultiplier: {CurrentMultiplier} to hive");
                instance.hive.scrapValue = Mathf.RoundToInt(instance.hive.scrapValue * CurrentMultiplier * 2.5f);
                ScanNodeProperties HiveScanNode = instance.hive.gameObject.GetComponentInChildren<ScanNodeProperties>();
                HiveScanNode.subText = $"Value: ${instance.hive.scrapValue}";
                HiveScanNode.scrapValue = instance.hive.scrapValue;
            });
            if (instance.hive.isInFactory)
                instance.hive.itemProperties.isConductiveMetal = false;
        }
    }
}
