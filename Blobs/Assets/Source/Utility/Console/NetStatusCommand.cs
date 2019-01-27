//using Steamworks;
//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using UnityEngine;

//namespace Legend
//{
//    public class NetStatusCommand : Command
//    {
//        public override string Description
//        {
//            get { return "/NetStatus: Show network status and lists steam lobbies."; }
//        }

//        public override string Execute(Player player, string args)
//        {
//            SteamLobby.ListLobbies(UpdateList, false);
//            return Launcher.Instance.GetStatus().Trim();
//        }

//        void UpdateList()
//        {
//            HUD.Instance.ShowMessage(SteamLobby.Lobbies.Count + " lobbies found!");

//            foreach (var lobbyId in SteamLobby.Lobbies)
//            {
//                HUD.Instance.ShowMessage(lobbyId.ToString());

//                var dataCount = SteamMatchmaking.GetLobbyDataCount(lobbyId);
//                for (int i = 0; i < dataCount; i++)
//                {
//                    string key;
//                    string value;
//                    SteamMatchmaking.GetLobbyDataByIndex(lobbyId, i, out key, 1024, out value, 1024);
//                    HUD.Instance.ShowMessage(String.Format("   {1}: {2}", lobbyId, key, value));
//                }

//                var owner = SteamMatchmaking.GetLobbyOwner(lobbyId);
//                if (owner != CSteamID.Nil)
//                {
//                    HUD.Instance.ShowMessage(String.Format("   {1}: {2}", lobbyId, "OWNER", owner));
//                    HUD.Instance.ShowMessage(String.Format("   {1}: {2}", lobbyId, "STEAMID", Level.AccountLink));
//                }
//            }
//        }
//    }
//}