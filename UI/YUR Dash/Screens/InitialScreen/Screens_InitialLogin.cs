using UnityEngine;
using UnityEngine.UI;

namespace YUR.SDK.Unity.UI
{
    public class Screens_InitialLogin : YURScreenController
    {
        public static Screens_InitialLogin inst;

        public Button Login;
        public Button SwitchUsers;
        public Button CreateAccount;

        private bool setup = false;
        public void Awake()
        {
            inst = this;
            Debug.Log("Inital Login SCreen loaded");
            Finished += Screens_LoginEmailPassword_Finished;
        }

        public void OnEnable()
        {
            if(setup)
            {
                if(SwitchUsers.gameObject.activeSelf)
                {
                    return;
                }
                if (YUR_Main.main.UserList.Count > 0)
                {
                    SwitchUsers.gameObject.SetActive(true);
                    SwitchUsers.onClick.AddListener(delegate
                    {
                        YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_UserSelection.instance, false);
                    });
                }
                else
                {
                    SwitchUsers.gameObject.SetActive(false);
                }


            }
        }

        private void Screens_LoginEmailPassword_Finished()
        {
            SetBackButtonVisible(false);
            
            if(YUR_Main.main.UserList.Count > 0)
            {
                SwitchUsers.gameObject.SetActive(true);
                SwitchUsers.onClick.AddListener(delegate
                {
                    YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_UserSelection.instance, false);
                });
            }
            else
            {
                SwitchUsers.gameObject.SetActive(false);
            }

            CreateAccount.onClick.AddListener(delegate
            {
                YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_CreateAccount.instance, false);
            });

            Login.onClick.AddListener(delegate
            {
                YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_LoginEmailPassword.inst);
            });
            setup = true;
        }
    }

}