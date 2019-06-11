using System.Collections;
using UnityEngine;

namespace YUR.SDK.Unity.UserManagement
{
     public partial class YUR_UserManager : MonoBehaviour
     {
        internal void LogOut(string reason)
        {
            YUR_Main.main.Last_Played_User = YUR_Main.main.User_Manager.CurrentUser.loginCredentials.LocalId;
            Login.status = Login.StatusType.Logged_Out;
            Destroy(YUR_Users.CurrentUser);
            YUR_Users.CurrentUser = new YUR_CurrentUser();
            YUR_Users.CurrentUser = gameObject.AddComponent<YUR_CurrentUser>();
            Log_Out?.Invoke(reason);
        }

        /// <summary>
        /// Create an anonymous account. Name must be provided
        /// </summary>
        /// <param name="users_name"></param>
        internal void Create_Anonymous_Account(string users_name)
        {
            Logging_In?.Invoke(string.Empty);
            StartCoroutine(Acquire_Anonymous_Tokens(users_name));
        }


        /// <summary>
        /// Create an account with a User's Email and Password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="displayName"></param>
        internal void Create_Account_Email_Password(string email, string password, string displayName)
        {
            StartCoroutine(Create_New_Account_Email_Password(email, password, displayName));
        }

        /// <summary>
        /// Login in User with Email and Password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public void Login_Email_Password(string email, string password)
        {
            Logging_In?.Invoke(string.Empty);
            try
            {
                StartCoroutine(Acquire_Access_Tokens(email, password));
            }
            catch (System.Exception e)
            {
                YUR_Log.Error(e.ToString());
            }
           
        }

        /// <summary>
        /// Login user by their userUID. User must be stored locally
        /// </summary>
        /// <param name="userUID">Unique identifier for users</param>
        /// <returns>String displaying login info</returns>
        internal string Login_USERID(string userUID)
        {
            try
            {
                Logging_In?.Invoke("Login using a USER ID Invoked");
                YUR_Log.Server_Log("Retrieving locally stored Refresh token");
                var combine = Utilities.YUR_Conversions.PathCombine(Application.persistentDataPath, YUR_Constants.USERS_FILEPATH);
                var usersPath = Utilities.YUR_Conversions.PathCombine(combine, userUID + ".json");
                YUR_Log.Log("User Found at: " + usersPath);
                var userRefresh = System.IO.File.ReadAllText(usersPath);

                YUR_Log.Log("File contents: " + userRefresh);
                YUR_CurrentUser.Local_User_Info_Reference tmp = new YUR_CurrentUser.Local_User_Info_Reference();
                tmp = Utilities.YUR_Conversions.ConvertStringToObject<YUR_CurrentUser.Local_User_Info_Reference>(userRefresh);
                YUR_Log.Log("Refresh Token: " + tmp.refresh_token);
                YUR_Log.Server_Log("Refresh token acquired, passing to Native DLL");

                StartCoroutine(Get_IDtoken(tmp.refresh_token));
            }
            catch(System.Exception e)
            {
                YUR_Log.Error("Login with Refresh Token Error: " + e);
            }
            YUR_Log.Server_Log("Waiting for response from server");
            return "Logging in User";
        }

        internal IEnumerator SavingData<T>(T ClassToPass)
        {
            if(ClassToPass is UserData.Biometrics)
            {
                StartCoroutine(Set_UserData(YUR_CurrentUser.DataType.biometrics));
            }
            else if (ClassToPass is UserData.GeneralCalorieData)
            {
                StartCoroutine(Set_UserData(YUR_CurrentUser.DataType.general_calories));
            }
            else if (ClassToPass is UserData.GameData)
            {
                StartCoroutine(Set_UserData(YUR_CurrentUser.DataType.game_calories));
            }
            yield break;
        }


        /// <summary>
        /// Subscribed to the Already_Logged_In event
        /// </summary>
        /// <param name="response">Pertinent information</param>
        /// <returns>Pertinent information</returns>
        internal virtual string Users_Already_Logged_In(string response)
        {
            YUR_Log.Server_Log("Login attempt while other user still logged in was made!");
            return response;
        }

        /// <summary>
        /// Subscribed to the Logging in Event
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual string Users_Logging_In(string response)
        {
            Login.status = Login.StatusType.Logging_In;
            YUR_Log.Server_Log("An account began to Log in");
            return response;
        }

        /// <summary>
        /// Subscribed to the Unsuccessful Login Event
        /// </summary>
        /// <param name="response">Debugging information form server</param>
        /// <returns></returns>
        protected virtual string Users_Bad_Login(string response)
        {
            Login.status = Login.StatusType.Logged_Out;
            YUR_Log.Server_Log("Unsuccessful login attempt! Error Information provided");
            YUR_Log.Error("Bad Login: " + response);
            return response;
        }

        /// <summary>
        /// Subscribed to the Successful Login Event
        /// </summary>
        /// <param name="response">Debugging information form server</param>
        /// <returns>String Reporting Login Status</returns>
        protected virtual string Users_Successful_Login(string response)
        {
            Login.status = Login.StatusType.Logged_In;
            YUR_Log.Server_Log("Login attempt was successful!");
            return response;
        }

        /// <summary>
        /// Used for initial login. Gets required login and persitence tokens
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private IEnumerator Acquire_Access_Tokens(string email, string password)
        {
            YUR_Log.Log("Logging in with Email and Password");
            string response;
            yield return response = Systems.Interops.User_AccountAuthorization.Login_User(email, password);
            YUR_Log.Log("Received data from Native DLL: " + response);
            if (response.StartsWith("--1"))
            {
                string error = "Login Credentials are invalid";
                if (response.Contains("EMAIL_NOT_FOUND"))
                {
                    error = "Email does not exist, try again";
                }
                Bad_Login?.Invoke(error);
                yield break;
            }

            YUR_Log.Log("Successfully authenticated, begin retrieving data!");
            YUR_Log.Server_Log("Received Data: " + response);
            yield return CurrentUser.loginCredentials = Utilities.YUR_Conversions.ConvertStringToObject<LoginCredentials>(response);
            YUR_Log.Log("Test");
            YUR_Log.Log("ActiveUserAccount  : " + CurrentUser.loginCredentials.RefreshToken);
            yield return StartCoroutine(Get_UserData());
            YUR_Log.Log("ActiveUserAccount Refresh Token: " + CurrentUser.loginCredentials.RefreshToken);
            YUR_Log.Log("ActiveUser PhotoURL: " + CurrentUser.Profile.PhotoURL);
            YUR_Log.Log("ActiveUserAccount DisplayName: " + CurrentUser.loginCredentials.DisplayName);
            yield return YUR_CurrentUser.Store_RefreshToken(CurrentUser.loginCredentials.DisplayName, CurrentUser.Profile.PhotoURL, CurrentUser.loginCredentials.RefreshToken);
            Successful_Login?.Invoke("Successfull Login!");
            yield break;

        }

        /// <summary>
        /// Used for initially Creating an anonymous account
        /// </summary>
        /// <param name="users_name">The display name of the user</param>
        /// <returns></returns>
        private IEnumerator Acquire_Anonymous_Tokens(string users_name)
        {
            YUR_Log.Log("Beginning to Login as an Anonymous User");
            string response;
            yield return response = Systems.Interops.User_AccountCreation.CreateAnonymousAccount();
            if (response.StartsWith("--1"))
            {
                Bad_Login?.Invoke(response);
                yield break;
            }
            YUR_Log.Log("Successfully authenticated, begin retrieving data!");
            yield return CurrentUser.loginCredentials = Utilities.YUR_Conversions.ConvertStringToObject<LoginCredentials> (response);
            YUR_Log.Log("Test");
            YUR_Log.Log("ActiveUserAccount  : " + CurrentUser.loginCredentials.RefreshToken);
            yield return StartCoroutine(Get_UserData());
            yield return YUR_Main.main.User_Manager.CurrentUser.Data_Biometrics.Name = users_name;
            yield return YUR_CurrentUser.Store_RefreshToken(CurrentUser.Data_Biometrics.Name, CurrentUser.Profile.PhotoURL, CurrentUser.loginCredentials.RefreshToken);
            Successful_Login?.Invoke("Successfull Login!");
            yield break;
        }

        /// <summary>
        /// Used for Creating a new account. Creates an account and then signs the user in.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        private IEnumerator Create_New_Account_Email_Password(string email, string password, string displayName)
        {
            YUR_Log.Server_Log("Creating an account with Email and Password");
            string response;
            yield return response = Systems.Interops.User_AccountCreation.CreateAccount(email, password, displayName);
            if (response.StartsWith("--1"))
            {
                Bad_Login?.Invoke("Account Creation Failed: " + response);
                yield break;
            }
            YUR_Log.Server_Log("Account creation was successful, waiting for profile to build");

            yield return new WaitForSeconds(3);
            Logging_In?.Invoke("Attempting to Login to account");

            yield return StartCoroutine(Acquire_Access_Tokens(email, password));
            yield break;
        }

        /// <summary>
        /// Get's the usersdata from database
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private IEnumerator Get_UserData(YUR_CurrentUser.DataType dataType = YUR_CurrentUser.DataType.all)
        {
            YUR_Log.Server_Log("Retrieving UserData");
            if (dataType == YUR_CurrentUser.DataType.all || dataType == YUR_CurrentUser.DataType.biometrics)
            {
                YUR_Log.Server_Log("Retrieving Biometric Data with ID token: " + CurrentUser.loginCredentials.IDtoken);
                string biodata = Systems.Interops.User_AccountAccess.Retrieve_BiometricData(CurrentUser.loginCredentials.IDtoken);
                YUR_Log.Server_Log("Biometric Data: " + biodata);
                yield return CurrentUser.Data_Biometrics = JsonUtility.FromJson<UserData.Biometrics>(biodata);
                
                YUR_Log.Server_Log("Biometric data acquired");
            }
                

            if (dataType == YUR_CurrentUser.DataType.all || dataType == YUR_CurrentUser.DataType.game_calories)
            {
                YUR_Log.Server_Log("Retrieving Game Data with ID token: " + CurrentUser.loginCredentials.IDtoken);
                string gamedata = Systems.Interops.User_AccountAccess.Retrieve_Game_Data(YUR_Main.main.game_ID, CurrentUser.loginCredentials.IDtoken);
                YUR_Log.Server_Log("Game Data: " + gamedata);
                yield return CurrentUser.Data_Current_Game = Utilities.YUR_Conversions.ConvertStringToObject<UserData.GameData>(gamedata);

                YUR_Log.Server_Log("Game data acquired");
            }


            if (dataType == YUR_CurrentUser.DataType.all || dataType == YUR_CurrentUser.DataType.general_calories)
            {
                YUR_Log.Server_Log("Retrieving Biometric Data with ID token: " + CurrentUser.loginCredentials.IDtoken);
                yield return CurrentUser.Data_General_Calories = Utilities.YUR_Conversions.ConvertStringToObject<UserData.GeneralCalorieData>(Systems.Interops.User_AccountAccess.Retrieve_General_Calorie_Data(CurrentUser.loginCredentials.IDtoken));
                YUR_Log.Server_Log("General Calorie data acquired");
            }

            yield break;
        }

        /// <summary>
        /// Updates the users data in the database from this instance
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private IEnumerator Set_UserData(YUR_CurrentUser.DataType dataType = YUR_CurrentUser.DataType.all)
        {
            bool success = true;
            string reponse;
            if (dataType == YUR_CurrentUser.DataType.all || dataType == YUR_CurrentUser.DataType.biometrics)
            {
                string objectAsString;
                yield return objectAsString = Utilities.YUR_Conversions.ConvertObjectToString(CurrentUser.Data_Biometrics);
                yield return reponse = Systems.Interops.User_AccountAccess.Set_Biometric_Data(objectAsString, CurrentUser.loginCredentials.IDtoken, out success);
                if (!success)
                    yield return StartCoroutine(Refresh_Token_Set_Data(YUR_CurrentUser.DataType.biometrics));
            }

            if (dataType == YUR_CurrentUser.DataType.all || dataType == YUR_CurrentUser.DataType.game_calories)
            {

                string objectAsString;
                yield return objectAsString = Utilities.YUR_Conversions.ConvertObjectToString(CurrentUser.Data_Current_Game);
                yield return reponse = Systems.Interops.User_AccountAccess.Set_Game_Data(objectAsString, CurrentUser.loginCredentials.IDtoken, out success);
                if (!success)
                    yield return StartCoroutine(Refresh_Token_Set_Data(YUR_CurrentUser.DataType.game_calories));
            }

            if (dataType == YUR_CurrentUser.DataType.all || dataType == YUR_CurrentUser.DataType.general_calories)
            {
                string objectAsString;
                yield return objectAsString = Utilities.YUR_Conversions.ConvertObjectToString(CurrentUser.Data_Biometrics);
                yield return reponse = Systems.Interops.User_AccountAccess.Set_Biometric_Data(objectAsString, CurrentUser.loginCredentials.IDtoken, out success);
                if (!success)
                    yield return StartCoroutine(Refresh_Token_Set_Data(YUR_CurrentUser.DataType.general_calories));
            }
            yield break;
        }

        /// <summary>
        /// Obtains required login credentials using a valid refreshtoken and then updates all of the users login details
        /// </summary>
        /// <param name="refreshToken">Valid refresh token retrieved from initial login</param>
        /// <returns></returns>
        private IEnumerator Get_IDtoken(string refreshToken) 
        {
            YUR_Log.Log("Getting Login Token with Refresh Token");
            string refreshResponse;
            refreshResponse = Systems.Interops.User_AccountAuthorization.Retrieve_IDToken(refreshToken);
            if (refreshResponse.StartsWith("--1"))
            {
                Bad_Login?.Invoke(refreshResponse);
                yield break;
            }
            
            if(!CurrentUser.Convert_Refresh_Login(refreshResponse))
            {
                YUR_Log.Error("Get IDToken could not use Convert Refresh Login Method. Login was unnsuccessful.");
                Login.status = Login.StatusType.Logging_Out;
                Logout.ActiveUser();
                yield break;
            }

            yield return StartCoroutine(Get_UserData());
            Successful_Login?.Invoke("Successfull Login!");
            yield break;

        }

        /// <summary>
        /// Gets specified data from database
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        internal IEnumerator Refresh_Token_Get_Data(YUR_CurrentUser.DataType dataType)
        {
            string refreshResponse;
            yield return refreshResponse = Systems.Interops.User_AccountAuthorization.Retrieve_IDToken(CurrentUser.loginCredentials.IDtoken);
            if (refreshResponse.StartsWith("--1"))
            {
                Bad_Login?.Invoke(refreshResponse);
                yield break;
            }
            YUR_Log.Log("Refresh token worked! Retrieving all other data");
            yield return CurrentUser.Convert_Refresh_Login(refreshResponse);
            yield return StartCoroutine(Get_UserData(dataType));
            
            yield break;
        }

        /// <summary>
        /// Updates specified data from database
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        internal IEnumerator Refresh_Token_Set_Data(YUR_CurrentUser.DataType dataType)
        {
            string refreshResponse;
            yield return refreshResponse = Systems.Interops.User_AccountAuthorization.Retrieve_IDToken(CurrentUser.loginCredentials.IDtoken);
            if (refreshResponse.StartsWith("--1"))
            {
                Bad_Login?.Invoke(refreshResponse);
                yield break;
            }
            YUR_Log.Log("Refresh token worked! Setting all other data");
            yield return CurrentUser.Convert_Refresh_Login(refreshResponse);
            StartCoroutine(Set_UserData(dataType));
            yield break;
        }


    }
}
