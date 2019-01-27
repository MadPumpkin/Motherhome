using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;

namespace Legend
{
    public static class CommandsAll
    {
    //    [ConsoleCommand(Name = "b", Description = "Broadcast a mesage.")]
    //    static void CmdBroadcast(string[] arguments)
    //    {
    //        if (GameServer.Instance == null)
    //        {
    //            Console.Write("Not connected to a game server.");
    //            return;
    //        }

    //        var message = new StringBuilder();
    //        foreach (var arg in arguments)
    //        {
    //            if (message.Length > 0)
    //                message.Append(" ");
    //            message.Append(arg);
    //        }
    //        GameServer.Instance.CmdBroadcast(message.ToString(), Utility.Purple);
    //    }

    //    [ConsoleCommand(Description = "Shutdown the game and exit. Usage: quit [now]")]
    //    static void CmdQuit(string[] arguments)
    //    {
    //        if ((arguments.Length > 0 && arguments[0] == "now") || !Level.IsHeadlessMode)
    //        {
    //            Application.Quit();
    //            return;
    //        }

    //        GameServer.Instance.CmdBroadcast("Server shutting down in 5 seconds.", Utility.Purple);
    //        GameServer.Instance.Delay(Application.Quit, 5f);
    //    }

    //    [ConsoleCommand(Description = "List connected players.")]
    //    static void CmdWho(string[] arguments)
    //    {
    //        Console.Write("Connected players:");
    //        foreach (var p in Player.Players)
    //        {
    //            Console.Write(String.Format("{0} {1}", p.name, p.Roles));
    //        }
    //    }

    //    [ConsoleCommand(Description = "Gets the latest language translations from the internet.")]
    //    static void CmdLocalize(string[] arguments)
    //    {
    //        I2.Loc.LocalizationManager.Sources[0].Import_Google(true, false);

    //        Level.Instance.Delay(() =>
    //        {
    //            ItemBase.LoadItems();
    //            Level.Instance.LoadItemTypeData();
    //            HUD.Instance.ShowMessage("Reloaded item base.");
    //        }, 10f);

    //        Console.Write("Okay (This can only update once per minute)");
    //    }

    //    [ConsoleCommand(Description = "Prints the current network status.")]
    //    static void NetStatus(string[] arguments)
    //    {
    //        SteamLobby.ListLobbies(UpdateList, false);
    //        Console.Write(Launcher.Instance.GetStatus().Trim());
    //    }

    //    static void UpdateList()
    //    {
    //        HUD.Instance.ShowMessage(SteamLobby.Lobbies.Count + " lobbies found!");

    //        foreach (var lobbyId in SteamLobby.Lobbies)
    //        {
    //            HUD.Instance.ShowMessage(lobbyId.ToString());

    //            var dataCount = SteamMatchmaking.GetLobbyDataCount(lobbyId);
    //            for (int i = 0; i < dataCount; i++)
    //            {
    //                string key;
    //                string value;
    //                SteamMatchmaking.GetLobbyDataByIndex(lobbyId, i, out key, 1024, out value, 1024);
    //                HUD.Instance.ShowMessage(String.Format("   {1}: {2}", lobbyId, key, value));
    //            }

    //            var owner = SteamMatchmaking.GetLobbyOwner(lobbyId);
    //            if (owner != CSteamID.Nil)
    //            {
    //                HUD.Instance.ShowMessage(String.Format("   {1}: {2}", lobbyId, "OWNER", owner));
    //                HUD.Instance.ShowMessage(String.Format("   {1}: {2}", lobbyId, "STEAMID", Level.AccountLink));
    //            }
    //        }
    //    }


    //    [ConsoleCommand(Description = "Prints the current memory status.")]
    //    static void Memory(string[] arguments)
    //    {
    //        var proc = System.Diagnostics.Process.GetCurrentProcess();
    //        proc.Refresh();
    //        Console.Write("Process Private Memory: " + proc.PrivateMemorySize64);
    //        Console.Write("GC Memory: " + GC.GetTotalMemory(false));
    //        GC.Collect();
    //        Console.Write("GC Memory After Collect: " + GC.GetTotalMemory(true));
    //    }

    //    [ConsoleCommand(Description = "List steam servers.")]
    //    static void SteamServers(string[] arguments)
    //    {
    //        var serverListResponse = new ISteamMatchmakingServerListResponse
    //            (
    //                new ISteamMatchmakingServerListResponse.ServerResponded(
    //                    (request, index) => {
    //                        var serverDetails = SteamMatchmakingServers.GetServerDetails(request, index);
    //                        HUD.Instance.ShowMessage(serverDetails.GetServerName());
    //                        Console.Write(serverDetails.GetServerName());
    //                    }),
    //                new ISteamMatchmakingServerListResponse.ServerFailedToRespond(
    //                    (request, index) => {
    //                        var serverDetails = SteamMatchmakingServers.GetServerDetails(request, index);
    //                        HUD.Instance.ShowMessage(serverDetails.GetServerName());
    //                        Console.Write("Failed to Respond: " + serverDetails.GetServerName());
    //                    }),
    //                new ISteamMatchmakingServerListResponse.RefreshComplete((request, response) => HUD.Instance.ShowMessage("Steam server list complete"))
    //            );

    //        SteamMatchmakingServers.RequestInternetServerList(new AppId_t(Level.SteamAppId), new MatchMakingKeyValuePair_t[0], 0, serverListResponse);
    //    }
    }
}