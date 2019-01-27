using System;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    public class MainMenu : Menu
    {
        public static MainMenu Instance;

        public ulong SteamLobbyId;
        public Text CurrentGames;
        public Text CurrentPlayers;
        public GameObject Countess;

        public MainMenu()
        {
            Instance = this;
            ShowBackground = false;
        }

        static bool isFirstRun = true;
        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateItemIcons();

            //PhotonNetwork.networkingPeer.CloudRegion
            //CurrentGames.text = PhotonNetwork.countOfRooms + " Games";
            //CurrentPlayers.text = PhotonNetwork.countOfPlayers + " Players Online";

            if (isFirstRun)
            {
                isFirstRun = false;

                if (SteamLobbyId != 0)
                {
                    Hide();
                    StartMenu.Instance.Hide();
                }
                else
                {
#if UNITY_EDITOR
#endif
                }
            }

            Countess.SetActive(true);
        }

        void OnDisable()
        {
            if (Countess != null)
                Countess.SetActive(false);
        }

        public void Options()
        {
            OptionsMenu.Instance.Show();
        }

        public void Quit()
        {
            //print("MainMenu.Quit");
            Hide();
            StartMenu.Instance.Show();
        }

        void UpdateItemIcons()
        {
            //foreach (Transform child in Upgrades)
            //{
            //    Destroy(child.gameObject);
            //}

            //foreach (ItemId id in Enum.GetValues(typeof(ItemId)))
            //{
            //    var type = ItemBase.Get(id);
            //    if (type != null && !type.Name.StartsWith("Item"))
            //    {
            //        var icon = Instantiate(Level.Instance.ItemIconPrefab) as GameObject;
            //        icon.transform.SetParent(Upgrades, false);
            //        icon.GetComponent<ItemIcon>().UpdateIcon(type == null ? null : type, !SaveData.Current.IsFoundItem(id));
            //    }
            //}
        }
    }
}