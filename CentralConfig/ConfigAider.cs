using BepInEx.Configuration;
using HarmonyLib;
using LethalLevelLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace CentralConfig
{
    public class ConfigAider : MonoBehaviour
    {
        private static ConfigAider _instance;
        public static ConfigAider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("ConfigAider").AddComponent<ConfigAider>();
                }
                return _instance;
            }
        }

        public static Color MagentaColor = Color.magenta;
        public static string CauterizeString(string inputString)
        {
            string cleanedString = new string(inputString.Where(c => char.IsLetterOrDigit(c)).ToArray());
            return cleanedString.ToLower();
        }
        public static string LightlyToastString(string inputString)
        {
            string cleanedString = new string(inputString.Where(c => c != ':' && c != '-' && c != ',' && c != '~' && c != '\'').ToArray());
            return cleanedString;
        }

        public static List<Item> GrabFullItemList()
        {
            if (PintoBoyCompat.enabled)
                PintoBoyCompat.RenamePintoBois();

            return StartOfRound.Instance.allItemsList.itemsList;
        }

        public static List<EnemyType> allEnemies;
        public static List<EnemyType> GrabFullEnemyList()
        {
            if (allEnemies == null)
            {
                allEnemies = new List<EnemyType>();

                foreach (EnemyType enemy in OriginalContent.Enemies)
                    if (!allEnemies.Contains(enemy))
                    {
                        allEnemies.Add(enemy);
                        // CentralConfig.instance.mls.LogMessage($"Added enemy: {enemy.enemyName} from OriginalContent");
                    }
                foreach (ExtendedEnemyType extendedEnemy in PatchedContent.ExtendedEnemyTypes)
                    if (!allEnemies.Contains(extendedEnemy.EnemyType))
                    {
                        allEnemies.Add(extendedEnemy.EnemyType);
                        // CentralConfig.instance.mls.LogMessage($"Added enemy: {extendedEnemy.EnemyType.enemyName} from PatchedContent");
                    }
                foreach (LethalLib.Modules.Enemies.SpawnableEnemy spawnableEnemy in LethalLib.Modules.Enemies.spawnableEnemies)
                    if (!allEnemies.Contains(spawnableEnemy.enemy))
                    {
                        allEnemies.Add(spawnableEnemy.enemy);
                        // CentralConfig.instance.mls.LogMessage($"Added enemy: {spawnableEnemy.enemy.enemyName} from LethalLib");
                    }
                if (LBCompatability.enabled)
                    allEnemies = LBCompatability.AddLBEnemies(allEnemies);
                if (DiversityCompat.enabled)
                    allEnemies = DiversityCompat.AddWalker(allEnemies);
                if (FootBallCompat.enabled)
                    allEnemies = FootBallCompat.AddFootball(allEnemies);
                if (RollingGiantCompat.enabled)
                    allEnemies = RollingGiantCompat.AddRollingGiant(allEnemies);
                if (PeeperCompat.enabled)
                    allEnemies = PeeperCompat.AddPeeper(allEnemies);
                /*foreach (EnemyType enemy in Resources.FindObjectsOfTypeAll<EnemyType>())
                {
                    if (!allEnemies.Contains(enemy))
                    {
                        allEnemies.Add(enemy);
                    }
                }*/
            }
            return allEnemies;
        }
        public List<ContentTag> allContentTagsList;
        public List<ContentTag> GrabFullTagList()
        {
            if (allContentTagsList == null)
            {
                allContentTagsList = new List<ContentTag>();

                foreach (ExtendedLevel level in PatchedContent.ExtendedLevels)
                {
                    // CentralConfig.instance.mls.LogInfo($"Extended Level: {level.NumberlessPlanetName}");
                    foreach (ContentTag tag in level.ContentTags)
                    {
                        // CentralConfig.instance.mls.LogInfo($"Content Tag: {tag.contentTagName}");
                        if (!allContentTagsList.Contains(tag))
                        {
                            allContentTagsList.Add(tag);
                        }
                    }
                }
                List<string> newTags = SplitStringsByDaComma(CentralConfig.SyncConfig.NewTags);
                foreach (string tag in newTags)
                {
                    if (!allContentTagsList.Any(ct => ct.contentTagName == tag))
                    {
                        ContentTag newTag = ContentTag.Create(tag, MagentaColor);
                        allContentTagsList.Add(newTag);
                    }
                }
            }
            return allContentTagsList;
        }

        public static List<string> SplitStringsByDaComma(string newInputString)
        {
            return newInputString.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        /*public static List<StringWithRarity> ConvertStringToStringWithRarityList(string newInputString, Vector2 clampRarity)
        {
            List<StringWithRarity> returnList = new List<StringWithRarity>();

            var pairs = newInputString.Split(',');

            foreach (string pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var String = parts[0].Trim();
                    int Rarity = int.Parse(parts[1].Trim());

                    if (clampRarity != Vector2.zero)
                        Rarity = Mathf.Clamp(Rarity, Mathf.RoundToInt(clampRarity.x), Mathf.RoundToInt(clampRarity.y));

                    returnList.Add(new StringWithRarity(String, Rarity));
                }
            }

            return returnList;
        }*/

        // Enemies

        public static string ConvertEnemyListToString(List<SpawnableEnemyWithRarity> spawnableEnemiesList)
        {
            string returnString = string.Empty;

            var sortedEnemiesList = spawnableEnemiesList.OrderBy(spawnableEnemyWithRarity => spawnableEnemyWithRarity.enemyType.enemyName).ToList();

            foreach (SpawnableEnemyWithRarity spawnableEnemyWithRarity in sortedEnemiesList)
            {
                if (RollingGiantCompat.enabled)
                {
                    returnString += RollingGiantCompat.RGSEWRTS(spawnableEnemyWithRarity);
                }
                else
                {
                    if (spawnableEnemyWithRarity.enemyType.enemyName != "Lasso" && spawnableEnemyWithRarity.rarity > 0)
                    {
                        returnString += LightlyToastString(spawnableEnemyWithRarity.enemyType.enemyName) + ":" + spawnableEnemyWithRarity.rarity.ToString() + ",";
                    }
                }
            }

            if (returnString.Contains(",") && returnString.LastIndexOf(",") == (returnString.Length - 1))
                returnString = returnString.Remove(returnString.LastIndexOf(","), 1);

            if (returnString == string.Empty)
                returnString = "Default Values Were Empty";

            return returnString;
        }
        public static List<SpawnableEnemyWithRarity> ConvertStringToEnemyList(string newInputString, Vector2 clampRarity)
        {
            if (string.IsNullOrEmpty(newInputString) || newInputString == "Default Values Were Empty")
            {
                return new List<SpawnableEnemyWithRarity>();
            }
            List<SpawnableEnemyWithRarity> returnList = new List<SpawnableEnemyWithRarity>();

            List<EnemyType> AllEnemies = GrabFullEnemyList();

            var pairs = newInputString.Split(',');

            foreach (string pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var EnemyName = parts[0].Trim();
                    EnemyName = CauterizeString(EnemyName);
                    int Rarity;
                    bool added = false;

                    if (!int.TryParse(parts[1].Trim(), out Rarity))
                    {
                        CentralConfig.instance.mls.LogInfo($"Cannot Parse Rarity: {parts[1].Trim()} after EnemyName entry {EnemyName}");
                        Rarity = 0;
                    }

                    if (clampRarity != Vector2.zero)
                        Rarity = Mathf.Clamp(Rarity, Mathf.RoundToInt(clampRarity.x), Mathf.RoundToInt(clampRarity.y));

                    foreach (EnemyType enemyType in AllEnemies)
                    {
                        string enemyname = CauterizeString(enemyType.enemyName);
                        if (enemyname == EnemyName)
                        {
                            SpawnableEnemyWithRarity newEnemy = new SpawnableEnemyWithRarity(enemyType, Rarity);
                            returnList.Add(newEnemy);
                            added = true;
                            // CentralConfig.instance.mls.LogMessage($"Added enemy {enemyType.enemyName} with rarity {Rarity} from string {EnemyName}");
                            break;
                        }
                        else
                        {
                            // CentralConfig.instance.mls.LogMessage($"Did not add enemy {enemyType.enemyName} from string {EnemyName}");
                        }
                    }
                    if (!added)
                    {
                        foreach (EnemyType enemyType in AllEnemies)
                        {
                            if (enemyType.enemyPrefab != null)
                            {
                                ScanNodeProperties enemyScanNode = enemyType.enemyPrefab.GetComponentInChildren<ScanNodeProperties>();
                                if (enemyScanNode != null)
                                {
                                    string headerText = CauterizeString(enemyScanNode.headerText);
                                    if (headerText == EnemyName)
                                    {
                                        SpawnableEnemyWithRarity newEnemy = new SpawnableEnemyWithRarity(enemyType, Rarity);
                                        returnList.Add(newEnemy);
                                        added = true;
                                        // CentralConfig.instance.mls.LogMessage($"Added enemy {enemyType.enemyName} with rarity {Rarity} from scan node name {EnemyName}");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            /*StringBuilder sb = new StringBuilder();
            sb.AppendLine("Return List:");
            foreach (SpawnableEnemyWithRarity enemyType in returnList)
            {
                sb.AppendLine($"Item Name: {enemyType.enemyType.enemyName}, Rarity: {enemyType.rarity}");
            }
            CentralConfig.instance.mls.LogInfo(sb.ToString());*/

            return returnList;
        }
        public static List<SpawnableEnemyWithRarity> RemoveDuplicateEnemies(List<SpawnableEnemyWithRarity> enemies)
        {
            var result = new List<SpawnableEnemyWithRarity>();

            if (CentralConfig.SyncConfig.AlwaysKeepZeros)
            {
                var groupedEnemies = enemies.GroupBy(e => e.enemyType.enemyName);
                foreach (var group in groupedEnemies)
                {
                    if (group.Any(e => e.rarity == 0))
                    {
                        result.Add(group.First(e => e.rarity <= 0));
                    }
                    else
                    {
                        result.AddRange(group);
                    }
                }
            }
            else
            {
                var groupedEnemies = enemies.GroupBy(e => e.enemyType.enemyName);
                foreach (var group in groupedEnemies)
                {
                    result.AddRange(group);
                }
            }

            if (!CentralConfig.SyncConfig.KeepSmallerDupes)
            {
                return result.GroupBy(e => e.enemyType.enemyName).Select(g => g.OrderByDescending(e => e.rarity).First()).ToList();
            }
            else
            {
                return result.GroupBy(e => e.enemyType.enemyName).Select(g => g.OrderByDescending(e => e.rarity).Last()).ToList();
            }
        }
        public static List<SpawnableEnemyWithRarity> IncreaseEnemyRarities(List<SpawnableEnemyWithRarity> enemies)
        {
            List<SpawnableEnemyWithRarity> returnList = new List<SpawnableEnemyWithRarity>();
            List<string> ignoreListEntries = SplitStringsByDaComma(MiscConfig.CreateMiscConfig.EnemyShuffleBlacklist.Value).Select(entry => CauterizeString(entry)).ToList();
            List<SpawnableEnemyWithRarity> WhiteListEnemies = enemies.Where(e => !ignoreListEntries.Any(b => CauterizeString(e.enemyType.enemyName).Equals(b))).ToList();
            List<SpawnableEnemyWithRarity> BlackListEnemies = enemies.Where(e => ignoreListEntries.Any(b => CauterizeString(e.enemyType.enemyName).Equals(b))).ToList();
            foreach (SpawnableEnemyWithRarity enemy in BlackListEnemies)
            {
                returnList.Add(enemy);
            }
            foreach (SpawnableEnemyWithRarity enemy in WhiteListEnemies)
            {
                if (!EnemyShuffler.EnemyAppearances.ContainsKey(enemy.enemyType))
                {
                    if (ShuffleSaver.EnemyAppearanceString.ContainsKey(enemy.enemyType.enemyName))
                    {
                        EnemyShuffler.EnemyAppearances.Add(enemy.enemyType, ShuffleSaver.EnemyAppearanceString[enemy.enemyType.enemyName]);
                        CentralConfig.instance.mls.LogInfo($"Remembered saved Enemy Key: {enemy.enemyType.enemyName}, Days: {EnemyShuffler.EnemyAppearances[enemy.enemyType]}");
                    }
                    else
                    {
                        EnemyShuffler.EnemyAppearances.Add(enemy.enemyType, 0);
                        ShuffleSaver.EnemyAppearanceString.Add(enemy.enemyType.enemyName, 0);
                        // CentralConfig.instance.mls.LogInfo($"Added new Enemy Key: {enemy.enemyType.enemyName}");
                    }
                }
                if (!ShuffleSaver.EnemyAppearanceString.ContainsKey(enemy.enemyType.enemyName))
                {
                    ShuffleSaver.EnemyAppearanceString.Add(enemy.enemyType.enemyName, EnemyShuffler.EnemyAppearances[enemy.enemyType]);
                }

                int LastAppearance = EnemyShuffler.EnemyAppearances[enemy.enemyType];
                int multiplier = ShuffleSaver.enemyrandom.Next(MiscConfig.CreateMiscConfig.EnemyShuffleRandomMin, MiscConfig.CreateMiscConfig.EnemyShuffleRandomMax + 1);

                if (LastAppearance == 0)
                {
                    SpawnableEnemyWithRarity newEnemy = new SpawnableEnemyWithRarity(enemy.enemyType, enemy.rarity);
                    returnList.Add(newEnemy);
                }
                else
                {
                    SpawnableEnemyWithRarity newEnemy = new SpawnableEnemyWithRarity(enemy.enemyType, enemy.rarity);
                    if (MiscConfig.CreateMiscConfig.EnemyShufflerPercent && !(enemy.rarity < 0 && MiscConfig.CreateMiscConfig.RolloverNegatives))
                    {
                        newEnemy.rarity = (int)Math.Round((LastAppearance * (enemy.rarity * (multiplier / 100f))) + enemy.rarity);
                        newEnemy.rarity = Mathf.Clamp(newEnemy.rarity, 0, 99999);
                    }
                    else
                    {
                        if (enemy.rarity < 0 && MiscConfig.CreateMiscConfig.RolloverNegatives)
                        {
                            enemy.rarity = LastAppearance;
                        }
                        newEnemy.rarity = enemy.rarity + (LastAppearance * multiplier);
                        newEnemy.rarity = Mathf.Clamp(newEnemy.rarity, -99999, 99999);
                    }
                    returnList.Add(newEnemy);
                }
            }
            /*foreach (SpawnableEnemyWithRarity Old in enemies)
            {
                foreach (SpawnableEnemyWithRarity New in returnList)
                {
                    if (New.enemyType == Old.enemyType)
                    {
                        if (New.rarity != Old.rarity)
                        {
                            CentralConfig.instance.mls.LogInfo($"Enemy: {Old.enemyType.enemyName} rarity increased from {Old.rarity} to {New.rarity}");
                        }
                    }
                }
            }*/
            return returnList;
        }
        public static List<SpawnableEnemyWithRarity> ReplaceEnemies(List<SpawnableEnemyWithRarity> EnemyList, string ReplaceConfig)
        {
            if (string.IsNullOrEmpty(ReplaceConfig) || ReplaceConfig == "Default Values Were Empty")
            {
                return EnemyList; // if the config is unused just returns the list untouched
            }

            List<SpawnableEnemyWithRarity> returnList = new List<SpawnableEnemyWithRarity>(); // makes a new list of enemies
            List<string> replacedEnemies = new List<string>(); // To remember what was replaced
            List<SpawnableEnemyWithRarity> backupList = new List<SpawnableEnemyWithRarity>(EnemyList); // clone the list to keep a backup
            var random = new System.Random(StartOfRound.Instance.randomMapSeed);

            var pairs = ReplaceConfig.Split(','); // breaks the string by comma to get each argument of x enemy replacing y

            foreach (var pair in pairs) // foreach part between the commas
            {
                string originalName;
                string replacementName;
                int rarityReplacement = -1;
                int chanceToReplace = 100;
                string SuccessReplacementLogMessage;

                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var original = parts[0].Trim(); // first enemy name is the original
                    var replacement = parts[1].Trim(); // second enemy name is the replacement

                    var OptRarity = original.Split('-'); // breaks the first bit into the name and rarity 
                    if (OptRarity.Length == 2)
                    {
                        originalName = OptRarity[0].Trim();
                        int.TryParse(OptRarity[1].Trim(), out rarityReplacement);
                    }
                    else
                    {
                        originalName = original; // just leaves the name as the original if it doesn't have a rarity attached
                    }

                    var ReplaceChance = replacement.Split('~'); // breaks the second bit into the replacement name and replacement chance
                    if (ReplaceChance.Length == 2)
                    {
                        replacementName = ReplaceChance[0].Trim();
                        int.TryParse(ReplaceChance[1].Trim(), out chanceToReplace);
                    }
                    else
                    {
                        replacementName = replacement; // just leaves the replacement name as the replacement if it doesn't have a chance attached
                    }

                    bool replaced = false;

                    EnemyList = EnemyList.OrderBy(e => e.enemyType.enemyName).ToList();
                    for (int i = 0; i < EnemyList.Count; i++)
                    {
                        var enemy = EnemyList[i];
                        string cauterizedEnemy = CauterizeString(enemy.enemyType.enemyName);
                        string cauterizedOGName = CauterizeString(originalName);
                        if (cauterizedEnemy == cauterizedOGName) // if the entry matches an original name
                        {
                            if (random.Next(100) < chanceToReplace) // the enemy is only replaced if the chance to replace is greater than the random synced value from 0 to 99
                            {
                                EnemyType replacementEnemyType = GetEnemyTypeByName(replacementName);
                                int newRarity = (rarityReplacement != -1) ? rarityReplacement : enemy.rarity;
                                SpawnableEnemyWithRarity newEnemy = new SpawnableEnemyWithRarity(replacementEnemyType, newRarity);

                                SuccessReplacementLogMessage = $"Replaced enemy: {originalName} with {replacementName}";
                                if (rarityReplacement != -1)
                                {
                                    SuccessReplacementLogMessage += $", using rarity override of {rarityReplacement}.";
                                }
                                {
                                    newEnemy.rarity = enemy.rarity; // uses the same rarity
                                    SuccessReplacementLogMessage += $", using the original enemy rarity of {enemy.rarity}.";
                                }

                                returnList.Add(newEnemy); // adds this new enemy to the returnList
                                replacedEnemies.Add(CauterizeString(originalName)); // adds the replacement to the string list I made
                                SuccessReplacementLogMessage += $"\nChance to replace was: {chanceToReplace}%";
                                // CentralConfig.instance.mls.LogInfo(SuccessReplacementLogMessage);

                                replaced = true; // indicate that a replacement was made
                                EnemyList.Remove(enemy);
                                break; // break out of the inner loop to stop checking further enemies
                            }
                            else
                            {
                                // CentralConfig.instance.mls.LogInfo($"Didn't replace enemy: {originalName} with {replacementName}, chance was only {chanceToReplace}%");
                            }
                        }
                        else
                        {
                            // CentralConfig.instance.mls.LogInfo($"Enemy: {originalName} was not found in the enemy list.");
                        }
                    }

                    if (replaced)
                    {
                        continue; // move on to the next pair if a replacement was made
                    }
                }
            }

            backupList = backupList.OrderBy(e => e.enemyType.enemyName).ToList();
            foreach (var enemy in backupList) // goes through the enemy list again
            {
                if (!replacedEnemies.Contains(CauterizeString(enemy.enemyType.enemyName))) // if it wasn't replaced by another enemy
                {
                    SpawnableEnemyWithRarity oldEnemy = new SpawnableEnemyWithRarity(enemy.enemyType, enemy.rarity);
                    returnList.Add(oldEnemy); // yeah its added to the returnList untouched
                    // CentralConfig.instance.mls.LogInfo("Added non replaced enemy: " + oldEnemy.enemyType.enemyName);
                }
            }
            return returnList; // and gets the list back
        }
        public static EnemyType GetEnemyTypeByName(string enemyName)
        {
            foreach (var enemyType in GrabFullEnemyList())
            {
                if (CauterizeString(enemyType.enemyName) == CauterizeString(enemyName))
                {
                    return enemyType;
                }
            }
            return null;
        }
        public static List<SpawnableEnemyWithRarity> MultiplyEnemyRarities(List<SpawnableEnemyWithRarity> enemies, string ConfigString)
        {
            if (string.IsNullOrEmpty(ConfigString) || ConfigString == "Default Values Were Empty")
            {
                return enemies; // if the config is unused just returns the list untouched
            }

            List<SpawnableEnemyWithRarity> returnList = new List<SpawnableEnemyWithRarity>(); // makes a new list of enemies
            List<string> handledEnemies = new List<string>(); // To remember what was already handled
            List<SpawnableEnemyWithRarity> backupList = new List<SpawnableEnemyWithRarity>(enemies); // clone the list to keep a backup

            var pairs = ConfigString.Split(','); // breaks the string by comma to get each argument of x enemy replacing y

            foreach (var pair in pairs) // foreach part between the commas
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var EnemyName = parts[0].Trim();
                    float multiplier;

                    if (!float.TryParse(parts[1].Trim(), out multiplier))
                    {
                        CentralConfig.instance.mls.LogInfo($"Cannot Parse Multiplier: {parts[1].Trim()} after EnemyName entry {EnemyName}");
                        multiplier = 1f;
                    }

                    bool handled = false;

                    enemies = enemies.OrderBy(e => e.enemyType.enemyName).ToList();
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        var enemy = enemies[i];
                        string cauterizedEnemy = CauterizeString(enemy.enemyType.enemyName);
                        string cauterizedOGName = CauterizeString(EnemyName);
                        if (cauterizedEnemy == cauterizedOGName) // if the entry matches an original name
                        {
                            SpawnableEnemyWithRarity newEnemy = new SpawnableEnemyWithRarity(GetEnemyTypeByName(EnemyName), (int)(enemy.rarity * multiplier));
                            returnList.Add(newEnemy); // adds this new enemy to the returnList
                            handledEnemies.Add(CauterizeString(EnemyName));
                            handled = true;

                            enemies.Remove(enemy);
                            // CentralConfig.instance.mls.LogInfo($"Enemy: {EnemyName} was multiplied by {multiplier}x for a new rarity of {(int)(enemy.rarity * multiplier)} and added to the returnlist");
                            break;
                        }
                    }

                    if (handled)
                    {
                        continue; // move on to the next pair if a replacement was made
                    }
                }
            }
            backupList = backupList.OrderBy(e => e.enemyType.enemyName).ToList();
            foreach (var enemy in backupList) // goes through the enemy list again
            {
                if (!handledEnemies.Contains(CauterizeString(enemy.enemyType.enemyName))) // if it wasn't handled already
                {
                    SpawnableEnemyWithRarity oldEnemy = new SpawnableEnemyWithRarity(enemy.enemyType, enemy.rarity);
                    returnList.Add(oldEnemy);
                    // CentralConfig.instance.mls.LogInfo($"Enemy: {enemy.enemyType.enemyName} was added to the returnlist with no multiplier");
                }
            }
            return returnList; // and gets the list back
        }
        public static string GetBigList(int Type)
        {
            string returnString = string.Empty;

            List<ExtendedLevel> allExtendedLevels = PatchedContent.ExtendedLevels;

            var sortedExtendedLevels = allExtendedLevels.OrderBy(level => level.NumberlessPlanetName).ToList();

            foreach (ExtendedLevel level in sortedExtendedLevels)
            {
                if (level.NumberlessPlanetName != "Gordion" && level.NumberlessPlanetName != "Liquidation")
                {
                    if (Type == 0)
                    {
                        string ThisLevelEnemies = ConvertEnemyListToString(level.SelectableLevel.Enemies);

                        returnString += LightlyToastString(level.NumberlessPlanetName) + "-" + ThisLevelEnemies + "~";
                    }
                    else if (Type == 1)
                    {
                        string ThisLevelEnemies = ConvertEnemyListToString(level.SelectableLevel.DaytimeEnemies);

                        returnString += LightlyToastString(level.NumberlessPlanetName) + "-" + ThisLevelEnemies + "~";
                    }
                    else if (Type == 2)
                    {
                        string ThisLevelEnemies = ConvertEnemyListToString(level.SelectableLevel.OutsideEnemies);

                        returnString += LightlyToastString(level.NumberlessPlanetName) + "-" + ThisLevelEnemies + "~";
                    }
                }
            }
            if (returnString.Contains("~") && returnString.LastIndexOf("~") == (returnString.Length - 1))
                returnString = returnString.Remove(returnString.LastIndexOf("~"), 1);

            if (returnString == string.Empty)
                returnString = "Default Values Were Empty";

            return returnString;
        }
        public static void SetBigList(int Type, string BigString)
        {
            List<ExtendedLevel> allExtendedLevels = PatchedContent.ExtendedLevels;

            var sortedExtendedLevels = allExtendedLevels.OrderBy(level => level.NumberlessPlanetName).ToList();

            foreach (ExtendedLevel level in sortedExtendedLevels)
            {
                if (level.NumberlessPlanetName != "Gordion" && level.NumberlessPlanetName != "Liquidation")
                {
                    string levelName = LightlyToastString(level.NumberlessPlanetName) + "-";
                    int startIndex = BigString.IndexOf(levelName);

                    if (startIndex != -1)
                    {
                        int endIndex = BigString.IndexOf("~", startIndex);
                        if (endIndex == -1)
                        {
                            endIndex = BigString.Length;
                        }

                        string ClippedString = BigString.Substring(startIndex + levelName.Length, endIndex - startIndex - levelName.Length);

                        // CentralConfig.instance.mls.LogInfo(level.NumberlessPlanetName + " returns a type " + Type + " list of: " + ClippedString);

                        if (Type == 0 && ClippedString != "Default Values Were Empty" && ClippedString != "")
                        {
                            Vector2 clamprarity = new Vector2(-99999, 99999);
                            List<SpawnableEnemyWithRarity> EnemyList = ConvertStringToEnemyList(ClippedString, clamprarity);
                            if (EnemyList.Count > 0)
                            {
                                level.SelectableLevel.Enemies = EnemyList;
                            }
                        }
                        else if (Type == 1 && ClippedString != "Default Values Were Empty" && ClippedString != "")
                        {
                            Vector2 clamprarity = new Vector2(-99999, 99999);
                            List<SpawnableEnemyWithRarity> EnemyList = ConvertStringToEnemyList(ClippedString, clamprarity);
                            if (EnemyList.Count > 0)
                            {
                                level.SelectableLevel.DaytimeEnemies = EnemyList;
                            }
                        }
                        else if (Type == 2 && ClippedString != "Default Values Were Empty" && ClippedString != "")
                        {
                            Vector2 clamprarity = new Vector2(-99999, 99999);
                            List<SpawnableEnemyWithRarity> EnemyList = ConvertStringToEnemyList(ClippedString, clamprarity);
                            if (EnemyList.Count > 0)
                            {
                                level.SelectableLevel.OutsideEnemies = EnemyList;
                            }
                        }
                    }
                    else
                    {
                        // CentralConfig.instance.mls.LogInfo("BigString does not contain: " + level.NumberlessPlanetName);
                    }
                }
                // CentralConfig.instance.mls.LogInfo(level.SelectableLevel.levelID + " is linked to " + level.NumberlessPlanetName);
            }
        }
        public static List<SpawnableEnemyWithRarity> RemoveZeroRarityEnemies(List<SpawnableEnemyWithRarity> enemylist)
        {
            List<SpawnableEnemyWithRarity> returnList = new List<SpawnableEnemyWithRarity>();

            foreach (SpawnableEnemyWithRarity enemy in enemylist)
            {
                if (enemy.rarity != 0)
                {
                    returnList.Add(enemy);
                }
            }
            return returnList;
        }

        // Scrap

        public static string ConvertItemListToString(List<SpawnableItemWithRarity> spawnableItemsList)
        {
            string returnString = string.Empty;

            var sortedItemsList = spawnableItemsList.OrderBy(spawnableItemWithRarity => spawnableItemWithRarity.spawnableItem.itemName).ToList();

            foreach (SpawnableItemWithRarity spawnableItemWithRarity in sortedItemsList)
            {
                if (PintoBoyCompat.enabled)
                {
                    returnString += PintoBoyCompat.PBSIWRTS(spawnableItemWithRarity);
                }
                else
                {
                    if (spawnableItemWithRarity.rarity > 0)
                        returnString += LightlyToastString(spawnableItemWithRarity.spawnableItem.itemName) + ":" + spawnableItemWithRarity.rarity.ToString() + ",";
                }
            }
            if (returnString.Contains(",") && returnString.LastIndexOf(",") == (returnString.Length - 1))
                returnString = returnString.Remove(returnString.LastIndexOf(","), 1);

            if (returnString == string.Empty)
                returnString = "Default Values Were Empty";
            return returnString;
        }
        public static List<SpawnableItemWithRarity> ConvertStringToItemList(string newInputString, Vector2 clampRarity)
        {
            if (string.IsNullOrEmpty(newInputString) || newInputString == "Default Values Were Empty")
            {
                return new List<SpawnableItemWithRarity>();
            }
            List<SpawnableItemWithRarity> returnList = new List<SpawnableItemWithRarity>();

            List<Item> allItems = GrabFullItemList();

            var pairs = newInputString.Split(',');

            foreach (string pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var ItemName = parts[0].Trim();
                    ItemName = CauterizeString(ItemName);
                    int Rarity;

                    if (!int.TryParse(parts[1].Trim(), out Rarity))
                    {
                        CentralConfig.instance.mls.LogInfo($"Cannot Parse Rarity: {parts[1].Trim()} after ItemName entry {ItemName}");
                        Rarity = 0;
                    }

                    if (clampRarity != Vector2.zero)
                        Rarity = Mathf.Clamp(Rarity, Mathf.RoundToInt(clampRarity.x), Mathf.RoundToInt(clampRarity.y));

                    foreach (Item item in allItems)
                    {
                        string cauterizedItemName = CauterizeString(item.itemName);

                        if (cauterizedItemName == ItemName)
                        {
                            SpawnableItemWithRarity newItem = new SpawnableItemWithRarity(item, Rarity);
                            returnList.Add(newItem);
                            // CentralConfig.instance.mls.LogMessage($"Added scrap {item.itemName} with rarity {Rarity} from string {ItemName}");
                            break;
                        }
                        else
                        {
                            // CentralConfig.instance.mls.LogMessage($"Did not add scrap {item.itemName} from string {ItemName}");
                        }
                    }
                }
            }
            /*StringBuilder sb = new StringBuilder();
            sb.AppendLine("Return List:");
            foreach (SpawnableItemWithRarity item in returnList)
            {
                sb.AppendLine($"Item Name: {item.spawnableItem.itemName}, Rarity: {item.rarity}");
            }
            CentralConfig.instance.mls.LogInfo(sb.ToString());*/

            return returnList;
        }
        public static List<SpawnableItemWithRarity> RemoveLowerRarityDuplicateItems(List<SpawnableItemWithRarity> items)
        {
            return items.GroupBy(e => e.spawnableItem.itemName).Select(g => g.OrderByDescending(e => e.rarity).First()).ToList();
        }
        public static List<SpawnableItemWithRarity> IncreaseScrapRarities(List<SpawnableItemWithRarity> items)
        {
            List<SpawnableItemWithRarity> returnList = new List<SpawnableItemWithRarity>();
            List<string> ignoreListEntries = SplitStringsByDaComma(MiscConfig.CreateMiscConfig.ScrapShuffleBlacklist.Value).Select(entry => CauterizeString(entry)).ToList();
            List<SpawnableItemWithRarity> WhiteListItems = items.Where(i => !ignoreListEntries.Any(b => CauterizeString(i.spawnableItem.itemName).Equals(b))).ToList();
            List<SpawnableItemWithRarity> BlackListItems = items.Where(i => ignoreListEntries.Any(b => CauterizeString(i.spawnableItem.itemName).Equals(b))).ToList();
            foreach (SpawnableItemWithRarity item in BlackListItems)
            {
                returnList.Add(item);
            }
            foreach (SpawnableItemWithRarity item in WhiteListItems)
            {
                if (!ScrapShuffler.ScrapAppearances.ContainsKey(item.spawnableItem))
                {
                    if (ShuffleSaver.ScrapAppearanceString.ContainsKey(item.spawnableItem.itemName))
                    {
                        ScrapShuffler.ScrapAppearances.Add(item.spawnableItem, ShuffleSaver.ScrapAppearanceString[item.spawnableItem.itemName]);
                        CentralConfig.instance.mls.LogInfo($"Remembered saved Item Key: {item.spawnableItem.itemName}, Days: {ScrapShuffler.ScrapAppearances[item.spawnableItem]}");
                    }
                    else
                    {
                        ScrapShuffler.ScrapAppearances.Add(item.spawnableItem, 0);
                        ShuffleSaver.ScrapAppearanceString.Add(item.spawnableItem.itemName, 0);
                        // CentralConfig.instance.mls.LogInfo($"Added new Item Key: {item.spawnableItem.itemName}");
                    }
                }
                if (!ShuffleSaver.ScrapAppearanceString.ContainsKey(item.spawnableItem.itemName))
                {
                    ShuffleSaver.ScrapAppearanceString.Add(item.spawnableItem.itemName, ScrapShuffler.ScrapAppearances[item.spawnableItem]);
                }

                int LastAppearance = ScrapShuffler.ScrapAppearances[item.spawnableItem];
                int multiplier = ShuffleSaver.scraprandom.Next(MiscConfig.CreateMiscConfig.ScrapShuffleRandomMin, MiscConfig.CreateMiscConfig.ScrapShuffleRandomMax + 1); // not just '+ x' but + 'x * (config min/max)' 

                if (LastAppearance == 0) // appearred last game
                {
                    SpawnableItemWithRarity newItem = new SpawnableItemWithRarity(item.spawnableItem, item.rarity);
                    returnList.Add(newItem);
                }
                else // didn't
                {
                    SpawnableItemWithRarity newItem = new SpawnableItemWithRarity(item.spawnableItem, item.rarity);
                    if (MiscConfig.CreateMiscConfig.ScrapShufflerPercent && !(item.rarity < 0 && MiscConfig.CreateMiscConfig.RolloverNegatives))
                    {
                        newItem.rarity = (int)Math.Round((LastAppearance * (item.rarity * (multiplier / 100f))) + item.rarity);
                        newItem.rarity = Mathf.Clamp(newItem.rarity, 0, 99999);
                    }
                    else
                    {
                        if (item.rarity < 0 && MiscConfig.CreateMiscConfig.RolloverNegatives)
                        {
                            item.rarity = LastAppearance;
                        }
                        newItem.rarity = item.rarity + (LastAppearance * multiplier);
                        newItem.rarity = Mathf.Clamp(newItem.rarity, -99999, 99999);

                    }
                    returnList.Add(newItem);
                }
            }
            /*foreach (SpawnableItemWithRarity Old in items)
            {
                foreach (SpawnableItemWithRarity New in returnList)
                {
                    if (New.spawnableItem == Old.spawnableItem)
                    {
                        if (New.rarity != Old.rarity)
                        {
                            CentralConfig.instance.mls.LogInfo($"Scrap: {Old.spawnableItem.itemName} rarity increased from {Old.rarity} to {New.rarity}");
                        }
                    }
                }
            }*/
            return returnList;
        }
        public static List<SpawnableItemWithRarity> RemoveZeroRarityItems(List<SpawnableItemWithRarity> itemList)
        {
            List<SpawnableItemWithRarity> returnList = new List<SpawnableItemWithRarity>();

            foreach (SpawnableItemWithRarity item in itemList)
            {
                if (item.rarity != 0)
                {
                    returnList.Add(item);
                }
            }
            return returnList;
        }

        // Weather

        /*public static string ConvertWeatherArrayToString(RandomWeatherWithVariables[] randomWeatherArray)
        {
            string returnString = "";
            bool noneExists = false;

            foreach (RandomWeatherWithVariables randomWeather in randomWeatherArray)
            {
                if (randomWeather.weatherType.ToString() == "None")
                {
                    noneExists = true;
                }
                returnString += randomWeather.weatherType + ",";
            }

            if (!noneExists)
            {
                returnString = "None," + returnString;
            }

            if (returnString.Contains(",") && returnString.LastIndexOf(",") == (returnString.Length - 1))
            {
                returnString = returnString.Remove(returnString.LastIndexOf(","), 1);
            }

            return returnString;
        }
        public static RandomWeatherWithVariables[] ConvertStringToWeatherArray(string newInputString)
        {
            if (string.IsNullOrEmpty(newInputString) || newInputString == "Default Values Were Empty")
            {
                return new RandomWeatherWithVariables[] { new RandomWeatherWithVariables { weatherType = LevelWeatherType.None } };
            }

            string[] weatherStrings = newInputString.Split(',');

            RandomWeatherWithVariables[] returnArray = new RandomWeatherWithVariables[weatherStrings.Length];

            for (int i = 0; i < weatherStrings.Length; i++)
            {
                if (Enum.TryParse(weatherStrings[i], out LevelWeatherType weatherType))
                {
                    returnArray[i] = new RandomWeatherWithVariables { weatherType = weatherType };
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Return Array:");
            foreach (RandomWeatherWithVariables weatherType in returnArray)
            {
                sb.AppendLine($"{weatherType.weatherType}");
            }
            // CentralConfig.instance.mls.LogInfo(sb.ToString());
            return returnArray;
        }*/

        // Tags

        public static string ConvertTagsToString(List<ContentTag> contentTags)
        {
            string returnString = string.Empty;
            var sortedTagList = contentTags.OrderBy(contentTag => contentTag.contentTagName).ToList();
            HashSet<string> uniqueTags = new HashSet<string>();

            foreach (ContentTag contentTag in sortedTagList)
            {
                string tagName = CauterizeString(contentTag.contentTagName);
                if (uniqueTags.Add(tagName))
                {
                    returnString += tagName + ",";
                }
            }

            if (returnString.EndsWith(","))
                returnString = returnString.Remove(returnString.LastIndexOf(","), 1);

            if (string.IsNullOrEmpty(returnString))
                returnString = "Default Values Were Empty";

            return returnString;
        }
        public static List<ContentTag> ConvertStringToTagList(string newInputString)
        {
            if (string.IsNullOrEmpty(newInputString) || newInputString == "Default Values Were Empty")
            {
                return new List<ContentTag>();
            }
            List<ContentTag> returnList = new List<ContentTag>();

            List<ContentTag> allContentTagsList = Instance.GrabFullTagList();

            var tags = newInputString.Split(',');

            foreach (string tag in tags)
            {
                string cauterizedConfigTag = CauterizeString(tag);
                foreach (ContentTag contentTag in allContentTagsList)
                {
                    string cauterizedTagName = CauterizeString(contentTag.contentTagName);
                    if (cauterizedTagName == cauterizedConfigTag)
                    {
                        returnList.Add(contentTag);
                        break;
                    }
                }
            }
            /*StringBuilder sb = new StringBuilder();
            sb.AppendLine("Return List:");
            foreach (ContentTag tag in returnList)
            {
                sb.AppendLine($"{tag.contentTagName}");
            }
            CentralConfig.instance.mls.LogInfo(sb.ToString());*/

            return returnList;
        }

        public static string GetMoonsWithTag(ContentTag contentTag)
        {
            int TagNumber = 0;
            string returnString = string.Empty;

            List<ExtendedLevel> allExtendedLevels = PatchedContent.ExtendedLevels;

            var sortedExtendedLevels = allExtendedLevels.OrderBy(level => level.NumberlessPlanetName).ToList();

            foreach (ExtendedLevel level in sortedExtendedLevels)
            {
                string TagsOnMoon = ConvertTagsToString(level.ContentTags);

                if (TagsOnMoon.Contains(CauterizeString(contentTag.contentTagName)))
                {
                    returnString += level.NumberlessPlanetName + ",";
                    TagNumber++;
                }
            }
            if (returnString.EndsWith(","))
                returnString = returnString.Remove(returnString.LastIndexOf(","), 1);

            if (string.IsNullOrEmpty(returnString))
                returnString = "Default Values Were Empty";

            // CentralConfig.instance.mls.LogInfo("Tag:" + CauterizeString(contentTag.contentTagName) + " has " + TagNumber + " moons associated with it.");

            return returnString;
        }

        public static void RemoveDuplicateTags(List<ContentTag> tags)
        {
            List<string> stringlist = new List<string>();

            foreach (ContentTag tag in tags)
            {
                if (!stringlist.Contains(tag.contentTagName))
                {
                    // CentralConfig.instance.mls.LogInfo($"New Tag found: {tag.contentTagName}");
                    stringlist.Add(tag.contentTagName);
                }
                else
                {
                    // CentralConfig.instance.mls.LogInfo($"Repeat Tag found: {tag.contentTagName}");
                    LevelManager.CurrentExtendedLevel.ContentTags.Remove(tag);
                }
            }
        }

        // Misc String

        public static List<StringWithRarity> IncreaseDungeonRarities(List<StringWithRarity> strings, ExtendedDungeonFlow dungeonflow, string flowName)
        {
            List<StringWithRarity> returnList = new List<StringWithRarity>();

            if (!DungeonShuffler.DungeonAppearances.ContainsKey(dungeonflow))
            {
                if (ShuffleSaver.DungeonAppearanceString.ContainsKey(flowName))
                {
                    DungeonShuffler.DungeonAppearances.Add(dungeonflow, ShuffleSaver.DungeonAppearanceString[flowName]);
                    CentralConfig.instance.mls.LogInfo($"Remembered saved Dungeon Key: {flowName}, Days: {DungeonShuffler.DungeonAppearances[dungeonflow]}");
                }
                else
                {
                    DungeonShuffler.DungeonAppearances.Add(dungeonflow, 0);
                    ShuffleSaver.DungeonAppearanceString.Add(flowName, 0);
                    // CentralConfig.instance.mls.LogInfo($"Added new Dungeon Key: {flowName}");
                }
            }
            if (!ShuffleSaver.DungeonAppearanceString.ContainsKey(flowName))
            {
                ShuffleSaver.DungeonAppearanceString.Add(flowName, DungeonShuffler.DungeonAppearances[dungeonflow]);
            }

            int LastAppearance = DungeonShuffler.DungeonAppearances[dungeonflow];
            foreach (StringWithRarity String in strings)
            {
                int multiplier = ShuffleSaver.dungeonrandom.Next(MiscConfig.CreateMiscConfig.DungeonShuffleRandomMin, MiscConfig.CreateMiscConfig.DungeonShuffleRandomMax + 1);
                // CentralConfig.instance.mls.LogInfo($"Dungeon: {flowName} Multiplier: {multiplier} selected between {MiscConfig.CreateMiscConfig.DungeonShuffleRandomMin} and {MiscConfig.CreateMiscConfig.DungeonShuffleRandomMax}");

                if (LastAppearance == 0)
                {
                    StringWithRarity newflow = new StringWithRarity(String.Name, String.Rarity);
                    returnList.Add(newflow);
                }
                else
                {
                    StringWithRarity newflow = new StringWithRarity(String.Name, 0);
                    if (MiscConfig.CreateMiscConfig.DungeonShufflerPercent && !(String.Rarity < 0 && MiscConfig.CreateMiscConfig.RolloverNegatives))
                    {
                        newflow.Rarity = (int)Math.Round((LastAppearance * (String.Rarity * (multiplier / 100f))) + String.Rarity);
                        newflow.Rarity = Mathf.Clamp(newflow.Rarity, 0, 99999);
                    }
                    else
                    {
                        if (String.Rarity < 0 && MiscConfig.CreateMiscConfig.RolloverNegatives)
                        {
                            String.Rarity = LastAppearance;
                        }
                        newflow.Rarity = String.Rarity + (LastAppearance * multiplier);
                        newflow.Rarity = Mathf.Clamp(newflow.Rarity, -99999, 99999);
                    }
                    returnList.Add(newflow);
                }
            }
            /*foreach (StringWithRarity Old in strings)
            {
                foreach (StringWithRarity New in returnList)
                {
                    if (New.Name == Old.Name)
                    {
                        if (New.Rarity != Old.Rarity)
                        {
                            CentralConfig.instance.mls.LogInfo($"Dungeon: {flowName} Match: {Old.Name} rarity increased from {Old.Rarity} to {New.Rarity}");
                        }
                    }
                }
            }*/
            return returnList;
        }
        public static List<Vector2WithRarity> IncreaseDungeonRaritiesVector2(List<Vector2WithRarity> vectors, ExtendedDungeonFlow dungeonflow, string flowName)
        {
            List<Vector2WithRarity> returnList = new List<Vector2WithRarity>();

            if (!DungeonShuffler.DungeonAppearances.ContainsKey(dungeonflow))
            {
                if (ShuffleSaver.DungeonAppearanceString.ContainsKey(flowName))
                {
                    DungeonShuffler.DungeonAppearances.Add(dungeonflow, ShuffleSaver.DungeonAppearanceString[flowName]);
                    CentralConfig.instance.mls.LogInfo($"Remembered saved Dungeon Key: {flowName}, Days: {DungeonShuffler.DungeonAppearances[dungeonflow]}");
                }
                else
                {
                    DungeonShuffler.DungeonAppearances.Add(dungeonflow, 0);
                    ShuffleSaver.DungeonAppearanceString.Add(flowName, 0);
                    // CentralConfig.instance.mls.LogInfo($"Added new Dungeon Key: {flowName}");
                }
            }
            if (!ShuffleSaver.DungeonAppearanceString.ContainsKey(flowName))
            {
                ShuffleSaver.DungeonAppearanceString.Add(flowName, DungeonShuffler.DungeonAppearances[dungeonflow]);
            }

            int LastAppearance = DungeonShuffler.DungeonAppearances[dungeonflow];
            foreach (Vector2WithRarity Vector in vectors)
            {
                int multiplier = ShuffleSaver.dungeonrandom.Next(MiscConfig.CreateMiscConfig.DungeonShuffleRandomMin, MiscConfig.CreateMiscConfig.DungeonShuffleRandomMax + 1);

                if (LastAppearance == 0)
                {
                    Vector2WithRarity newflow = new Vector2WithRarity(Vector.Min, Vector.Max, Vector.Rarity);
                    returnList.Add(newflow);
                }
                else
                {
                    Vector2WithRarity newflow = new Vector2WithRarity(Vector.Min, Vector.Max, 0);
                    if (MiscConfig.CreateMiscConfig.DungeonShufflerPercent && !(Vector.Rarity < 0 && MiscConfig.CreateMiscConfig.RolloverNegatives))
                    {
                        newflow.Rarity = (int)Math.Round((LastAppearance * (Vector.Rarity * (multiplier / 100f))) + Vector.Rarity);
                        newflow.Rarity = Mathf.Clamp(newflow.Rarity, 0, 99999);
                    }
                    else
                    {
                        if (Vector.Rarity < 0 && MiscConfig.CreateMiscConfig.RolloverNegatives)
                        {
                            Vector.Rarity = LastAppearance;
                        }
                        newflow.Rarity = Vector.Rarity + (LastAppearance * multiplier);
                        newflow.Rarity = Mathf.Clamp(newflow.Rarity, -99999, 99999);
                    }
                    returnList.Add(newflow);
                }
            }
            /*foreach (Vector2WithRarity Old in vectors)
            {
                foreach (Vector2WithRarity New in returnList)
                {
                    if (New.Min == Old.Min && New.Max == Old.Max)
                    {
                        if (New.Rarity != Old.Rarity)
                        {
                            CentralConfig.instance.mls.LogInfo($"Dungeon: {flowName} Match: {Old.Min}-{Old.Max} rarity increased from {Old.Rarity} to {New.Rarity}");
                        }
                    }
                }
            }*/
            return returnList;
        }
        public static string ConvertStringWithRarityToString(List<StringWithRarity> names)
        {
            /*StringBuilder sb = new StringBuilder();
            sb.AppendLine("Input List:");
            foreach (StringWithRarity Inputs in names)
            {
                sb.AppendLine($"{Inputs.Name}: {Inputs.Rarity}");
            }
            CentralConfig.instance.mls.LogInfo(sb.ToString());*/
            string returnString = string.Empty;

            string rebalancedmoons = "Assurаncе,Mаrch,Offеnsе,Adаmance,Dіne,Tіtan,Embrіon,Rеnd,Vоw";
            List<string> rmlist = SplitStringsByDaComma(rebalancedmoons).Select(entry => CauterizeString(entry)).ToList();

            var sortednames = names.OrderBy(name => name.Name).ToList();

            foreach (StringWithRarity name in sortednames)
                if (name.Rarity > 0 && !rmlist.Contains(CauterizeString(name.Name)))
                    returnString += name.Name + ":" + name.Rarity.ToString() + ",";

            if (returnString.Contains(",") && returnString.LastIndexOf(",") == (returnString.Length - 1))
                returnString = returnString.Remove(returnString.LastIndexOf(","), 1);

            if (returnString == string.Empty)
                returnString = "Default Values Were Empty";

            return returnString;
        }
        public static List<StringWithRarity> ConvertModStringToStringWithRarityList(string newInputString, Vector2 clampRarity)
        {
            if (string.IsNullOrEmpty(newInputString) || newInputString == "Default Values Were Empty" || newInputString == "Default Values Were Empty:0" || newInputString == ":0")
            {
                return new List<StringWithRarity>();
            }
            List<StringWithRarity> returnList = new List<StringWithRarity>();

            var pairs = newInputString.Split(',');

            foreach (string pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var ModName = parts[0].Trim();
                    int Rarity;

                    if (!int.TryParse(parts[1].Trim(), out Rarity))
                    {
                        CentralConfig.instance.mls.LogInfo($"Cannot Parse Rarity: {parts[1].Trim()} after ModName entry {ModName}");
                        Rarity = 0;
                    }

                    if (clampRarity != Vector2.zero)
                        Rarity = Mathf.Clamp(Rarity, Mathf.RoundToInt(clampRarity.x), Mathf.RoundToInt(clampRarity.y));

                    returnList.Add(new StringWithRarity(ModName, Rarity));
                }
            }
            /*StringBuilder sb = new StringBuilder();
            sb.AppendLine("Return ModList:");
            foreach (StringWithRarity modName in returnList)
            {
                sb.AppendLine($"{modName.Name}: {modName.Rarity}");
            }
            CentralConfig.instance.mls.LogInfo(sb.ToString());*/

            return returnList;
        }
        public static List<StringWithRarity> ConvertTagStringToStringWithRarityList(string newInputString, Vector2 clampRarity)
        {
            if (string.IsNullOrEmpty(newInputString) || newInputString == "Default Values Were Empty" || newInputString == "Default Values Were Empty:0" || newInputString == ":0")
            {
                return new List<StringWithRarity>();
            }
            List<StringWithRarity> returnList = new List<StringWithRarity>();

            var pairs = newInputString.Split(',');

            foreach (string pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var TagName = parts[0].Trim();
                    int Rarity;

                    if (!int.TryParse(parts[1].Trim(), out Rarity))
                    {
                        CentralConfig.instance.mls.LogInfo($"Cannot Parse Rarity: {parts[1].Trim()} after TagName entry {TagName}");
                        Rarity = 0;
                    }

                    if (clampRarity != Vector2.zero)
                        Rarity = Mathf.Clamp(Rarity, Mathf.RoundToInt(clampRarity.x), Mathf.RoundToInt(clampRarity.y));

                    returnList.Add(new StringWithRarity(TagName, Rarity));
                }
            }
            /*StringBuilder sb = new StringBuilder();
            sb.AppendLine("Return TagList:");
            foreach (StringWithRarity tagName in returnList)
            {
                sb.AppendLine($"{tagName.Name}: {tagName.Rarity}");
            }
            CentralConfig.instance.mls.LogInfo(sb.ToString());*/

            return returnList;
        }

        // This method should make the planet name list have to be the exact name

        public static List<StringWithRarity> ConvertPlanetNameStringToStringWithRarityList(string newInputString, Vector2 clampRarity)
        {
            if (string.IsNullOrEmpty(newInputString) || newInputString == "Default Values Were Empty" || newInputString == "Default Values Were Empty:0" || newInputString == ":0")
            {
                return new List<StringWithRarity>();
            }
            List<StringWithRarity> returnList = new List<StringWithRarity>();

            var pairs = newInputString.Split(',');

            foreach (string pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var PlanetName = parts[0].Trim();
                    int Rarity;

                    if (!int.TryParse(parts[1].Trim(), out Rarity))
                    {
                        CentralConfig.instance.mls.LogInfo($"Cannot Parse Rarity: {parts[1].Trim()} after PlanetName entry {PlanetName}");
                        Rarity = 0;
                    }

                    if (clampRarity != Vector2.zero)
                        Rarity = Mathf.Clamp(Rarity, Mathf.RoundToInt(clampRarity.x), Mathf.RoundToInt(clampRarity.y));

                    foreach (ExtendedLevel level in PatchedContent.ExtendedLevels)
                    {
                        if (level.NumberlessPlanetName == PlanetName)
                        {
                            returnList.Add(new StringWithRarity(PlanetName, Rarity));
                            //CentralConfig.instance.mls.LogMessage($"Added level {level.NumberlessPlanetName} with rarity {Rarity} from string {PlanetName}");
                            break;
                        }
                        else
                        {
                            //CentralConfig.instance.mls.LogMessage($"Did not add level {level.NumberlessPlanetName} from string {PlanetName}");
                        }
                    }
                }
            }
            /*StringBuilder sb = new StringBuilder();
            sb.AppendLine("Return PlanetList:");
            foreach (StringWithRarity planetName in returnList)
            {
                sb.AppendLine($"{planetName.Name}: {planetName.Rarity}");
            }
            CentralConfig.instance.mls.LogInfo(sb.ToString());*/

            return returnList;
        }

        // For Route Price settings

        public static string ConvertVector2WithRaritiesToString(List<Vector2WithRarity> values)
        {
            string returnString = string.Empty;

            foreach (Vector2WithRarity vector2withRarity in values)
                if (vector2withRarity.Rarity > 0)
                    returnString += vector2withRarity.Min + "-" + vector2withRarity.Max + ":" + vector2withRarity.Rarity + ",";

            if (returnString.Contains(",") && returnString.LastIndexOf(",") == (returnString.Length - 1))
                returnString = returnString.Remove(returnString.LastIndexOf(","), 1);

            if (returnString == string.Empty)
                returnString = "Default Values Were Empty";

            return returnString;
        }
        public static List<Vector2WithRarity> ConvertStringToVector2WithRarityList(string newInputString, Vector2 clampRarity)
        {
            if (string.IsNullOrEmpty(newInputString) || newInputString == "Default Values Were Empty" || newInputString == "Default Values Were Empty:0" || newInputString == ":0" || newInputString == "0-0:0")
            {
                return new List<Vector2WithRarity>();
            }
            List<Vector2WithRarity> returnList = new List<Vector2WithRarity>();

            var pairs = newInputString.Split(',');

            foreach (string pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var RouteRange = parts[0].Trim();
                    int Rarity;

                    if (!int.TryParse(parts[1].Trim(), out Rarity))
                    {
                        CentralConfig.instance.mls.LogInfo($"Cannot Parse Rarity: {parts[1].Trim()} after RouteRange entry {RouteRange}");
                        Rarity = 0;
                    }

                    var duos = RouteRange.Split('-');
                    if (duos.Length == 2)
                    {
                        int LowerRange;
                        int UpperRange;

                        if (!int.TryParse(duos[0].Trim(), out LowerRange))
                        {
                            CentralConfig.instance.mls.LogInfo($"Cannot Parse LowerRange: {parts[1].Trim()} after RouteRange entry {RouteRange}");
                            LowerRange = 0;
                        }
                        if (!int.TryParse(duos[1].Trim(), out UpperRange))
                        {
                            CentralConfig.instance.mls.LogInfo($"Cannot Parse UpperRange: {parts[1].Trim()} after RouteRange entry {RouteRange}");
                            UpperRange = 0;
                        }

                        if (clampRarity != Vector2.zero)
                            Rarity = Mathf.Clamp(Rarity, Mathf.RoundToInt(clampRarity.x), Mathf.RoundToInt(clampRarity.y));

                        returnList.Add(new Vector2WithRarity(new Vector2(LowerRange, UpperRange), Rarity));
                    }
                }
            }
            /*StringBuilder sb = new StringBuilder();
            sb.AppendLine("Return Vector2List:");
            foreach (Vector2WithRarity routerange in returnList)
            {
                sb.AppendLine($"{routerange.Min}-{routerange.Max}: {routerange.Rarity}");
            }
            CentralConfig.instance.mls.LogInfo(sb.ToString());*/

            return returnList;
        }

        // Dungeons

        /*public static List<ExtendedDungeonFlowWithRarity> ConvertStringToDungeonFlowWithRarityList(string newInputString, Vector2 clampRarity)
        {
            if (string.IsNullOrEmpty(newInputString) || newInputString == "Default Values Were Empty" || newInputString == "Default Values Were Empty:0" || newInputString == ":0")
            {
                return new List<ExtendedDungeonFlowWithRarity>();
            }
            List<ExtendedDungeonFlowWithRarity> returnList = new List<ExtendedDungeonFlowWithRarity>();

            List<ExtendedDungeonFlow> alldungeonFlows = PatchedContent.ExtendedDungeonFlows;

            var pairs = newInputString.Split(',');

            foreach (string pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length == 2)
                {
                    var DungeonName = parts[0].Trim();
                    int Rarity = int.Parse(parts[1].Trim());

                    if (clampRarity != Vector2.zero)
                        Rarity = Mathf.Clamp(Rarity, Mathf.RoundToInt(clampRarity.x), Mathf.RoundToInt(clampRarity.y));

                    foreach (ExtendedDungeonFlow flow in alldungeonFlows)
                    {
                        if (flow.DungeonName.Equals(DungeonName))
                        {
                            ExtendedDungeonFlowWithRarity newFlow = new ExtendedDungeonFlowWithRarity(flow, Rarity);
                            returnList.Add(newFlow);
                            // CentralConfig.instance.mls.LogMessage($"Added dungeon {flow.DungeonName} with rarity {Rarity} from string {DungeonName}");
                            break;
                        }
                        else
                        {
                            // CentralConfig.instance.mls.LogMessage($"Did not add dungeon {flow.DungeonName} from string {DungeonName}");
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Return List:");
            foreach (ExtendedDungeonFlowWithRarity dungeon in returnList)
            {
                sb.AppendLine($"Item Name: {dungeon.extendedDungeonFlow.DungeonName}, Rarity: {dungeon.rarity}");
            }
            CentralConfig.instance.mls.LogInfo(sb.ToString());

            return returnList;
        }*/

        // For updating enemy spawn curves

        public static AnimationCurve MultiplyYValues(AnimationCurve curve, float multiplier, string LevelName, string TypeOf, int minKeyframes = 10)
        {
            // CentralConfig.instance.mls.LogInfo($"{LevelName} {TypeOf} - Started processing curve");
            if (curve == null || curve.length == 0)
            {
                // CentralConfig.instance.mls.LogWarning($"{LevelName} {TypeOf} - Curve does not exist or has no keyframes. Skipping.");
                return null;
            }
            curve = AddKeyframes(curve, minKeyframes);

            Keyframe[] keyframes = new Keyframe[curve.length];

            for (int i = 0; i < curve.length; i++)
            {
                Keyframe key = curve[i];
                float originalValue = key.value;
                key.value = key.value * multiplier;
                keyframes[i] = key;

                // CentralConfig.instance.mls.LogInfo($"{LevelName} {TypeOf} Keyframe {i}: Original Y-Value = {originalValue}, New Y-Value = {key.value}");
            }

            // CentralConfig.instance.mls.LogInfo($"{LevelName} {TypeOf} - Finished processing curve");

            return new AnimationCurve(keyframes);
        }
        public static AnimationCurve AddKeyframes(AnimationCurve curve, int minKeyframes)
        {
            List<Keyframe> keyframes = new List<Keyframe>(curve.keys);

            while (keyframes.Count < minKeyframes)
            {
                for (int i = 0; i < keyframes.Count - 1; i++)
                {
                    if (keyframes.Count >= minKeyframes)
                        break;

                    Keyframe k1 = keyframes[i];
                    Keyframe k2 = keyframes[i + 1];

                    float newTime = (k1.time + k2.time) / 2;
                    float newValue = (k1.value + k2.value) / 2;

                    Keyframe newKeyframe = new Keyframe(newTime, newValue);
                    keyframes.Insert(i + 1, newKeyframe);
                }
            }

            return new AnimationCurve(keyframes.ToArray());
        }
        public static AnimationCurve ScaleXValues(AnimationCurve curve, float scaleFactor, string LevelName, string TypeOf, int minKeyframes = 10)
        {
            if (curve == null || curve.length == 0)
            {
                // CentralConfig.instance.mls.LogWarning($"{LevelName} {TypeOf} - Curve does not exist or has no keyframes. Skipping.");
                return null;
            }
            curve = AddKeyframes(curve, minKeyframes);

            Keyframe[] keyframes = new Keyframe[curve.length];

            for (int i = 0; i < curve.length; i++)
            {
                Keyframe key = curve[i];
                float originalTime = key.time;
                key.time = key.time / scaleFactor;
                keyframes[i] = key;

                // CentralConfig.instance.mls.LogInfo($"{LevelName} {TypeOf} - Keyframe {i}: Original X-Value = {originalTime}, New X-Value = {key.time}");
            }

            return new AnimationCurve(keyframes);
        }
        public static void LogCurve(AnimationCurve curve, EnemyType enemy)
        {
            foreach (var key in curve.keys)
            {
                CentralConfig.instance.mls.LogInfo($"Enemy: {enemy.enemyName}, Time: {key.time}, Value: {key.value}");
            }
        }
        [HarmonyPatch(typeof(HangarShipDoor), "Start")]
        public static class FlattenCurves
        {
            static void Postfix()
            {
                if (!NetworkManager.Singleton.IsHost || !CentralConfig.SyncConfig.FlattenCurves)
                {
                    return;
                }

                AnimationCurve flatCurve = new AnimationCurve();
                flatCurve.AddKey(0f, 1f);
                flatCurve.AddKey(1f, 1f);
                flatCurve.keys[0].inTangent = 0f;
                flatCurve.keys[0].outTangent = 0f;
                flatCurve.keys[1].inTangent = 0f;
                flatCurve.keys[1].outTangent = 0f;

                foreach (EnemyType enemy in allEnemies)
                {
                    enemy.probabilityCurve = flatCurve;
                    // LogCurve(enemy.probabilityCurve, enemy);
                }
            }
        }

        // Modified cfg cleaner from Kitten :3

        public void CleanConfig(ConfigFile cfg)
        {
            StartCoroutine(CQueen(cfg));
        }
        private IEnumerator CQueen(ConfigFile cfg)
        {
            while (!ApplyMoonConfig.Ready || !ApplyDungeonConfig.Ready || !ApplyTagConfig.Ready || !ApplyWeatherConfig.Ready)
            {
                yield return null;
            }
            ConfigCleaner(cfg);
        }
        private void ConfigCleaner(ConfigFile cfg)
        {
            if (!CentralConfig.SyncConfig.KeepOrphans)
            {
                PropertyInfo orphanedEntriesProp = cfg.GetType().GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);
                var orphanedEntries = (Dictionary<ConfigDefinition, string>)orphanedEntriesProp.GetValue(cfg, null);
                orphanedEntries.Clear();
                cfg.Save();
                CentralConfig.instance.mls.LogInfo("Config Cleaned");
            }
            else
            {
                CentralConfig.instance.mls.LogInfo("Orphaned Entries were kept.");
            }
        }
    }

    // Saving eneny + scrap lists and reverting them.

    [HarmonyPatch(typeof(RoundManager), "GenerateNewFloor")]
    [HarmonyPriority(777)]
    public class ResetEnemyAndScrapLists
    {
        static void Prefix()
        {
            string PlanetName = LevelManager.CurrentExtendedLevel.NumberlessPlanetName;

            if (NetworkManager.Singleton.IsHost && (CentralConfig.SyncConfig.EnemyShuffle || CentralConfig.SyncConfig.DoEnemyWeatherInjections || CentralConfig.SyncConfig.DoEnemyTagInjections || CentralConfig.SyncConfig.DoEnemyInjectionsByDungeon))
            {
                if (OriginalEnemyAndScrapLists.OriginalIntLists.ContainsKey(PlanetName) && OriginalEnemyAndScrapLists.OriginalDayLists.ContainsKey(PlanetName) && OriginalEnemyAndScrapLists.OriginalNoxLists.ContainsKey(PlanetName))
                {
                    LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies = OriginalEnemyAndScrapLists.OriginalIntLists[PlanetName];
                    LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies = OriginalEnemyAndScrapLists.OriginalDayLists[PlanetName];
                    LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies = OriginalEnemyAndScrapLists.OriginalNoxLists[PlanetName];
                    CentralConfig.instance.mls.LogInfo("Reverted Enemy lists for: " + PlanetName);
                }
            }
            if (NetworkManager.Singleton.IsHost && (CentralConfig.SyncConfig.ScrapShuffle || CentralConfig.SyncConfig.DoScrapWeatherInjections || CentralConfig.SyncConfig.DoScrapTagInjections || CentralConfig.SyncConfig.DoScrapInjectionsByDungeon))
            {
                if (OriginalEnemyAndScrapLists.OriginalItemLists.ContainsKey(PlanetName))
                {
                    LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap = OriginalEnemyAndScrapLists.OriginalItemLists[PlanetName];
                    CentralConfig.instance.mls.LogInfo("Reverted Scrap list for: " + PlanetName);
                }
            }
        }
    }
    [HarmonyPatch(typeof(RoundManager), "GenerateNewFloor")]
    [HarmonyPriority(676)]
    public class FetchEnemyAndScrapLists
    {
        static void Prefix()
        {
            string PlanetName = LevelManager.CurrentExtendedLevel.NumberlessPlanetName;

            if (NetworkManager.Singleton.IsHost && (CentralConfig.SyncConfig.EnemyShuffle || CentralConfig.SyncConfig.DoEnemyWeatherInjections || CentralConfig.SyncConfig.DoEnemyTagInjections || CentralConfig.SyncConfig.DoEnemyInjectionsByDungeon))
            {
                if (!OriginalEnemyAndScrapLists.OriginalIntLists.ContainsKey(PlanetName) || !OriginalEnemyAndScrapLists.OriginalDayLists.ContainsKey(PlanetName) || !OriginalEnemyAndScrapLists.OriginalNoxLists.ContainsKey(PlanetName))
                {
                    OriginalEnemyAndScrapLists.OriginalIntLists[PlanetName] = new List<SpawnableEnemyWithRarity>(LevelManager.CurrentExtendedLevel.SelectableLevel.Enemies);
                    OriginalEnemyAndScrapLists.OriginalDayLists[PlanetName] = new List<SpawnableEnemyWithRarity>(LevelManager.CurrentExtendedLevel.SelectableLevel.DaytimeEnemies);
                    OriginalEnemyAndScrapLists.OriginalNoxLists[PlanetName] = new List<SpawnableEnemyWithRarity>(LevelManager.CurrentExtendedLevel.SelectableLevel.OutsideEnemies);
                    CentralConfig.instance.mls.LogInfo("Saved Enemy lists for: " + PlanetName);
                }
            }
            if (NetworkManager.Singleton.IsHost && (CentralConfig.SyncConfig.ScrapShuffle || CentralConfig.SyncConfig.DoScrapWeatherInjections || CentralConfig.SyncConfig.DoScrapTagInjections || CentralConfig.SyncConfig.DoScrapInjectionsByDungeon))
            {
                if (!OriginalEnemyAndScrapLists.OriginalItemLists.ContainsKey(PlanetName))
                {
                    OriginalEnemyAndScrapLists.OriginalItemLists[PlanetName] = new List<SpawnableItemWithRarity>(LevelManager.CurrentExtendedLevel.SelectableLevel.spawnableScrap);
                    CentralConfig.instance.mls.LogInfo("Saved Scrap list for: " + PlanetName);
                }
            }
        }
    }

    public static class OriginalEnemyAndScrapLists
    {
        public static Dictionary<string, List<SpawnableEnemyWithRarity>> OriginalIntLists = new Dictionary<string, List<SpawnableEnemyWithRarity>>();
        public static Dictionary<string, List<SpawnableEnemyWithRarity>> OriginalDayLists = new Dictionary<string, List<SpawnableEnemyWithRarity>>();
        public static Dictionary<string, List<SpawnableEnemyWithRarity>> OriginalNoxLists = new Dictionary<string, List<SpawnableEnemyWithRarity>>();
        public static Dictionary<string, List<SpawnableItemWithRarity>> OriginalItemLists = new Dictionary<string, List<SpawnableItemWithRarity>>();
    }
}