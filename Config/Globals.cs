using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace Bot_Quota_GoldKingZ;

public class Globals
{
    public CounterStrikeSharp.API.Modules.Timers.Timer? BotCheckTimer;
    public CounterStrikeSharp.API.Modules.Timers.Timer? BotSpawnTimer;
    public bool isWaveModeActive = false;
    public int currentWaveBotCount = 1;
    public int targetBotCount = 0;
    public int spawnCheckCount = 0;
    public bool firstWaveNotStarted = false; // True until first wave actually begins
    
    // Auto-respawn system (using mp_respawn_on_death_ct)
    public bool autoRespawnEnabled = false;
    public int respawnsNeeded = 0;
    public int respawnsUsed = 0;
    
    // Store original server cvar values to restore later
    public Dictionary<string, string> savedCvars = new Dictionary<string, string>();
    
    // Team assignments
    public CsTeam humanTeam = CsTeam.Terrorist;
    public CsTeam botTeam = CsTeam.CounterTerrorist;
    
    // Track plugin-added bots
    public HashSet<ulong> pluginBots = new HashSet<ulong>();
}