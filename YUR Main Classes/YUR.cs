using UnityEngine;
using System.IO;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace YUR.SDK.Unity
{
    /// <summary>
    /// All of YUR Methods and Start up methods are kept here. The dash is loaded in from an Asset Bundle within the top level directory
    /// </summary>
    [AddComponentMenu("YUR/YUR")]
    public partial class YUR : MonoBehaviour
    {
        public void Start()
        {
            _yur = this;
            if (!enabled)
            {
                YUR_Log.Error("YUR is not enabled!");
                return;
            }
          
            /// Load General Settings ///
            Settings = Resources.Load("YURGeneralSettings") as YURSettingsScriptableObject;
            AutoUpdate = Settings.AutomaticUpdates;


            /// Setup all AOT variables inside here
            if(Application.platform == RuntimePlatform.Android)
            {
                AutoUpdate = false;
            }

            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            YUR_Log.Log("Starting to get YUR Object and startup system");
            StartCoroutine(InstantiateYURObjects());
            _eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            YUR_GO = new GameObject("YUR Main Object");
            YUR_Main.Completed_Startup += YUR_Main_Completed_Startup;
            Fit = YUR_Script_Helper.Setup_YUR_Object(YUR_GO, Settings.GameID, Settings.debugging, Settings.ErrorDebugging, Settings.ServerDebugging, Settings.WriteDebuggingToFile, Settings.AutomaticallySignInUser);
            YURGesture = YUR_GO.AddComponent<YUR_GestureRecognition>();
            platform = Settings.platform;

            if(platform != VRUiKits.Utils.VRPlatform.VIVE_STEAM2)
            {
                LeftControllerTrigger = Settings.LeftControllerButton;
                RightControllerTrigger = Settings.RightControllerButton;
            }
        }
        
        /// <summary>
        /// Invoked when YUR has finished setting up
        /// </summary>
        /// <param name="user_found">True if a user was found to log in</param>
        private void YUR_Main_Completed_Startup(bool user_found)
        {
            Debug.Log("<b>Completed startup for YUR</b>");
            UserManagement.YUR_UserManager.Successful_Login += YUR_UserManager_Successful_Login;
            UserManagement.YUR_UserManager.Log_Out += YUR_UserManager_Log_Out;
            PrepareScreenSystem(user_found);
        }

        /// <summary>
        /// Invoked when a user has been logged in successfully
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private string YUR_UserManager_Successful_Login(string response)
        {
            YUR_Log.Log("User Successfully Logged In");
            calorieCounter = Tracking.Calories.CalorieCounter.CalorieDisplayObject.AddComponent<CalorieCounter>();
            calorieCounter.Completed += CalorieCounter_Completed;

            return response;
        }

        /// <summary>
        /// Waits for the Calorie Counter to complete its setup
        /// </summary>
        private void CalorieCounter_Completed()
        {
            YUR_Log.Log("Calorie Counter Setup, setting presets");
            YUR_Log.Log("_altered_counter == true");
            calorieCounter.CalorieCountDisplay.fontSize = _counter_fontsize;
            calorieCounter.CalorieCountDisplay.color = _counter_color;
            YUR_Log.Log("_altered_label == true");
            calorieCounter.CalorieLabel.fontSize = _label_fontsize;
            calorieCounter.CalorieLabel.color = _label_color;
            if (CalorieDisplayFonts != null)
            {
                YUR_Log.Log("Font asset not null! Applying...");
                calorieCounter.CalorieCountDisplay.font = CalorieDisplayFonts;
                calorieCounter.CalorieLabel.font = CalorieDisplayFonts;
            }
        }

        /// <summary>
        /// Called when a user has been logged out
        /// </summary>
        /// <param name="response">Describes reason for loggout</param>
        /// <returns></returns>
        private string YUR_UserManager_Log_Out(string response)
        {
            YUR_Log.Log("User Successfully Logged Out");
            return response;
        }

        /// <summary>
        /// Setup YUR Gui to display appropriate screen when called to display
        /// </summary>
        /// <param name="PreviousLogin"></param>
        public void PrepareScreenSystem(bool PreviousLogin)
        {
            yURScreenSystem = YUR_GO.AddComponent<UI.YURScreenSystem>();
        }

        private void OnDisable()
        {
            YUR_Main.Completed_Startup -= YUR_Main_Completed_Startup;
           
        }

        private void OnApplicationQuit()
        {
            Workouts.Workout.EndingWorkout("Quit Game Mid Workout");
        }
    }


    /// <summary>
    /// This portion of YUR is for handling the Asset Bundles necessary for the rest of YUR to work
    /// </summary>
    public partial class YUR : MonoBehaviour
    {
        [HideInInspector]
        public bool AutoUpdate;
        [HideInInspector]
        public string Version = (Application.platform == RuntimePlatform.Android ? "-.-.-" : System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion);
        [HideInInspector]
        public string assetBundleName = "yur.yur";
        [HideInInspector]
        public AssetBundle YURAssetBundle;
        [HideInInspector]
        public const string yursdkname = "YUR.SDK.Unity.dll";

        // TODO : Add functionality to update other DLL's
        //[HideInInspector]
        //public const string UserInteractionDll = "UserInteraction.dll";
        //[HideInInspector]
        //public const string OculusInteraction = "YUR.SDK.Unity.Oculus.dll";
        //[HideInInspector]
        //public const string SteamInteraction = "YUR.SDK.Unity.Steam.dll";

        [HideInInspector]
        public const string GitHubRepoDownloadURI = "https://github.com/YURInc/YUR-Unity-Package/releases/download/";
        [HideInInspector]
        public string GitHubRepoReleaseTagsURI = "https://api.github.com/repos/YURInc/YUR-Unity-Package/tags";
        [HideInInspector]
        public string AssetBundlesLocalFilePath;

        IEnumerator InstantiateYURObjects()
        {
            yield return AssetBundlesLocalFilePath = (Application.platform == RuntimePlatform.Android ? Application.streamingAssetsPath + "/": ("file:///" + Application.dataPath + "/AssetBundles/Standalone_AssetBundles/"));
            /// Get Local Version, make sure updated
            /// 
            YUR_Log.Log("Getting local asset bundle");

            string URI;
            yield return URI = AssetBundlesLocalFilePath + assetBundleName;
            YUR_Log.Log("Local Asset bundle path: " + URI);
            YUR_Log.Log("Streaming assets path: " + Application.streamingAssetsPath);
            UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(URI, 0);
            yield return request.SendWebRequest();
            if (request.error != null)
            {

                YUR_Log.Warning("YUR was unable to locate the necessary Asset Bundles locally, acquiring latest version from GitHub...");
                YUR_Log.Log("Request Error Information: " + request.error);
                StartCoroutine(DownloadYURAssetBundle("latest",false));
                DownloadYURSDK("latest");
            }
            else
            {
                YUR_Log.Log("Local asset bundle found!");
                YURAssetBundle = DownloadHandlerAssetBundle.GetContent(request);
                var dashPre = YURAssetBundle.LoadAsset<GameObject>("YUR");
                YUR_Dash = Instantiate(dashPre);
                YUR_Log.Log("YUR Dash has been instantiated");

                // YUR DASH loaded, check to see if a new version is available to download and automatically install
                YUR_Log.Log("Checking github for latest version");
                UnityWebRequest unityWebRequest = new UnityWebRequest(GitHubRepoReleaseTagsURI);
                unityWebRequest.SetRequestHeader("Accept", "application/vnd.github.v3+json");
                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                yield return unityWebRequest.SendWebRequest();
                YUR_Log.Log("Version Request returned: ");
                if (!string.IsNullOrEmpty(unityWebRequest.error))
                {
                    YUR_Log.Log("Could not check github for latest version. Using current version for now.");
                    YUR_Log.Log("<b>Github: </b> \n" + unityWebRequest.error);
                    yield break;
                }

                if(unityWebRequest.downloadHandler == null)
                {
                    YUR_Log.Log("Download handler is null?");
                }
                YUR_Log.Log("Converting json to object array");

                LatestTags[] Tags;
                Tags = JsonHelper.getJsonArray<LatestTags>(unityWebRequest.downloadHandler.text);

                YUR_Log.Log("Comparing versions: ");
                YUR_Log.Log("Local: " + Version);
                YUR_Log.Log("Server: " + Tags[0].name);

                yield return int.TryParse(Version.Substring(0,1), out int seml1);
                yield return int.TryParse(Version.Substring(2,1), out int seml2);
                yield return int.TryParse(Version.Substring(4,1), out int seml3);

                yield return int.TryParse(Tags[0].name.Substring(0,1), out int semp1);
                yield return int.TryParse(Tags[0].name.Substring(2,1), out int semp2);
                yield return int.TryParse(Tags[0].name.Substring(4,1), out int semp3);

                YUR_Log.Log("<b>Version Major</b>:\nLocal: " + seml1 + "\nServer: " + semp1);
                YUR_Log.Log("<b>Version Minor</b>:\nLocal: " + seml2 + "\nServer: " + semp2);
                YUR_Log.Log("<b>Version Quick</b>:\nLocal: " + seml3 + "\nServer: " + semp3);

                // If Local Version is larger, do nothing. TODO: Return this back to an else if statement now that substring is working properly
                if (seml1 >= semp1)
                {
                    YUR_Log.Log("Major version up to date \nLocal: " + seml1 + "\nServer: " + semp1);
                    if (seml1 == semp1)
                    {
                        if (seml2 >= semp2)
                        {
                            YUR_Log.Log("Minor Version up to date\nLocal: " + seml2 + "\nServer: " + semp2);

                            if (seml2 == semp2)
                            {
                                if (seml3 >= semp3)
                                {
                                    YUR_Log.Log("Quick Fix Version up to date, no update required \nLocal: " + seml3 + "\nServer: " + semp3);
                                }
                                else
                                {
                                    YUR_Log.Warning("A more recent, quick fix version of YUR is available to download.");
                                    if(!AutoUpdate)
                                    {
                                        YUR_Log.Warning("Updates are disabled, please manually update YUR or enable automatic updates");
                                        yield break;
                                    }
                                    yield return StartCoroutine(DownloadYURAssetBundle(Tags[0].name, true));
                                    StartCoroutine(DownloadYURSDK(Tags[0].name));
                                }
                            }
                            else
                            {
                                YUR_Log.Log("Current version is newer! No update required :)");
                                yield break;
                            }
                        }
                        else
                        {
                            YUR_Log.Warning("A more recent, minor version of YUR is available to download. Download and update will happen in the background. YUR will be disabled while updating");
                            if (!AutoUpdate)
                            {
                                YUR_Log.Warning("Updates are disabled, please manually update YUR or enable automatic updates");
                                yield break;
                            }
                            yield return StartCoroutine(DownloadYURAssetBundle(Tags[0].name,true));
                            StartCoroutine(DownloadYURSDK(Tags[0].name));
                        }
                    }
                    else
                    {
                        YUR_Log.Log("Current version is newer! No update required :)");
                        yield break;
                    }
                }
                else
                {
                    YUR_Log.Warning("A more recent, major version of YUR is available to download. Download and update will happen in the background. YUR will be disabled while updating");
                    if (!AutoUpdate)
                    {
                        YUR_Log.Warning("Updates are disabled, please manually update YUR or enable automatic updates");
                        yield break;
                    }
                    yield return StartCoroutine(DownloadYURAssetBundle(Tags[0].name, true));
                    StartCoroutine(DownloadYURSDK(Tags[0].name));
                }
            }
        }

        [System.Serializable]
        public class LatestTags
        {
            public string name;
        }

        /// <summary>
        /// Adds basic JSON Utility functionality to support wrapping arrays.
        /// </summary>
        public class JsonHelper
        {
            public static T[] getJsonArray<T>(string json)
            {
                string newJson = "{ \"array\": " + json + "}";
                YUR_Log.Log("Wrapped Json: \n" + newJson);
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
                return wrapper.array;
            }

            [Serializable]
            private class Wrapper<T>
            {
                public T[] array;
            }
        }

        IEnumerator UpdateAssetPackage(string package_version)
        {
            string URI = GitHubRepoDownloadURI + package_version + "/YUR" + package_version + ".unitypackage";

            UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(URI);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.error != null)
            {
                YUR_Log.Warning("YUR was unable to get the necessary asset bundle from the server. Error: " + request.error);
            }
            else
            {
                YUR_Log.Log("YUR most recent asset package downloaded successfully");
                bool AssetsDownloaded;
                yield return AssetsDownloaded = SaveDownloadedPackage(request, "YUR" + package_version + ".unitypackage");

                if (AssetsDownloaded)
                {
                    YUR_Log.Log("Successfully downloaded package! Please go into YUR Setup Window to finish the update!", this);
                }
                else
                {
                    YUR_Log.Warning("The package could not be saved, please visit https://yur.chat for assistance");
                }
            }

        }

        IEnumerator DownloadYURSDK(string version_to_download)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                YUR_Log.Warning("Will not automatically update inside editor, Asset Package should be acquired automatically during asset bundle install");
                yield break;
            }
            if(!AutoUpdate)
            {
                YUR_Log.Warning("Automatic Updating is disabled", this);
                yield break;
            }
            YUR_Log.Log("Starting to retrieve SDK DLL from github repository");
            string URI = GitHubRepoDownloadURI + version_to_download + "/" +  yursdkname;

            UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(URI);
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();
            if (request.error != null)
            {
                Debug.LogWarning("YUR was unable to get the necessary asset bundle from the server. Error: " + request.error);
            }
            else
            {
                Debug.Log("YUR most recent asset bundle downloaded successfully");
                bool AssetsDownloaded;
                yield return AssetsDownloaded = SaveDownloadedLibrary(request,yursdkname);

                if(AssetsDownloaded)
                {
                    YUR_Log.Log("Successfully downloaded SDK, will be loaded next time game is ran. It is recommended to restart if this is a Major version update.");
                }
                else
                {
                    YUR_Log.Warning("The library was not able to be saved. Please contact YUR at https://yur.chat");
                }
            }

        }

        /// <summary>
        /// Download most recent version of YUR asset bundle if not already installed
        /// </summary>
        /// <returns></returns>
        IEnumerator DownloadYURAssetBundle(string version_to_download, bool priorInstalled)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                yield return StartCoroutine(UpdateAssetPackage(version_to_download));
                yield break;
            }
            if (!AutoUpdate)
            {
                YUR_Log.Warning("Automatic Updating is disabled", this);
                yield break;
            }
            //if (version_to_download == "latest")
            //{
            //    // Add support for using latest, for now tags are used.
            //}

            YUR_Log.Log("Downloading asset bundle");
            string URI = GitHubRepoDownloadURI + version_to_download + "/" + assetBundleName;
            UnityEngine.Networking.UnityWebRequest request = new UnityEngine.Networking.UnityWebRequest(URI);
            yield return request.SendWebRequest();
            if (request.error != null)
            {
                YUR_Log.Warning("YUR was unable to get the necessary asset bundle from the server. Error: " + request.error);
                YUR_Log.Warning((priorInstalled ? "Attempting to use prior installed version..." : "No prior version of YUR installed. Please join our discord at https://yur.chat for assistance."));
            }
            else
            {
                YUR_Log.Log("YUR most recent asset bundle downloaded successfully");
                bool AssetsDownloaded;
                yield return AssetsDownloaded = SaveDownloadedAsset(request);
            }
        }

        // TODO Finish downloading library, then move onto downloading asset bundle
        /// <summary>
        /// Save the downloaded dll to its appropriate location
        /// </summary>
        /// <param name="request"></param>
        /// <param name="libraryName"></param>
        /// <param name="Managed"></param>
        /// <returns></returns>
        public bool SaveDownloadedLibrary(UnityWebRequest request, string libraryName, bool Managed = true)
        {
            YUR_Log.Log("Saving YUR DLL for next use. ");
            try
            {
                string pathLOCAL;
                string fullPath;
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    pathLOCAL = "file:///" + Application.dataPath + "/YUR/Plugins/" + (Managed ? "" : "x64/");
                   
                }
                //else if (Application.platform == RuntimePlatform.Android && Application.platform != RuntimePlatform.WindowsEditor)
                //{
                //    pathLOCAL = "file:///" + Application.dataPath + "/YUR/" + 
                //}
                else
                {
                    pathLOCAL = "file:///" + Application.dataPath + (Managed ? "/Managed/" : "/Plugins/" + (Managed ? "" : "Android/"));
                }
                fullPath = pathLOCAL + libraryName;
                // Create the directory if it doesn't already exist
                if (!Directory.Exists(pathLOCAL))
                {
                    Directory.CreateDirectory(pathLOCAL);
                }

                // Initialize the byte string
                byte[] bytes = request.downloadHandler.data;

                // Creates a new file, writes the specified byte array to the file, and then closes the file. 
                // If the target file already exists, it is overwritten.
                File.WriteAllBytes(fullPath, bytes);
                YUR_Log.Log("Finished saving. Restart to update");
                return true;
            }
            catch (Exception e)
            {
                YUR_Log.Error(e.ToString());
                return false;
            }
        }

        public bool SaveDownloadedAsset(UnityWebRequest objSERVER)
        {
            YUR_Log.Log("Saving YUR asset bundle for next use");
            try
            {
                string pathLOCAL = AssetBundlesLocalFilePath;
                // Create the directory if it doesn't already exist
                if (!Directory.Exists(pathLOCAL))
                {
                    Directory.CreateDirectory(pathLOCAL);
                }

                pathLOCAL += assetBundleName;

                // Initialize the byte string
                byte[] bytes = objSERVER.downloadHandler.data;

                // Creates a new file, writes the specified byte array to the file, and then closes the file. 
                // If the target file already exists, it is overwritten.
                File.WriteAllBytes(pathLOCAL, bytes);
                YUR_Log.Log("Finished saving asset");
                return true;
            }
            catch (Exception e)
            {
                YUR_Log.Error("------------------ YUR ERROR ------------------ \n" + e);
                return false;
            }

        }

        public bool SaveDownloadedPackage(UnityWebRequest objSERVER, string version_name)
        {
            YUR_Log.Log("Saving YUR asset bundle for next use");
            try
            {
                string pathLOCAL = "file:///" + Application.dataPath + "/YUR/";
                // Create the directory if it doesn't already exist
                if (!Directory.Exists(pathLOCAL))
                {
                    Directory.CreateDirectory(pathLOCAL);
                }

                pathLOCAL += version_name;

                // Initialize the byte string
                byte[] bytes = objSERVER.downloadHandler.data;

                // Creates a new file, writes the specified byte array to the file, and then closes the file. 
                // If the target file already exists, it is overwritten.
                File.WriteAllBytes(pathLOCAL, bytes);
                YUR_Log.Log("Finished saving asset");
                return true;
            }
            catch (Exception e)
            {
                YUR_Log.Error("------------------ YUR ERROR ------------------ \n" + e);
                return false;
            }

        }
    }


    /// <summary>
    /// All of YUR Settings fields and properties are kept here
    /// </summary>
    public partial class YUR : MonoBehaviour
    {
        /// <summary>
        /// Loaded on runtime, managed by helper script in editor
        /// </summary>
        [HideInInspector]
        public YURSettingsScriptableObject Settings;

        public static YUR Yur { get { return _yur; } set { _yur = value; } }
        private static YUR _yur;

        public UnityEngine.EventSystems.EventSystem eventSystem { get { return _eventSystem; } set { _eventSystem = value; } }

        private UnityEngine.EventSystems.EventSystem _eventSystem;

        /// <summary>
        /// Choosing appropriate platform is necessary for automatic GUI to work
        /// </summary>
        [HideInInspector]
        public VRUiKits.Utils.VRPlatform platform;

        /// <summary>
        /// Controller trigger input name defined in the input manager. This is necessary for the gesture recognition system.
        /// </summary>
        [HideInInspector]
        public CalorieCounter calorieCounter;
        [HideInInspector]
        public YUR_GestureRecognition YURGesture;
        public GameObject Camera { get { return _mainCamera; } set { _mainCamera = value; } }
        private GameObject _mainCamera;

        [HideInInspector]
        public GameObject GUITransform;
        [HideInInspector]
        public Color BackgroundColor = new Color()
        {
            r = 0.9245283f,
            g = 0.5177358f,
            b = 0,
            a = 0.7686275f
        };

        [HideInInspector]
        public Vector3 YURGUIPositionOffset;
        [HideInInspector]
        public Vector3 YURGUIRotationOffset;
        [HideInInspector]
        public float YURGUIScale;

        [HideInInspector]
        public GameObject CalorieDisplay;
        [HideInInspector]
        public Vector3 CalorieDisplayPositionOffset;
        [HideInInspector]
        public Vector3 CalorieDisplayRotationOffset;

        /// <summary>
        /// Prefab that contains all necessary screens for User if using Pre-built UI.
        /// </summary>
        //public GameObject YURDash { get { return YUR_Dash; } set { YUR_Dash = value; } }
        [HideInInspector]
        public GameObject YUR_Dash;

        public UI.YURScreenSystem YuRScreenSystem { get { return yURScreenSystem; } set { yURScreenSystem = value; } }
        private UI.YURScreenSystem yURScreenSystem;
        /// <summary>
        /// Used for Accessing internal script of the DLL
        /// </summary>
        public YUR_Main Main { get { return Fit; } set { Fit = value; } }
        private YUR_Main Fit;

        /// <summary>
        /// Game Object housing YUR main functionality
        /// </summary>
        public GameObject YURGameObject { get { return YUR_GO; } set { YUR_GO = value; } }
        private GameObject YUR_GO;

        [HideInInspector]
        public float _label_fontsize = 4;
        [HideInInspector]
        public Color _label_color = Color.white;

        [HideInInspector]
        public float _counter_fontsize = 4;
        [HideInInspector]
        public Color _counter_color = Color.white;
        [HideInInspector]
        public TMPro.TMP_FontAsset CalorieDisplayFonts;
        [HideInInspector]
        public string LeftControllerTrigger;
        [HideInInspector]
        public string RightControllerTrigger;


    }

}