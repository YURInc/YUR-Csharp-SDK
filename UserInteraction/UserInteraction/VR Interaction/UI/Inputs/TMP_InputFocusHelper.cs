using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using TMPro;

namespace VRUiKits.Utils
{
    public class TMP_InputFocusHelper : MonoBehaviour, ISelectHandler
    {
        private VRUiKits.Utils.UIKitInputField input;

        void Awake()
        {
            input = GetComponent<VRUiKits.Utils.UIKitInputField>();
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