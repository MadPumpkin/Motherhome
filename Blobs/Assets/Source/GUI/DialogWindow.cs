using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    public enum DialogWindowType
    {
        Okay,
        YesNo
    }

    public enum DialogWindowResult
    {
        Okay,
        Yes,
        No
    }

    public class DialogWindow : MonoBehaviour
    {
        const float messageDisplayTime = 5f;

        public Text Header;
        public Text Content;
        public bool IsClosedOnKeyPress = true;
        public GameObject PromptWindow;
        public Button[] PromptWindowButtons;

        [NonSerialized]
        public DialogWindowResult Result;

        DialogWindowType type;
        float showMessageUntil;
        internal Action OnClose;
        string[] messages;
        int currentMessage;
        //TypewriterEffect contentTyper;

        void Update()
        {
            if (Time.unscaledTime > showMessageUntil)
            {
                //if (Input.anyKeyDown)
                    NextMessage();
            }
            else
            {
                if (Input.anyKeyDown)
                {
                    Content.GetComponent<TypewriterEffect>().Complete();
                    showMessageUntil = Time.unscaledTime;
                }
            }
        }

        void ProcessMessages()
        {
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = messages[i].Replace("\\n", "\n");
            }
            //var textGen = new TextGenerator();
            //var generationSettings = Content.GetGenerationSettings(Content.rectTransform.rect.size);
            //var width = textGen.GetPreferredWidth(newText, generationSettings);
            //var height = textGen.GetPreferredHeight(newText, generationSettings);
        }

        public void Show(string title, string[] messages, DialogWindowType type = DialogWindowType.Okay)
        {
            //print("Show");

            Cursor.visible = true;

            this.type = type;
            this.messages = messages;
            ProcessMessages();

            //foreach (var line in messages)
            //{
            //    print(line);
            //}

            gameObject.SetActive(true);
            transform.localScale = Vector3.zero; // to avoid intial frame glitch on first show
            PromptWindow.SetActive(false);

            Header.text = title;
            Header.GetComponent<TypewriterEffect>().Restart();

            currentMessage = -1;
            NextMessage();

            GetComponent<Animator>().SetTrigger("Show");

            //Time.timeScale = 0;
        }

        public void NextMessage()
        {
            //Player.Instance.PlayBeeps(6);

            currentMessage++;
            if (currentMessage >= messages.Length)
            {
                ClearMessage();
            }
            else
            {
                //print("next message");
                showMessageUntil = Time.unscaledTime + messageDisplayTime;
                Content.text = messages[currentMessage];
                Content.GetComponent<TypewriterEffect>().Restart();

                if (type != DialogWindowType.Okay && currentMessage >= messages.Length - 1)
                {
                    PromptWindow.SetActive(true);
                    PromptWindowButtons[0].Select();
                    PromptWindowButtons[0].OnSelect(null); // https://answers.unity.com/questions/1159573/eventsystemsetselectedgameobject-doesnt-highlight.html
                }
            }
        }

        public void ClearMessage()
        {
            //print("ClearMessage");

            if (type != DialogWindowType.Okay)
            {
                PromptWindow.SetActive(true);
            }
            else
            {
                //print("clear message");
                //Time.timeScale = 1;
                Close();
            }
        }

        public void Close()
        {
            Cursor.visible = false;

            GetComponent<Animator>().SetTrigger("Hide");
            this.Delay(() =>
            {
                gameObject.SetActive(false);

                if (OnClose != null)
                {
                    var onClose = OnClose; // copy the value since we need to clear it before it gets run, since it might get set inside and we want that preserved. QuestGiver will break.
                    OnClose = null;
                    onClose();
                }
            }, 0.25f);
        }

        public void OptionPicked(int id)
        {
            switch (type)
            {
                case DialogWindowType.YesNo:
                    if (id == 0)
                        Result = DialogWindowResult.Yes;
                    else
                        Result = DialogWindowResult.No;
                    break;
            }

            Close();
        }
    }
}