using Legend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class HUD : MonoBehaviour
{
    public static HUD Instance;

    public Text NotificationLabel;
    //public Text TimerLabel;
    public CommandOption[] CommandOptions;

    public GameObject TextInputRoot;
    public InputField TextInput;
    public RectTransform InventoryCounterParent;
    public DialogWindow DialogWindow;

    //public Text Ammo;
    //public Text MaxAmmo;
    public Text Coins;
    public Text Bombs;
    public Text Keys;
    public GridLayoutGroup HealthReadout;
    public Text ConsumableLabel;
    public Slider ManaSlider;
    public GameObject Menus;
    public GameObject HideableHud;
    public GameObject ChatLog;
    public GameObject ChatMessagePrefab;

    public Slider BossHealthMeter;
    public Text BossNameLabel;

    public int ChatLogLimit = 30;
    public bool ShowChatLog = false;

    //void OnGUI()
    //{
    //    GUILayout.Label(Camera.main.transparencySortMode + " " + Camera.main.transparencySortAxis);
    //}

    public HUD()
    {
        Instance = this;
    }

    void Awake()
    {
        Menus.SetActive(true);
        TextInputRoot.SetActive(false);
        ClearCommandOptions();
        DialogWindow.gameObject.SetActive(false);
        BossHealthMeter.gameObject.SetActive(false);
        ChangeWeapon();

        foreach (Transform child in ChatLog.transform)
            Destroy(child.gameObject);
    }

    public void Show()
    {
        HideableHud.SetActive(true);
    }

    public void Hide()
    {
        HideableHud.SetActive(false);
    }

    public void Toggle()
    {
        HideableHud.SetActive(!HideableHud.activeSelf);
    }

    void OnDisable()
    {
        NotificationLabel.enabled = false;
    }

    public void ChangeWeapon() // call this when weapon changes
    {
        //if (Player.Instance != null)
        //    MaxAmmo.text = Player.Instance.Weapon.ClipSize.ToString();
    }

    bool IsSomethingSelected()
    {
        var selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (selected == null)
            return false;
        var selectable = selected.GetComponent<Selectable>();
        if (selectable == null)
            return false;
        return selected.activeInHierarchy && selectable.enabled && selectable.interactable;
    }

    int lastBombs = -1;
    int lastCoins = -1;
    int lastKeys = -1;

    void Update()
    {
        if (!TextInputRoot.activeSelf)
        {
            if (Player.Instance != null && Input.GetButtonDown("Talk") && !IsSomethingSelected())
            {
                foreach (var child in ChatLog.GetComponentsInChildren<Animator>())
                {
                    child.Play("Show");
                }

                TextInputRoot.SetActive(true);
                TextInput.ActivateInputField();

                if (Player.Instance != null)
                {
                    Player.Instance.Movement.MovementEnabled = false;
                }
                else
                {
                    TextInput.textComponent.color = Color.white;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                TextInput.text = Settings.LastChat;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseChatInput();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                var text = TextInput.text.Trim();
                if (text.Length > 0)
                {
                    Settings.LastChat = text;

                    if (text.StartsWith("/"))
                    {
                        //ShowMessage(text, Utility.Green);
                        Console.OnOutput = (s) => ShowMessage(s, Color.white, false);
                        Console.ExecuteCommand(text.Substring(1));
                        Console.OnOutput = null;
                    }
                    else
                    {
                        if (text.Length > 512)
                            text = text.Substring(0, 512);

                        //if (Player.Instance != null)
                        //    Player.Instance.CmdBroadcast("[" + Player.Instance.name + "] " + text, Player.Instance.Visual.NameColorValue);
                        //else
                            ShowMessage(text, Utility.Green);
                    }
                }

                TextInput.text = "";
                CloseChatInput();
            }
        }

        if (Player.Instance == null)
            return;

        //if (GameMatch.Instance != null && GameMatch.Instance.ShowTimer)
        //{
        //    TimerLabel.enabled = true;
        //    TimerLabel.text = String.Format("{0:N1}s", GameMatch.Instance.TimeToNextPhase - Time.time);
        //}
        //else
        //{
        //    TimerLabel.enabled = false;
        //}

        //if (Player.Instance.Room == null || Player.Instance.Room.Bosses == null)
        //{
        //    if (BossHealthMeter.gameObject.activeSelf)
        //    {
        //        BossHealthMeter.gameObject.SetActive(false);
        //    }
        //}
        //else
        //{
        //    var bossHealth = 0f;
        //    var maxBossHealth = 0f;
        //    var bossName = "";
        //    foreach (var boss in Player.Instance.Room.Bosses)
        //    {
        //        if (boss != null && !boss.IsDead)
        //        {
        //            var damageable = boss.GetComponent<Damageable>();
        //            bossHealth += damageable.Health;
        //            maxBossHealth += damageable.MaxHealth;
        //            bossName = boss.name;
        //        }
        //    }

        //    if (maxBossHealth > 0)
        //    {
        //        if (!BossHealthMeter.gameObject.activeSelf)
        //        {
        //            BossHealthMeter.gameObject.SetActive(true);
        //        }
        //        BossHealthMeter.value = bossHealth / maxBossHealth;
        //        BossNameLabel.text = bossName;
        //    }
        //    else
        //    {
        //        if (BossHealthMeter.gameObject.activeSelf)
        //        {
        //            BossHealthMeter.gameObject.SetActive(false);
        //        }
        //    }
        //}
    }

    private void CloseChatInput()
    {
        TextInputRoot.SetActive(false);

        if (Player.Instance != null)
            Player.Instance.Movement.MovementEnabled = true;

        foreach (var child in ChatLog.GetComponentsInChildren<Animator>())
        {
            child.Play("Hidden");
        }

        var selectable = FindObjectsOfType<Selectable>().Where(s => s.enabled && s.interactable).FirstOrDefault();
        if (selectable != null)
        {
            selectable.Select();
            selectable.OnSelect(null);
        }

        //TextInput.ActivateInputField();
    }

    public void ShowMessage(string message = "", Color color = default(Color), bool echoToConsole = true)
    {
        if (String.IsNullOrEmpty(message))
            return;

        if (ChatLog.transform.childCount >= ChatLogLimit)
        {
            var child = ChatLog.transform.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        if (echoToConsole)
            print(message);

        if (message.Length > 2048)
            message = message.Substring(0, 2048);

        if (color == default(Color))
            color = Color.white;

        var chatMessage = this.Spawn(ChatMessagePrefab, ChatLog.transform);
        var label = chatMessage.GetComponentInChildren<Text>();
        label.text = message;
        label.color = color;
        chatMessage.transform.localScale = Vector3.one;
    }

    internal GameObject SetCommandOption(InputAction icon, string text)
    {
        ClearCommandOptions();
        return AddCommandOption(icon, text);
    }

    internal GameObject AddCommandOption(InputAction icon, string text)
    {
        var result = CommandOptions[(int)icon];

        if (text == null)
        {
            result.gameObject.SetActive(false);
            return result.gameObject;
        }

        if (!result.gameObject.activeSelf)
            result.gameObject.SetActive(true);
        var option = result.GetComponentInChildren<CommandOption>();
        option.Set(icon, text);

        return result.gameObject;
    }

    public void ClearCommandOptions()
    {
        foreach (var option in this.CommandOptions)
        {
            if (option != null)
                option.gameObject.SetActive(false);
        }
    }

    //Coroutine hideNotificationLabel;

    public void ShowFlavorText(string text, Color color = default(Color))
    {
        //print(text);
        if (color == default(Color))
            color = Color.white;
        NotificationLabel.text = text;
        NotificationLabel.color = color;
        //NotificationLabel.enabled = !String.IsNullOrEmpty(text);
        //NotificationLabel.GetComponent<TypewriterEffect>().Restart();

        NotificationLabel.GetComponent<Animator>().Play("Play");

        //if (hideNotificationLabel != null)
        //    StopCoroutine(hideNotificationLabel);
        //hideNotificationLabel = this.Delay(() =>
        //{
        //    NotificationLabel.enabled = false;
        //}, time);
    }
}
