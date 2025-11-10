using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Timers;
using Bot_Quota_GoldKingZ;

namespace Bot_Quota_GoldKingZ;

[MinimumApiVersion(80)]
public class BotQuotaGoldKingZ : BasePlugin
{
    public override string ModuleName => "Bot Wave";
    public override string ModuleVersion => "2.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "Bot Wave survival mode for 1-4 players";
    
    public static BotQuotaGoldKingZ? Instance { get; private set; }
    public Globals g_Main = new();

    public override void Load(bool hotReload)
    {
        try
        {
            Console.WriteLine("========================================");
            Console.WriteLine("[Bot Wave] Loading plugin...");
       Console.WriteLine("========================================");
   
       Instance = this;
      
      // Register listeners for map events
 RegisterListener<Listeners.OnMapStart>(OnMapStart);
  RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
            
      // Register event handlers
 RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
     RegisterEventHandler<EventRoundStart>(OnRoundStart);
  RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
          RegisterEventHandler<EventPlayerTeam>(OnPlayerTeam);
      RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
      RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
  
       Console.WriteLine("[Bot Wave] Plugin loaded successfully!");
      Console.WriteLine("========================================");
        }
        catch (Exception ex)
        {
        Console.WriteLine($"[Bot Wave] ERROR during load: {ex.Message}");
    throw;
      }
    }

    public override void Unload(bool hotReload)
    {
        Console.WriteLine("[Bot Wave] Plugin unloading...");
  if (g_Main.isWaveModeActive)
      {
          DisableWaveMode();
      }
        Instance = null;
    }

    private void OnMapStart(string mapName)
    {
   Console.WriteLine($"[Bot Wave] Map started: {mapName}");
        // Reset wave mode on map change
        if (g_Main.isWaveModeActive)
        {
     Console.WriteLine("[Bot Wave] Map changed - disabling wave mode");
            g_Main.isWaveModeActive = false;
        g_Main.currentWaveBotCount = 1;
     }
    }

    private void OnMapEnd()
    {
 Console.WriteLine("[Bot Wave] Map ending");
        // Clean up wave mode
    if (g_Main.isWaveModeActive)
{
            g_Main.isWaveModeActive = false;
            g_Main.currentWaveBotCount = 1;
   }
    }

    [ConsoleCommand("css_wave", "Enable/disable Bot Wave mode")]
    [CommandHelper(minArgs: 0, usage: "[number|off|disable] [password]", whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnWaveCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null || !player.IsValid) return;

  try
  {
            string arg = commandInfo.ArgCount > 1 ? commandInfo.GetArg(1).ToLower() : "1";

 if (arg == "off" || arg == "disable" || arg == "0")
     {
DisableWaveMode();
player.PrintToChat(Localizer["Wave.TurnedOff"]);
  return;
     }

  if (!int.TryParse(arg, out int waveNumber) || waveNumber <= 0)
     {
     player.PrintToChat(Localizer["Wave.PleaseUseNumber"]);
  return;
      }

    int humanPlayerCount = GetHumanPlayerCount();
     
    // Check for password override
   bool hasOverride = false;
      if (commandInfo.ArgCount > 2)
  {
   string password = commandInfo.GetArg(2);
      if (password == "glove")
      {
    hasOverride = true;
  Console.WriteLine($"[Bot Wave] {player.PlayerName} used password override for player limit");
 player.PrintToChat(Localizer["Wave.SpecialCodeAccepted"]);
}
  }
        
  if (humanPlayerCount > 4 && !hasOverride)
       {
 player.PrintToChat(Localizer["Wave.OnlyFourPlayers"]);
     return;
     }

  EnableWaveMode(waveNumber);
       
    if (hasOverride && humanPlayerCount > 4)
    {
   Server.PrintToChatAll(Localizer["Wave.StartingWithOverride", waveNumber, humanPlayerCount]);
       }
 else
     {
    Server.PrintToChatAll(Localizer["Wave.StartingAtWave", waveNumber]);
   }
}
    catch (Exception ex)
 {
  Console.WriteLine($"[Bot Wave] Error in wave command: {ex.Message}");
      }
    }

    [ConsoleCommand("css_addbot", "Add a bot to CT team (testing)")]
    [CommandHelper(minArgs: 0, usage: "", whoCanExecute: CommandUsage.CLIENT_ONLY)]
    public void OnAddBotCommand(CCSPlayerController? player, CommandInfo commandInfo)
    {
     if (player == null || !player.IsValid) return;

        try
        {
Console.WriteLine($"[Bot Wave] {player.PlayerName} used !addbot command");
 
 // Add one bot to CT team
            string teamCmd = g_Main.botTeam == CsTeam.Terrorist ? "t" : "ct";
  Server.ExecuteCommand($"bot_add_{teamCmd}");
     
   player.PrintToChat($" [Bot Wave] Adding 1 bot to {teamCmd.ToUpper()} team...");
         Console.WriteLine($"[Bot Wave] Executed: bot_add_{teamCmd}");
    
  // Wait a moment for the bot to be added, then force respawn
       AddTimer(0.2f, () =>
     {
            try
      {
   // Find the newest bot (just added)
         var bots = Utilities.GetPlayers()
                   .Where(p => p != null && p.IsValid && p.IsBot && !p.IsHLTV && p.Team == g_Main.botTeam)
      .OrderByDescending(p => p.UserId) // Newest bot has highest UserId
      .ToList();
         
if (bots.Count > 0)
         {
   var newBot = bots.First();
       Console.WriteLine($"[Bot Wave] Found new bot: {newBot.PlayerName}, attempting to respawn");
        
  // Force bot to respawn
    if (newBot.PlayerPawn != null && newBot.PlayerPawn.IsValid)
         {
    // Respawn the bot
           newBot.Respawn();
      Console.WriteLine($"[Bot Wave] Bot {newBot.PlayerName} respawned!");
       player.PrintToChat($" [Bot Wave] Bot spawned on map!");
  }
          else
         {
          Console.WriteLine($"[Bot Wave] Bot pawn not valid, trying alternate method");
           // Try using server command to force respawn
          Server.ExecuteCommand($"mp_respawn_on_death_ct 1");
      AddTimer(0.1f, () => {
          Server.ExecuteCommand($"mp_respawn_on_death_ct 0");
      });
            }
               }
   else
          {
      Console.WriteLine($"[Bot Wave] No bots found to respawn");
         }
       
     // Count total bots
       var botCount = Utilities.GetPlayers().Count(p => p != null && p.IsValid && p.IsBot && !p.IsHLTV);
         Console.WriteLine($"[Bot Wave] Total bots in server: {botCount}");
      }
      catch (Exception ex)
           {
  Console.WriteLine($"[Bot Wave] Error in respawn timer: {ex.Message}");
    }
   });
   }
     catch (Exception ex)
        {
    Console.WriteLine($"[Bot Wave] Error in addbot command: {ex.Message}");
   player.PrintToChat(" [Bot Wave] Error adding bot!");
        }
    }

    private void EnableWaveMode(int startWave)
    {
  Console.WriteLine($"[Bot Wave] Enabling wave mode, starting at wave {startWave}");
        Console.WriteLine($"[Bot Wave] Human team: {g_Main.humanTeam}, Bot team: {g_Main.botTeam}");
     
        g_Main.isWaveModeActive = true;
        g_Main.currentWaveBotCount = startWave;
      g_Main.firstWaveNotStarted = true;
 
        // Save current server cvar values
        SaveServerCvar("mp_autoteambalance");
   SaveServerCvar("mp_limitteams");
        SaveServerCvar("mp_teambalance_enabled");
  SaveServerCvar("mp_force_pick_time");
      
        Console.WriteLine($"[Bot Wave] Saved {g_Main.savedCvars.Count} cvar values");
   
        // Disable all auto-balancing mechanisms
        Server.ExecuteCommand("mp_autoteambalance 0");
     Server.ExecuteCommand("mp_limitteams 0");
     Server.ExecuteCommand("mp_teambalance_enabled 0");
        Server.ExecuteCommand("mp_force_pick_time 0");
        
        // Kick all existing bots
      Console.WriteLine("[Bot Wave] Kicking all existing bots");
      Server.ExecuteCommand("bot_kick");
   
      // Move all humans to T side IMMEDIATELY (before restart) - but keep spectators in spec
        Console.WriteLine("[Bot Wave] Moving all players to T side");
   var allPlayers = Utilities.GetPlayers().Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV).ToList();
        foreach (var player in allPlayers)
        {
    // Skip players in spectator - let them stay there
     if (player.Team == CsTeam.Spectator)
            {
           Console.WriteLine($"[Bot Wave] Keeping {player.PlayerName} in spectator");
     continue;
            }
          
    if (player.Team != g_Main.humanTeam)
    {
            player.ChangeTeam(g_Main.humanTeam);
     Console.WriteLine($"[Bot Wave] Moved {player.PlayerName} to {g_Main.humanTeam}");
        }
        }
        
        // Now restart the game - players are already on correct team
Console.WriteLine("[Bot Wave] Restarting game to begin wave mode");
      Server.ExecuteCommand("mp_restartgame 1");
    }

    private void DisableWaveMode()
    {
  Console.WriteLine("[Bot Wave] Disabling wave mode");
        
        g_Main.isWaveModeActive = false;
 g_Main.currentWaveBotCount = 1;
        g_Main.targetBotCount = 0;
  g_Main.autoRespawnEnabled = false;
    g_Main.respawnsNeeded = 0;
        g_Main.respawnsUsed = 0;
  g_Main.firstWaveNotStarted = false;
    
 // Kill any active timers
    g_Main.BotSpawnTimer?.Kill();
        g_Main.BotSpawnTimer = null;

     // Disable auto-respawn if it was on
Server.ExecuteCommand("mp_respawn_on_death_ct 0");
        Server.ExecuteCommand("mp_respawn_on_death_t 0");
 
        // Kick all bots using multiple methods to ensure they're removed
var bots = Utilities.GetPlayers().Where(p => p != null && p.IsValid && p.IsBot && !p.IsHLTV).ToList();
        Console.WriteLine($"[Bot Wave] Found {bots.Count} bots to kick");
      
   foreach (var bot in bots)
        {
 if (bot != null && bot.IsValid && bot.IsBot)
  {
   Console.WriteLine($"[Bot Wave] Kicking bot: {bot.PlayerName}");
     Server.ExecuteCommand($"bot_kick {bot.PlayerName}");
  }
  }
    
    // Also use bot_kick all as backup
Server.ExecuteCommand("bot_kick");
  
// Restore all saved cvar values
 RestoreAllCvars();

 Console.WriteLine("[Bot Wave] Wave mode disabled");
    }

    private void SaveServerCvar(string cvarName)
    {
        try
  {
            // Store common default values for these cvars
   // Since we can't easily read current values, we'll use standard defaults
       string defaultValue = cvarName switch
    {
       "mp_autoteambalance" => "1",
    "mp_limitteams" => "2",
     "mp_teambalance_enabled" => "1",
 "mp_force_pick_time" => "15",
   _ => "1"
  };
     
       g_Main.savedCvars[cvarName] = defaultValue;
       Console.WriteLine($"[Bot Wave] Will restore {cvarName} to default: {defaultValue}");
        }
      catch (Exception ex)
        {
 Console.WriteLine($"[Bot Wave] Error saving cvar {cvarName}: {ex.Message}");
   }
    }

    private void RestoreAllCvars()
    {
    Console.WriteLine($"[Bot Wave] Restoring {g_Main.savedCvars.Count} saved cvars");
    
      foreach (var kvp in g_Main.savedCvars)
        {
            try
            {
   Server.ExecuteCommand($"{kvp.Key} {kvp.Value}");
 Console.WriteLine($"[Bot Wave] Restored {kvp.Key} = {kvp.Value}");
         }
   catch (Exception ex)
            {
   Console.WriteLine($"[Bot Wave] Error restoring {kvp.Key}: {ex.Message}");
    }
        }
        
        // Clear the saved cvars dictionary
    g_Main.savedCvars.Clear();
    }

    private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        try
 {
  var player = @event.Userid;
if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)
       return HookResult.Continue;

     Server.NextFrame(() =>
{
     try
    {
if (!player.IsValid) return;
    
    int humanCount = GetHumanPlayerCount();
  
     if (!g_Main.isWaveModeActive && humanCount >= 1 && humanCount <= 4)
  {
   player.PrintToChat(Localizer["Wave.TypeToStart"]);
     }

 if (g_Main.isWaveModeActive && humanCount > 4)
         {
   Server.PrintToChatAll(Localizer["Wave.TooManyPlayers"]);
  DisableWaveMode();
 }
    }
     catch (Exception ex)
  {
        Console.WriteLine($"[Bot Wave] Error in player connect: {ex.Message}");
       }
          });
        }
   catch (Exception ex)
    {
   Console.WriteLine($"[Bot Wave] Error in OnPlayerConnectFull: {ex.Message}");
    }
      
        return HookResult.Continue;
    }

    private HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
    {
        try
        {
       if (!g_Main.isWaveModeActive) return HookResult.Continue;

   var player = @event.Userid;
            if (player == null || !player.IsValid || player.IsHLTV)
          return HookResult.Continue;

          int newTeam = @event.Team;
      int humanTeamNum = (int)g_Main.humanTeam;
      int botTeamNum = (int)g_Main.botTeam;

      // Handle human players - force to T side
    if (!player.IsBot)
       {
     if (newTeam != humanTeamNum && newTeam != (int)CsTeam.None && newTeam != (int)CsTeam.Spectator)
       {
          Console.WriteLine($"[Bot Wave] Redirecting human {player.PlayerName} from team {newTeam} to team {humanTeamNum}");
    
         Server.NextFrame(() =>
  {
       if (player.IsValid && !player.IsBot)
      {
         player.ChangeTeam(g_Main.humanTeam);
        }
     });
   }
            }
         // Handle bots - force to CT side
     else if (player.IsBot)
     {
    if (newTeam != botTeamNum && newTeam != (int)CsTeam.None)
    {
 Console.WriteLine($"[Bot Wave] Redirecting bot {player.PlayerName} from team {newTeam} to team {botTeamNum}");
          
     Server.NextFrame(() =>
{
       if (player.IsValid && player.IsBot)
       {
            player.ChangeTeam(g_Main.botTeam);
      }
  });
    }
    }
        }
  catch (Exception ex)
        {
    Console.WriteLine($"[Bot Wave] Error in OnPlayerTeam: {ex.Message}");
     }

   return HookResult.Continue;
    }

    private HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
 try
      {
       if (!g_Main.isWaveModeActive) return HookResult.Continue;

 var player = @event.Userid;
     if (player == null || !player.IsValid || player.IsBot || player.IsHLTV)
      return HookResult.Continue;

// Ensure human players are on correct team when they spawn
if (player.Team != g_Main.humanTeam)
     {
    Server.NextFrame(() =>
       {
    if (player.IsValid && !player.IsBot)
  {
 Console.WriteLine($"[Bot Wave] Correcting {player.PlayerName} team on spawn");
    player.ChangeTeam(g_Main.humanTeam);
}
    });
   }
}
  catch (Exception ex)
   {
 Console.WriteLine($"[Bot Wave] Error in OnPlayerSpawn: {ex.Message}");
   }

      return HookResult.Continue;
 }

    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
 try
 {
 if (!g_Main.isWaveModeActive || !g_Main.autoRespawnEnabled) return HookResult.Continue;

var victim = @event.Userid;
   if (victim == null || !victim.IsValid || !victim.IsBot) return HookResult.Continue;

  // Check if dead bot was on CT team
    if (victim.Team != g_Main.botTeam) return HookResult.Continue;

     // Increment respawn counter
 g_Main.respawnsUsed++;

        // Calculate remaining respawns (countdown)
    int respawnsRemaining = g_Main.respawnsNeeded - g_Main.respawnsUsed;
  
          Console.WriteLine($"[Bot Wave] Bot death detected. Respawns used: {g_Main.respawnsUsed}/{g_Main.respawnsNeeded}, Remaining: {respawnsRemaining}");

    // Check if we've used all our respawns
if (g_Main.respawnsUsed >= g_Main.respawnsNeeded)
   {
       // Disable auto-respawn - let remaining bots die naturally
  Console.WriteLine($"[Bot Wave] Respawn limit reached! Disabling mp_respawn_on_death_ct");
   Server.ExecuteCommand("mp_respawn_on_death_ct 0");
    g_Main.autoRespawnEnabled = false;
   
 Server.PrintToChatAll(Localizer["Wave.NoMoreRespawns"]);
       }
   else
         {
   // Show progress every 5 respawns or when 3 or less remain
    if (g_Main.respawnsUsed % 5 == 0 || respawnsRemaining <= 3)
 {
 Server.PrintToChatAll(Localizer["Wave.RespawnsLeft", respawnsRemaining]);
 }
  }
  }
        catch (Exception ex)
        {
    Console.WriteLine($"[Bot Wave] Error in OnPlayerDeath: {ex.Message}");
      }

      return HookResult.Continue;
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
 try
        {
Console.WriteLine($"[Bot Wave] OnRoundStart fired. Wave mode active: {g_Main.isWaveModeActive}");
            
      if (!g_Main.isWaveModeActive) return HookResult.Continue;

   // Reset auto-respawn system
  g_Main.autoRespawnEnabled = false;
  g_Main.respawnsNeeded = 0;
        g_Main.respawnsUsed = 0;

        // Kill any existing spawn timer
g_Main.BotSpawnTimer?.Kill();
            g_Main.BotSpawnTimer = null;

  if (g_Main.firstWaveNotStarted)
      {
     // FIRST WAVE - Do setup IMMEDIATELY (no delay)
    Console.WriteLine("[Bot Wave] First wave - immediate setup");
      g_Main.firstWaveNotStarted = false;
      
  // Move humans immediately (but keep spectators in spec)
       var humans = Utilities.GetPlayers().Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV).ToList();
   foreach (var human in humans)
    {
      // Skip spectators
            if (human.Team == CsTeam.Spectator)
        {
    continue;
    }
          
    if (human.Team != g_Main.humanTeam)
  {
     human.ChangeTeam(g_Main.humanTeam);
       }
 }
    
   // Spawn bots immediately
          int waveTarget = g_Main.currentWaveBotCount;
     AddBots(waveTarget);
        
     // Check spawn limit after delay
       AddTimer(1.0f, () => CheckSpawnLimit(waveTarget));
   }
    else
    {
   // NORMAL WAVE - Use NextFrame
  Server.NextFrame(() => DoNormalRoundStart());
     }
      }
  catch (Exception ex)
{
 Console.WriteLine($"[Bot Wave] Error in OnRoundStart: {ex.Message}");
   }

        return HookResult.Continue;
    }

    private void DoNormalRoundStart()
    {
        try
{
     Console.WriteLine("[Bot Wave] Processing normal round start");

         // Force all humans to T side (but keep spectators in spec)
 var humans = Utilities.GetPlayers().Where(p => p != null && p.IsValid && !p.IsBot && !p.IsHLTV).ToList();
      foreach (var human in humans)
      {
            // Skip spectators
    if (human.Team == CsTeam.Spectator)
 {
       continue;
   }
          
     if (human.Team != g_Main.humanTeam)
{
   Console.WriteLine($"[Bot Wave] Moving {human.PlayerName} to {g_Main.humanTeam}");
 human.ChangeTeam(g_Main.humanTeam);
  }
   }

   // Count existing bots
      var existingBots = Utilities.GetPlayers().Count(p => p != null && p.IsValid && p.IsBot && !p.IsHLTV);
   int waveTarget = g_Main.currentWaveBotCount;
      
 Console.WriteLine($"[Bot Wave] Existing bots: {existingBots}, Wave target: {waveTarget}");

   // Spawn needed bots
    if (existingBots < waveTarget)
   {
     int toSpawn = waveTarget - existingBots;
       Console.WriteLine($"[Bot Wave] Attempting to spawn {toSpawn} bots");
AddBots(toSpawn);
  }

   // Check spawn limit after delay
 AddTimer(1.0f, () => CheckSpawnLimit(waveTarget));
   }
        catch (Exception ex)
 {
  Console.WriteLine($"[Bot Wave] Error in normal round start: {ex.Message}");
  }
    }

    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
     try
    {
  if (!g_Main.isWaveModeActive) return HookResult.Continue;

            // Ignore round end from mp_restartgame
 if (g_Main.firstWaveNotStarted)
     {
    Console.WriteLine("[Bot Wave] Ignoring round end from restart/activation");
     return HookResult.Continue;
 }

    CsTeam winner = (CsTeam)@event.Winner;

  // Only increase wave if HUMANS won
            if (winner == g_Main.humanTeam)
  {
      // Waves go forever with auto-respawn
    g_Main.currentWaveBotCount++;
    Server.PrintToChatAll(Localizer["Wave.YouWonNext", g_Main.currentWaveBotCount]);
      
// Show special messages at milestone waves
 if (g_Main.currentWaveBotCount == 20)
{
Server.PrintToChatAll(Localizer["Wave.Milestone20"]);
  }
    else if (g_Main.currentWaveBotCount == 50)
     {
 Server.PrintToChatAll(Localizer["Wave.Milestone50"]);
    }
   else if (g_Main.currentWaveBotCount == 100)
 {
  Server.PrintToChatAll(Localizer["Wave.Milestone100"]);
      }
     else if (g_Main.currentWaveBotCount % 25 == 0)
   {
          Server.PrintToChatAll(Localizer["Wave.MilestoneOther", g_Main.currentWaveBotCount]);
   }
        }
     else if (winner == g_Main.botTeam)
   {
   // HUMANS LOST - keep same wave count
      Server.PrintToChatAll(Localizer["Wave.YouLostTryAgain", g_Main.currentWaveBotCount]);
}
      }
    catch (Exception ex)
  {
  Console.WriteLine($"[Bot Wave] Error in OnRoundEnd: {ex.Message}");
 }

        return HookResult.Continue;
    }

 private void CheckSpawnLimit(int waveTarget)
    {
try
        {
   var aliveCTBots = Utilities.GetPlayers().Count(p => p != null && p.IsValid && p.IsBot && !p.IsHLTV && p.Team == g_Main.botTeam);
   
Console.WriteLine($"[Bot Wave] Spawn check: {aliveCTBots} CT bots spawned, wave target: {waveTarget}");
 
 if (aliveCTBots < waveTarget)
   {
     // Hit spawn limit! Enable auto-respawn
    g_Main.respawnsNeeded = waveTarget - aliveCTBots;
  g_Main.respawnsUsed = 0;
    g_Main.autoRespawnEnabled = true;
   
   Console.WriteLine($"[Bot Wave] Spawn limit hit! Need {g_Main.respawnsNeeded} respawns");
  Console.WriteLine($"[Bot Wave] Enabling mp_respawn_on_death_ct 1");
 
  Server.ExecuteCommand("mp_respawn_on_death_ct 1");
   
    Server.PrintToChatAll(Localizer["Wave.FightBots", waveTarget, waveTarget]);
  Server.PrintToChatAll(Localizer["Wave.SpawnLimitReached", aliveCTBots, g_Main.respawnsNeeded]);
    }
  else
 {
// Normal wave, all bots spawned
   Console.WriteLine($"[Bot Wave] All {aliveCTBots} bots spawned successfully");
     Server.PrintToChatAll(Localizer["Wave.FightBots", waveTarget, waveTarget]);
        }
   }
   catch (Exception ex)
 {
       Console.WriteLine($"[Bot Wave] Error in spawn limit check: {ex.Message}");
        }
    }

    private void AddBots(int count)
    {
 // Add bots ONLY to CT side (enemy team = bot team)
     string teamCmd = g_Main.botTeam == CsTeam.Terrorist ? "t" : "ct";
      
  Console.WriteLine($"[Bot Wave] Adding {count} bots to {teamCmd} team");
        
for (int i = 0; i < count; i++)
        {
      Server.ExecuteCommand($"bot_add_{teamCmd}");
        }
    }

    private int GetHumanPlayerCount()
    {
        return Utilities.GetPlayers().Count(p => 
 p != null && 
  p.IsValid && 
    !p.IsBot && 
  !p.IsHLTV && 
 p.Connected == PlayerConnectedState.PlayerConnected
 );
    }
}