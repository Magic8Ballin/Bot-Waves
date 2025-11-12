# [CS2] Bot Wave Survival (2.0.1)

A cooperative survival mode where 1-4 players fight increasingly difficult waves of bots.

![bot-quote](https://github.com/user-attachments/assets/c88a8ba3-dfaf-4265-9e22-1a4174370d8d)

---

## 🎮 Features

- **Dynamic Wave Scaling**: Wave difficulty scales with team size
  - Minimum increment per wave: configurable (default +1)
  - Actual increment: Max(MinimumWaveIncrement, Number of Players)
  - Example: 3 players will increment by 3 bots per wave
- **Dynamic Round Time**: Round time automatically adjusts based on wave number
  - Configurable base time (default: 115 seconds)
  - Configurable increment per bot after threshold (default: +3s per bot after wave 10)
  - Example: Wave 20 with 10+ threshold = 115s + (10 bots × 3s) = 145 seconds
- **Auto-Respawn System**: When map spawn limits are hit, bots will respawn to reach the target wave count
- **Admin Override**: Password system to allow unlimited players
  - Use `!wave [number] [password]` to start with 5+ players
  - Once override is used, unlimited players can join without disabling wave mode
  - Override persists until wave mode is manually disabled or map changes
- **Auto-Disable on Population**: When a 5th player joins during normal mode (no override), wave mode automatically disables with a thank you message
- **Helpful Command Guide**: Automatic !wave command instructions shown at the start of every round when 1-4 players are present
- **Team Management**: Humans automatically placed on T side, bots on CT side
- **Server Protection**: Optional saving and restoration of server cvars when wave mode is enabled/disabled
- **Comprehensive Configuration**: Over 20 configurable settings for wave mode, round times, bot spawning, messages, and debugging

---

## 📦 Dependencies
[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-2.x-2d2d2d?logo=sourceengine)](https://www.sourcemm.net/downloads.php?branch=dev)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

---

## 📥 Installation

1. Install Metamod:Source and CounterStrikeSharp
2. Download latest release
3. Extract to `addons/counterstrikesharp/plugins/BotWaves/` directory
4. Configure `addons/counterstrikesharp/configs/plugins/BotWaves/BotWaves.json`
5. Restart server or load plugin with `css_plugins load BotWaves`

---

## 🎯 Commands

| Command | Description | Usage |
|---------|-------------|-------|
| `!wave` or `!wave 1` | Start wave mode at wave 1 | `!wave` |
| `!wave <number>` | Start wave mode at specific wave | `!wave 10` |
| `!wave <number> <password>` | Start wave mode with admin override for 5+ players | `!wave 10 yourpassword` |
| `!wave off` | Disable wave mode | `!wave off` |
| `!wave disable` | Disable wave mode (alternative) | `!wave disable` |

> [!NOTE]
> All commands are CLIENT_ONLY and must be used by a player in-game.

---

## ⚙️ Configuration

> [!NOTE]
> Located in `addons/counterstrikesharp/configs/plugins/BotWaves/BotWaves.json`

### Wave Mode Settings

| Property | Description | Default | Type |
|----------|-------------|---------|------|
| `MaxPlayersWithoutPassword` | Maximum players allowed without admin password | `4` | Integer |
| `AdminPasswordOverride` | Password to bypass player limit | `"glove"` | String |
| `DisableWaveOnFifthPlayer` | Auto-disable wave mode when 5th player joins (without override) | `true` | Boolean |

### Round Time Settings

| Property | Description | Default | Type |
|----------|-------------|---------|------|
| `EnableDynamicRoundTime` | Enable automatic round time adjustment based on wave | `true` | Boolean |
| `BaseRoundTimeSeconds` | Base round time in seconds | `115` | Integer |
| `RoundTimeIncrementPerBot` | Additional seconds per bot after threshold | `3` | Integer |
| `WaveThresholdForTimeIncrease` | Wave number where time increment starts | `10` | Integer |

### Bot Spawn Settings

| Property | Description | Default | Type |
|----------|-------------|---------|------|
| `SpawnLimitCheckDelay` | Delay in seconds before checking spawn limits | `2.0` | Float |

### Wave Scaling Settings

| Property | Description | Default | Type |
|----------|-------------|---------|------|
| `EnableDynamicScaling` | Enable dynamic wave scaling (currently unused) | `true` | Boolean |
| `MinimumWaveIncrement` | Minimum bots to add per wave victory | `1` | Integer |

### Respawn System Settings

| Property | Description | Default | Type |
|----------|-------------|---------|------|
| `EnableAutoRespawn` | Enable bot auto-respawn when spawn limit is hit | `true` | Boolean |
| `ShowRespawnMessages` | Show respawn countdown messages in chat | `true` | Boolean |
| `ShowRespawnEveryXDeaths` | Show respawn message every X bot deaths | `5` | Integer |

### Chat Message Settings

| Property | Description | Default | Type |
|----------|-------------|---------|------|
| `ShowWaveStartMessages` | Show wave start messages in chat | `true` | Boolean |
| `ShowWaveEndMessages` | Show wave end (win/loss) messages in chat | `true` | Boolean |
| `ShowHelpMessages` | Show !wave command help at round start (1-4 players) | `true` | Boolean |

### Server Protection Settings

| Property | Description | Default | Type |
|----------|-------------|---------|------|
| `SaveServerCvars` | Save server cvars when wave mode is enabled | `true` | Boolean |
| `RestoreCvarsOnDisable` | Restore saved cvars when wave mode is disabled | `true` | Boolean |

### Debug Settings

| Property | Description | Default | Type |
|----------|-------------|---------|------|
| `EnableDebugMode` | Enable detailed debug logging | `false` | Boolean |
| `LogBotSpawns` | Log bot spawn events | `false` | Boolean |
| `LogTeamChanges` | Log team assignment changes | `false` | Boolean |
| `LogRoundEvents` | Log round start/end events | `false` | Boolean |

---

## 🌍 Language

> [!NOTE]
> Located in `addons/counterstrikesharp/plugins/BotWaves/lang/en.json`

<details>
<summary>🖼️ Preview Colors In Game (Click to expand 🔽)</summary>

![Color Preview](https://github.com/oqyh/cs2-Game-Manager/assets/48490385/3df7caa9-34a7-47da-94aa-8d682f59e85d)
</details>

```json
{
	//==========================
	//        Colors
	//==========================
	//{Yellow} {Gold} {Silver} {Blue} {DarkBlue} {BlueGrey} {Magenta} {LightRed}
	//{LightBlue} {Olive} {Lime} {Red} {Purple} {Grey}
	//{Default} {White} {Darkred} {Green} {LightYellow}
	//==========================
	//        Placeholders
	//==========================
	//{nextline} = Print on next line
	//{0}, {1}, etc. = Dynamic values (wave number, bot count, etc.)
	//==========================

	"Wave.TurnedOff": " {green}[Bot Wave]{default} Turned off.",
	"Wave.PleaseUseNumber": " {green}[Bot Wave]{default} Please use a number.",
	"Wave.SpecialCodeAccepted": " {green}[Bot Wave]{default} Special code accepted!",
	"Wave.OnlyFourPlayers": " {green}[Bot Wave]{default} Only 1-4 players can play.",
	"Wave.StartingWithOverride": " {green}[Bot Wave]{default} Starting at wave {0} with {1} players!",
	"Wave.StartingAtWave": " {green}[Bot Wave]{default} Starting at wave {0}!",
	"Wave.FifthPlayerJoined": " {green}[Bot Wave]{default} A 5th player joined the server. Bot Waves disabled.{nextline} {lime}Thanks for helping populate the server! Enjoy the regular AWP gameplay.",
	
	"Wave.HelpStart": " {green}[Bot Wave]{default} Type {lime}!wave{default} to start wave mode!",
	"Wave.HelpCustomWave": " {green}[Bot Wave]{default} Use {lime}!wave <number>{default} to start at a specific wave (e.g., !wave 10)",
	"Wave.HelpTurnOff": " {green}[Bot Wave]{default} Use {lime}!wave off{default} to disable wave mode",
	
	"Wave.YouWonNext": " {green}[Bot Wave]{default} You won! Next: Wave {0}",
	"Wave.YouLostTryAgain": " {green}[Bot Wave]{default} You lost. Try wave {0} again!",

	"Wave.FightBots": " {green}[Bot Wave]{default} Wave {0} - Fight {1} bots!",
	"Wave.SpawnLimitReached": " {green}[Bot Wave]{default} Only {0} bots fit on map. {1} will re-spawn!",
	"Wave.NoMoreRespawns": " {green}[Bot Wave]{default} No more bot re-spawns! Kill the rest!",
	"Wave.RespawnsLeft": " {green}[Bot Wave]{default} Bot re-spawns left: {0}"
}
```

---

## 🎯 How It Works

### Wave Progression
1. Players start a wave using `!wave` or `!wave <number>`
2. Humans are placed on T side, bots spawn on CT side
3. Win condition: Eliminate all bots
4. Loss condition: All humans die
5. **Victory**: Next wave = Current wave + Max(MinimumWaveIncrement, Player Count)
6. **Defeat**: Retry the same wave

### Example Wave Progression (3 Players, Min Increment = 1)
- Wave 1: 1 bot
- Wave 2: 4 bots (1 + 3)
- Wave 3: 7 bots (4 + 3)
- Wave 4: 10 bots (7 + 3)

### Spawn Limit System
When a map's bot limit is reached:
1. Only partial bots spawn initially
2. Auto-respawn is enabled for CT bots
3. As bots die, they respawn until the wave target is met
4. Chat messages inform players of remaining respawns

### Player Limit Override
- Default: 1-4 players only
- Admin can use password to allow 5+ players
- Password persists for the entire wave mode session
- Players can join/leave freely once override is active

---

## 📜 Changelog

<details>
<summary>📋 View Version History (Click to expand 🔽)</summary>

### [2.0.1]
- **Update**: Help message feature now implemented
- **Update**: Dynamic round time system fully functional
- **Update**: Auto-respawn system for spawn limit handling
- **Update**: Comprehensive configuration system
- **Update**: Server cvar protection (save/restore)
- **Fix**: Wave increment now properly uses stored player count from round start

### [2.0.0]
- **Major Update**: Complete wave system rework
- Implemented dynamic wave scaling based on player count
- Added mid-round join protection
- Introduced auto-respawn system for bots
- Admin override password for 5+ players
- Team management (T vs CT)

### [1.0.4]
- Fix Cfg Flood

### [1.0.3]
- Fix Some Bugs
- Removed CheckPlayersByTimer
- Added configuration documentation

### [1.0.2]
- Fix Bug Counting
- Added Debug Information

### [1.0.1]
- Fix Bug

### [1.0.0]
- Initial Release

</details>

---

## 🐛 Troubleshooting

### Bots not spawning
- Check console for spawn errors
- Enable `LogBotSpawns` in config
- Verify map supports bot navigation

### Players on wrong team
- Enable `LogTeamChanges` in config
- Check if another plugin is interfering with team assignments

### Round time not changing
- Verify `EnableDynamicRoundTime` is `true`
- Check `WaveThresholdForTimeIncrease` setting
- Enable `LogRoundEvents` for diagnostics

### Wave increment issues
- Check `MinimumWaveIncrement` value
- Enable `EnableDebugMode` to see increment calculations in console

---

## 📝 Credits

**Author**: Gold KingZ  
**Version**: 2.0.1  
**Plugin Type**: CounterStrikeSharp Plugin  

---
