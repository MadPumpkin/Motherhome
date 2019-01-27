using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    public class Menu : MonoBehaviour
    {
        public static Menu CurrentMenu;

        public Selectable SelectOnEnable;
        public AudioClip MenuMusic;
        public bool IsCloseable;
        public bool ShowCursor = true;
        public bool CloseAllPreviousMenus;

        protected Menu previousMenu;
        protected bool ShowBackground = true;

        [NonSerialized]
        public bool IsShown;

        public static void HideAll()
        {
            while (CurrentMenu != null)
                CurrentMenu.Hide();
        }

        protected virtual void OnEnable()
        {
            //print(this.name + " Menu.OnEnable");

            if (SelectOnEnable != null)
            {
                SelectOnEnable.Select();
                SelectOnEnable.OnSelect(null); // https://answers.unity.com/questions/1159573/eventsystemsetselectedgameobject-doesnt-highlight.html
            }
        }

        protected virtual void Update()
        {
            //print(this.name + " Update");

            if (
                    IsCloseable &&
                    (
                        Player.Instance != null &&
                        (Input.GetButtonDown("Cancel") || Input.GetKeyDown(KeyCode.Escape))
                    )
                )
            {
                Hide();
            }
        }

        public virtual void Show()
        {
            if (CurrentMenu != this)
            {
                if (CloseAllPreviousMenus)
                {
                    while (CurrentMenu != null && CurrentMenu != this)
                        CurrentMenu.Hide();
                }

                previousMenu = null;
                if (ShowCursor)
                {
                    //print("showing cursor");
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }

                if (CurrentMenu != null)
                {
                    previousMenu = CurrentMenu;
                    CurrentMenu.gameObject.SetActive(false);
                }

                CurrentMenu = this;
                //LogMenuStack();

                MenuRoot.Instance.Show(ShowBackground);
                IsShown = true;
                gameObject.SetActive(true);

                //if (HUD.Instance.ItemDescriptionDialog.gameObject.activeSelf)
                //    HUD.Instance.ItemDescriptionDialog.gameObject.SetActive(false);

                if (HUD.Instance.DialogWindow.gameObject.activeSelf)
                {
                    HUD.Instance.DialogWindow.Close();
                    HUD.Instance.DialogWindow.gameObject.SetActive(false);
                }

                //if (SelectOnEnable != null)
                //{
                //    SelectOnEnable.Select();
                //    SelectOnEnable.OnSelect(null);
                //}

                if (MenuMusic != null)
                    MusicManager.PlayTempMusic(MenuMusic, 2.3f);
            }
        }

        static void LogMenuStack()
        {
            var result = new StringBuilder("Menu Stack: ");
            var menu = CurrentMenu;
            while (menu != null)
            {
                result.Append(menu + " -> ");
                menu = menu.previousMenu;
            }
            Debug.LogError(result.ToString());
        }

        public virtual void Hide()
        {
            if (!IsShown)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (CurrentMenu == this)
                {
                    if (MenuMusic != null)
                        MusicManager.StopTempMusic();
                    CurrentMenu = null;
                }
                else
                {
                    //throw new Exception("Trying to hide shown menu that isn't current");
                    //return;
                }

                gameObject.SetActive(false);
                IsShown = false;

                while (previousMenu != null && !previousMenu.IsShown) // skip closed menus
                    previousMenu = previousMenu.previousMenu;

                if (previousMenu != null)
                {
                    previousMenu.gameObject.SetActive(true);
                    CurrentMenu = previousMenu;
                    MenuRoot.Instance.Show(previousMenu.ShowBackground);
                    previousMenu = null;

                    //print(CurrentMenu);
                }
                else
                {
                    MenuRoot.Instance.Hide();
                    Cursor.visible = false;
                }

                //LogMenuStack();
            }
        }
    }
}