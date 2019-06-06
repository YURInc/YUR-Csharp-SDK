using System;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;

namespace YUR.SDK.Unity.Systems.Interops
{
    /// <summary>
    /// Calorie Tracking references to YUR.dll
    /// </summary>
    class CalorieTracking
    {

        public const string libfile = "YUR";

        [DllImportAttribute(libfile, EntryPoint = "BioAdjust", CallingConvention = CallingConvention.StdCall)]
        internal static extern float BioAdjust(int sex, float height_cm, float weight_kg, int age);

        [DllImportAttribute(libfile, EntryPoint = "OnePassCalculation", CallingConvention = CallingConvention.StdCall)]
        internal static extern float OnePassCalculation(float hVelx,float hVely,float hVelz,float lVelx,float lVely,float lVelz,float rVelx,float rVely,float rVelz,float timeAccuracy,float Weight,float BioAdjust,bool Kilograms, float FixedUpdateFPS = 90);
    }

    class Workouts
    {
        public const string libfile = "YUR";
        [DllImportAttribute(libfile, EntryPoint = "Upload_Workout", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr Upload_Workout([In, MarshalAs(UnmanagedType.LPStr)] string workout_json_string, string idToken);
        /// <summary>
        /// Upload recently completed workout to the users account
        /// </summary>
        /// <param name="workout_json_string">Workout Class as Json string</param>
        /// <param name="idToken">User account access token</param>
        /// <returns></returns>
        internal static string UploadWorkout(string workout_json_string, string idToken)
        {
            return Marshal.PtrToStringAnsi(Upload_Workout(workout_json_string, idToken));
        }
    }

    class User_AccountCreation
    {
        public const string libfile = "YUR";
        [DllImportAttribute(libfile, EntryPoint = "Create_Account_Email_Password", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr Create_Account_Email_Password([In, MarshalAs(UnmanagedType.LPStr)] string email, [In, MarshalAs(UnmanagedType.LPStr)] string password, [In, MarshalAs(UnmanagedType.LPStr)] string displayName);
        internal static string CreateAccount(string email, string password, string displayName)
        {
            return Marshal.PtrToStringAnsi(Create_Account_Email_Password(email, password, displayName));
        }

        [DllImportAttribute(libfile, EntryPoint = "Anonymous_Account_Creation", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr Anonymous_Account_Creation();
        internal static string CreateAnonymousAccount()
        {
            return Marshal.PtrToStringAnsi(Anonymous_Account_Creation());
        }
    }

    public class User_AccountAuthorization
    {
        public const string libfile = "YUR";

        /// TODO : Finish C++ code
        [DllImportAttribute(libfile, EntryPoint = "Get_IDToken", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr Get_IDToken([In, MarshalAs(UnmanagedType.LPStr)] string refreshToken);
        internal static string Retrieve_IDToken(string refreshToken)
        {
            YUR_Log.Server_Log("Passing over to native dll, waiting for response...");
            return Marshal.PtrToStringAnsi(Get_IDToken(refreshToken));
        }

        [DllImportAttribute(libfile, EntryPoint = "LoginYURUser", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr LoginYURUser([In, MarshalAs(UnmanagedType.LPStr)] string email, [In, MarshalAs(UnmanagedType.LPStr)] string password);
        /// <summary>
        /// Get the Tokens necessary for all other User Account access actions
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>idToken and Refresh token</returns>
        internal static string Login_User(string email, string password)
        {
            YUR_Log.Log("Piping login over to Native DLL");
            return Marshal.PtrToStringAnsi(LoginYURUser(email, password));
        }
    }

    class User_AccountAccess
    {
        public const string libfile = "YUR";
        [DllImportAttribute(libfile, EntryPoint = "Get_BiometricData", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr Get_BiometricData([In, MarshalAs(UnmanagedType.LPStr)] string authorization);
        /// <summary>
        /// Get the Biometric Data for the passed idToken user
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns>Biometric Data JSON as a string. Must be converted to Class</returns>
        internal static string Retrieve_BiometricData(string idToken)
        {
            return Marshal.PtrToStringAnsi(Get_BiometricData(idToken));
        }

        [DllImportAttribute(libfile, EntryPoint = "Get_GeneralCalorieData", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr Get_GeneralCalorieData([In, MarshalAs(UnmanagedType.LPStr)] string authorization);
        /// <summary>
        /// Get the General Calorie Data for the passed idToken user
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns>General Calorie Data JSON as a string. Must be converted to Class</returns>
        internal static string Retrieve_General_Calorie_Data(string idToken)
        {
            return Marshal.PtrToStringAnsi(Get_GeneralCalorieData(idToken));
        }

        [DllImportAttribute(libfile, EntryPoint = "Get_GameData", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr Get_GameData([In, MarshalAs(UnmanagedType.LPStr)] string gameID, [In, MarshalAs(UnmanagedType.LPStr)] string authorization);
        /// <summary>
        /// Get the Game Data for the passed idToken user
        /// </summary>
        /// <param name="idToken"></param>
        /// <returns>Game Data JSON as a string. Must be converted to Class</returns>
        internal static string Retrieve_Game_Data(string gameID, string idToken)
        {
            string JSON ="{" + "\"gameID\": \"" + gameID + "\"}";
            return Marshal.PtrToStringAnsi(Get_GameData(JSON, idToken));
        }

        [DllImportAttribute(libfile, EntryPoint = "Set_BiometricData", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr Set_BiometricData([In, MarshalAs(UnmanagedType.LPStr)] string json, [In, MarshalAs(UnmanagedType.LPStr)] string authorization);
        /// <summary>
        /// Update the Biometric Data for the passed idToken user
        /// </summary>
        /// <param name="biometric_data_json">Biometric data class as a JSON</param>
        /// <param name="idToken"></param>
        /// <returns>Biometric Data JSON as a string. Must be converted to Class</returns>
        internal static string Set_Biometric_Data(string biometric_data_json, string idToken, out bool success)
        {
            success = false;
            string Biometric_Response = Marshal.PtrToStringAnsi(Set_BiometricData(biometric_data_json, idToken));
            if (Biometric_Response.StartsWith("--1"))
            {
                YUR_Main.main.User_Manager.Refresh_Token_Set_Data(YUR_CurrentUser.DataType.biometrics);
                success = false;
            }
            else
            {
                success = true;
            }
            return Biometric_Response;
        }

        [DllImportAttribute(libfile, EntryPoint = "Set_GeneralCalorieData", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr Set_GeneralCalorieData([In, MarshalAs(UnmanagedType.LPStr)] string json, [In, MarshalAs(UnmanagedType.LPStr)] string authorization);
        /// <summary>
        /// Update General Calorie Data for the passed idToken user
        /// </summary>
        /// <param name="general_calorie_data_json"></param>
        /// <param name="idToken"></param>
        /// <returns></returns>
        internal static string Set_General_Calorie_Data(string general_calorie_data_json, string idToken, out bool success)
        {
            success = false;
            string General_Calorie_Response = Marshal.PtrToStringAnsi(Set_GeneralCalorieData(general_calorie_data_json, idToken));
            if (General_Calorie_Response.StartsWith("--1"))
            {
                YUR_Main.main.User_Manager.Refresh_Token_Set_Data(YUR_CurrentUser.DataType.general_calories);
                success = false;
            }
            else
            {
                success = true;
            }
            return General_Calorie_Response;
        }

        [DllImportAttribute(libfile, EntryPoint = "Set_GameData", CallingConvention = CallingConvention.StdCall)]
        protected static extern IntPtr Set_GameData([In, MarshalAs(UnmanagedType.LPStr)] string json, [In, MarshalAs(UnmanagedType.LPStr)] string authorization);
        /// <summary>
        /// Update the Game Data for the passed idToken user
        /// </summary>
        /// <param name="game_data_json">Game Data class as a JSON</param>
        /// <param name="idToken"></param>
        /// <returns></returns>
        internal static string Set_Game_Data(string game_data_json, string idToken, out bool success)
        {
            success = false;
            string General_Calorie_Response = Marshal.PtrToStringAnsi(Set_GameData(game_data_json, idToken));
            if (General_Calorie_Response.StartsWith("--1"))
            {
                YUR_Main.main.User_Manager.Refresh_Token_Set_Data(YUR_CurrentUser.DataType.general_calories);
                success = false;
            }
            else
            {
                success = true;
            }
            return General_Calorie_Response;
        }
    }
}


