using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    public class Counter : MonoBehaviour
    {
        public string Name;
        public float DefaultValue;
        public Sprite Icon;
        public Color Color = Color.white;
        public string Format = "{0}";
        public bool ShowVisual;

        [NonSerialized]
        public float Total;
        [NonSerialized]
        public float Current;

        //Image image;
        //Text text;
        //GameObject guiObject;

        void Awake()
        {
            Current = DefaultValue;
            Total = DefaultValue;
            UpdateGUI();
        }

        public virtual void Increment(float amount = 1)
        {
            Total += amount;
            Current += amount;

            UpdateGUI();

            //hud.AddCombatText(String.Format("+{0} xp", amount), Utility.ExperienceColor);

            //Xp += amount;
            //TotalXp += amount;
            //if (Xp >= XpToNextLevel)
            //{
            //    LevelUp();
            //    return true;
            //}
            //return false;
        }

        public virtual bool Decrement(float amount)
        {
            if (Current >= amount)
            {
                Current -= amount;
                UpdateGUI();
                return true;
            }
            return false;
        }

        public void UpdateGUI()
        {
            if (ShowVisual)
            {
                //if (guiObject == null)
                //    CreateGUI();

                //if (Current == DefaultValue)
                //{
                //    if (guiObject.activeSelf)
                //        guiObject.SetActive(false);
                //}
                //else
                //{
                //    if (!guiObject.activeSelf)
                //        guiObject.SetActive(true);

                //    text.text = String.Format(Format, Current, Total);
                //}
            }
        }

        void CreateGUI()
        {
            //guiObject = new GameObject(Name);
            //guiObject.transform.SetParent(Inventory.GUIParent);
            //guiObject.transform.localScale = Vector3.one;
            //guiObject.transform.localPosition = Vector3.zero;

            //image = guiObject.AddComponent<Image>();
            //image.rectTransform.sizeDelta = new Vector2(Inventory.FontSize, Inventory.FontSize);
            //image.rectTransform.anchoredPosition = Vector3.zero;
            //image.color = Color;
            //image.sprite = Icon;

            //var outline = guiObject.AddComponent<Outline>();
            //outline.effectColor = Color.black;
            //outline.effectDistance /= 2;

            //var textObject = new GameObject("Text");
            //textObject.transform.SetParent(guiObject.transform);
            //textObject.transform.localScale = Vector3.one;

            //text = textObject.AddComponent<Text>();
            //text.rectTransform.sizeDelta = new Vector2(200, Inventory.FontSize * 2);
            //text.rectTransform.anchorMin = Vector2.zero;
            //text.rectTransform.anchorMax = Vector2.one;
            //text.rectTransform.anchoredPosition = new Vector2(0.5f, 1);
            //text.rectTransform.localPosition = new Vector3(Inventory.FontSize + 100, 0, 0);
            //text.font = Inventory.Font;
            //text.fontSize = Inventory.FontSize;
            //text.color = Color;
            //text.alignment = TextAnchor.MiddleLeft;

            //outline = textObject.AddComponent<Outline>();
            //outline.effectColor = Color.black;
            //outline.effectDistance /= 2;
        }

        public virtual CounterData Save()
        {
            var result = new CounterData() { Name = Name, Total = Total, Current = Current };
            return result;
        }
    }
}

