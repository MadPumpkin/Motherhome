using UnityEngine;
using UnityEngine.UI;

namespace Legend
{
    /// <summary>
    /// Trivial script that fills the label's contents gradually, as if someone was typing.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class TypewriterEffect : MonoBehaviour
    {
        public float TypingDuration = 0.5f;
        public AudioClip[] SoundEffect;

        Text label;
        string content;
        float startTime;
        string lastTextValue;

        void OnEnable()
        {
            startTime = Time.unscaledTime;
            if (label == null)
            {
                label = GetComponent<Text>();
                content = label.text;
            }
            label.text = lastTextValue = "";
        }

        void Update()
        {
            if (label.text != lastTextValue)
            {
                Restart();
            }
            else
            {
                var fraction = (Time.unscaledTime - startTime) / TypingDuration;
                if (fraction < 1)
                {
                    var length = (int)(content.Length * fraction);
                    if (length >= 0)
                        label.text = lastTextValue = content.Substring(0, length);
                }
                else
                {
                    label.text = lastTextValue = content;
                    enabled = false;
                }
            }
        }

        public void Restart()
        {
            if (label != null)
            {
                content = label.text;
                label.text = lastTextValue = "";
            }
            startTime = Time.unscaledTime;
            enabled = true;
            //print("playing sound");
            Camera.main.PlaySound(SoundEffect);
        }

        public void Complete()
        {
            label.text = lastTextValue = content;
            startTime = 0;
            //enabled = false;
        }
    }
}