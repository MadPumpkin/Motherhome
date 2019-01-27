//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using UnityEngine;
//using UnityEngine.Networking;

//namespace Legend
//{
//    public class SpawnCommand : Command
//    {
//        public override string Description
//        {
//            get { return "/Spawn: Spawns a wave of enemies."; }
//        }

//        public override string Execute(Player player, string args)
//        {
//            if (!NetworkServer.active)
//                return "Only host can spawn.";

//            if (player.Room == null)
//                return "No player room set.";

//            var prefab = Level.Instance.Placeables[51];
//            var position = player.Room.RandomPosition();

//            var spawned = GameObject.Instantiate(prefab, position, Quaternion.identity, player.Room.transform);
//            NetworkServer.Spawn(spawned);

//            return "Okay";
//        }
//    }
//}