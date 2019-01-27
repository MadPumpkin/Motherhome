using System;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    public class CommandOption : MonoBehaviour
    {
        //public InputIcon IconLabel;

        public GameObject Right;
        public Text RightLabel;

        public void Set(InputAction action, string text)
        {
            RightLabel.text = text;
            //IconLabel.Key = action;
        }
    }
}