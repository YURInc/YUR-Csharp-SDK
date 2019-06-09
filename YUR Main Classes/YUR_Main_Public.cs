#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using YUR.SDK.Unity.UserManagement;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#endif

// 1. select PC, Mac & Linux Standalone then click Switch Platform in the bottom left
// 2. click on the YUR .unitypackage wherever you've downloaded it
// 3. 

namespace YUR.SDK.Unity
{

    //[AddComponentMenu("YUR/YUR")]
    public partial class YUR_Main : MonoBehaviour
    {
        public delegate void Startup(bool user_found);
        public static event Startup Completed_Startup;
        public static YUR_Main main;
        public string game_ID;
        public bool auto_sign_in;
        public float TimeStep;

        private Workouts.Workout workout;
        private Tracking.Calories calories;

        public YUR_UserManager User_Manager;
        public List<string> UserList;

        public string Last_Played_User;

        /// <summary>
        /// Called upon creation of YUR object
        /// </summary>
        /// <param name="game_ID"></param>
        /// <param name="Fixed_Time_Step"></param>
        /// <param name="debug"></param>
        /// <param name="error_debug"></param>
        public void StartUp(string game_ID = "yurapp", float Fixed_Time_Step = 90, bool debug = false, bool error_debug = true, bool server_debug = false, bool log_to_file = false, bool auto_sign_in = false, bool editor_debugging = true)
        {
            main = this;

            DontDestroyOnLoad(main);
            DontDestroyOnLoad(this);

            this.game_ID = game_ID;
            TimeStep = Fixed_Time_Step;
            this.auto_sign_in = auto_sign_in;

            YUR_Log.Debugging = debug;
            YUR_Log.Error_Logging = error_debug;
            YUR_Log.Server_Logging = server_debug;
            YUR_Log.Log_To_File = log_to_file;
            YUR_Log.Editor_Debugging = editor_debugging;
            YUR_Log.Log("Starting YUR");

            workout = gameObject.AddComponent<Workouts.Workout>();
            calories = new GameObject("YUR Calorie Display Object").AddComponent<Tracking.Calories>();
            DontDestroyOnLoad(workout); // TODO Check DontDestroyOnLoads are not over done. Placed everywhere as a quick fix
            DontDestroyOnLoad(calories);

            YUR_UserManager.Successful_Login += new YUR_UserManager.Login_Status_Changed(Active_User_Logged_In_Successfully);
            YUR_UserManager.Bad_Login += new YUR_UserManager.Login_Status_Changed(Active_User_Unable_To_Login);
            YUR_Log.Log("Subscribed to events");
            try
            {
                YUR_Log.Log("Getting Last Logged In User ID");
                string path = Utilities.YUR_Conversions.PathCombine(Application.persistentDataPath, YUR_Constants.LAST_ACTIVE_USER_FILEPATH);
                Directory.CreateDirectory(path);
                path += "active.txt";
                YUR_Log.Log("Looking here: " + path);
                if (File.Exists(path))
                {
                    Last_Played_User = File.ReadAllText(path);
                    YUR_Log.Log("Last uid: " + Last_Played_User);
                }
                else
                    YUR_Log.Log("Last user was not saved!");

                StartCoroutine(LoadUsers());
            }
            catch (System.Exception e)
            {
                YUR_Log.Error(e.ToString());
            }
        }

        public void OnApplicationQuit()
        {
            YUR_Log.Log("Storing last user logged in");
            string path = Utilities.YUR_Conversions.PathCombine(Application.persistentDataPath, YUR_Constants.LAST_ACTIVE_USER_FILEPATH + "active.txt");
            File.WriteAllText(path, User_Manager.CurrentUser.loginCredentials.LocalId);
        }

        /// <summary>
        /// Object created check
        /// </summary>
        void Awake()
        {
            Completed_Startup += YUR_Main_Completed_Startup;
            YUR_Log.Log("Building YUR Object. Waiting for attach.");
        }

        private void YUR_Main_Completed_Startup(bool user_found)
        {
            YUR_Log.Log("Startup completed. User Found for Login: " + user_found);
            if(auto_sign_in && user_found)
            {
                if (Last_Played_User == null || Last_Played_User == "" || Last_Played_User == string.Empty)
                    return;
                User_Manager.Login_USERID(Last_Played_User);
            }
            //if(user_found)
                //YUR_Main.main.User_Manager.Login_USERID(Last_Played_User);
        }
    }


    public class YUR_Script_Helper : MonoBehaviour
    {
        /// <summary>
        /// Creating YUR object through script should be completed using this method.
        /// </summary>
        /// <param name="game_object">The gameobject in which to add YUR to</param>
        /// <param name="game_ID">The identifier that will be used in YUR's database. Must be pre-authenticated with YUR</param>
        /// <param name="fixed_frame_rate">Default of 90 FPS, Ensure that this matches the Fixed Updates per Second</param>
        /// <returns></returns>
        public static YUR_Main Setup_YUR_Object(GameObject game_object, string game_ID = "yurapp", bool debug = false, bool error_debug = true, bool server_debug = true, bool log_to_file = false, bool auto_sign_in = false, bool editor_debugging = true)
        {
            YUR_Main yur = game_object.AddComponent<YUR_Main>();
            DontDestroyOnLoad(yur);
            yur.StartUp(game_ID, 1 / Time.fixedDeltaTime, debug, error_debug, server_debug, log_to_file, auto_sign_in, editor_debugging);
            return yur;
        }
    }


    // public class menuItems
    // {
    // 	[MenuItem("YUR/Install...")]
    // 	private static void NewMenuOption(){
    // 		PlayerPrefs.DeleteAll();
    //     }
    // }
}

