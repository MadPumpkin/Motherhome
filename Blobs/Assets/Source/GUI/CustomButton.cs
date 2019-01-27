using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Legend
{
    [RequireComponent(typeof(Selectable))]
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, ISubmitHandler
    {
        // prevents multiple items being selected at once
        // https://forum.unity3d.com/threads/button-keyboard-and-mouse-highlighting.294147/
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!EventSystem.current.alreadySelecting)
                EventSystem.current.SetSelectedGameObject(this.gameObject);
        }

        public void OnSelect(BaseEventData eventData)
        {
            //print("OnSelect");
            if (CameraController.Instance != null)
                CameraController.Instance.Ears.PlaySound(World.Instance.ButtonSelect);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            this.GetComponent<Selectable>().OnPointerExit(null);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //print("I was clicked");
            CameraController.Instance.Ears.PlaySound(World.Instance.ButtonSubmit);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            //print("I was clickedfdsafdsa");
            CameraController.Instance.Ears.PlaySound(World.Instance.ButtonSubmit);
        }
    }
}