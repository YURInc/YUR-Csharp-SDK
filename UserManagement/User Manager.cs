using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using YUR.SDK.Unity.UserData;

namespace YUR.SDK.Unity.UserManagement
{
    public partial class YUR_UserManager : MonoBehaviour
    {
        public static YUR_UserManager YUR_Users;
        public YUR_CurrentUser CurrentUser;

        /// <summary>
        /// Login events
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public delegate string Login_Status_Changed(string response);
        public static event Login_Status_Changed Successful_Login;
        public static event Login_Status_Changed Logging_In;
        public static event Login_Status_Changed Bad_Login;
        public static event Login_Status_Changed Log_Out;

        void Awake()
        {
            YUR_Users = this;
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(YUR_Users);
            CurrentUser = new YUR_CurrentUser();
            CurrentUser = gameObject.AddComponent<YUR_CurrentUser>();
            Successful_Login += Users_Successful_Login;
            Bad_Login += Users_Bad_Login;
            Logging_In += Users_Logging_In;
        }

        /// <summary>
        /// Retrieve list of Previously logged in users user id's from local storage
        /// </summary>
        /// <param name="UserID_List"></param>
        /// <returns>True if users found, otherwise false</returns>
        public bool GET_LIST_USERS_IDS(ref List<string> UserID_List)
        {
            try
            {
                YUR_Log.Log("Retrieving List of User IDS");
                UserID_List = new List<string>();

                string usersFolder = Utilities.YUR_Conversions.PathCombine(Application.persistentDataPath, YUR_Constants.USERS_FILEPATH);
                Directory.CreateDirectory(usersFolder);
                List<string> list = Directory.GetFiles(usersFolder, "*.json", SearchOption.TopDirectoryOnly).ToList();
                YUR_Log.Log("List of User Files acquired");
                if (list.Count < 1)
                {
                    return false;
                }
                foreach (string filename in list)
                {
                    int length = usersFolder.Length;
                    YUR_Log.Log("Length to index: " + length);
                    YUR_Log.Log("Users Folder Path: " + usersFolder);
                    YUR_Log.Log("        File Path: " + filename);
                    string userID = filename.Substring(0, filename.Length - 5);

                    YUR_Log.Log("User ID: " + userID.Substring(length));
                    UserID_List.Add(userID.Substring(length));
                }
            }
            catch (System.Exception e)
            {
                YUR_Log.Error(e.ToString());
            }

                return true;

            }

    }
}