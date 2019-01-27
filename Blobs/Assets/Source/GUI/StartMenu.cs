using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    public class StartMenu : Menu
    {
        public static StartMenu Instance;

        public StartMenu()
        {
            ShowBackground = false;
            Instance = this;
        }

        public List<GameObject> ActivateWhenShown;

        public List<GameObject> DeactivateOnShow;
        public List<Behaviour> DisableOnShow;

        public Transform Mods;
        public GameObject ModReadoutPrefab;
        public Text BuildLabel;

        public GameObject Countess;

        protected override void OnEnable()
        {
            //Debug.LogError("StartMenu.OnEnable");

            base.OnEnable();

            ShowTitleRoom();

            Countess.SetActive(true);
            BuildLabel.text = Utility.Version;
        }

        static bool isFirstRun = true;

        void Start()
        {
            //Debug.LogError("StartMenu.Start " + isFirstRun);
            UpdateMods();

            if (isFirstRun)
            {
                if (!CheckCommandLine())
                {
//#if UNITY_EDITOR
//                    AutoloadData = SaveData.Load();
//#endif
                }
                isFirstRun = false;
            }

            if (Level.IsHeadlessMode)
            {
                Hide();
            }
        }

        void OnDisable()
        {
            if (Countess != null)
                Countess.SetActive(false);
        }

        public void ShowTitleRoom()
        {
            //print("ShowTitleRoom");
            var pos = Camera.main.transform.position;
            pos.x = 0;
            pos.y = 0;
            Camera.main.transform.position = pos;

            foreach (var obj in ActivateWhenShown)
                obj.SetActive(true);

            foreach (var obj in DeactivateOnShow)
                obj.SetActive(false);

            foreach (var obj in DisableOnShow)
                obj.enabled = false;

            HUD.Instance.Hide();
        }

        public void HideTitleRoom()
        {
            //print("HideTitleRoom");
            foreach (var obj in ActivateWhenShown)
                obj.SetActive(false);
            foreach (var obj in DeactivateOnShow)
                obj.SetActive(true);
            foreach (var obj in DisableOnShow)
                obj.enabled = true;
        }

        bool CheckCommandLine()
        {
            var result = false;
            var args = Environment.GetCommandLineArgs();

            //Launcher.Instance.LogFeedback(String.Join(", ", args));
            //Debug.LogError(String.Join(", ", args));

            for (int i = 0; i < args.Length; i++)
            {
                // Steam Direct Connect is like this: +connect 32.42.21.54:1701
                //if (args[i] == "+connect" && i < args.Length - 1)
                //{
                //    var addressPort = args[i + 1].Split(':');
                //    if (addressPort.Length == 2)
                //    {
                //        int port;
                //        if (int.TryParse(addressPort[1], out port))
                //        {
                //            AutoloadData = SaveData.Load();
                //            Launcher.Instance.ConnectTo(addressPort[0], port);
                //            result = true;
                //        }
                //    }
                //}

                ////+connect_lobby <64-bit lobby id>
                //if (args[i] == "+connect_lobby" && i < args.Length - 1)
                //{
                //    ulong steamLobbyId;
                //    if (ulong.TryParse(args[i + 1], out steamLobbyId))
                //    {
                //        //Debug.LogError("+connect_lobby " + steamLobbyId);
                //        AutoloadData = SaveData.Load();
                //        MainMenu.Instance.SteamLobbyId = steamLobbyId;
                //        result = true;
                //    }
                //}

                //if (args[i] == "-port" && i < args.Length - 1)
                //{
                //    int port;
                //    if (int.TryParse(args[i + 1], out port))
                //        Launcher.Instance.networkPort = port;
                //}
            }

            return result;

            // for testing auto join from command line
            //Debug.LogError("Forcing lobby");
            //AutoloadData = SaveData.Load();
            //MainMenu.Instance.SteamLobbyId = 1000;
            //return true;
        }

        protected override void Update()
        {
            //if (AutoloadData != null)
            //{
            //    //Debug.LogError("StartMenu.AutoLoad");

            //    SaveData.Current = AutoloadData;
            //    EditCharacterMenu.IsFirstRun = false;
            //    MainMenu.Instance.Show();
            //    AutoloadData = null;
            //}

            base.Update();
        }

        public void UpdateMods()
        {
            //foreach (Transform child in Mods)
            //{
            //    Destroy(child.gameObject);
            //}

            //if (World.FoundMods.Count > 0)
            //{
            //    Mods.GetComponent<Text>().enabled = false;

            //    foreach (var mod in World.FoundMods)
            //    {
            //        var readout = Instantiate(ModReadoutPrefab) as GameObject;
            //        readout.transform.SetParent(Mods, false);
            //        readout.GetComponentInChildren<Text>().text = String.Format("Mod {0} v{1} by {2}\n{3}", mod.Name, mod.Version, mod.Author, mod.Description);

            //        var button = readout.GetComponentInChildren<Button>();
            //        if (World.Mods.Contains(mod))
            //            Destroy(button.gameObject);
            //        else
            //        {
            //            var modValue = mod; // to excape lambda variable capture
            //            button.onClick.AddListener(delegate { World.Instance.EnableMod(modValue); });
            //        }
            //    }
            //}
        }

        public void NewGame()
        {
            //EditCharacterMenu.Instance.Show();
        }

        public void Load()
        {
            //print("StartMenu.Load");

            //LoadMenu.Instance.Show();
        }

        public void Options()
        {
            OptionsMenu.Instance.Show();
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}