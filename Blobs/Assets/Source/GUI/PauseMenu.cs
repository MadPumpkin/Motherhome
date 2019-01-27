using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    public class PauseMenu : Menu
    {
        public static PauseMenu Instance;

        public GameObject[] Tabs;
        public AudioClip PlayOnTabChange;

        public OptionsMenu OptionsMenu;
        public Menu ControlsMenu;
        public GameObject LoadingScreen;
        public GameOverMenu GameOverMenu;

        public Text TimerText;
        public Text LevelLabel;
        public Text LevelNumberLabel;
        public Text SeedLabel;

        public PauseMenu()
        {
            Instance = this;
        }

        void Start()
        {
            OptionsMenu.gameObject.SetActive(false);
            ControlsMenu.gameObject.SetActive(false);
            LoadingScreen.gameObject.SetActive(false);
            GameOverMenu.gameObject.SetActive(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void Update()
        {
            if (Player.Instance != null)
            {
                //var time = Time.time - Player.Instance.RunStart;
                //TimerText.text = Utility.FormatSeconds(time);
            }

            base.Update();
        }

        public void Restart()
        {
            MessageBox.Show("Are you sure you want to restart the game?"._(), (result) =>
            {
                if (result == DialogResult.Yes)
                {
                    //if (Level.Instance.LevelNumber > 0 && UnityEngine.Networking.ClientScene.readyConnection != null)
                    //{
                    //    Player.Instance.CmdBroadcast(Player.Instance.name + " restarted!", Utility.Red);
                    //    Player.Instance.CmdJumpToLevel(0);
                    //}

                    if (PauseMenu.Instance.gameObject.activeSelf)
                        PauseMenu.Instance.Hide();
                }
            }, MessageBoxButtons.YesNo);
        }

        public void Quit()
        {
            Hide();

            //if (NetworkServer.active && Player.Players.Count > 1)
            //{
            //    MessageBox.Show("You are hosting the server and all players will be disconnected. Are you sure?"._(), (result) =>
            //    {
            //        if (result == DialogResult.Yes)
            //        {
            //            if (Player.Instance != null)
            //                Player.Instance.RunComplete(GameEndReason.Quit);
            //            Level.Instance.LeaveGameAndReturnToMainMenu();
            //        }
            //    }, MessageBoxButtons.YesNo);
            //}
            //else
            //{
            //    if (Player.Instance != null)
            //        Player.Instance.RunComplete(GameEndReason.Quit);
            //    Level.Instance.LeaveGameAndReturnToMainMenu();
            //}
        }
    }
}