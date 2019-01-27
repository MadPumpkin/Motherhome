using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Text;
using System.IO;
using System.Reflection;
using UnityEngine.Networking;

namespace Legend
{
    public class Level : MonoBehaviour
    {
        public const string GameName = "Motherhome";

        public static Level Instance;
        public static bool CheatsEnabled;

        public static string AccountLink = "";
        public static string AccountName = "";

        public static bool Modded;
        public static List<IMod> Mods = new List<IMod>();
        public static List<IMod> FoundMods = new List<IMod>();
        static Dictionary<IMod, string> modLocations = new Dictionary<IMod, string>();

        [Header("Audio")]
        public AudioClip ItemSpawned;
        public AudioClip LevelChange;
        public AudioClip BuyItem;

        public AudioClip Error;
        public AudioClip BossDefeated;
        public AudioClip GameOverFanfare;
        public AudioClip[] PlayOnUpgrade;
        public AudioClip[] SpawnRumbles;
        public AudioClip HiddenRoomMusic;
        public AudioClip HiddenRoomEnterStinger;
        public AudioClip ShopRoomMusic;
        public AudioClip ShopRoomEnterStinger;
        public AudioClip TreasureRoomMusic;
        public AudioClip TreasureRoomEnterStinger;
        public AudioClip BossDefeatedRoomMusic;
        public AudioClip AchievementSound;

        [Header("Prefabs")]
        public GameObject PlayerPrefab;
        public GameObject ItemIconPrefab;
        public GameObject CalloutPrefab;
        public GameObject PriceLabelPrefab;
        public GameObject UpgradePopUpPrefab;
        public GameObject BombPrefab;

        public List<GameObject> Placeables;

        public List<GameObject> Pickups;
        public List<GameObject> EnemyDropPickups;
        public List<GameObject> Upgrades;
        public GameObject Scroll;
        public List<GameObject> Doors;
        public GameObject Explosion;
        public Sprite UnknownItemIcon;
        public Sprite[] PotionSprites;
        public Sprite[] RoleSprites;
        public GameObject GUIRoot;

        [NonSerialized]
        public int LevelNumber = -1;

        public static bool IsHeadlessMode
        {
            get
            {
#if UNITY_EDITOR
                //return true;
                return UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
#else
                return UnityEngine.SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
#endif
            }
        }
        public static bool IsTestBuild { get { return Application.dataPath.Contains(@"Experimental/Networked WebGL/WIndows Build"); } }

        public Level()
        {
            Instance = this;
#if UNITY_EDITOR
            CheatsEnabled = true;
#endif
        }

        void Awake()
        {
            // do this here again because when running in editor and play scene is loaded that redirects to launcher it will create a level but destroy it in the wrong order, for us at least.
            Instance = this;

            Utility.SetupExceptionHandling();

            var commandLineArgs = new List<string>(System.Environment.GetCommandLineArgs());
            // If -logfile was passed, we try to put our own logs next to the engine's logfile
            var engineLogFileLocation = ".";
            var logfileArgIdx = commandLineArgs.IndexOf("-logfile");
            if (logfileArgIdx >= 0 && commandLineArgs.Count >= logfileArgIdx)
            {
                engineLogFileLocation = System.IO.Path.GetDirectoryName(commandLineArgs[logfileArgIdx + 1]);
            }

            var logName = IsHeadlessMode ? "game_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff") : "game";
            GameDebug.Init(engineLogFileLocation, logName);

            if (IsHeadlessMode)
            {
#if UNITY_STANDALONE_WIN
                string consoleTitle;

                var overrideTitle = ArgumentForOption(commandLineArgs, "-title");
                if (overrideTitle != null)
                    consoleTitle = overrideTitle;
                else
                    consoleTitle = Application.productName + " Console";

                consoleTitle += " [" + System.Diagnostics.Process.GetCurrentProcess().Id + "]";

                var consoleUI = new ConsoleTextWin(consoleTitle, false);
//#elif UNITY_STANDALONE_LINUX
//            var consoleUI = new ConsoleTextLinux();
#else
            UnityEngine.Debug.Log("WARNING: starting without a console");
            var consoleUI = new ConsoleNullUI();
#endif
                Console.Init(consoleUI);

                //SaveData.Current = new SaveData() { SaveNumber = 0 };
                //DestroyImmediate(GUIRoot);
                //DestroyImmediate(Camera.main.gameObject);
                //Resources.UnloadUnusedAssets();
            }
            else
            {
                var consoleUI = Instantiate(Resources.Load<ConsoleGUI>("ConsoleGUI"));
                DontDestroyOnLoad(consoleUI);
                Console.Init(consoleUI);

                iTween.CameraFadeAdd();

                Console.AddCommands(typeof(CommandsClient));
            }

            Console.Write("Run help for a command list.");

            ConfigVar.Init();

            Console.SetOpen(false);
            Console.ProcessCommandLineArguments(commandLineArgs.ToArray());

            Debug.Log(GameName + " Version " + Utility.Version);
            //Utility.HandleException(GameName + " Version " + Utility.Version, "", LogType.Exception);

            Console.AddCommands(typeof(CommandsAll));

            //if (CheatsEnabled || IsHeadlessMode)
            //    CommandsServer.RegisterCommands();
        }

        // Pick argument for argument(!). Given list of args return null if option is
        // not found. Return argument following option if found or empty string if none given.
        // Options are expected to be prefixed with + or -
        public static string ArgumentForOption(List<string> args, string option)
        {
            var idx = args.IndexOf(option);
            if (idx < 0)
                return null;
            if (idx < args.Count - 1)
                return args[idx + 1];
            return "";
        }

        void Start()
        {
            LoadMods("Mods");

            // maybe do some kind of SteamUGC.GetSubscribedItems() and SteamUGC.GetItemInstallInfo() here instead.
            //LoadMods("..\\..\\workshop\\content\\" + SteamAppId.ToString());
        }

        void OnEnable()
        {
            Settings.SetValues();

            //foreach (var menu in FindObjectsOfType<Menu>())
            //{
            //    menu.Hide();
            //}

            //StartMenu.Instance.Show();
        }

        void LoadMods(string modFolder)
        {
            //modFolder = Path.GetFullPath(modFolder);
            print(String.Format("Scanning for mods in {0}"._(), modFolder));
            try
            {
                if (!Directory.Exists(modFolder))
                    return;
                var resultMessage = new StringBuilder();
                foreach (var assemblyFilename in Directory.GetFiles(modFolder, "*.dll", SearchOption.AllDirectories))
                {
                    var bytes = File.ReadAllBytes(assemblyFilename);
                    var assembly = Assembly.Load(bytes);
                    foreach (Type t in assembly.GetTypes())
                    {
                        if (t.GetInterface("IMod") != null)
                        {
                            var constructor = t.GetConstructor(new Type[] { });
                            if (constructor != null)
                            {
                                var mod = (IMod)constructor.Invoke(new object[] { });
                                FoundMods.Add(mod);
                                modLocations[mod] = Path.GetDirectoryName(assemblyFilename);
                                //mod.OnLoad(Path.GetDirectoryName(assemblyFilename));
                                HUD.Instance.ShowMessage(String.Format("Mod {0} v{1} by {2} Loaded from {3}"._(), mod.Name, mod.Version, mod.Author, assemblyFilename));
                                HUD.Instance.ShowMessage(mod.Description);

                                resultMessage.AppendFormat("Mod {0} v{1} by {2} Loaded from {3}"._(), mod.Name, mod.Version, mod.Author, assemblyFilename);
                                resultMessage.AppendLine();
                                resultMessage.Append(mod.Description);

                                EnableMod(mod);
                                break;
                            }
                        }
                    }
                }

                if (resultMessage.Length > 0)
                {
                    //MessageBox.Show(resultMessage.ToString());
                    print(resultMessage);
                    //HUD.Instance.ShowMessage(resultMessage.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                if (e.InnerException != null)
                {
                    Debug.LogError("INNER EXCEPTION");
                    Debug.LogError(e.InnerException);
                }
            }
        }

        public void EnableMod(IMod mod)
        {
            Modded = true;
            Mods.Add(mod);
            mod.OnLoad(modLocations[mod]);

            HUD.Instance.ShowMessage(String.Format("{0} Enabled"._(), mod.Name));

            //var menu = FindObjectOfType<MainTitleMenu>();
            //if (menu != null)
            //    menu.UpdateMods();
        }

        void Update()
        {
            //if (Debug.developerConsoleVisible)
            //    Debug.developerConsoleVisible = false;

            Console.ConsoleUpdate();

            if (Player.Instance == null || Player.Instance.Movement == null || !Player.Instance.Movement.MovementEnabled || !Player.Instance.Movement.MovementReallyEnabled)
                return;

            //if (Input.GetKeyDown(KeyCode.Alpha1) && (Level.CheatsEnabled || LevelNumber == 0))
            //{
            //    Player.Instance.CmdJumpToLevel(1);
            //}

            //if (Level.CheatsEnabled)
            //{
            //    if (Input.GetKeyDown(KeyCode.F1))
            //    {
            //        //foreach (Unlock unlock in Enum.GetValues(typeof(Unlock)))
            //        //{
            //        //    SaveData.Current.GrantUnlock(unlock);
            //        //}

            //        //foreach (var type in ItemBase.FindableTypes)
            //        //    SaveData.Current.FoundItem(type.Id);

            //        Player.Instance.Coins = 50;
            //        Player.Instance.Bombs = 50;
            //        Player.Instance.Keys = 50;

            //        Player.Instance.Health.MaxHealth = 8;
            //        Player.Instance.Health.Health = 8;
            //        Player.Instance.Health.Shield = 8;
            //        Player.Instance.Damage = 30;
            //        Player.Instance.AttackRate = 30;
            //        Player.Instance.Speed = 30;
            //        Player.Instance.BulletSpeed = 30;
            //        Player.Instance.Range = 30;

            //        Player.Instance.Roles = Roles.Treasurer | Roles.Cartographer | Roles.Vitalist | Roles.Loremaster;

            //        MiniMap.Instance.UpdateMap(true);
            //    }

            //    if (Input.GetKeyDown(KeyCode.F2))
            //    {
            //        if (Player.Instance != null && Player.Instance.Room != null)
            //        {
            //            foreach (var p in GameObject.FindObjectsOfType<Projectile>())
            //            {
            //                if (p.Targets == TargetType.Players || p.Targets == TargetType.All)
            //                {
            //                    if (Player.Instance.Room.ContainsPoint(p.transform.position))
            //                    {
            //                        p.ExplodeOnCollision = false;
            //                        Destroy(p.gameObject);
            //                    }
            //                }
            //            }

            //            foreach (var e in Player.Instance.Room.GetEnemies())
            //            {
            //                foreach (var move in e.GetComponentsInChildren<Move>())
            //                    move.StopAllCoroutines(); // stop spawning & shooting

            //                e.IsExplodeOnDeath = false;
            //                e.Health.Die(new Damage() { Type = DamageType.Unblockable });
            //                if (!e.Health.IsDestroyOnDie)
            //                    e.Health.FadeAndDestroy();
            //            }
            //        }
            //    }

            //    if (Input.GetKeyDown(KeyCode.Alpha2))
            //        Player.Instance.CmdJumpToLevel(2);
            //    if (Input.GetKeyDown(KeyCode.Alpha3))
            //        Player.Instance.CmdJumpToLevel(3);
            //    if (Input.GetKeyDown(KeyCode.Alpha4))
            //        Player.Instance.CmdJumpToLevel(4);
            //    if (Input.GetKeyDown(KeyCode.Alpha5))
            //        Player.Instance.CmdJumpToLevel(5);
            //    if (Input.GetKeyDown(KeyCode.Alpha6))
            //        Player.Instance.CmdJumpToLevel(6);
            //    if (Input.GetKeyDown(KeyCode.Alpha7))
            //        Player.Instance.CmdJumpToLevel(7);
            //    //if (Input.GetKeyDown(KeyCode.Alpha8))
            //    //    JumpToLevel(8);
            //    if (Input.GetKeyDown(KeyCode.Alpha0))
            //        Player.Instance.CmdJumpToLevel(0);
            //}
        }
    }
}