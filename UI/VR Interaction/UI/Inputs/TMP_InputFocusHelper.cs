﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

namespace VRUiKits.Utils
{
    public class TMP_InputFocusHelper : MonoBehaviour, ISelectHandler
    {
        private TMP_InputField input;

        void Awake()
        {
            input = GetComponent<TMP_InputField>();
        }

        public void OnSelect(BaseEventData eventData)
        {
            /*
            Set keyboard target explicitly for some 3rd party packages which lost focus when
            user click on keyboard.
            */
            TMP_KeyboardManager.Target = input;
            StartCoroutine(ActivateInputFieldWithCaret());
        }

        IEnumerator ActivateInputFieldWithCaret()
        {
            input.ActivateInputField();

            yield return new WaitForEndOfFrame();

            if (EventSystem.current.currentSelectedGameObject == input.gameObject)
            {
                input.MoveTextEnd(false);
            }
        }
    }
}