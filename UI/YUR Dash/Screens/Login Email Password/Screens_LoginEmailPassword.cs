using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace YUR.SDK.Unity.UI
{
    public class Screens_LoginEmailPassword : YURScreenController
    {
        public static Screens_LoginEmailPassword inst;

        public GameObject LoginIssues;
        public GameObject EmailInput;
        public GameObject PasswordInput;
        public GameObject Submit;

        void Awake()
        {
            inst = this;
            Finished += Screens_LoginEmailPassword_Finished;
            BackButtonPressed += Screens_LoginEmailPassword_BackButtonPressed;
        }

        void OnEnable()
        {
            YUR_Log.Log("Enabled Screens_LoginEmailPassword");
            YURScreenCoordinator.ScreenCoordinator.Keyboard.SetActive(true);
        }

        private void Screens_LoginEmailPassword_BackButtonPressed()
        {
            EmailInput.GetComponent<YURInputSetup>().Input.text = "";
            PasswordInput.GetComponent<YURInputSetup>().Input.text = "";
        }

        public void RelativeLayout(RectTransform rt, float x, float y, float w, float h, float pivotx)
        {
            rt.anchorMin = new Vector2(x, y);
            rt.anchorMax = new Vector2(x + w, y + h);
            rt.pivot = new Vector2(pivotx, 1f);
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }

        private void Screens_LoginEmailPassword_Finished()
        {
            
            //LoginIssues = (GameObject)Instantiate(Resources.Load("YUR Error Report"), gameObject.transform);
            //Submit = (GameObject)Instantiate(Resources.Load("YUR Selection Button"), gameObject.transform);
            //EmailInput = (GameObject)Instantiate(Resources.Load("YUR Input"), gameObject.transform);
            //PasswordInput = (GameObject)Instantiate(Resources.Load("YUR Input"), gameObject.transform);

            //EmailInput.GetComponent<RectTransform>().localPosition = new Vector3(0, 39.5f, EmailInput.GetComponent<RectTransform>().localPosition.z);
            EmailInput.GetComponent<YURInputSetup>().Input.text = "";
            EmailInput.GetComponent<YURInputSetup>().Label.text = "Email";
            EmailInput.GetComponent<YURInputSetup>().PlaceHolder.text = "i.e. \"username@gmail.com\"";

            //PasswordInput.GetComponent<RectTransform>().localPosition = new Vector3(0, -26.9f, PasswordInput.GetComponent<RectTransform>().localPosition.z);
            PasswordInput.GetComponent<YURInputSetup>().Input.text = "";
            PasswordInput.GetComponent<YURInputSetup>().Label.text = "Password";
            PasswordInput.GetComponent<YURInputSetup>().PlaceHolder.text = "6 Characters or more";

            //Submit.GetComponent<RectTransform>().localPosition = new Vector3(0, -111, PasswordInput.GetComponent<RectTransform>().localPosition.z);
            Submit.GetComponentInChildren<TextMeshProUGUI>().text = "Submit";

            LoginIssues.GetComponentInChildren<TextMeshProUGUI>().text = "";

            Submit.GetComponent<Button>().onClick.AddListener(delegate
            {
                UserManagement.YUR_UserManager.Successful_Login += YUR_UserManager_Successful_Login;
                UserManagement.YUR_UserManager.Bad_Login += YUR_UserManager_Bad_Login;
                string Login_Issues;
                if (Login.Email_Password(EmailInput.GetComponent<YURInputSetup>().Input.text, PasswordInput.GetComponent<YURInputSetup>().Input.text, out Login_Issues))
                {
                    YURScreenCoordinator.ScreenCoordinator.Keyboard.SetActive(false);
                }
                LoginIssues.GetComponentInChildren<TextMeshProUGUI>().text = Login_Issues;
            });
        }

        private string YUR_UserManager_Bad_Login(string response)
        {
            YURScreenCoordinator.ScreenCoordinator.Keyboard.SetActive(true, KeyboardCanvas.KeyboardStyle.KeyboardNumPad);
            LoginIssues.GetComponentInChildren<TextMeshProUGUI>().text = response;
            UserManagement.YUR_UserManager.Successful_Login -= YUR_UserManager_Successful_Login;
            UserManagement.YUR_UserManager.Bad_Login -= YUR_UserManager_Bad_Login;
            return response;
        }

        private string YUR_UserManager_Successful_Login(string response)
        {
            UserManagement.YUR_UserManager.Successful_Login -= YUR_UserManager_Successful_Login;
            UserManagement.YUR_UserManager.Bad_Login -= YUR_UserManager_Bad_Login;
            YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_MainMenu.inst, true);
            return response;
        }
    }
}