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
    public static class CommandsClient
    {
        [ConsoleCommand()]
        static void Cheater(string[] arguments)
        {
            Level.CheatsEnabled = !Level.CheatsEnabled;
            Console.Write("Cheats Enabled " + Level.CheatsEnabled);
        }

        //[ConsoleCommand(Description = "Sends you back to the start of the overworld.")]
        //static void Stuck(string[] arguments)
        //{
        //    if (Player.Instance != null)
        //    {
        //        var spawnRoom = Room.AllRooms.Where(r => r.Type == RoomType.Start).RandomOrDefault();
        //        if (spawnRoom == null)
        //            Console.Write("Unable to find spawn room.");

        //        Player.Instance.CmdGoto(spawnRoom.gameObject, Direction.None, 0);

        //        Console.Write("Stay Calm and Respawn!");
        //    }
        //}

        //[ConsoleCommand(Description = "Resets your steam stats and achievements. Used for testing. Usage: /SteamResetAchievements Sure")]
        //static void SteamResetAchievements(string[] arguments)
        //{
        //    if (arguments.Length < 1 || arguments[0].ToUpperInvariant() != "SURE")
        //    {
        //        Console.Write("Usage: /SteamResetAchievements Sure");
        //    }

        //    SaveData.Current.Unlocks.Clear();
        //    SaveData.Current.Save();
        //    Steamworks.SteamUserStats.ResetAllStats(true);

        //    Console.Write("Achievements Reset");
        //}

        //[ConsoleCommand(Description = "Connect to a server. Default address: 127.0.0.1, Default port: 1701. Usage: /connect [address] [port]")]
        //static void Connect(string[] arguments)
        //{
        //    Level.Instance.LeaveGameAndReturnToMainMenu();

        //    var address = "127.0.0.1";
        //    var port = 1701;

        //    if (arguments.Length > 0 && !String.IsNullOrEmpty(arguments[0]))
        //        address = arguments[0];
        //    if (arguments.Length > 1)
        //        Int32.TryParse(arguments[1], out port);

        //    Launcher.Instance.ConnectTo(address, port);
        //    // String.Format("Connecting to {0}:{1}", address, port);
        //}

        //[ConsoleCommand(Description = "Toggles showing of the network stats graph.")]
        //static void NetGraph(string[] arguments)
        //{
        //    Settings.ShowNetGraph = !Settings.ShowNetGraph;
        //    Console.Write("Okay");
        //}

        //[ConsoleCommand(Description = "Toggles showing of the HUD.")]
        //static void HudToggle(string[] arguments)
        //{
        //    HUD.Instance.Toggle();
        //}

        //[ConsoleCommand(Description = "Chant emote.")]
        //static void Chant(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.SpellChant, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Dance emote.")]
        //static void Dance(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Swing, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Fall emote.")]
        //static void Fall(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Fall, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Happy emote.")]
        //static void Happy(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Happy, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Hurt emote.")]
        //static void Hurt(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Hurt, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Jump emote.")]
        //static void Jump(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Jump, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "No emote.")]
        //static void No(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.ShakeHead, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Ready emote.")]
        //static void Ready(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Ready, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Sad emote.")]
        //static void Sad(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Sad, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Shock emote.")]
        //static void Shock(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Shocked, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Sit emote.")]
        //static void Sit(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Sit, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Wave emote.")]
        //static void Wave(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Wave, 3, Direction.None);
        //}

        //[ConsoleCommand(Description = "Yes emote.")]
        //static void Yes(string[] arguments)
        //{
        //    if (Player.Instance != null && !Player.Instance.Health.IsDead)
        //    {
        //        Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Nod, 0.3f, Direction.None);
        //        Player.Instance.Delay(() => Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Nod, 0.3f, Direction.None), .35f);
        //        Player.Instance.Delay(() => Player.Instance.Visual.CmdSetTempAnimation(CharacterAnimation.Nod, 0.3f, Direction.None), .7f);
        //    }
        //}
    }
}