using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace YUR.SDK.Unity.UI { 
    public class BirthDateHelpers : MonoBehaviour
    {
        public TextMeshProUGUI PlaceHolder;
        public TextMeshProUGUI Input;

        //[HideInInspector]
        //public TMP_InputField InputField;

        [HideInInspector]
        public VRUiKits.Utils.UIKitInputField InputField;

        private void Awake()
        {
            InputField = gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>();

        }
    }
}