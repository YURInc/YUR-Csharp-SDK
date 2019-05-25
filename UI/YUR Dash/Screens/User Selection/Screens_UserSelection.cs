using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace YUR.SDK.Unity.UI
{
    public class Screens_UserSelection : YURScreenController
    {
        public static Screens_UserSelection instance;

        [Header("Screen Features")]
        public TextMeshProUGUI ProfileName;
        public Image ProfilePicture;

        public int UserIndex;
        public int UserCount;
        public string[] UsersNames;

        [Header("Buttons")]
        public Button Next;
        public Button Prev;
        public Button SignInUser;
        public Button NewSignIn;


        private void Awake()
        {
            instance = this;
        }

        public void OnEnable()
        {
            
            UserCount = YUR_Main.main.UserList.Count;
            UsersNames = new string[UserCount];
            for (int i = 0; i < UserCount; i++)
            { 
                if(YUR_Main.main.UserList[i] == YUR_Main.main.User_Manager.CurrentUser.loginCredentials.LocalId)
                {
                    UserIndex = i;
                    
                }
            }

            if(Login.Status == Login.StatusType.Logged_In)
            {
                ProfileName.text = YUR_Main.main.User_Manager.CurrentUser.Data_Biometrics.Name;
                /// Get User picture here
                /// 
                UsersNames[UserIndex] = YUR_Main.main.User_Manager.CurrentUser.Data_Biometrics.Name;
            }
            else
            {
                UserIndex = 0;
                YUR_CurrentUser.Local_User_Info_Reference Reference = YUR_CurrentUser.Preview_User(YUR_Main.main.UserList[UserIndex]);

                ProfileName.text = Reference.name;
                UsersNames[UserIndex] = Reference.name;
            }

            NewSignIn.onClick.AddListener(delegate
            {
                Logout.ActiveUser("Logging out from switch user screen");
                YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_InitialLogin.inst, true);
            });

            SignInUser.onClick.AddListener(delegate
            {
                if (YUR_Main.main.UserList[UserIndex] == YUR_Main.main.User_Manager.CurrentUser.loginCredentials.LocalId)
                {
                    YURScreenCoordinator.ScreenCoordinator.PresentNewScreen(Screens_MainMenu.inst, true);
                }
                else
                {
                    UserManagement.YUR_UserManager.Successful_Login += YUR_UserManager_Successful_Login;
                    UserManagement.YUR_UserManager.Bad_Login += YUR_UserManager_Bad_Login;
                    Login.User_ID(YUR_Main.main.UserList[UserIndex]);

                }
            });

            Next.onClick.AddListener(delegate
            {
                if (UserIndex + 1 > UserCount - 1)
                {
                    UserIndex = 0;
                }
                else
                {
                    UserIndex++;
                }

                if(!string.IsNullOrEmpty(UsersNames[UserIndex]))
                {
                    ProfileName.text = UsersNames[UserIndex];
                }
                else
                {
                    YUR_CurrentUser.Local_User_Info_Reference Reference = YUR_CurrentUser.Preview_User(YUR_Main.main.UserList[UserIndex]);

                    ProfileName.text = Reference.name;
                    UsersNames[UserIndex] = Reference.name;
                    
                }

                /// Get User Profile Picture

            });

            Prev.onClick.AddListener(delegate
            {
                if (UserIndex - 1 < 0)
                {
                    UserIndex = UserCount - 1;
                }
                else
                {
                    UserIndex--;
                }
                if (!string.IsNullOrEmpty(UsersNames[UserIndex]))
                {
                    ProfileName.text = UsersNames[UserIndex];
                }
                else
                {
                    YUR_CurrentUser.Local_User_Info_Reference Reference = YUR_CurrentUser.Preview_User(YUR_Main.main.UserList[UserIndex]);

                    ProfileName.text = Reference.name;
                    UsersNames[UserIndex] = Reference.name;

                }
            });
        }

        private string YUR_UserManager_Bad_Login(string response)
        {
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
