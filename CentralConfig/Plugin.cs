using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CSync.Extensions;
using CSync.Lib;
using HarmonyLib;
using System.Runtime.Serialization;
using static CentralConfig.ConfigAider;
using static CentralConfig.DungeonShuffler;
using static CentralConfig.EnemyShuffler;
using static CentralConfig.MiscConfig;
using static CentralConfig.ResetChanger;
using static CentralConfig.ScrapShuffler;
using static CentralConfig.ShuffleSaver;
using static CentralConfig.WaitForDungeonsToRegister;
using static CentralConfig.WaitForMoonsToRegister;
using static CentralConfig.WaitForTagsToRegister;
using static CentralConfig.WaitForWeathersToRegister;

namespace CentralConfig
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("imabatby.lethallevelloader", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("evaisa.lethallib", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.sigurd.csync", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("mrov.WeatherRegistry", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Xilef.LethalBestiary", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Kittenji.FootballEntity", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Chaos.Diversity", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("nomnomab.rollinggiant", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("me.loaforc.facilitymeltdown", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Pinta.PintoBoy", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Kyxino.LethalUtils", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("KeepScrap", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("NoDeathDespawn", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Bob123.LCM_KeepScrap", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Kirpichyov.SaveShipItemsOnDeath", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("LCNoPropsLost", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.adibtw.loadstone", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.adibtw.loadstone.Nightly", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("giosuel.Imperium", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("x753.Peepers", BepInDependency.DependencyFlags.SoftDependency)]
    public class CentralConfig : BaseUnityPlugin
    {
        private const string modGUID = "impulse.CentralConfig";
        private const string modName = "CentralConfig";
        private const string modVersion = "0.16.2";
        public static Harmony harmony = new Harmony(modGUID);

        public ManualLogSource mls;

        public static CentralConfig instance;

        public static int LastUsedSeed;

        public static float shid = 0;

        public static bool HarmonyTouch = false;
        public static bool HarmonyTouch2 = false;
        public static bool HarmonyTouch3 = false;
        public static bool HarmonyTouch4 = false;
        public static bool HarmonyTouch5 = false;
        public static bool HarmonyTouch6 = false;
        public static bool HarmonyTouch7 = false;

        public static CreateMoonConfig ConfigFile;

        public static CreateDungeonConfig ConfigFile2;

        public static CreateMiscConfig ConfigFile3;

        public static CreateTagConfig ConfigFile4;

        public static CreateWeatherConfig ConfigFile5;

        public static GeneralConfig SyncConfig;

        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this);

            SyncConfig = new GeneralConfig(base.Config);

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            ConfigFile = new CreateMoonConfig(base.Config);

            ConfigFile2 = new CreateDungeonConfig(base.Config);

            ConfigFile3 = new CreateMiscConfig(base.Config);

            ConfigFile4 = new CreateTagConfig(base.Config);

            ConfigFile5 = new CreateWeatherConfig(base.Config);

            // Main Configs
            harmony.PatchAll(typeof(WaitForMoonsToRegister)); // HangerShipDoor Start() (Prefix)
            harmony.PatchAll(typeof(WaitForDungeonsToRegister)); // HangerShipDoor Start() (Prefix)
            harmony.PatchAll(typeof(MiscConfig)); // HangerShipDoor Start() (Prefix)
            harmony.PatchAll(typeof(WaitForWeathersToRegister)); // HangerShipDoor Start() (Prefix)
            harmony.PatchAll(typeof(WaitForTagsToRegister)); // HangerShipDoor Start() (Prefix)
            harmony.PatchAll(typeof(FrApplyMoon)); // HangerShipDoor Start() (Postfix)
            harmony.PatchAll(typeof(FrApplyDungeon)); // HangerShipDoor Start() (Postfix)
            harmony.PatchAll(typeof(FrApplyWeather)); // HangerShipDoor Start() (Postfix)
            harmony.PatchAll(typeof(FrApplyTag)); // HangerShipDoor Start() (Postfix)

            // Time Settings
            harmony.PatchAll(typeof(TimeFix)); // MoveGlobalTime (Prefix)
            harmony.PatchAll(typeof(DayTimePassFix)); // PassTimeToNextDay (Prefix)
            harmony.PatchAll(typeof(UpdateTimeFaster)); // MoveGlobalTime (Postfix)
            harmony.PatchAll(typeof(FixTimeUntilDeadlineonDC)); // Disconnect (Postfix)

            // Dungeon Generation
            // harmony.PatchAll(typeof(NewDungeonGenerator)); // GenerateNewFloor (Prefix)
            // harmony.PatchAll(typeof(InnerGenerateWithRetries)); // InnerGenerate (Prefix)

            // Scan Nodes
            harmony.PatchAll(typeof(IncreaseNodeDistanceOnShipAndMain)); // GenerateNewFloor (Postfix)
            harmony.PatchAll(typeof(IncreaseNodeDistanceOnFE)); // MoveGlobalTime (Postfix)
            harmony.PatchAll(typeof(ExtendScan)); // AssignNewNodes (Transplier)
            harmony.PatchAll(typeof(UpdateScanNodes)); // TeleportPlayerServerRpc (Postfix)
            harmony.PatchAll(typeof(AddPlayerToDict)); // PlayerControllerB Start() (Postfix)

            // Shufflers
            harmony.PatchAll(typeof(CatchItemsInShip)); // PassTimeToNextDay (Postfix)
            harmony.PatchAll(typeof(IncreaseScrapAppearances)); // SpawnScrapInLevel (Postfix)
            harmony.PatchAll(typeof(CheckForEnemySpawns)); // AdvanceHourAndSpawnNewBatchOfEnemies (Postfix)
            harmony.PatchAll(typeof(UpdateEnemyDictionary)); // PassTimeToNextDay (Postfix)
            harmony.PatchAll(typeof(UpdateDungeonDictionary)); // PassTimeToNextDay (Postfix)
            harmony.PatchAll(typeof(SaveShuffleDataStrings)); // PassTimeToNextDay (Postfix)

            // Temp Enemy/Scrap stuff
            harmony.PatchAll(typeof(ResetMoonsScrapAfterWeather)); // SpawnScrapInLevel (Prefix)
            harmony.PatchAll(typeof(ApplyWeatherScrapMultipliers)); // SpawnScrapInLevel (Prefix)
            harmony.PatchAll(typeof(ResetEnemyAndScrapLists)); // GenerateNewFloor (Prefix)
            harmony.PatchAll(typeof(FetchEnemyAndScrapLists)); // GenerateNewFloor (Prefix)
            harmony.PatchAll(typeof(EnactTagInjections)); // GenerateNewFloor (Prefix)
            harmony.PatchAll(typeof(EnactWeatherInjections)); // GenerateNewFloor (Prefix)
            harmony.PatchAll(typeof(EnactDungeonInjections)); // GenerateNewFloor (Postfix)
            harmony.PatchAll(typeof(IncreaseLungValue)); // LungProp Start() (Postfix)
            harmony.PatchAll(typeof(IncreaseHiveValue)); // SpawnHiveNearEnemy Postfix()

            // Enemies
            harmony.PatchAll(typeof(FreeEnemies)); // SpawnScrapInLevel (Prefix)
            harmony.PatchAll(typeof(FlattenCurves)); // HangarShipDoor (Start) (Postfix)
            harmony.PatchAll(typeof(MoarEnemies1)); // PlotOutEnemiesForNextHour (Transplier)
            harmony.PatchAll(typeof(MoarEnemies2)); // SpawnDaytimeEnemiesOutside (Transplier)
            harmony.PatchAll(typeof(MoarEnemies3)); // SpawnEnemiesOutside(Transplier)

            // Other
            harmony.PatchAll(typeof(RandomNextPatch)); // System.Random Next() (min < max) (Prefix)
            harmony.PatchAll(typeof(RenameCelest)); // GetNumberlessPlanetName (Prefix)
            harmony.PatchAll(typeof(ResetOnDisconnect)); // Disconnect (Postfix)
            harmony.PatchAll(typeof(ChangeFineAmount)); // ApplyPenalty (Prefix)
            harmony.PatchAll(typeof(RemoveAllPlayersDeadMessage)); // FillEndGameStats (Prefix)
            harmony.PatchAll(typeof(ShipleaveCalc)); // ShipLeave (Postfix)
            harmony.PatchAll(typeof(HUDManagerPatch)); // FillEndGameStats (Postfix)

            // Logging stuff
            // harmony.PatchAll(typeof(ShowIntEnemyCount));
            // harmony.PatchAll(typeof(CountTraps));
            // harmony.PatchAll(typeof(LogFinalSize));
            // harmony.PatchAll(typeof(LogScrapValueMultipler));
        }
    }
    [DataContract]
    public class GeneralConfig : SyncedConfig2<GeneralConfig>
    {
        [DataMember] public SyncedEntry<string> Important { get; private set; }
        [DataMember] public SyncedEntry<string> BlacklistMoons { get; private set; }
        [DataMember] public SyncedEntry<bool> IsWhiteList { get; private set; }
        [DataMember] public SyncedEntry<bool> DoGenOverrides { get; private set; }
        [DataMember] public SyncedEntry<bool> DoScrapOverrides { get; private set; }
        [DataMember] public SyncedEntry<bool> DoScraplistOverrides { get; private set; }
        [DataMember] public SyncedEntry<bool> DoEnemyOverrides { get; private set; }
        [DataMember] public SyncedEntry<bool> DoTrapOverrides { get; private set; }
        // [DataMember] public SyncedEntry<bool> DoMoonWeatherOverrides { get; private set; }
        [DataMember] public SyncedEntry<bool> DoDangerBools { get; private set; }
        [DataMember] public SyncedEntry<string> BlackListDungeons { get; private set; }
        [DataMember] public SyncedEntry<bool> IsDunWhiteList { get; private set; }
        [DataMember] public SyncedEntry<bool> UseNewGen { get; private set; }
        [DataMember] public SyncedEntry<bool> DoDunSizeOverrides { get; private set; }
        [DataMember] public SyncedEntry<bool> DoDungeonSelectionOverrides { get; private set; }
        [DataMember] public SyncedEntry<bool> DoFineOverrides { get; private set; }
        [DataMember] public SyncedEntry<string> BlacklistTags { get; private set; }
        [DataMember] public SyncedEntry<bool> IsTagWhiteList { get; private set; }
        [DataMember] public SyncedEntry<bool> DoEnemyTagInjections { get; private set; }
        [DataMember] public SyncedEntry<bool> DoScrapTagInjections { get; private set; }
        [DataMember] public SyncedEntry<bool> FreeEnemies { get; private set; }
        [DataMember] public SyncedEntry<bool> ScaleEnemySpawnRate { get; private set; }
        [DataMember] public SyncedEntry<bool> RenameCelest { get; private set; }
        [DataMember] public SyncedEntry<bool> RemoveDuplicateEnemies { get; private set; }
        [DataMember] public SyncedEntry<string> BlacklistWeathers { get; private set; }
        [DataMember] public SyncedEntry<bool> IsWeatherWhiteList { get; private set; }
        [DataMember] public SyncedEntry<bool> DoEnemyWeatherInjections { get; private set; }
        [DataMember] public SyncedEntry<bool> DoScrapWeatherInjections { get; private set; }
        [DataMember] public SyncedEntry<bool> DoEnemyInjectionsByDungeon { get; private set; }
        [DataMember] public SyncedEntry<bool> DoScrapInjectionsByDungeon { get; private set; }
        [DataMember] public SyncedEntry<int> UnShrankDungenTries { get; private set; }
        [DataMember] public SyncedEntry<bool> DoScanNodeOverrides { get; private set; }
        [DataMember] public SyncedEntry<bool> KeepSmallerDupes { get; private set; }
        [DataMember] public SyncedEntry<bool> AlwaysKeepZeros { get; private set; }
        [DataMember] public SyncedEntry<bool> BigEnemyList { get; private set; }
        [DataMember] public SyncedEntry<int> RandomSeed { get; private set; }
        [DataMember] public SyncedEntry<bool> KeepOrphans { get; private set; }
        [DataMember] public SyncedEntry<bool> TimeSettings { get; private set; }
        [DataMember] public SyncedEntry<bool> UpdateTimeFaster { get; private set; }
        [DataMember] public SyncedEntry<int> BracketTries { get; private set; }
        [DataMember] public SyncedEntry<bool> LogEnemies { get; private set; }
        [DataMember] public SyncedEntry<string> NewTags { get; private set; }
        [DataMember] public SyncedEntry<bool> GlobalEnemyAndScrap { get; private set; }
        [DataMember] public SyncedEntry<bool> EnemySpawnTimes { get; private set; }
        [DataMember] public SyncedEntry<bool> ScrapShuffle { get; private set; }
        [DataMember] public SyncedEntry<bool> EnemyShuffle { get; private set; }
        [DataMember] public SyncedEntry<bool> DungeonShuffler { get; private set; }
        [DataMember] public SyncedEntry<bool> FlattenCurves { get; private set; }
        [DataMember] public SyncedEntry<string> OoO { get; private set; }
        [DataMember] public SyncedEntry<bool> ScaleScrapValueByPlayers { get; private set; }
        [DataMember] public SyncedEntry<bool> ScaleDungeonSizeByPlayers { get; private set; }
        public GeneralConfig(ConfigFile cfg) : base("CentralConfig") // This config generates on opening the game
        {
            ConfigManager.Register(this);

            Important = cfg.BindSyncedEntry("!READ THIS!",
                "Important Info !!!",
                "This does nothing btw.",
                "The bulk of settings are created and applied individually foreach different entry in a group. They are not generated until after enabling the main true/false toggle for the related setting then rebooting the game.\nEach initial entry is marked with either 'Host Only' or 'All Players'. 'Host Only' settings are only set by the host and do not run on clients. They do not need to be synced.\n'All Players' settings MUST BE SYNCED and set on all players in the lobby.\nIf in doubt just send the config file to everyone.");

            BlacklistMoons = cfg.BindSyncedEntry("_MoonLists_", // These are used to decide what more in-depth config values should be made
                "Blacklisted Moons (All Players (for general, time, and/or misc settings))",
                "Liquidation,Gordion",
                "Excludes the listed moons from the config. If they are already created, they will be removed on config regeneration.");

            IsWhiteList = cfg.BindSyncedEntry("_MoonLists_",
                "Is Moon Blacklist a Whitelist? (All Players (for general, time, and/or misc settings))",
                false,
                "If set to true, only the moons listed above will be generated.");

            GlobalEnemyAndScrap = cfg.BindSyncedEntry("~Global~",
                "Global Enemy and Scrap Injection (Host Only)",
                false,
                "If set to true, allows adding/replacing enemies to the indoor, daytime, and nighttime enemy pools as well as adding scrap onto every moon through only a few settings.");

            DoGenOverrides = cfg.BindSyncedEntry("_Moons_",
                "Enable General Overrides? (All Players)",
                false,
                "If set to true, allows altering of some basic properties including the route price, risk level, and description for each moon.");

            DoScrapOverrides = cfg.BindSyncedEntry("_Moons_",
                "Enable Scrap Overrides? (Host Only)",
                false,
                "If set to true, allows altering of the min/max scrap count and a multiplier for the individual scrap item's values.");

            DoScraplistOverrides = cfg.BindSyncedEntry("_Moons_",
                "Enable Scrap List Overrides? (Host Only)",
                false,
                "If set to true, allows altering of the scrap list for each moon.");

            DoEnemyOverrides = cfg.BindSyncedEntry("_Moons_",
                "Enable Enemy Overrides? (Host Only)",
                false,
                "If set to true, allows altering of the max power counts and lists for enemies on each moon (Interior, Nighttime, and Daytime).");

            BigEnemyList = cfg.BindSyncedEntry("~Big Lists~",
                "Big Enemy Lists? (Host Only)",
                false,
                "If set to true, you will be able to set the enemy lists for every moon with just three strings. This is helpful if you want to copy-paste from a spreadsheet.\nIf you set enemy lists per moon with the setting above, they will contribute to the default value but be overridden by the big lists given the specific moon is found in the big list string.");

            OoO = cfg.BindSyncedEntry("_Enemies_",
                "Order of Operations (Host Only)",
                "add,multiply,replace",
                "Determines the order that adding enemies, multiplying enemy rarities, and replacing enemies occurs in.\nUsed for global, tag, current weather, and current dungeon enemy injections.\nAcceptable values include \"replace,multiply,add\",\"Multiply,Replace,Add\",\"MULTIPLY,ADD,REPLACE\" etc. Do not put other stuff in it like \"glipglorp\".");

            FreeEnemies = cfg.BindSyncedEntry("_Enemies_",
                "Free Them? (Host Only)",
                false,
                "If set to true, extends the 20 inside/day/night enemy caps to the maximum (127) and sets the interior enemy spawn waves to be hourly instead of every other hour.\nA *small* third effect of this setting is that enemies will instantly emerge from their vent when spawned.");

            ScaleEnemySpawnRate = cfg.BindSyncedEntry("_Enemies_",
                "Scale Enemy Spawn Rate? (Host Only)",
                false,
                "When enabled, this setting adjusts the enemy spawn rate to match the new enemy powers. Note that this requires the ‘Enemy Overrides’ setting to be true.\nFor example, Experimentation has a default max power of 4 for interior enemies. If you set this to 6, interior enemies will spawn ~1.5x as fast.\nThis applies to interior, day, and night enemy spawns.");

            EnemySpawnTimes = cfg.BindSyncedEntry("_Enemies_",
                "Accelerate Enemy Spawning? (Host Only)",
                false,
                "If set to true, allows you to set a new value per moon that tweaks the enemy spawn timing.");

            FlattenCurves = cfg.BindSyncedEntry("_Enemies_",
                "Flatten Enemy Curves? (Host Only)",
                false,
                "If set to true, this setting flattens all personal enemy probability curves to a constant value of 1. This ensures that enemy spawning is based solely on their rarity, making the enemy spawn pool rarities accurate.");

            DoTrapOverrides = cfg.BindSyncedEntry("_Moons_",
                "Enable Trap Overrides? (Host Only)",
                false,
                "If set to true, allows altering of the min/max count for each trap on each moon.");

            /*DoMoonWeatherOverrides = cfg.BindSyncedEntry("_Moons_",
                "Enable Weather Overrides?",
                false,
                "If set to true, allows altering of the possible weathers to each moon.\nBeware that adding new weathers to moons that didn't have them before will likely cause funky buggies.\nDO NOT USE WITH WEATHER REGISTRY!!");*/

            UpdateTimeFaster = cfg.BindSyncedEntry("~Misc~",
                "Accurate Clock? (All Players)",
                true,
                "If set to true, the in-game clock will be updated when time moves instead of every 3 seconds.");

            TimeSettings = cfg.BindSyncedEntry("_Moons_",
                "Enable Time Settings? (All Players)",
                false,
                "If set to true, allows setting if time exists, the time speed multiplier, and if time should wait until the ship lands to begin moving (All per moon).");

            DoDangerBools = cfg.BindSyncedEntry("_Moons_",
                "Enable Misc Overrides? (All Players)",
                false,
                "If set to true, allows altering of miscellaneous traits of moons such as hidden/unhidden status, and locked/unlocked status (Keep this false for Selene's Choice compat).");

            RenameCelest = cfg.BindSyncedEntry("_Moons_",
                "Rename Celest? (All Players)",
                false,
                "If set to true, Celest will be renamed to Celeste. This fixes any config entry mismatches between her and Celestria.");

            BlackListDungeons = cfg.BindSyncedEntry("_DungeonLists_",
                "Blacklisted Dungeons (All Players (for dungeon size))",
                "",
                "Excludes the listed dungeons from the config. If they are already created, they will be removed on config regeneration.");

            IsDunWhiteList = cfg.BindSyncedEntry("_DungeonLists_",
                "Is Dungeon Blacklist a Whitelist? (All Players (for dungeon size))",
                false,
                "If set to true, only the dungeons listed above will be generated.");

            UseNewGen = cfg.BindSyncedEntry("_Dungeons_",
                "Enable Dungeon Generation Safeguards? (All Players)",
                true,
                "If set to true, this refines the dungeon loading process to retry with various imput sizes.\nInstead of a hard-cap of 20 attempts, the dungeon will go through and attempt to generate with a different size until it succeeds.\nThis *should* only fail if the dungeon has its own generation issues or its size multiplier is reduced below an acceptable size to begin with.");

            UnShrankDungenTries = cfg.BindSyncedEntry("_Dungeons_",
                "Retries before Changing Size (All Players)",
                20,
                "The number of attempts made by the dungeon to generate using its original input size multiplier before it begins to adjust the multiplier either upwards or downwards.\nPreviously, the size adjustment used to happen after just one failed attempt.");

            BracketTries = cfg.BindSyncedEntry("_Dungeons_",
                "Tries per Dungeon Size Bracket (All Players)",
                25,
                "The number of generation attempts spent on each size bracket (The brackets are: 20x-10x, 10x-5x, 5x-3x, 3x-2x, and 2x-1x for reference).\nIncreasing this value gives an exponentially higher chance of generation success but takes more time.");

            DoDunSizeOverrides = cfg.BindSyncedEntry("_Dungeons_",
                "Enable Dungeon Size Overrides? (All Players)",
                false,
                "If set to true, allows altering various dungeon size related numbers. This includes the moon-tied size multipliers, the Dungeon's Map Tile Size (which is divided from the moon's size), a Dungeon specific min/max size clamp applied after the MTS is factored in, the scaler to determine how strict the size clamp should be, and a Dungeon specific min/max random size multiplier applied after the clamping.");

            DoDungeonSelectionOverrides = cfg.BindSyncedEntry("_Dungeons_",
                "Enable Dungeon Selection Overrides? (Host Only)",
                false,
                "If set to true, allows altering the dungeon selection pool tied to the dungeon by moon name, level tags, route price range, and mod name.");

            DoEnemyInjectionsByDungeon = cfg.BindSyncedEntry("_Dungeons_",
                "Enable Enemy Injection by Current Dungeon? (Host Only)",
                false,
                "If set to true, allows adding/replacing enemies on levels based on the current dungeon (inside, day, and night).");

            DoScrapInjectionsByDungeon = cfg.BindSyncedEntry("_Dungeons_",
                "Enable Scrap Injection by Current Dungeon? (Host Only)",
                false,
                "If set to true, allows adding scrap to levels based on matching tags.");

            KeepOrphans = cfg.BindSyncedEntry("~Misc~",
                "Keep Orphaned Entries? (Personal)",
                false,
                "If set to true, the config will not 'clean' itself after processing, this will result in a more crowded/messy config but will prevent unloaded entries from being removed.");

            RandomSeed = cfg.BindSyncedEntry("~Misc~",
                "Starting Random Seed (Host Only)",
                -1,
                "Leave at -1 to have it be random. The seed will be updated daily regardless of this setting.");

            LogEnemies = cfg.BindSyncedEntry("~Misc~",
                "Log Current Enemy/Scrap Tables? (Host Only)",
                false,
                "If set to true, the console will log the current indoor, daytime, and nighttime enemy spawn pools as well as the current scrap pool 10 seconds after loading into the level.\nOnly accurate on the host, as enemy and scrap pools are ultimately decided by the host.");

            DoFineOverrides = cfg.BindSyncedEntry(">Fines<",
                "Enable Fine Overrides? (All Players)",
                false,
                "If set to true, allows you to set the fine for dead/missing players and the reduction on the fine for having brought the body back to the ship.");

            DoScanNodeOverrides = cfg.BindSyncedEntry(">ScanNodes<",
                "Enable Scan Node Extensions? (All Players)",
                false,
                "If set to true, allows you to set the min/max ranges for the scan nodes on the ship, main entrance, and fire exits (if you have ScannableFireExit installed).");

            BlacklistTags = cfg.BindSyncedEntry("_TagLists_",
                "Blacklisted Tags (Host Only)",
                "vanilla,custom,free,paid,argon,canyon,company,forest,marsh,military,ocean,rocky,tundra,valley,wasteland,volcanic,rosiedev,sfdesat,starlancermoons,tolian",
                "Excludes the listed tags from the config. If they are already created, they will be removed on config regeneration.");

            IsTagWhiteList = cfg.BindSyncedEntry("_TagLists_",
                "Is Tag Blacklist a Whitelist? (Host Only)",
                true,
                "If set to true, only the tags listed above will be generated.");

            DoEnemyTagInjections = cfg.BindSyncedEntry("_Tags_",
                "Enable Enemy Injection by Tag? (Host Only)",
                false,
                "If set to true, allows adding/replacing enemies on levels based on matching tags (inside, day, and night).");

            DoScrapTagInjections = cfg.BindSyncedEntry("_Tags_",
                "Enable Scrap Injection by Tag? (Host Only)",
                false,
                "If set to true, allows adding scrap to levels based on matching tags.");

            RemoveDuplicateEnemies = cfg.BindSyncedEntry("_Enemies_",
                "Remove Duplicate Enemies? (Host Only)",
                false,
                "If set to true, after enemy spawn lists are updated by various means, any time there are 2 or more of the same enemy, only the entry for the enemy with the highest rarity will be kept.\nThis means that if 4 sources add the bracken at various rarities, only the highest value is kept. (In \"Flowerman:5,Flowerman:75,Flowerman:66,Flowerman:73\" The bracken will only be added once with a rarity of 75.)");

            KeepSmallerDupes = cfg.BindSyncedEntry("_Enemies_",
                "Keep Smallest Rarity? (Host Only)",
                false,
                "If this and Remove Duplicates is set to true, the lowest rarity of a given enemy with be kept instead of its highest rarity.");

            AlwaysKeepZeros = cfg.BindSyncedEntry("_Enemies_",
                "Always Keep Zeros? (Host Only)",
                false,
                "If this and Remove Duplicates is set to true, any enemies with that have an entry of 0 rarity will be kept regardless. This allows you to blacklist enemies from specific weathers/tags/dungeons by putting EnemyName:0 under the setting.");

            BlacklistWeathers = cfg.BindSyncedEntry("_WeatherLists_",
                "Blacklisted Weathers (Host Only)",
                "DustClouds",
                "Excludes the listed weathers from the config. If they are already created, they will be removed on config regeneration.");

            IsWeatherWhiteList = cfg.BindSyncedEntry("_WeatherLists_",
                "Is Weather Blacklist a Whitelist? (Host Only)",
                false,
                "If set to true, only the weathers listed above will be generated.");

            DoEnemyWeatherInjections = cfg.BindSyncedEntry("_Weathers_",
                "Enable Enemy Injection by Current Weather? (Host Only)",
                false,
                "If set to true, allows adding/replacing enemies on levels based on the current weather (inside, day, and night).");

            DoScrapWeatherInjections = cfg.BindSyncedEntry("_Weathers_",
                "Enable Scrap Injection by Current Weather? (Host Only)",
                false,
                "If set to true, allows adding scrap to levels based on the current weather as well as multipliers to the scrap amount and individiual scrap values.");

            NewTags = cfg.BindSyncedEntry("_TagLists_",
                "New Tags (Host Only)",
                "Smunguss,Glorble,Badungle",
                "New 'tags' that are considered by this mod's settings (Remember to not blacklist them above).");

            ScrapShuffle = cfg.BindSyncedEntry("~Shufflers~",
                "Scrap Shuffler (Host Only)",
                false,
                "If set to true, scrap that could have but did not spawn on a given day will be more likely to spawn the next day, provided that the scrap is in the next level's final scrap pool as well.\nThis temporary selection chance boost increases every day the specific scrap was in the scrap pool but was not selected. The boost returns to 0 when ANY amount of that scrap is spawned.");

            EnemyShuffle = cfg.BindSyncedEntry("~Shufflers~",
                "Enemy Shuffler (Host Only)",
                false,
                "If set to true, enemies that could have but did spawn on a given day will be more likely to spawn the next day, provided that the enemy is in one of the next level's final enemy pools as well.\nThis temporary selection chance boost increases every day the specific enemy was in the spawn pool but was not spawned. The boost returns to 0 when ANY number of that enemy is spawned inside, during the day, or during the night.");

            DungeonShuffler = cfg.BindSyncedEntry("~Shufflers~",
                "Dungeon Shuffler (Host Only)",
                false,
                "If set to true, dungeons that are not selected after a given day will be more likely to be selected the next day, provided that the dungeon is possible to be selected by the next level.\nThis temporary selection chance boost increases every day that dungeon was selectable but was not chosen. The boost returns to 0 when the dungeon is selected.");

            ScaleScrapValueByPlayers = cfg.BindSyncedEntry("<Player Count Scaling>",
                "Adjust Scrap Value for PlayerCount? (Host Only)",
                false,
                "If set to true, the value of scrap will be adjusted based on the number of players in the lobby, !!This happens at the START of the match and only affects newly spawned scrap!!\nPlayerDiff = PlayerCount - PlayerThreshold Then ScrapValuePercent *= (1 + Percent / 100) ^ -PlayerDiff\nDefault Example: (1.1) ^ -(4-2) = 82.6% of the multiplier.\nAnother Example: (1.05) ^ -(6-3) = 86.4% of the multiplier.");

            ScaleDungeonSizeByPlayers = cfg.BindSyncedEntry("<Player Count Scaling>",
                "Adjust Dungeon Size for PlayerCount? (All Players)",
                false,
                "If set to true, the size multiplier will scale with the number of players in the lobby. More players will result in a larger interior.\nThe same formula used to calculate the scrap value adjustment is used again here, but with a positive exponent, which creates a direct relationship between player count and dungeon size.\nDefault Example: (1.1) ^ (4-2) = 21% dungeon size increase.\nAnother Example: (1.15) ^ (3-4) = 13% dungeon size decrease.");
        }
    }
}
