using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    public class CombatText : MonoBehaviour
    {
        static List<CombatText> texts = new List<CombatText>();

        public float MaxAge;
        public Vector3 InitialOffset;
        public Vector3 Movement;

        Text text;

        void Start()
        {
            texts.Add(this);
            Destroy(gameObject, MaxAge);
            transform.position += InitialOffset;
            //CheckForOverlap();
        }

        void OnDestroy()
        {
            texts.Remove(this);
        }

        void Update()
        {
            transform.position += Movement * Time.deltaTime;
        }

        void CheckForOverlap()
        {
            CheckForOverlap(texts.OrderBy((t) => t.transform.position.y));
        }

        void CheckForOverlap(IEnumerable<CombatText> texts)
        {
            if (text == null)
                return;

            //print("CheckForOverlap");

            var myBounds = GetBounds();
            foreach (var combatText in texts)
            {
                //print("dfsa");
                //print(GetBounds());
                //print(combatText.GetBounds());

                if (combatText != this && combatText.text != null)
                {
                    var targetBounds = combatText.GetBounds();
                    //print(targetBounds);

                    if (myBounds.Overlaps(targetBounds))// && myBounds.y <= targetBounds.y)
                    {
                        combatText.transform.position = new Vector3(combatText.transform.position.x, myBounds.y + 0.51f);
                        myBounds.y = combatText.transform.position.y;
                    }
                }
            }
        }

        Rect GetBounds()
        {
            var result = text.rectTransform.rect;
            result.width *= text.rectTransform.localScale.x;
            result.height *= text.rectTransform.localScale.y;
            result.x = transform.position.x;
            result.y = transform.position.y;
            return result;
        }

        internal static void Add(string message, Color color, Vector3 position, Transform parent = null)
        {
            //print(message);
            var newGameObject = Instantiate(World.Instance.CombatTextPrefab) as GameObject;
            newGameObject.transform.SetParent(parent);
            //newGameObject.transform.localScale = parent.localScale;
            newGameObject.transform.position = position;

            var combatText = newGameObject.GetComponentInChildren<CombatText>();

            combatText.text = newGameObject.GetComponentInChildren<Text>();
            combatText.text.text = message;
            combatText.text.color = color;

            var gradient = newGameObject.GetComponentInChildren<UIGradient>();
            gradient.StartColor = color;
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            v -= 0.33f;
            gradient.EndColor = Color.HSVToRGB(h, s, v);

            iTween.FadeTo(combatText.gameObject, 0, combatText.MaxAge);
        }
    }
}