using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YUR.SDK.Unity.UI {
    public class LoginButtonScript : MonoBehaviour
    {
        public Button LoginButton;
        private void Awake()
        {
            LoginButton = gameObject.GetComponent<Button>();
            LoginButton.onClick.AddListener(delegate
            {
                YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_LoginEmailPassword.inst);
            });
        }
    }
}