using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace YUR.SDK.Unity
{
    public class YURInputSetup : VRUiKits.Utils.TMP_InputFocusHelper
    {
        public TextMeshProUGUI Input;
        public TextMeshProUGUI PlaceHolder;
        public TextMeshProUGUI Label;
    }
}
