using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace Legend
{
    public class MenuRoot : MonoBehaviour
    {
        public static MenuRoot Instance;

        public Image BlurImage;
        public Image DarkBackground;

        public MenuRoot()
        {
            Instance = this;
        }

        public void Show(bool showBackground = true)
        {
            //print("MenuRoot.Show");
            //iTween.FadeTo(gameObject, 0.5f, 0.5f);
            gameObject.SetActive(true);
            BlurImage.enabled = showBackground;

            // this is to prevent the random z-fighting bug on the blur when looking at certain directions
            // to test that bug turn this off and pause while looking in many directions, sometimes blur will not happen
            //BlurImage.gameObject.transform.position += new Vector3(0, 0, 0.1f);

            DarkBackground.enabled = showBackground;
        }

        public void Hide()
        {
            if (gameObject.activeSelf)
            {
                //iTween.FadeTo(gameObject, 0, 0.5f);
                BlurImage.enabled = false;
                //this.Delay(() =>
                //{
                    gameObject.SetActive(false);
                    DarkBackground.enabled = false;
                //}, 0.5f);
            }
        }
    }
}