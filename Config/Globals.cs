using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace Bot_Quota_GoldKingZ;

public class Globals
{
    // Timers
    public CounterStrikeSharp.API.Modules.Timers.Timer? BotSpawnTimer;
    
    // Wave mode state
    public bool isWaveModeActive = false;
    public int currentWaveBotCount = 1;
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
}