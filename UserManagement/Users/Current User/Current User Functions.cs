using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

namespace YUR.SDK.Unity
{
    public partial class YUR_CurrentUser : MonoBehaviour
    {

        /// <summary>
        /// Stores logged in Users refresh token and identifiers to local storage. To be called after all data is acquired.
        /// </summary>
        public static bool Store_RefreshToken(string Name, string PhotoURL, string Refresh_Token)
        {
            var userFolder = Utilities.YUR_Conversions.PathCombine(Application.persistentDataPath, YUR_Constants.USERS_FILEPATH);
            var userFolderPath = Utilities.YUR_Conversions.PathCombine(userFolder, YUR_Main.main.User_Manager.CurrentUser.loginCredentials.LocalId + ".json");
            YUR_Log.Log("Store_RefreshToken() userFold path: " + userFolderPath);
            Local_User_Info_Reference userInfo = new Local_User_Info_Reference(Name, PhotoURL, Refresh_Token);
            YUR_Log.Log("Setup data, ready to store");
            var data = JsonUtility.ToJson(userInfo);
            File.WriteAllText(userFolderPath, data);
            return true;
        }

        /// <summary>
        /// Reads the data stored for the provided USERID
        /// </summary>
        /// <param name="USERID">Unique user Identifier</param>
        public static Local_User_Info_Reference Preview_User(string USERID)
        {
            var userDataPath = Path.Combine(Application.persistentDataPath, YUR_Constants.USERS_FILEPATH);
            var userFile = Path.Combine(userDataPath, USERID + ".json");
            return JsonUtility.FromJson<Local_User_Info_Reference>(File.ReadAllText(userDataPath));

        }

        /// <summary>
        /// Change response from Refresh Token login into valid login credentials
        /// </summary>
        /// <param name="jsonResponse"></param>
        /// <param name="yURaccount"></param>
        /// <returns></returns>
        public bool Convert_Refresh_Login(string jsonResponse)
        {
            try
            {
                YUR_Log.Log("Extracting data from class: " + jsonResponse);
                Temp_Class temp_class = new Temp_Class();
                temp_class = JsonUtility.FromJson<Temp_Class>(jsonResponse);

                YUR_Log.Log("Temporary class: " + JsonUtility.ToJson(temp_class));

                loginCredentials.LocalId = temp_class.user_id;
                loginCredentials.RefreshToken = temp_class.refresh_token;
                loginCredentials.IDtoken = temp_class.id_token;

                YUR_Log.Log("Conversion, items passed: " + JsonUtility.ToJson(this));
                if (loginCredentials.IDtoken == "" || loginCredentials.IDtoken == string.Empty || loginCredentials.IDtoken == null)
                    return false;
                return true;
            }
            catch(Exception e)
            {
                YUR_Log.Error("Convert Refresh Login Failed with Error: " + e);
                return false;
            }
        }

        public class Temp_Class
        {
            public string id_token;
            public string refresh_token;
            public string user_id;
        }

        public class Local_User_Info_Reference
        {
            public string name;
            public string photoURL;
            public string refresh_token;

            public Local_User_Info_Reference()
            {

            }

            public Local_User_Info_Reference(string Name, string PhotoURL, string Refresh_Token)
            {
                name = Name;
                photoURL = PhotoURL;
                refresh_token = Refresh_Token;
            }
            //public void Setup()
            //{
               
            //    name = YUR_Main.main.User_Manager.ActiveUser.Data_Biometrics.Name;
            //    photoURL = YUR_Main.main.User_Manager.ActiveUserAccount.Profile.PhotoURL;
            //    refresh_token = YUR_Main.main.User_Manager.ActiveUserAccount.RefreshToken;
            //}
        }
    }
}