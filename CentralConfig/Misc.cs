using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using GameNetcodeStuff;
using HarmonyLib;
using LethalLevelLoader;
using LethalLevelLoader.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using UnityEngine;

namespace CentralConfig
{
    [HarmonyPatch(typeof(HangarShipDoor), "Start")]
    public class MiscConfig
    {
        public static CreateMiscConfig Config;

        [DataContract]
        public class CreateMiscConfig : ConfigTemplate
        {
            [DataMember] public static SyncedEntry<float> FineAmount { get; private set; }
            [DataMember] public static SyncedEntry<float> InsuranceReduction { get; private set; }
            [DataMember] public static SyncedEntry<float> CompanyFineAmount { get; private set; }
            [DataMember] public static SyncedEntry<float> CompanyInsuranceReduction { get; private set; }
            [DataMember] public static SyncedEntry<bool> ProportionalFine { get; private set; }
            [DataMember] public static SyncedEntry<int> ShipMinScan { get; private set; }
            [DataMember] public static SyncedEntry<int> ShipMaxScan { get; private set; }
            [DataMember] public static SyncedEntry<int> MEMinScan { get; private set; }
            [DataMember] public static SyncedEntry<int> MEMaxScan { get; private set; }
            [DataMember] public static SyncedEntry<int> FEMinScan { get; private set; }
            [DataMember] public static SyncedEntry<int> FEMaxScan { get; private set; }
            [DataMember] public static SyncedEntry<string> BoostString { get; private set; }
            [DataMember] public static SyncedEntry<int> BoostCreditThreshold { get; private set; }
            [DataMember] public static SyncedEntry<int> SSVBPThreshold { get; private set; }
            [DataMember] public static SyncedEntry<float> SSVBPPercentIncrease { get; private set; }
            [DataMember] public static SyncedEntry<float> SSVBPMinIncrease { get; private set; }
            [DataMember] public static SyncedEntry<float> SSVBPMaxIncrease { get; private set; }
            [DataMember] public static SyncedEntry<float> SSVBPIncreaseChange { get; private set; }
            [DataMember] public static SyncedEntry<int> SDSBPThreshold { get; private set; }
            [DataMember] public static SyncedEntry<float> SDSBPPercentIncrease { get; private set; }
            [DataMember] public static SyncedEntry<float> SDSBPMinIncrease { get; private set; }
            [DataMember] public static SyncedEntry<float> SDSBPMaxIncrease { get; private set; }
            [DataMember] public static SyncedEntry<float> SDSBPIncreaseChange { get; private set; }
            [DataMember] public static SyncedEntry<int> ScrapShuffleRandomMin { get; private set; }
            [DataMember] public static SyncedEntry<int> ScrapShuffleRandomMax { get; private set; }
            [DataMember] public static SyncedEntry<bool> ScrapShufflerPercent { get; private set; }
            [DataMember] public static SyncedEntry<string> ScrapShuffleBlacklist { get; private set; }
            [DataMember] public static SyncedEntry<int> EnemyShuffleRandomMin { get; private set; }
            [DataMember] public static SyncedEntry<int> EnemyShuffleRandomMax { get; private set; }
            [DataMember] public static SyncedEntry<bool> EnemyShufflerPercent { get; private set; }
            [DataMember] public static SyncedEntry<string> EnemyShuffleBlacklist { get; private set; }
            [DataMember] public static SyncedEntry<int> DungeonShuffleRandomMin { get; private set; }
            [DataMember] public static SyncedEntry<int> DungeonShuffleRandomMax { get; private set; }
            [DataMember] public static SyncedEntry<bool> DungeonShufflerPercent { get; private set; }
            [DataMember] public static SyncedEntry<bool> ShuffleSave { get; private set; }
            [DataMember] public static SyncedEntry<bool> ShuffleFirst { get; private set; }
            [DataMember] public static SyncedEntry<bool> RemoveZeros { get; private set; }
            [DataMember] public static SyncedEntry<bool> RolloverNegatives { get; private set; }
            public CreateMiscConfig(ConfigFile cfg) : base(cfg, "CentralConfig", 0)
            {
                if (CentralConfig.SyncConfig.DoFineOverrides)
                {
                    FineAmount = cfg.BindSyncedEntry(">Fines<",
                        "Penalty for each fallen player",
                        20f,
                        "This is the percentage of current credits that will be deducted for each fallen player.");

                    InsuranceReduction = cfg.BindSyncedEntry(">Fines<",
                        "Penalty reduction for player retrieval",
                        40f,
                        "This value determines the reduction in penalty if a fallen player's body is retrived. For instance, setting this to 0 means no penalty will be applied for having successfully retrived players, 50 means half the penalty is applied upon retrival, and 100 means the penalty remains the same regardless of retrival. It can be any value between 0 and 100.");

                    CompanyFineAmount = cfg.BindSyncedEntry(">Fines<",
                        "Penalty for each fallen player (on Gordion)",
                        0f,
                        "This is the percentage of current credits that will be deducted for each fallen player on Gordion.");

                    CompanyInsuranceReduction = cfg.BindSyncedEntry(">Fines<",
                        "Penalty reduction for player retrieval (on Gordion)",
                        0f,
                        "This value determines the reduction in penalty on Gordion if a fallen player's body is retrived. For instance, setting this to 0 means no penalty will be applied for having successfully retrived players, 50 means half the penalty is applied upon retrival, and 100 means the penalty remains the same regardless of retrival. It can be any value between 0 and 100.");

                    ProportionalFine = cfg.BindSyncedEntry(">Fines<",
                        "Proportional Fine",
                        false,
                        "Should the fine to proportional to the ratio of dead players over total players?\nFor instance, if the penatly if 50% and half of the lobby dies, the fine will be 25% as opposed to being 50% for each dead player.");

                    BoostString = cfg.BindSyncedEntry(">Fines<",
                        "Rank Rewards",
                        "0,5,10,15,20,25",
                        "Credit reward for the previous match's Letter Grade. The values here respectively assign as the rewards for F, D, C, B, A, S.\nThis setting will not function if non-whole numbers or letters are input. There must be exactly six values.");

                    BoostCreditThreshold = cfg.BindSyncedEntry(">Fines<",
                        "Boost Credit Threshold",
                        120,
                        "If the current credits are below this value, they will be directly added to the credit count (e.g., 60 + 25 = 85).\nIf the current credits are above or equal to this value, the added credits will be a percentage of the current credits (e.g., 120 * 1.25 = 150).\nSet this value very high if you want the credit boost to only be linear.");
                }

                if (CentralConfig.SyncConfig.DoScanNodeOverrides)
                {
                    ShipMinScan = cfg.BindSyncedEntry(">ScanNodes<",
                        "Min Ship Scan Distance",
                        17,
                        "You must be this far away to scan the ship (Vanilla is 17).");

                    ShipMaxScan = cfg.BindSyncedEntry(">ScanNodes<",
                        "Max Ship Scan Distance",
                        2147483647,
                        "Max distance you can scan the ship from (Vanilla is 110).\nReduce if you encounter lag.");

                    MEMinScan = cfg.BindSyncedEntry(">ScanNodes<",
                        "Min Main Entrance Scan Distance",
                        17,
                        "You must be this far away to scan the main entrance (Vanilla is 17).");

                    MEMaxScan = cfg.BindSyncedEntry(">ScanNodes<",
                        "Max Main Entrance Scan Distance",
                        2147483647,
                        "Max distance you can scan the main entrance from (Vanilla is 100).\nReduce if you encounter lag.");

                    FEMinScan = cfg.BindSyncedEntry(">ScanNodes<",
                        "Min Fire Exit Scan Distance",
                        12,
                        "You must be this far away to scan fire exits (Default is 12).\nYou must have ScannableFireExit in order for this to work.");

                    FEMaxScan = cfg.BindSyncedEntry(">ScanNodes<",
                        "Max Fire Exit Scan Distance",
                        2147483647,
                        "Max distance you can scan fire exits from (Default is 52).\nYou must have ScannableFireExit in order for this to work.\nReduce if you encounter lag.");
                }

                if (CentralConfig.SyncConfig.ScaleScrapValueByPlayers)
                {
                    SSVBPThreshold = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Scrap Value PlayerCount Threshold",
                        3,
                        "Defines the 'standard' lobby size. If there are fewer players than this, scrap will be worth more. If there are more players, scrap will be worth less.");

                    SSVBPPercentIncrease = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Scrap Value Percent",
                        10f,
                        "Percentage factor to apply to the scrap value multiplier based on the difference between the actual player count and the threshold above.\nThis adjustment is exponential/logarithmic, so applying a 10% factor twice results in approximately 82.6% (i.e., (1.1)^-1 = 0.909, and (1.1)^-2 = 0.826).");

                    SSVBPIncreaseChange = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Scrap Value Percent Change",
                        5f,
                        "The absolute difference between the PlayerCount and Threshold is multiplied by this value before being added to the Scale Scrap Value Percent.\nExample: If this setting is 5, setting above is 10% and there is 1 player the lobby with a threshold of 3, it will increment by 44% instead of 21%.\nExample 2: If this setting is 10, setting above is 25% and there is 6 player the lobby with a threshold of 2, it will decrease by 86.5% instead of 41%");

                    SSVBPMinIncrease = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Scrap Value Min",
                        0.5f,
                        "The Scrap Value Adjustment for player count will never multiply the global scrap value by less than this value.");

                    SSVBPMaxIncrease = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Scrap Value Max",
                        1.5f,
                        "The Scrap Value Adjustment for player count will never multiply the global scrap value by more than this value.");
                }
                if (CentralConfig.SyncConfig.ScaleDungeonSizeByPlayers)
                {
                    SDSBPThreshold = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Dungeon Size PlayerCount Threshold",
                        4,
                        "Defines the 'standard' lobby size. If there are fewer players than this, the dungeon will be smaller. If there are more players, the dungeon will be larger.");

                    SDSBPPercentIncrease = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Dungeon Size Percent",
                        5f,
                        "Percentage factor to apply to the dungeon size multiplier based on the difference between the actual player count and the threshold above.\nThis adjustment is exponential/logarithmic, so applying a 20% factor twice results in approximately 144% (i.e., (1.2)^1 = 1.20, and (1.2)^2 = 1.44).");

                    SDSBPIncreaseChange = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Dungeon Size Percent Change",
                        2.5f,
                        "The absolute difference between the PlayerCount and Threshold is multiplied by this value before being added to the Scale Dungeon Size Percent.\nExample: If this setting is 5, setting above is 20% and there is 1 player the lobby with a threshold of 3, it will increment by 69% instead of 44%.\nExample 2: If this setting is 7, setting above is 8% and there is 6 player the lobby with a threshold of 2, it will decrease by 70.8% instead of 26.5%");

                    SDSBPMinIncrease = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Dungeon Size Min",
                        0.5f,
                        "The Dungeon Size Adjustment for player count will never multiply the dungeon size by less than this value.");

                    SDSBPMaxIncrease = cfg.BindSyncedEntry("<Player Count Scaling>",
                        "Scale Dungeon Size Max",
                        1.5f,
                        "The Dungeon Size Adjustment for player count will never multiply the dungeon size by more than this value.");
                }

                if (CentralConfig.SyncConfig.ScrapShuffle)
                {
                    ScrapShuffleRandomMin = cfg.BindSyncedEntry("~Shufflers~",
                        "Scrap Shuffler Random Min",
                        0,
                        "The number of days since the last appearance of this scrap is multiplied by a random value before being applied added to the scrap's rarity in the current scrap pool. If it is (1, 1) then it will increase the scrap's rarity by exactly 1 per day since it last spawned.\nBy default, the scrap's rarity will be increased by 0, 1 or 2 * the number of days since it last spawned.");

                    ScrapShuffleRandomMax = cfg.BindSyncedEntry("~Shufflers~",
                        "Scrap Shuffler Random Max",
                        2,
                        "The number of days since the last appearance of this scrap is multiplied by a random value before being applied added to the scrap's rarity in the current scrap pool. If it is (1, 1) then it will increase the scrap's rarity by exactly 1 per day since it last spawned.\nBy default, the scrap's rarity will be increased by 0, 1 or 2 * the number of days since it last spawned.");

                    ScrapShufflerPercent = cfg.BindSyncedEntry("~Shufflers~",
                        "Scrap Shuffler Percent?",
                        false,
                        "If set to true, the random value from above will be a percent of the rarity times the days since it last appeared.\nFalse: Rarity += DayCount * RandomValue\nTrue: Rarity *= (DayCount * (Randomvalue/100))");

                    ScrapShuffleBlacklist = cfg.BindSyncedEntry("~Shufflers~",
                        "Blacklisted Scrap",
                        "Default Values Are Empty",
                        "Scrap listed here in 'ScrapName,ScrapName' format will be ignored by the shuffle.");
                }
                if (CentralConfig.SyncConfig.EnemyShuffle)
                {
                    EnemyShuffleRandomMin = cfg.BindSyncedEntry("~Shufflers~",
                        "Enemy Shuffler Random Min",
                        0,
                        "The number of days since the last appearance of this enemy is multiplied by a random value before being applied added to the enemy's rarity in all the current enemy pools. If it is (1, 1) then it will increase the enemy's rarity by exactly 1 per day since it last spawned.\nBy default, the enemy's rarity will be increased by 0, 1 or 2 * the number of days since it last spawned.");

                    EnemyShuffleRandomMax = cfg.BindSyncedEntry("~Shufflers~",
                        "Enemy Shuffler Random Max",
                        2,
                        "The number of days since the last appearance of this enemy is multiplied by a random value before being applied added to the enemy's rarity in all the current enemy pools. If it is (1, 1) then it will increase the enemy's rarity by exactly 1 per day since it last spawned.\nBy default, the enemy's rarity will be increased by 0, 1 or 2 * the number of days since it last spawned.");

                    EnemyShufflerPercent = cfg.BindSyncedEntry("~Shufflers~",
                        "Enemy Shuffler Percent?",
                        false,
                        "If set to true, the random value from above will be a percent of the rarity times the days since it last appeared.\nFalse: Rarity += DayCount * RandomValue\nTrue: Rarity *= (DayCount * (Randomvalue/100))");

                    EnemyShuffleBlacklist = cfg.BindSyncedEntry("~Shufflers~",
                        "Blacklisted Enemies",
                        "Default Values Are Empty",
                        "Enemies listed here in 'EnemyName,EnemyName' format will be ignored by the shuffle.");

                    ShuffleFirst = cfg.BindSyncedEntry("~Shufflers~",
                        "Shuffle Enemies First?",
                        false,
                        "If set to true, the shuffler will increase enemy rarities before the add,multiply,replace enemy injections.");
                }
                if (CentralConfig.SyncConfig.DungeonShuffler)
                {
                    DungeonShuffleRandomMin = cfg.BindSyncedEntry("~Shufflers~",
                        "Dungeon Shuffler Random Min",
                        0,
                        "The number of days since the last selection of this interior is multiplied by a random value before being applied added to the dungeon's rarity in all matches. If it is (1, 1) then it will increase the dungeon's rarity by exactly 1 per day since it was last selected.\nBy default, the dungeon's rarity will be increased by 0-20% * the number of days since it last spawned.");

                    DungeonShuffleRandomMax = cfg.BindSyncedEntry("~Shufflers~",
                        "Dungeon Shuffler Random Max",
                        20,
                        "The number of days since the last selection of this interior is multiplied by a random value before being applied added to the dungeon's rarity in all matches. If it is (1, 1) then it will increase the dungeon's rarity by exactly 1 per day since it was last selected.\nBy default, the dungeon's rarity will be increased by 0-20% * the number of days since it last spawned.");

                    DungeonShufflerPercent = cfg.BindSyncedEntry("~Shufflers~",
                        "Dungeon Shuffler Percent?",
                        true,
                        "If set to true, the random value from above will be a percent of the rarity times the days since it last appeared.\nFalse: Rarity += DayCount * RandomValue\nTrue: Rarity *= (DayCount * (Randomvalue/100))");
                }
                if (CentralConfig.SyncConfig.EnemyShuffle || CentralConfig.SyncConfig.ScrapShuffle || CentralConfig.SyncConfig.DungeonShuffler)
                {
                    ShuffleSave = cfg.BindSyncedEntry("~Shufflers~",
                    "Save Shuffle Data",
                    false,
                    "If set to true, the shuffle data for enemies and scrap will be committed to the save file and loaded on start-up. This means that the counters for how many days since they last spawned will be preserved, and the rarity boosters will be applied accordingly on next landing.\nIf this setting remains false, the shuffle will only exist in the session and be forgotten on reboot.");

                    RolloverNegatives = cfg.BindSyncedEntry("~Shufflers~",
                    "Rollover Zero or Less",
                    false,
                    "If set to true, entries with a rarity of 0 or less will always increase linearly instead of by percent.\nThis setting has a very specific use, enemies or scrap with a negative rarity will gradually increase until they become positive, effectively creating a minimum number of days between their appearances on that level.");
                }
                if (CentralConfig.SyncConfig.EnemyShuffle || CentralConfig.SyncConfig.ScrapShuffle)
                {
                    RemoveZeros = cfg.BindSyncedEntry("~Shufflers~",
                    "Remove Zero Rarity Scrap and Enemies?",
                    true,
                    "If set to true, the shuffler will remove all scrap/enemies with a rarity of 0 before boosting rarities. This ensures they won’t gain days when they may have a rarity of 0 to prevent them from spawning on specific moons, during certain weather conditions, or in particular dungeons.");
                }
            }
        }
    }
    [HarmonyPatch(typeof(HUDManager), "ApplyPenalty")]
    public class ChangeFineAmount
    {
        static float Grungus;
        static float Shmunguss;
        static float totalPlayers = GameNetworkManager.Instance.connectedPlayers + 1;
        static string Recovered;
        static string Bodies;
        static bool Prefix(HUDManager __instance, int playersDead, int bodiesInsured)
        {
            if (!CentralConfig.SyncConfig.DoFineOverrides)
            {
                return true;
            }
            if (LevelManager.CurrentExtendedLevel.NumberlessPlanetName == "Gordion")
            {
                Grungus = MiscConfig.CreateMiscConfig.CompanyFineAmount / 100f;
                Shmunguss = 1 / (MiscConfig.CreateMiscConfig.CompanyInsuranceReduction / 100f);
            }
            else
            {
                Grungus = MiscConfig.CreateMiscConfig.FineAmount / 100f;
                Shmunguss = 1 / (MiscConfig.CreateMiscConfig.InsuranceReduction / 100f);
            }
            Terminal terminal = Object.FindFirstObjectByType<Terminal>();
            int groupCredits = terminal.groupCredits;
            int AdjustedbodiesInsured = Mathf.Max(bodiesInsured, 0);
            for (int i = 0; i < playersDead - AdjustedbodiesInsured; i++)
            {
                if (MiscConfig.CreateMiscConfig.ProportionalFine)
                {
                    terminal.groupCredits -= (int)(groupCredits * Grungus * (playersDead / totalPlayers));
                }
                else
                {
                    terminal.groupCredits -= (int)(groupCredits * Grungus);
                }
            }
            for (int j = 0; j < AdjustedbodiesInsured; j++)
            {
                if (MiscConfig.CreateMiscConfig.ProportionalFine)
                {
                    terminal.groupCredits -= (int)(groupCredits * (Grungus / Shmunguss) * (playersDead / totalPlayers));
                }
                else
                {
                    terminal.groupCredits -= (int)(groupCredits * (Grungus / Shmunguss));
                }
            }
            if (terminal.groupCredits < 0)
            {
                terminal.groupCredits = 0;
            }
            int totalFinePercentage;
            if (MiscConfig.CreateMiscConfig.ProportionalFine)
            {
                int uninsuredBodies = playersDead - AdjustedbodiesInsured;
                float uninsuredProportion = uninsuredBodies / totalPlayers;
                float insuredProportion = AdjustedbodiesInsured / totalPlayers;

                totalFinePercentage = (int)Mathf.Round(Grungus * 100f * (uninsuredProportion + (insuredProportion / Shmunguss)));
            }
            else
            {
                totalFinePercentage = (int)Mathf.Round(((Grungus * (playersDead - AdjustedbodiesInsured)) + ((Grungus / Shmunguss) * AdjustedbodiesInsured)) * 100f);
            }
            if (playersDead > 1)
            {
                Bodies = "There were " + playersDead + " casualties, ";
            }
            else
            {
                Bodies = "There was " + playersDead + " casualty, ";
            }
            if (AdjustedbodiesInsured > 0)
            {
                Recovered = "but " + AdjustedbodiesInsured + " bodies were recovered.";
            }
            else
            {
                Recovered = "and " + AdjustedbodiesInsured + " bodies were recovered.";
            }
            if (totalFinePercentage == 0)
                __instance.statsUIElements.penaltyAddition.text = Bodies + Recovered + "\nNo fine will be extracted from the crew.";
            else if (totalFinePercentage < 0)
                __instance.statsUIElements.penaltyAddition.text = Bodies + Recovered + "\nThe crew shall be rewarded for their valiant sacrifice.";
            else
                __instance.statsUIElements.penaltyAddition.text = Bodies + Recovered + "\nA fine of " + totalFinePercentage + "% will be extracted from the crew.";
            if (groupCredits - terminal.groupCredits < 0)
                __instance.statsUIElements.penaltyTotal.text = $"PAID: ${Mathf.Abs(groupCredits - terminal.groupCredits)}";
            else
                __instance.statsUIElements.penaltyTotal.text = $"DUE: ${groupCredits - terminal.groupCredits}";
            return false;
        }
    }
    [HarmonyPatch(typeof(HUDManager), "FillEndGameStats")]
    public static class RemoveAllPlayersDeadMessage
    {
        static void Prefix(HUDManager __instance)
        {
            bool isScrapKeeperActive = CheckForScrapKeepers();

            Color overlayColor = __instance.statsUIElements.allPlayersDeadOverlay.color;
            overlayColor.a = isScrapKeeperActive ? 1f : 0f;
            __instance.statsUIElements.allPlayersDeadOverlay.color = overlayColor;
        }
        static bool CheckForScrapKeepers()
        {
            if (LUCompatibility.enabled)
                if (LUCompatibility.ReturnLUScrapKeeper())
                    return false;
            if (KSCompatibility.enabled)
                return false;
            if (NDDCompatibility.enabled)
                return false;
            if (BobKSCompatibility.enabled)
                return false;
            if (KirSSIODCompatibility.enabled)
                return false;
            if (NPLCompatibility.enabled)
                return false;
            return true;
        }
        static void Postfix(HUDManager __instance)
        {
            if (!CentralConfig.SyncConfig.DoFineOverrides)
                return;

            Terminal terminal = Object.FindFirstObjectByType<Terminal>();

            string[] boostArray = MiscConfig.CreateMiscConfig.BoostString.Value.Split(',');
            if (boostArray.Length != 6)
            {
                CentralConfig.instance.mls.LogError("Credit boosts string must contain exactly 6 values.");
                return;
            }

            List<int> boostList = new List<int>();
            foreach (string boost in boostArray)
            {
                if (int.TryParse(boost, out int boostValue))
                {
                    boostList.Add(boostValue);
                }
                else
                {
                    CentralConfig.instance.mls.LogError($"Invalid credit boost value: {boost}");
                    return;
                }
            }

            if (terminal.groupCredits < MiscConfig.CreateMiscConfig.BoostCreditThreshold)
            {
                switch (__instance.statsUIElements.gradeLetter.text)
                {
                    case "F":
                        terminal.groupCredits += boostList[0];
                        break;
                    case "D":
                        terminal.groupCredits += boostList[1];
                        break;
                    case "C":
                        terminal.groupCredits += boostList[2];
                        break;
                    case "B":
                        terminal.groupCredits += boostList[3];
                        break;
                    case "A":
                        terminal.groupCredits += boostList[4];
                        break;
                    case "S":
                        terminal.groupCredits += boostList[5];
                        break;
                    default:
                        CentralConfig.instance.mls.LogInfo("Letter Grade wasn't Between S and F.");
                        break;
                }
            }
            else
            {
                switch (__instance.statsUIElements.gradeLetter.text)
                {
                    case "F":
                        terminal.groupCredits = (int)(terminal.groupCredits * (1f + (boostList[0] / 100f)));
                        break;
                    case "D":
                        terminal.groupCredits = (int)(terminal.groupCredits * (1f + (boostList[1] / 100f)));
                        break;
                    case "C":
                        terminal.groupCredits = (int)(terminal.groupCredits * (1f + (boostList[2] / 100f)));
                        break;
                    case "B":
                        terminal.groupCredits = (int)(terminal.groupCredits * (1f + (boostList[3] / 100f)));
                        break;
                    case "A":
                        terminal.groupCredits = (int)(terminal.groupCredits * (1f + (boostList[4] / 100f)));
                        break;
                    case "S":
                        terminal.groupCredits = (int)(terminal.groupCredits * (1f + (boostList[5] / 100f)));
                        break;
                    default:
                        CentralConfig.instance.mls.LogInfo("Letter Grade wasn't Between S and F.");
                        break;
                }
            }

            if (terminal.groupCredits <= 0)
                terminal.groupCredits = 0;

            __instance.statsUIElements.quotaNumerator.text = StartOfRound.Instance.GetValueOfAllScrap(onlyScrapCollected: true, onlyNewScrap: true).ToString();
        }
    }
    [HarmonyPatch(typeof(TimeOfDay), "MoveGlobalTime")]
    public static class IncreaseNodeDistanceOnFE
    {
        public static Dictionary<char, ScanNodeProperties> fireExitScanProperties = new Dictionary<char, ScanNodeProperties>();
        public static bool IsDone = false;
        static void Postfix()
        {
            if (IsDone || !CentralConfig.SyncConfig.DoScanNodeOverrides)
            {
                return;
            }

            List<EntranceTeleport> entranceTeleports = Object.FindObjectsByType<EntranceTeleport>(FindObjectsSortMode.None).ToList();
            int numFireExits = (entranceTeleports.Count / 2) - 1;
            // CentralConfig.instance.mls.LogInfo("Doing it lol");

            char[] letters = { 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            if (numFireExits <= letters.Length)
            {
                letters = letters.Take(numFireExits).ToArray();
            }
            else
            {
                CentralConfig.instance.mls.LogInfo("There are more than 25 fire exits aaaaa (This shouldn't happen so def report plz).");
            }
            char[] dictLetters = letters.Except(fireExitScanProperties.Keys).ToArray();
            if (dictLetters.Length > 0)
            {
                foreach (char letter in dictLetters)
                {
                    string nodeName = "Environment/Teleports/EntranceTeleport" + letter + "/ScanNode";
                    GameObject FireExitScanNode = GameObject.Find(nodeName);
                    if (FireExitScanNode != null)
                    {
                        ScanNodeProperties FireExitScanProperties = FireExitScanNode.GetComponent<ScanNodeProperties>();
                        if (!fireExitScanProperties.ContainsKey(letter))
                        {
                            fireExitScanProperties[letter] = FireExitScanProperties;
                        }
                        FireExitScanProperties.minRange = MiscConfig.CreateMiscConfig.FEMinScan;
                        FireExitScanProperties.maxRange = MiscConfig.CreateMiscConfig.FEMaxScan;
                        FireExitScanProperties.requiresLineOfSight = false;
                        CentralConfig.instance.mls.LogInfo("Updated Fire Exit: " + letter);
                    }
                }
            }
            else
            {
                IsDone = true;
                CentralConfig.instance.mls.LogInfo("All Fire Exits Set Up.");
            }
        }
    }
    [HarmonyPatch(typeof(RoundManager), "GenerateNewFloor")]
    public static class IncreaseNodeDistanceOnShipAndMain
    {
        static void Postfix()
        {
            if (!CentralConfig.SyncConfig.DoScanNodeOverrides)
            {
                return;
            }

            IncreaseNodeDistanceOnFE.fireExitScanProperties.Clear();
            IncreaseNodeDistanceOnFE.IsDone = false;

            List<string> subTexts = new List<string> { "Remember the quota…", "ALERT", "Go away…", "Dare to enter?", "Get that scrap!", "???", "Face the fear", "No one has left alive…", "Turn back…", "Why?", "Take heed…", "Watch your back…", "You should be afraid…" };
            System.Random rand = new System.Random(StartOfRound.Instance.randomMapSeed + 42);
            string randomSubText = subTexts[rand.Next(subTexts.Count)];

            ScanNodeProperties[] allScanNodes = Object.FindObjectsByType<ScanNodeProperties>(FindObjectsSortMode.None);
            foreach (ScanNodeProperties scanNode in allScanNodes)
            {
                if (scanNode.headerText.ToLower() == "main entrance" || scanNode.headerText.ToLower() == "mainentrance")
                {
                    scanNode.minRange = MiscConfig.CreateMiscConfig.MEMinScan;
                    scanNode.maxRange = MiscConfig.CreateMiscConfig.MEMaxScan;
                    scanNode.requiresLineOfSight = false;
                    if (scanNode.subText == "")
                    {
                        scanNode.subText = randomSubText;
                    }
                }
                else if (scanNode.headerText.ToLower() == "ship")
                {
                    scanNode.minRange = MiscConfig.CreateMiscConfig.ShipMinScan;
                    scanNode.maxRange = MiscConfig.CreateMiscConfig.ShipMaxScan;
                    scanNode.requiresLineOfSight = false;
                    if (scanNode.subText == "")
                    {
                        scanNode.subText = "Home base";
                    }
                }
            }
        }
    }
    [HarmonyPatch(typeof(HUDManager), "AssignNewNodes")]
    public static class ExtendScan
    {
        public static float UltimateMax;
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if (CentralConfig.SyncConfig.DoScanNodeOverrides)
            {
                UltimateMax = Mathf.Max(MiscConfig.CreateMiscConfig.ShipMaxScan, Mathf.Max(MiscConfig.CreateMiscConfig.MEMaxScan, MiscConfig.CreateMiscConfig.FEMaxScan));
            }
            else
            {
                UltimateMax = 80f;
            }
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 80f)
                {
                    yield return new CodeInstruction(OpCodes.Ldc_R4, UltimateMax);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
    [HarmonyPatch(typeof(EntranceTeleport), "TeleportPlayerServerRpc")]
    public static class UpdateScanNodes
    {
        public static string WhereAreThey;
        public static int EntranceScanMax;
        public static int ShipScanMax;
        public static int FEScanMax;
        static void Postfix(EntranceTeleport __instance, int playerObj)
        {
            if (!__instance.IsHost || !CentralConfig.SyncConfig.DoScanNodeOverrides)
            {
                return;
            }

            __instance.StartCoroutine(PerformActionsWithDelay(__instance, playerObj));
        }
        static IEnumerator PerformActionsWithDelay(EntranceTeleport __instance, int playerObj)
        {
            yield return new WaitForSeconds(3);

            StartOfRound startOfRound = Object.FindFirstObjectByType<StartOfRound>();
            PlayerControllerB player = startOfRound.allPlayerScripts[playerObj];

            PlayerScanNodeProperties scanNodeProperties = PlayerScanNodeProperties.playerScanNodeProperties[player];

            ScanNodeProperties[] allScanNodes = Object.FindObjectsByType<ScanNodeProperties>(FindObjectsSortMode.None);

            if (player.isInsideFactory)
            {
                WhereAreThey = "Inside";
            }
            else
            {
                WhereAreThey = "Outside";
            }

            foreach (ScanNodeProperties scanNode in allScanNodes)
            {
                if (scanNode.headerText.ToLower() == "main entrance" || scanNode.headerText.ToLower() == "mainentrance" || scanNode.headerText.ToLower().Contains("entrance"))
                {
                    scanNode.maxRange = player.isInsideFactory ? scanNodeProperties.MEMinScan : scanNodeProperties.MEMaxScan;
                    EntranceScanMax = scanNode.maxRange;
                }
                else if (scanNode.headerText.ToLower() == "ship" || scanNode.headerText.ToLower().Contains("ship"))
                {
                    scanNode.maxRange = player.isInsideFactory ? scanNodeProperties.ShipMinScan : scanNodeProperties.ShipMaxScan;
                    ShipScanMax = scanNode.maxRange;
                }
                else if (scanNode.headerText.ToLower() == "fire exit" || scanNode.headerText.ToLower().Contains("fire exit"))
                {
                    scanNode.maxRange = player.isInsideFactory ? scanNodeProperties.FEMinScan : scanNodeProperties.FEMaxScan;
                    FEScanMax = scanNode.maxRange;
                }
            }
            CentralConfig.instance.mls.LogInfo("Player: " + player.playerUsername + " is now " + WhereAreThey + ". Max Scan Ranges set to: " + EntranceScanMax + ", " + ShipScanMax + " and " + FEScanMax);
        }
        public class PlayerScanNodeProperties
        {
            public int MEMinScan = MiscConfig.CreateMiscConfig.MEMinScan;
            public int MEMaxScan = MiscConfig.CreateMiscConfig.MEMaxScan;
            public int ShipMinScan = MiscConfig.CreateMiscConfig.ShipMinScan;
            public int ShipMaxScan = MiscConfig.CreateMiscConfig.ShipMaxScan;
            public int FEMinScan = MiscConfig.CreateMiscConfig.FEMinScan;
            public int FEMaxScan = MiscConfig.CreateMiscConfig.FEMaxScan;

            public static Dictionary<PlayerControllerB, PlayerScanNodeProperties> playerScanNodeProperties = new Dictionary<PlayerControllerB, PlayerScanNodeProperties>();
        }
    }
    [HarmonyPatch(typeof(PlayerControllerB), "Start")]
    public static class AddPlayerToDict
    {
        static void Postfix(PlayerControllerB __instance)
        {
            if (!CentralConfig.SyncConfig.DoScanNodeOverrides)
            {
                return;
            }
            UpdateScanNodes.PlayerScanNodeProperties scanNodeProperties = new UpdateScanNodes.PlayerScanNodeProperties
            {
                MEMinScan = MiscConfig.CreateMiscConfig.MEMinScan,
                MEMaxScan = MiscConfig.CreateMiscConfig.MEMaxScan,
                ShipMinScan = MiscConfig.CreateMiscConfig.ShipMinScan,
                ShipMaxScan = MiscConfig.CreateMiscConfig.ShipMaxScan,
                FEMinScan = MiscConfig.CreateMiscConfig.FEMinScan,
                FEMaxScan = MiscConfig.CreateMiscConfig.FEMaxScan

            };

            UpdateScanNodes.PlayerScanNodeProperties.playerScanNodeProperties[__instance] = scanNodeProperties;
        }
    }
}