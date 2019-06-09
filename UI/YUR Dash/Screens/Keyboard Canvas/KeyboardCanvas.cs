using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUR.SDK.Unity.UI
{
    public class KeyboardCanvas : MonoBehaviour
    {
        public Canvas GetCanvas { get { return canvas; } set { canvas = value; } }
        private Canvas canvas;

        public GameObject Keyboard;

        public enum KeyboardStyle
        {
            KeyboardNumPad
        }

        public void SetActive(bool isVisible, KeyboardStyle style = KeyboardStyle.KeyboardNumPad)
        {
            gameObject.SetActive(isVisible);
            Keyboard.SetActive(isVisible);
        }
    }
}