using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YUR.SDK.Unity
{
    /// <summary>
    /// Account creation is handled here
    /// </summary>
    public static class Create
    {
        /// <summary>
        /// Create a NEW anonymous account that can later be made into an authorized account
        /// </summary>
        /// <param name="name">Name for the user to see when logging in</param>
        /// <returns type="bool">True if no active user is logged in, false account was not created</returns>
        public static bool Anonymous_Account(string name)
        {
            if (!Login.UserLoggedIn())
            {
                YUR_Main.main.User_Manager.Create_Anonymous_Account(name);
                return true;
            }
            else
            {
                YUR_Main.main.User_Manager.Users_Already_Logged_In("Anonymous account creation cannot be completed while user is currently logged in");
                return false;
            }

        }

        /// <summary>
        /// Create a NEW account with a User's Email and Password
        /// </summary>
        /// <param name="Email">User's Email valid format (username@website.TLD)</param>
        /// <param name="Password">User's Password with at least 6 characters</param>
        /// <param name="displayName">Name user would like to see when selecting their account for login</param>
        /// <param name="Credentials_Issues">Issues with Email and Password will be reported out</param>
        /// <returns>True if account creation sent to server</returns>
        public static bool Email_Password(string Email, string Password, string displayName, out string Credentials_Issues)
        {
            Credentials_Issues = "";
            var valid_creds = Utilities.YUR_Login_Validation.IsEmailLoginValid(Email, Password, out Credentials_Issues);
            if (Login.UserLoggedIn())
            {
                YUR_Main.main.User_Manager.Users_Already_Logged_In("Email and Password account cannot be created while a user is currently logged in. \nCredentials: " + Credentials_Issues);
                return false;
            }
            if (!valid_creds)
            {
                YUR_Main.main.User_Manager.Users_Already_Logged_In(Credentials_Issues);
                return false;
            }

            YUR_Main.main.User_Manager.Create_Account_Email_Password(Email, Password, displayName);
            return true;
        }
    }

    public static class Logout
    {
        public static void ActiveUser(string LogOut = "External script call from Logout class")
        {

            YUR_Main.main.User_Manager.LogOut(LogOut);
        }
    }

    /// <summary>
    /// Account Login information and management is handled here
    /// </summary>
    public static class Login
    {

        /// <summary>
        /// Log a user in by their User ID (UID). Necessary for persistant logins
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static bool User_ID(string userID)
        {
            try
            {
                YUR_Main.main.User_Manager.Login_USERID(userID);
            }
            catch(Exception e)
            {
                YUR_Log.Error("User_ID login attempt error: " + e);
            }

            return true;
        }


        /// <summary>
        /// Login user with Email and Password Credentials. Out's issues with Credentials
        /// </summary>
        /// <param name="Email">User's Email valid format (username@website.TLD)</param>
        /// <param name="Password">User's Password with at least 6 characters</param>
        /// <param name="Credentials_Issues">Outputs relavent issues with login credentials</param>
        /// <returns>True if Login credentials are valid. Does not ensure successful login.</returns>
        public static bool Email_Password(string Email, string Password, out string Credentials_Issues)
        {
            Credentials_Issues = "";
            if (UserLoggedIn() || !Utilities.YUR_Login_Validation.IsEmailLoginValid(Email, Password, out Credentials_Issues))
                return false;

            YUR_Main.main.User_Manager.Login_Email_Password(Email, Password);
            return true;
        }

        /// <summary>
        /// Login users with Email and Password Credentials. Does out issues with credentials
        /// </summary>
        /// <param name="Email">User's Email valid format (username@website.TLD)</param>
        /// <param name="Password">User's Password with at least 6 characters</param>
        /// <returns>
        /// True if Login credentials are valid. Does not ensure successful login. 
        /// Use a subscription to Successful_Login, Bad_Login, Logging_In, Already_Logged_In events
        /// to determine server reponse.
        /// </returns>
        public static bool Email_Password(string Email, string Password)
        {
            return Email_Password(Email, Password, out string Empty);
        }

        /// <summary>
        /// Check whether a User is currently Logged in
        /// </summary>
        /// <returns>True if user is logged in</returns>
        public static bool UserLoggedIn()
        {
            return (Status == StatusType.Logged_In ? true : false);
        }

        /// <summary>
        /// Used for determining if a User is logged in or out
        /// </summary>
        public enum StatusType
        {
            Logged_In = 200,
            Logging_In = 100,
            Logged_Out = 0,
            Logging_Out = -100
        }

        /// <summary>
        /// Current Login Status of Current User
        /// </summary>
        public static StatusType Status { get { return status; } }


        internal static StatusType status = StatusType.Logged_Out;

    }
}
