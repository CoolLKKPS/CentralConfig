## 0.16.4
- Rebuild and hopefully fix things

## 0.16.3
- Add null checks.

## 0.16.2
- (Fork for me) Remove dungeon generation and hopefully fix dungeon generate completely failed and stuck on loading, usually dungeon generation safeguard can prevent most cases btw.

- The reason i done that cuz, i already saw too many mods messed dungeon generation things up, i just need a stable way, i don't have time to investigate too many dungeon things.

- This update will make some dungeon features no longer work.

## 0.16.1
- Peepers are now included in the enemytype list and can be spawned in spawn pools, enemyname is `peeper` it is not in default values because it has its own spawn mechanics typically.

- Made indoor beehives no longer conductive.

- Fixed disconnect null by using a seperate bool instead of a networkmanager property.

- Accepted new WR pr.

## 1.16.0
- Added new setting `Adjust Dungeon Size for PlayerCount` - Same math and location as the ScrapValue adjustment for PlayerCount but dungeon size is **directly** correlated to PlayerCount. This setting has its own `Threshold`,`Percent`,`PercentChange`, and `Min/Max` Values.

- Changed some descriptions for scaling settings to clarify calculations. !!This may default their values!!

- Tweaked `Fine Override` math and added more text variants, (now has custom text for when you make money from dying horribly).

- When using a scrap saver and all players die, the scrap collected that round is now displayed correctly in the EndGameStats, (was previously shown as 0).

- Fixed the mod breaking when a moon name contains an apostrophe.

- The default value for dungeon selection by route price now omits 0 rarity entries.

- The Cyrillic lettered moons used by RebalancedMoons no longer appear as duplicate moons in the config (they cannot be visited so there is no need to edit them). These moons are also omitted in the default values for dungeon selection by PlanetName.

- Edited some logs.

- The crown from Scoopy's Variety is now affected by the scrapValueMultiplier (as a treat<3).

## 0.15.8
- Fixed interior shuffler picking the same random number for every interior on day passing.

## 0.15.7
- Fixed compatibility with new WeatherRegistry Update (thank u mrov<3)

- Fixed time settings skipping a day(s) until deadline if the player disconnects mid day but doesn't close the application (savescumming).

- Cleared hard blacklist for locking/unlocking certain moons that were story log unlocks (you can now lock rosie's p moon).

## 0.15.6
- Fixed compat for Diversity 3.0.0 and beyond.

## 0.15.5
- Fixed compat with the new WeatherRegistry version.

- WR will never override the scrapValueMultiplier while using this mod but is factored into the scrapValueMultiplier calculation on my end.

## 0.15.4
- Rebuilt project in modern format. Thank u Robin!!

- This notable fixes errors with:
  - The 'All Scrap Lost' screen still appearing with LethalUtilties KeepScrap on.

- Integrated pausing time with Imperium while using time settings.

## 0.15.3
- Loadstone/LoadstoneNightly now has the async dungeon reenable before dungeon generation (like if it was shut off for black mesa last round, it goes back on after).

## 0.15.2
- Fixed 6 null refs when not using shufflers (the mod will work now without them on).

- Async Dungeon Generation is disabled only after Black Mesa is selected, I'll let loadstone reenable it afterwards when I can tell their guids apart.

## 0.15.1
- The totalScrapValueInLevel value is now actually updated to the accurate value.
This is only used when determining the letter grade.
Vanilla updates it already for gift boxes, nutcracker shotguns, and beehives but NOT the apparatus.
Several mods like lgu and sellbodies add ways to get more scrap/scrapvalue in the level and *probably* don't update the value.
So now the letter grade is also accurate.

## 0.15.0
- Added new setting `Adjust Scrap Value for PlayerCount` - Scrap value is adjusted based on the number of players in the lobby.
  - `Scale Scrap Value PlayerCount Threshold` - If there are fewer players than this threshold, scrap will be worth more. If there are more players, scrap will be worth less. (Default: 3)
  - `Scale Scrap Value Percent` - Percentage factor applied to the scrap value multiplier based on the difference between the actual player count and the threshold. (Default: 10)
  - `Scale Scrap Value Percent Change` - The absolute difference between the PlayerCount and Threshold is multiplied by this value before being added to the Scale Scrap Value Percent. (Default: 5)
  - `Scale Scrap Value Min and Scale Scrap Value Max` - Hardcap for the minimum and maximum multiplier from this feature. (Default: 0.5-1.5)
  - Calculation:
    - `PlayerCount - Threshold = PlayerDiff`
    - `(Percent + PercentChange * |PlayerDiff|) / 100 + 1 = Result`
    - `(Result) ^ -PlayerDiff`
    - The value is then clamped before the scrap value multiplier is multiplied by it.

- Apparatuses and beehives are now affected by the scrap value multiplier, this is compatible with facility meltdown.

- Added two new settings under `Fines`:
  - Credit rewards based on performance (ranks F to S).
  - Option to switch from a fixed value to a percentage increase.

- The EndGameStats screen now accurately displays the total scrap collected in the level when the ship leaves, instead of when it lands (I swear I added this forever ago but ig not).

- `Fines` and `ScanNodes` are now their own ‘pages’.
- Scrap/enemy/dungeon shuffler settings are only generated if the shuffler is active. **!!EXPECT ORPHANS!!**

- `PintoBoy` and `PintoBoy` are now `PintoBoyLJ` and `PintoBoyFD` in the config.

- Default enemy lists no longer have special characters in some cases.

- The penalty message now says “No fine will be extracted from the crew.” when it would have said “A fine of 0% will be extracted from the crew.”

- The “ALL SCRAP LOST/ALL PLAYERS DEAD” graphic does not appear if any mod allows you to keep scrap on death.

- Setting a moon to have an empty scrap pool no longer defaults.

- Fixed missing dictionary key for a moon when getting the tags on a moon.

- Fixed null references for clients on disconnect.

- Scrap/enemy/dungeon shufflers are not incremented or reset when the last moon was the company.

- Fixed zero rarity enemies being recognized incorrectly by the shuffler when set to run before enemy operations.

## 0.14.1
- Added bool to make the enemy shuffler apply before the add/multiply/replace functions.
False keeps it the same as before (..., Tags, Current Weather, Current Dungeon, ..., Enemy Shuffler, ...).
True changes the order to (..., Enemy Shuffler, Tags, Current weather, Current dungeon, ...).

- The dungeon no longer generates asynchronously with Loadstone (but Loadstone still carries out dozens of optimizations).
This was just a setting in DunGEN, shouldn't make too much of a different being off.
Very epically this finally fixes the Black Mesa critical loading failure !!!!

- Also more typo fixes <3

## 0.14.0
- Added new `_Enemies_` setting `Flatten Enemy Curves?`:
  - If true, all enemy’s personal spawn curves become 0,1 -> 1,1.
  - This makes the rarity pool the only factor in deciding which enemy to spawn when the pool is called to spawn one.

- Added enemy multipliers for each time/type on global, tag, weather, and dungeon enemy injection settings.
  - Used alongside Add and Replace Enemies.
  - `Flowerman:2` under the interior enemy multiplier under the mansion dungeon would make brackens have double rarity when the mansion is selected.
  - This is similar to how the Maneater is 1.7x more common in the mineshaft.
  - Floats/decimals are acceptable here, `MouthDog:0.33` on the night enemy multiplier under the foggy weather would make eyeless dogs 3 times less likely to spawn during foggy weather.

- The order of enemy operations is now configurable under `_Enemies_` (Default: add,multiply,replace).
  - These are the same operations used under global, tag, weather, and dungeon injection settings.
  - If you change the setting to `multiply,replace,add` it will now multiply first then replace enemies then add new enemies.

- Changed how scrap is determined to have been spawned during the last day as opposed to being on the ship (For Scrap Shuffler):
  - This fixes some funny items not getting their day count reset correctly after spawning (toy hammer, scp427, and gnome to name some I observed).

- Moved tag injection settings to be before weather (Now it’s Tag -> Weather -> Dungeon on landing).

- Duplicate tags are only read once per moon.

- Moved more host-only settings to only running on the host.

- Improved how enemy names are read in some methods.

- Added a gap between the enemy and scrap table logging for easier viewing.

- Organized some code to be easier to read and find usages of methods.

- Fixed tags not being added to the config correctly.

- Fixed Dungeon list reading the moon blacklist/whitelist.

- Silly typo fixes.

## 0.13.9
- Added checks to keep host methods only on the host, optimizes the clients better since they don't need shuffler data, etc.

- Fixed dungeon matching by planet name not applying for moons that have special characters.

- Fixed the `~Big Lists~` being unable to work with moons that have special characters.

## 0.13.8
- Redid the shuffle randoms to avoid repeating digits.

- '~','-',':', and ',' are ignored in enemy/scrap names when getting default tables.

- Divorced Scrap Lists from the other scrap settings. **THIS WILL RESET YOUR SCRAP LIST IF YOU DON'T KEEP ORPHANS.**

## 0.13.7
- Fixed percent increase for shuffler, it now calculates correctly.

- Kept in the shuffler logging for remembered keys on rejoin.

## 0.13.6
- forgor to remove the really thick debug lines.

## 0.13.5
- Added a new setting for each shuffler, you can have the RandomValue be a percent increase instead of a direct addition.
False: `Rarity += DayCount * RandomValue`
True: `Rarity *= (DayCount * (Randomvalue/100))` (rounds to nearest int)
-Note that the percent increase bound to the *original* value, not the *current* value. It is not compounded.
For example a +100% would be +100%, +200%, +300%, +400% and *not* +100%, +300%, +700%, +1500%

- Added shuffler settings for interior selection.
I know a mod has a similar function but this works the same way my other shufflers work.
-The dungeon weights are never decreased, they cannot go below their default value.
-The dungeon weights are not unaltered to begin with, the dungeon selection settings from either this mod or any other mod will be left with their values before temp boosts are applied over time.
-The value increases by a random range. (default: 0-20% increase per day)
-The shuffler is commited to the save file as long as the shuffle saver setting is true, it will pick up right where it was when the host disconnects/rejoins.
-Known "issue" - the `simulate` weight pools are not boosted for clients. This is only visual however since the host selects the dungeon.

- Added setting `Rollover Zero or Less` - If set to true, entries with a rarity of 0 or less will always increase linearly instead of by percent. This setting has a very specific use, enemies or scrap with a negative rarity will gradually increase until they become positive, effectively creating a minimum number of days between their appearances on that level. (This setting is that that function is compatible with the percent increase)

- After shufflers multiply rarity they are now clamped. Direct addition clamps between -99999 and 99999, percent increase clamps between 0 and 99999.

## 0.13.1
- Fixed blacklists not being read correctly.

## 0.13.0
- Added blacklists for scrap/enemies under `~Shufflers~`. Entries here will never be incremented by the shufflers.

- Added a setting to remove 0 rarity scrap/enemies right before a shuffler runs. This allows 0 rarity to continue to function as an override to remove scrap/enemies from specific moons, weather conditions, or dungeons without contributing to the shuffler day count.

- Added the registry list from LethalBestiary, enabling support for SCPs 173, 966, 1507, and 3199, as well as the Nightmare Freddy and Foxy mods.

- Manually added support for Diversity’s Walker, Kittenji’s Football enemy, and Nomnom’s Rolling Giant.

The mod uses a list of `EnemyType`s to match enemies from the config strings to actual enemies, similar to LLL’s own config. CC already included the LLL and LL registration lists, covering the majority of enemies. Any specific enemies not recognized by this mod will need to be manually added as soft compatibility. Just hit me up if you see one you want added.

- Loosened the requirements for string config arguments. Capitalization and special characters (including spaces) no longer matter. As long as the correct letters and numbers are in the right order, the config will work. For example, `scp966,scp999,scp049,scp956,scp3199` will now be accepted instead of needing the stricter `SCP966,SCP-999,Scp 049,SCP-956,scp3199`.

- Patched a string format bug where values meant to be integers (typically rarity arguments) would break the game if they weren’t numbers. Now, non-numeric values default to 0 and continue. For example, `Walker:,`, `Walker: ,`, or `Walker:NotANumber,` will all default to `Walker:0,`.

- Redesigned how scrap multipliers are calculated. A multiplier is now set to 0.4f (vanilla) and then multiplied by the moon’s scrap multiplier (if it exists) and the current weather’s scrap multiplier (if it exists). If you aren’t using scrap injection by weather and have WeatherRegistry installed, it will use the config from that mod for the current weather.

- The rarity range for the config has been changed from `0 - 99999` to `-99999 - 99999`, allowing shufflers to pull rarities from the negatives.

- The `AllEnemiesList` and `AllMoonTagsList` are now constructed only once, significantly reducing load times.

- The weather `DustClouds` is now blacklisted by default due to not functioning as a real weather without a mod to change it.

- Added more checks to the shufflers’ dictionaries to handle cases where both dictionaries are empty.

## 0.12.6
- Rewrote a lot of the helper methods for fun and sanity.

Most of this isn't super noticable to players but notably..
- Because the methods now split apart the input string into the exact needed pieces and define them all, I can relate objects to one another directly with full equivalency. This should elimate the issue of names containing other names getting confused. The only exception being `Dungeon Matching by Planet Name` as LLL's handling confuses moon names on its end.

- Added breaks to the parts of code where it is checking for a specific thing, once it finds the thing is stop looking for it saving a lot of time.
To give some math lets say you have 200 items and you want to find all and every, that will be 200^2 = 40,000 attempts. With a break, it will stop once it finds what it will need and therefore be 1 + 2 + 3 + 4.. + 199 + 200 = 21,000 effectively making the method take half the time it used to.

- When reading a config entry for enemies, scan node names (Bracken, Snare Flea, Barber, Old Bird, Spore Lizard, etc) are also recognized and assigned to the matching enemy. This does not work for `ReplaceEnemies`.

Between this and the last patch, loading into a lobby and loading in a new level will be significantly faster, especially as a client.

## 0.12.5
- Analyzed each settings to determine what needs to run on clients and what only needs to run on the host.

`'Host Only'` Only runs on the host and is synced by 
-Every enemy spawning setting (Moon specific enemy pools/big list, moon power counts, global/weather/tag/dungeon enemy injection/replacement, Free Them/scale enemy spawnrate/accelerate spawning, and remove duplicate entries/keep lowest rarity instead/always keep zeros).
-Every scrap spawning setting (Moon specific scrap pool, moon scrap counts and multiplier, global/weather/tag/dungeon scrap addition, weather applied scrap value and count multipliers).
-Also dungeon *selection*, all the shuffle related settings, the day 1 seed, and traps

`'All Players'` Needs to be synced between all players (and runs on everyone).
-Dungeon safeguards, its retry counts, and the dungeon size settings (All of them, moon applied size as well). Or else dungeon desyncs from being a different size.
-General, time related and misc moon settings.
-The scan node and fine settings.
-`Accurate Clock` (some mods hook into its tick time like LethalRegeneration).
-`Rename Celest?`

**THE GENERAL SETTINGS WILL BE DEFAULTED**
I have the keep orphaned true by default for now to prevent data loss, I still highly recommend you back up your config file.
After you boot up the first time and get the update settings, re-enable any general settings you use, reboot and all the foreach settings should be adopted so you can disable keep orphans.

- Fixed a really weird specific issue with WR caused by `Rename Celest?`

- Added important info at the very top of the config file as a null entry.

## 0.12.0
- Added `~Shufflers~` settings for scrap and enemy pools.
-Can be enabled for enemies (all pools), scrap, or both enemies and scrap.
-This setting increments a value tied to each enemy/scrap every time that the enemy/scrap was in the current day’s enemy/scrap pools but did not spawn.
-When constructing the current day’s enemy/scrap pools, it will increase that given enemy/scrap’s rarity by its respective value mentioned above.
-Before being added, the value is multiplied by a random value (range is configurable, default: 0-2).
-Each enemy only has one value attached to it, not one for indoor, day, and night. If that enemy was in the current predicted indoor, day, or night pool, the rarity increase will be applied to it where it is.
-Full order of operations is now:
`Default/Overridden moon-tied enemy/scrap pools -> Big List replaces that if true -> Global enemy replacement then enemy addition and scrap addition -> Current weather enemy replacement then enemy addition and scrap addition -> Tags (alphabetical order) enemy replacement then enemy addition and scrap addition -> Current dungeon enemy replacement then enemy addition and scrap addition -> Remove duplicates -> Apply ~Shufflers~ rarity boosts -> Use the new enemy/scrap pools.`
-You can toggle whether the rarity boosts are only created and exist in this session or if they are committed to the save file and loaded on joining.

- `Log Current Enemy Tables?` is now `Log Current Enemy/Scrap Tables?` and can be found under `~Misc~`.

- Removed unnecessary logging and host-driven logging now occurs only on the host.

- Fixed weather desync between host and clients on the first day.

- Fixed global scrap not being applied.

## 0.11.2
- Reverted changing the lengthOfHours to adjust the enemy spawn curves as it is unnecessary.
This fixes the time speed bug.

Accidentally made a method to 'squish' enemy spawn curves while testing stuff.
I will not let it go unused so...

- Added new `_Enemies_` setting, `Accelerate Enemy Spawning?`.
If set to true, you can set a new value separately for each moon that determines how fast the enemy spawn curves advance.
The default of 1x will not change anything.
2x will result in day enemies vanishing faster while night enemies appear twice as fast and the interior is overrun much sooner.
Using a value such as 0.25x will act in reverse and instead result in enemies spawning into the level much slower.

## 0.11.1
- Fixed Tag whitelist/blacklist not applying when capitalized.

## 0.11.0
- Introduced `~Global~` settings, allowing you to add or replace enemies and scrap on all moons (excluding blacklisted ones). These changes occur immediately after individual list configurations (if enabled). Replacement happens before adding new entries as usual.

- Added methods to save the original (unmodified by this mod) enemy and scrap tables before any changes. These new dictionaries are used on Disconnect() to revert lists to their original versions, preventing duplicate changes from global settings and ensuring temporary changes (current weather/dungeon) are reverted upon leaving.

- The mod now uses actual `ExtendedLevel`s and `ExtendedDungeonFlow`s for dictionary keys instead of their `NumberlessPlanetName`s and `DungeonName`s for optimization.

- Delayed when the `randomMapSeed` is logged, I can confirm synchronization with clients in LAN testing. :3

- Added setting to 'add' moon tags, for example you can input `Spooky,OuterSpace,Amogus` (don't forget to whitelist/not blacklist them), now you could put Spooky, OuterSpace, and Amogus on whatever moons you'd like. They additionally will get their own section in the config to add/replace enemies and add scrap onto those moons.

- I 'cauterize' tags in more places to further prevent duplicate entries from appearing.

- Reset the enemy table logging to prevent it from growing excessively by retaining old logs and just adding on the new log to it.

- Resolved the WeatherRegistry `SetPlanetsWeather()` null reference issue by moving the `lengthOfHours` change to a different method.

- Fixed the whoopsie of `scrap injection by dungeon` being under the tag (dungeonname) instead of dungeon (dungeonname).

- Moved the setting to enable Big Lists to be under the `~Big Lists~` section.
Note: **THIS CHANGE WILL DEFINETLY RESET YOUR BIG LISTS. PLEASE COPY AND PASTE THEM INTO A SPREADSHEET OR DISCORD CHANNEL BEFORE BOOTING UP TO AVOID DATA LOSS.**

## 0.10.10
- Added soft compat with WeatherRegistry. If it is installed, I get all the modded weathers through it (instead of hand typing them into the weather list).
Note: The weather scrap value multipliers from this mod take priority over WR's config which sets the multiplier without allowing for other mods have an affect on it.

- Removed setting to change what weathers can appear on what moons, please use WR for this, it is much more stable and has indepth control that is much less likely to break.

-Added two new size settings for each dungeon, Min/Max Random Size Nultiplier, a random value is picked between the range and is multiplied onto the current dungeon size.

The current size for a dungeon will be calculated as:
`(Moon's Size Multiplier / Dungeon's Map Tile Size) -> Clamped by Dungeon's Min/Max Size Multiplier THEN multiplied by a selected value from the Random Size Multiplier Min/Max(inclusive)`
I repeat that the random multiplier is applied AFTER the clamp.

- Changed Starting random seed data:
-If the `Starting Random Seed` config setting is -1, generate a random number, otherwise save that value.
-Only if the `randomMapSeed` is 0 or negative, then it will set it to the saved seed value from above.
-This only occurs on the host's end when joining their lobby.
-`randomMapSeed` is synced for each client on connection by vanilla code.

- Changing an indoor/day/night enemy table to either `""`, `"Default Values Were Empty"`, or only invalid entries will now correctly set the table to have no enemies instead of defaulting to its default value.

- Spent over an hour rewriting most of the ReadMe (it was incredibly outdated).

## 0.10.9
- Improved the ReplaceEnemies method, this includes:
-Optional rarity override value `Flowerman-69:MouthDog` (In this instance the MouthDog will use 69 as its rarity when it replaces the Bracken instead of inheriting the Bracken's rarity).
-Optional chance to replace `Flowerman:MouthDog~45` (In this instance the MouthDog does still inherit the Bracken's rarity but will only replace the Bracken 45% of the time).
You can also use both at the same time `Flowerman-69:MouthDog~45` (In this instance the MouthDog will use 69 as its rarity instead of the Bracken's rarity and will only replace the Bracken 45% of the time).
Or you can use neither as the setting previously worked `Flowerman:MouthDog` (In this instance the Mouthdog will inherit the Bracken's rarity and be guaranteed to replace them).
These four usages are interchangable so one config entry could look as such: `Flowerman:MouthDog,Centipede-40:RadMech,Girl:Walker~50,Butler-100,ButlerBees~35`
This applies to the ReplaceEnemies setting for level tags, current dungeon, and current weather.
The config description has been written over and improved to cover this all compactly and clearly.

- Adding/Replacing enemies and adding scrap by tag is now done in alphabetical order by tag name rather than semi-random. This should make the resulting tables more predictable for when a level contains various tags (IE. canyon->custom->free...).

- Added setting under `_Enemies_` to have the current indoor, day, and night enemy tables be logged. This occurs 10 seconds after the level loads to keep it visible. Helpful for if you want to know the kind of line ups you will get based on the moon's tables, tags, the current dungeon, and current weather (Written in EnemyName,rarity format and split by time of day).

## 0.10.8
- Greatly improved the size adjustment calculations for the dungeon safeguards.

Old:
if >1 then subtract 10% of current size.
if <1 then subtract 0.05x.

New:
if >20 then subtract 10% of current size
if <20 & >10 subtract 10/BracketTries.
if <10 & >5 subtract 5/BracketTries.
if <5 & >3 subtract 3/BracketTries.
if <3 & >2 subtract 2/BracketTries.
if <2 & >1 subtract 1/BracketTries.
if <1 subtract 0.02x.

This puts a much stronger emphasis on sizes that are likely to generate.

- Added a new config value to determine the # of attempts per Bracket of generation (Default 25).

- The safeguard dungeon has been improved:
-Size is now clamped between 1x and 2.2x.
-The currentDungeonType is now updated.
-The ambience is now updated.
-The seed is updated again.

- If your config is very silly and you pass a negative size dungeon, it will become 1x (don't be silly tho).

- The final # of attempts is logged in the same message as the final size after generating.

- The random seed is now -1 to be randomized. **UPDATE THIS VALUE ON OLDER FILES *NOW***

- LLL now renames Level3Flow to Mineshaft so CC doesn't anymore.

## 0.10.7
- Fixed the interior not being set correctly (resulting in it always being the facility).
Now the appropiate interior is set.

- Renamed 'Level3Flow' to 'Mineshaft', this is just a name and should have zero effect on code. I have OCD.

## 0.10.6
- Bumped LLL.

- Updated dungeon generation code, should work on v60 now.

## 0.10.5
- Added new Misc setting to update the clock when time moves as opposed to every 3 seconds.

- Separated time settings from the lock/unlock/hide/unhide settings (now these can be used alongside Selene's or other Moon Unlock mods).

- Updated the lengthOfHours before landing on the moon you are on, this keeps enemy spawn curves accurate to the in-game time.

- Added a clamp for the enemy spawn functions to prevent a min value from exceeding a max value when many enemies are due to spawn.

## 0.10.0
- Added a new setting under ~Misc~ to keep orphaned entries, this means that the config won't 'clean' itself but this can prevent accidental entry deletion due to updates or changing other settings (but will make the config more crowded and messy). I would still recommend to just keep a copy of the file or paste the text version in a safe spot.

- Added a new setting under ~Misc~ to let you choose the seed for Day 1, it is not used very much but is nice for debugging. Leave it at 0 to have it be randomized.

- Added a new setting for dungeon sizes, `MapTileSize` is tied to the dungeon and is divided from the moon size multiplier BEFORE clamps are applied. (Useful for balancing interiors of different '1x' sizes with one another).

- Fixed an issue where the dungeon generation seed was not updating correctly during regeneration. This resolves certain interiors not working with the safeguards (as it was basically the same failed attempt all 200 times). All interiors should function with the safeguard just fine now. Still keep me updated if you hit a safeguard loop/the selected dungeon fails to load please.

- Tweaked the code pathing and logging for the dungeon methods, should be cleaner and more informational now.

- The default values for the `Dungeon Size Scaler` setting are now inversed. This doesn't affect how it functions but means most interiors now start with `0` instead of `100` meaning they ignore the clamp by default. LLL's setting works the opposite way so I adjusted for that.

- Dungeons with a min or max size of `0x` now have the values set to `1x` and `2x` respectively to avoid crashing while using the default config.

- Fixed issue where the safeguard facility reference was taking all three facilities (and therefore using the ExtraLarge version,) now only the standard facility is saved.

- When using AnimationCurve based settings such as `Scale Enemy Spawn Rate?`, the new curve is created from a minimum of 10 keyframes from the first, this further keeps the shape of the original curve when creating the new one. 

## 0.9.8
- Fixed issue where list based things would get offset after being sorted to be alphabetical. The mod now uses LINQ OrderBy instead of Sort and is still alphabetical.

- The interior desync issues appear to be fixed, tested it a few times in multiplier with PureFPSZac and had no desyncs. Keep me updated if it desyncs again.

- Manually added support for enemy/scrap injection for the heatwave weather, I promise myself I will use better weather code (mrov) but not rn since I am eeby.

## 0.9.7
- Fixed issue where enemies wouldn't spawn if `Remove Duplicates?` was true but `Only Keep Zeros?` was false.

## 0.9.6
- Fixed big list enemies not applying correctly.

- Fixed null ref from not using scan nodes.

- Checked compact for LLL 1.3.5, the new LLL should have fixed the enemy spawning issue.

## 0.9.5
- Added the much requested Big Lists, with them you can set all enemy spawn lists from just three strings.

- If you are adding stuff by tag, now under each tag in the config is an entry showing which moons currently possess that tag. Changing this setting does not affect the game, it is just to give you an idea of how used the tag is and where.

- Updated the default tag list to instead be a whitelist of 20 of the most common tags, as most moon tags are unique to one or two moons and take up unnecessary space in the file.

- Tag names are now normalized to be all lowercase letters without special characters or spaces to prevent issues caused by typos on moons.

- Added an option to keep the lowest rarity of an enemy in a spawn pool instead of the highest rarity when removing duplicates.

- Added an option to ignore other rarities for an enemy if one entry has a rarity of 0, this allows uses EnemyName:0 under dungeon/weather/tag injections to make them unable to spawn under those conditions (e.g., Flowerman:0 in `Haunted Mansion (2) - Add Interior Enemies` prevents brackens from spawning inside when the current dungeon is the haunted mansion).

- Added a WIP(?) function to hide the ship/entrance/fire exit scan nodes while inside, leaving the logs in for now.

- Dungeon matching properties are now cleared before applying settings from selection overrides. This ensures that only the dungeon selection settings from CC are used (e.g., No uncontrollable hard coded injection by tag).

- The default values for various string-based config entries are now alphabetical. This includes moon, enemy, scrap, and tag lists.

- Moved the settings `Free Them?` and `Scale Enemy Spawn Rate?` to \_Enemies\_

- The default time speed has been adjusted to 1.4x to match the vanilla game.

- Fixed a divide by zero error occurring on moons with a max enemy power count of 0 when the scale enemy spawn rate is enabled.

- Resolved a missing dictionary key issue for blacklisted moons caused by time settings.

- Black Mesa has been blacklisted from the generation settings (Looking for help from interior devs to understand why some have this issue while most do not).

- Updated to the most recent LL and LLL (no changes were needed on my end). :3 

## 0.9.1
- Fixed default dungeon catch not loading.

- The Tomb and SCP interiors are now also skipped in the shrinking process.

- Changed how the ship and main entrance scan nodes are found, should now support ~95% of modded moons.

- Cleaned up config typos again.

## 0.9.0
- Added a new misc setting, scan node extentions!
This includes min/max scan distance on the ship and main entrance. It also adds subtext to the main entrance scan node if it has none.
You can also set the min/max scan distance for fire exits but you must have ScannableFireExit by TestAccount666 installed (or else it won't have a scan node to begin with).
The sphere cast distance for the scan is set to your highest max scan distance setting.

## 0.8.9
- Dungeon Generation now uses the intended, synced, and randomized seed (previously DunGen was providing a random seed of its own (that was not synced too) and my seed wasn't used or accessible).

- Blacklisted Rosie's Secret unlock moons from the isLocked config option.

- Fixed some config typos oopsies.

## 0.8.8
- If size overrides are false, it finishes the method by calling Generate() and the associated log has been specified.

- Now the dungeon attempts to generate 20 (configurable) times with the original input multiplier before changing it for further attempts.
This is because sometimes a multiplier of 20x does work but may take until the 11th try to generate correctly.

- After generating successfully, the log stating it has been loaded now contains the final size multiplier that it loaded with.

- Adding clarification to the log that is printed when the safeguard dungeon is loaded.

## 0.8.7
- Moved some checks so that dungeon size overrides are applied regardless of if Dungeon Generation Safeguards are true or false.

## 0.8.6
- Fixed dungeon seed not being randomized.

- Use a try-catch loop and send an exception when the dungeon generation fails.

- On total failure, I set the new dungeon to a saved reference of the facility instead of the 1st entry in the levels index (which may not be the facility).

- Had to blacklist the bunker from the modified innergenerate loop due to the rats refusing to cooperate.

## 0.8.5
- Completely reworked the dungeon generation change setting, refer to readme.

- Added enemy/scrap injection by current dungeon.

- Reorganized and streamlined the injections from weather, tags, and dungeons. They run in that order and always replace enemies before adding new ones.

- Rewrote some config entries to be more consistent and comprehensible.

- Added checks to all injections to prevent missing dictionary key errors when blacklisting stuff.

## 0.8.0
- Added weather settings!! This allows you to add/replace enemies (day, night, inside), add scrap, multiply the scrap count, and multiply the scrap values when a moon has this weather.

- Added replacing enemies by tag. So instead of say adding in say DriftWoodGiant:100, you can do ForestGiant:DriftWoodGiant and that would instead remove the forest giant and input the DriftWoodGiant with the same rarity.

- Added setting under \_Enemies\_ that when true, removes duplicate enemies from list and keeps the highest. For example after tags and/or weather all add the same enemy you will only have one entry for it. If the list has Flowerman:69,Flowerman:34,Flowerman:75,Flowerman:2 then only Flowerman:75 will be kept.

- Changed the moon specific scrapValueMultiplier to be *= so it stacks with other modifiers for the value.

- Moon tags are now registered if either adding enemies or scrap by tag are enabled.

- Fixed misc typos.

- Read through all the loggings and only now have the ones I think are important, people can understand, and aren't super wordy.

## 0.7.5
- Adding setting that if enabled, renames Celest to Celeste.

This allows it to be distinguished from Celestria correctly in the config.

## 0.7.1
- After the tag injections are put in, duplicates are removed and the higher rarity entry is kept

## 0.7.0
- Added tag configs, allows you to add enemies and/scrap to any moons that have that tag.

- Added dungeon injection by tag under the dungeon selection config

- Added two new moon settings under moons: 
- Free Them increases the enemy caps from 20 to 127 and makes interior enemies spawn on the hour instead of only every other hour.
- Scale Enemy Spawn Rate essentially makes the speed at which enemies spawn be multiplied by (configEnemy Power/originalEnemy Power) (for all three groups of enemies).

## 0.6.6
- Visiting the company early no longer spents a day.

- Removed the verbose dev logging I accidentally left in.

## 0.6.5
- The day is now moved even if the currrent level is timeless.

## 0.6.4
- Finally fixed time, now I count how much timeuntildeadline has been decreased during the day and subtract the remainder of the day at the end. This works as long as other mods do not change the actual length of hours or number of hours.

## 0.6.3
- Time fixes are not applied if misc moon settings are disabled

## 0.6.2
- Rewrote time entirely, the timeuntildeadline is no longer undated every second but is instead reduced by 1 in game day worth of time when the day passes to the next

## 0.6.1
- Fixed time again I hope

## 0.6.0
- Added fines under the misc config tab:
You can set the fine amount and the fine reduction for bringing back bodies
The company building has its own values for those
There is also a setting that if true, makes it so that the fine amount resembles 100% death, if only half of the players die, it will be half of the fine amount (then reduced further by the insured reduction).

## 0.5.8
- The bunker is now granted retries as well. This *should* be fixed on their end soon as it has to do with the RATS.

## 0.5.7
- The time fix is now always enabled, this should fix the Quantum Warp upgrade from LGU and the same setting in LLL.

- CozyOffice and Black Mesa are granted retries as they do not always load correctly first attempt.

## 0.5.6
- Hopefully fixed time this time?
It it now decreased a second a second, if WaitForShipToLand is true for the moon, it doesn't decrease for the first 20 seconds (enough time for the ship to land) 

## 0.5.5
- Moved the processes from the start match lever to the hanger door, this fixes some incompats.

- Sector-0 has 10 attempts to load now, this should almost guarantee loading success.

- Refactored the config cleaner, now it accurately checks if it is ready to clean.

- Config Aider is now a singleton

## 0.5.4
- Weather is now randomized on the first day

- Unsure if the dungeon was randomized the first day but it definitely will be now.

## 0.5.3
- Fixed weather desync.

- Default weather list no longer has None duplicated sometimes.

- Fixed interior seed desync, (and made the interior selection safer just in case).

- Apparatus should be fixed as a result (think that is a side effect of the interior seed desync).

## 0.5.2
- When grabbing the default values for lists, entries with a rarity of 0 are omitted.

- Cleaned up prefixes to immediately return the original method if the related setting is false.

- Rewrote the whole MoveGlobalTime method again so that time should move the same still but timeUntilDeadline should now be calculated the same as vanilla (just not time speed multiplier dependent).

- The scrap value multiplier is divided by 2.5 to make it correctly 1:1 with vanilla values.

## 0.5.1
- Added the mod oops

## 0.5.0
- First Version