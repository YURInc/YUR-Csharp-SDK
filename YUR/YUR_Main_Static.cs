#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Collections;
using YUR.SDK.Unity.UserManagement;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#endif

namespace YUR.SDK.Unity
{
    public partial class YUR_Main
    {
        internal static string Active_User_Logged_In_Successfully(string response)
        {
            YUR_Log.Log("Active User Logged In Successfully: " + response);
            return response;
        }

        internal static string Active_User_Unable_To_Login(string response)
        {
            YUR_Log.Error("Active User Unable To Login: " + response);
            return response;
        }

        internal static string Active_User_Logged_Out(string response)
        {
            YUR_Log.Log("Active User Logged Out");
            return response;
        }

    }
}
