#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using YUR.SDK.Unity.UserManagement;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#endif

// 1. select PC, Mac & Linux Standalone then click Switch Platform in the bottom left
// 2. click on the YUR .unitypackage wherever you've downloaded it
// 3. 
    /// ToDo
    /// Finish autologin 
    /// Setup for external login
    /// Refactor Login progress/ events

namespace YUR.SDK.Unity
{

    public partial class YUR_Main : MonoBehaviour
    {
        IEnumerator LoadUsers()
        {
            YUR_Log.Log("Loading Users");
            yield return User_Manager = new GameObject("[Users]").AddComponent<YUR_UserManager>();
            yield return User_Manager.GET_LIST_USERS_IDS(ref UserList);
            User_Manager.gameObject.transform.SetParent(YUR.Yur.YURGameObject.transform);
            bool user_found = false;
            if(UserList.Count == 0)
            {
                YUR_Log.Log("No users previously logged in!");
                YUR_Log.Log("YUR remains inactive until a user logs in");
            }
            else
            {
                YUR_Log.Log("Retrieving last user data: " + Last_Played_User);
                foreach (string st in UserList)
                {
                    YUR_Log.Log(st);
                    if(st == Last_Played_User)
                    {
                        YUR_Log.Log("User Found >> " + st);
                        user_found = true;
                        break;
                    }
                }
            }
            Completed_Startup?.Invoke(user_found);
            yield break;
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



namespace YUR.SDK.Unity.Configuration
{
    public class Editor
    {


        public string FilePath { get; }
        public PrintSettings pSettings = new PrintSettings();
        public event Action<Editor> ConfigChangedEvent;

        private readonly FileSystemWatcher _configWatcher;
        

        private bool _saving;

        public Editor(string filePath)
        {
            FilePath = filePath;
            if (File.Exists(FilePath))
            {
                Load();
                var text = File.ReadAllText(FilePath);
                Save();
            }
            else
            {
                Save();
            }

            _configWatcher = new FileSystemWatcher(Environment.CurrentDirectory)
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "YUR.cfg",
                EnableRaisingEvents = true
            };
            _configWatcher.Changed += _configWatcher_Changed; ;
        }

        private void _configWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (_saving)
            {
                _saving = false;
                return;
            }

            Load();

            ConfigChangedEvent?.Invoke(this);
        }

        public void Save()
        {

            _saving = true;
            Utilities.Config.YUR_ConfigurationManager.SaveConfig(this, FilePath);
            _saving = false;
            //ConfigSerializer.SaveConfig(this, FilePath);
        }

        public void Load()
        {
            Utilities.Config.YUR_ConfigurationManager.LoadConfig(this, FilePath);
        }


        public class PrintSettings
        {
            public bool Debugging = false;
            public bool Error_Reporting = true;

        }
    }
}


//public event Action<Config> ConfigChangedEvent;

//private readonly FileSystemWatcher _configWatcher;
//private bool _saving;

//public Config(string filePath)
//{
//    FilePath = filePath;
//    if (File.Exists(FilePath))
//    {
//        Load();
//        var text = File.ReadAllText(FilePath);
//        Save();
//    }
//    else
//    {
//        Save();
//    }

//    _configWatcher = new FileSystemWatcher(Environment.CurrentDirectory)
//    {
//        NotifyFilter = NotifyFilters.LastWrite,
//        Filter = "YURfit.cfg",
//        EnableRaisingEvents = true
//    };
//    _configWatcher.Changed += ConfigWatcherOnChanged;

//}
//~Config()
//{
//    _configWatcher.Changed -= ConfigWatcherOnChanged;

//}
//public void Save()
//{
//    _saving = true;
//    ConfigSerializer.SaveConfig(this, FilePath);
//    //Console.WriteLine(Plugin.modLog + "YURfit Config Saved");

//}
//public void Load()
//{
//    ConfigSerializer.LoadConfig(this, FilePath);
//    //Console.WriteLine(Plugin.modLog + "YURfit Config Loaded");
//}

//private void ConfigWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
//{
//    if (_saving)
//    {
//        _saving = false;
//        return;
//    }

//    Load();

//    if (ConfigChangedEvent != null)
//    {
//        ConfigChangedEvent(this);

//    }
//}