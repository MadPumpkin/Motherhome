using System;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    public class GameOverMenu : Menu
    {
        public static GameOverMenu Instance;

        public Text ResultLabel;
        public Text RunTimeLabel;
        public Text TimerLabel;

        public GameOverMenu()
        {
            Instance = this;
        }

        protected override void OnEnable()
        {
            //print("OnEnable GameOverMenu");
            HUD.Instance.Hide();

            //if (Player.RunReport.EndReason == GameEndReason.Victory)
            //{
            //    ResultLabel.text = "VICTORY"._();
            //    ResultLabel.color = Utility.LightBlue;

            //    var animator = GetComponent<Animator>();
            //    animator.Play("Roll Credits");
            //}
            //else
            //{
            //    ResultLabel.text = "DEFEAT"._();
            //    ResultLabel.color = Utility.DarkRed;

            //    var animator = GetComponent<Animator>();
            //    animator.Play("Game Over Idle");
            //}

            //if (Level.Instance != null && Level.Instance.CurrentLevel != null && Player.RunReport != null)
            //{
            //    RunTimeLabel.text = String.Format(
            //        "Level {0} {4} - {1}\nRun Time {2:N0}:{3:00}"._(),
            //        Level.Instance.LevelNumber,
            //        Level.Instance.CurrentLevel.Name,
            //        Player.RunReport.TimeSeconds / 60f,
            //        Player.RunReport.TimeSeconds % 60f,
            //        GameServer.Instance == null ? "None" : GameServer.Instance.Mode.ToString().AddSpaces()
            //        );
            //}

            base.OnEnable();
        }

        protected override void Update()
        {
            //TimerLabel.text = (Level.Instance.TimeToRestart - Time.time).ToString("N0");

            //if (Level.Instance.TimeToRestart < Time.time)
            //    Hide();

            base.Update();
        }

        public void LeaveGame()
        {
            //Hide();
            //Level.Instance.LeaveGameAndReturnToMainMenu();
        }
    }
}