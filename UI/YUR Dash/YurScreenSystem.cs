// YURScreenSystem Class: Responsible for setting up and position YUR Dash
using YUR.SDK.Unity.UserManagement;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

namespace YUR.SDK.Unity.UI
{
    public class YURScreenSystem : MonoBehaviour
    {
        public GameObject UIPointerKit;
        public GameObject EventSystem;
        public UnityEngine.EventSystems.EventSystem OriginalEventSystem;
        public VRUiKits.Utils.LaserInputModule LaserIM;
        
        public bool isActivated = false;

        public static YURScreenSystem ScreenSystem;

        public void Awake()
        {
            ScreenSystem = this;
            DontDestroyOnLoad(ScreenSystem);



            //action_single = Valve.VR.SteamVR_Input.GetSingleAction("YURPrimarySqueeze");
        }

        /// <summary>
        /// Show the YUR User Interface Screen
        /// </summary>
        /// <param name="UseYURUISystem">True to use the interaction system included with YUR</param>
        public void PresentYURGUI(bool UseYURUISystem = true)
        {
            if(isActivated) // If screen is already present, return;
            {
                return;
            }
            YUR_Log.Log("Activating YUR Dash");
            isActivated = true;
            if (Login.Status == Login.StatusType.Logging_In) // True, Subscribe to Successful Login
            {
                YUR_UserManager.Successful_Login += Successful_Login_PrepareScreenSystem;
                YUR_UserManager.Bad_Login += YUR_UserManager_Bad_Login;
                isActivated = false;
                return;

            }
            else if (Login.Status == Login.StatusType.Logged_In) // True, Show Main Menu on UI call
            {
                YURScreenCoordinator.ScreenCoordinator.Hierarchy.Add(YURScreenCoordinator.ScreenCoordinator.MainMenu);
                //UI.Background.Backgrounds.Instance.SetBackground(YURScreenCoordinator.ScreenCoordinator.MainMenu);
                
            }
            else // Display YUR Initial Login Screen
            {
                YURScreenCoordinator.ScreenCoordinator.Hierarchy.Add(YURScreenCoordinator.ScreenCoordinator.InitialMenu);
               // UI.Background.Backgrounds.Instance.SetBackground(YURScreenCoordinator.ScreenCoordinator.InitialMenu);
            }

            StartCoroutine(_presentYURGUI(UseYURUISystem)); // Setup and show the Dash
        }

        /// <summary>
        /// Close YUR Dash
        /// </summary>
        public void DeactivateYURDash()
        {
            foreach(YURScreenController screenController in YURScreenCoordinator.ScreenCoordinator.Hierarchy)
            {
                screenController.gameObject.SetActive(false);
            }
            YURScreenCoordinator.ScreenCoordinator.Hierarchy.Clear();
            YURScreenCoordinator.ScreenCoordinator.gameObject.SetActive(false);
            //EventSystem = null;
            Destroy(LaserIM);
            Destroy(UIPointerKit);
            UnityEngine.EventSystems.EventSystem.current = OriginalEventSystem;
            if(EventSystem != null)
                Destroy(EventSystem);
            Destroy(GameObject.Find("Helper Camera"));
            isActivated = false;

        }

        /// <summary>
        /// Create the dash and present it
        /// </summary>
        /// <returns></returns>
        private IEnumerator _presentYURGUI(bool UsingYURinteractionSystem)
        {
            GameObject Camera;
            try
            {
                Camera = YUR.Yur.Camera;
                YUR_Log.Log("Camera name: " + Camera.name);
            }
            catch(System.Exception e)
            {
                Debug.LogError("MainCamera tag is not set. YUR Dash will not work unless this is changed");
                Debug.LogError("Error: " + e);
                yield break;
            }

            Vector3 YUR_Position;
            Vector3 YUR_Direction;

            /// Display the ScreenCoordinator Object and set the background color of the Dash
            YURScreenCoordinator.ScreenCoordinator.gameObject.SetActive(true);
            YURScreenCoordinator.ScreenCoordinator.Background.GetComponent<UnityEngine.UI.Image>().color = YUR.Yur.BackgroundColor;


            /// If a previously specified gameobject is to be used for the location of dash.
            if (YUR.Yur.GUITransform != null)
            {
                Debug.Log("Displaying on GUITransform", YUR.Yur.GUITransform);
                YUR.Yur.YUR_Dash.transform.localScale = new Vector3(YUR.Yur.YUR_Dash.transform.localScale.x * (YUR.Yur.YURGUIScale  + 1), YUR.Yur.YUR_Dash.transform.localScale.y * (YUR.Yur.YURGUIScale + 1), YUR.Yur.YUR_Dash.transform.localScale.z * (YUR.Yur.YURGUIScale + 1));
                YUR.Yur.YUR_Dash.transform.SetParent(YUR.Yur.GUITransform.transform);
                YUR.Yur.YUR_Dash.transform.localPosition = YUR.Yur.YURGUIPositionOffset;
                YUR.Yur.YUR_Dash.transform.localEulerAngles = YUR.Yur.YURGUIRotationOffset;
            }
            else /// Else, position the dash relative to the camera position;
            {
                YUR_Log.Log("Moving Dash in front of player");
                YUR_Position = Camera.transform.position;
                YUR_Direction = Camera.transform.forward;
                YUR_Direction.y = 0;
                YUR_Log.Log("Dash Positioning: \nPosition: " + YUR_Position + "\nDirection: " + YUR_Direction + "\nYUR Dash Position: " + YUR.Yur.YUR_Dash.transform.position, YUR.Yur.YUR_Dash);

                YUR.Yur.YUR_Dash.transform.SetPositionAndRotation(
                    new Vector3(YUR_Position.x + (YUR_Direction.x * 2), YUR_Position.y, YUR_Position.z + (YUR_Direction.z * 2)),
                    Quaternion.LookRotation(YUR_Direction));

                YUR_Log.Log("Dash Positioning: \nPosition: " + YUR_Position + "\nDirection: " + YUR_Direction + "\nYUR Dash Position: " + YUR.Yur.YUR_Dash.transform.position, YUR.Yur.YUR_Dash);
            }

            /// Present the first screen to user
            YURScreenCoordinator.ScreenCoordinator.Hierarchy[0].gameObject.SetActive(true);
            bool leftHanded = false;
            bool instantiatedUIKit = false;

            int children;

            /// Deteremine the number of children in which the Camera object has, differing counts are for different platforms
            if (Camera.transform.parent)
                children = Camera.transform.parent.childCount;
            else
                children = Camera.transform.childCount;

            if (!UsingYURinteractionSystem) // If Not using YUR Interaction System, stop here
                yield break;

            if (YUR.Yur.YURAssetBundle == null)
            {
                string URI = "file:///" + Application.dataPath + "/AssetBundles/" + YUR.Yur.assetBundleName;
                UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestAssetBundle.GetAssetBundle(URI, 0);
                yield return request.SendWebRequest();
                if (request.error != null)
                {
                    yield break;
                }
                else
                {
                    YUR_Log.Log("Local asset bundle found!");
                    yield return YUR.Yur.YURAssetBundle = DownloadHandlerAssetBundle.GetContent(request);
                }
            }

            ////_____________________________________________________________________________________/
            /// Take Control of the Event System, is returned when YUR Dash is closed.
            //_____________________________________________________________________________________/
            OriginalEventSystem = UnityEngine.EventSystems.EventSystem.current;


            if (YUR.Yur.platform == VRUiKits.Utils.VRPlatform.OCULUS)
            {
                GameObject temppp;
                yield return temppp = YUR.Yur.YURAssetBundle.LoadAsset<GameObject>("LaserInputModuleOculus");

                YUR_Log.Log("Loaded the Laser Input Module for Oculus, instantiating");

                yield return EventSystem = (Instantiate(temppp) as GameObject);// = EventSystem.gameObject.AddComponent<VRUiKits.Utils.OculusLaserInputModule>();

                EventSystem.name = "Event System";
                UnityEngine.EventSystems.EventSystem.current = EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>();
                YUR.Yur.eventSystem = EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>();

                yield return LaserIM = EventSystem.GetComponent<VRUiKits.Utils.LaserInputModule>();
                YUR_Log.Log("Oculus Input Module Added");
            }
            else if (YUR.Yur.platform == VRUiKits.Utils.VRPlatform.VIVE_STEAM2)
            {
                var temppp = YUR.Yur.YURAssetBundle.LoadAsset<GameObject>("LaserInputModuleSteam2");

                yield return EventSystem = (Instantiate(temppp) as GameObject);// = EventSystem.gameObject.AddComponent<VRUiKits.Utils.OculusLaserInputModule>();
                EventSystem.name = "Event System";
                UnityEngine.EventSystems.EventSystem.current = EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>();
                YUR.Yur.eventSystem = EventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>();
                yield return LaserIM = EventSystem.GetComponent<VRUiKits.Utils.LaserInputModule>();
                YUR_Log.Log("Steam Input Module Added");
            }

            if (YUR.Yur.platform == VRUiKits.Utils.VRPlatform.OCULUS) /// If the platform is defined as being Oculus, use the Camera object to locate the Controllers
            {
                YUR_Log.Log("Locating Oculus Controllers...");
                for (int i = 0; i < YUR.Yur.Camera.transform.parent.childCount; i++)
                {
                    if (YUR.Yur.Camera.transform.parent.GetChild(i).gameObject.name.Contains("RightHand"))
                    {

                        var temppp = YUR.Yur.YURAssetBundle.LoadAsset<GameObject>("OculusUIKitPointer");
                        UIPointerKit = Instantiate(temppp, YUR.Yur.Camera.transform.parent.GetChild(i).gameObject.transform);
                        //UIPointerKit = (GameObject)Instantiate(Resources.Load("OculusUIKitPointer"), YUR.Yur.Camera.transform.parent.GetChild(i).gameObject.transform);
                        instantiatedUIKit = true;
                        leftHanded = false;
                        YUR_Log.Log("Found Oculus Controller Right");
                        break;
                    }
                    else if (YUR.Yur.Camera.transform.parent.GetChild(i).gameObject.name.Contains("LeftHand"))
                    {
                        var temppp = YUR.Yur.YURAssetBundle.LoadAsset<GameObject>("OculusUIKitPointer");
                        UIPointerKit = Instantiate(temppp, YUR.Yur.Camera.transform.parent.GetChild(i).gameObject.transform);
                        //UIPointerKit = (GameObject)Instantiate(Resources.Load("OculusUIKitPointer"), YUR.Yur.Camera.transform.parent.GetChild(i).gameObject.transform);
                        instantiatedUIKit = true;
                        leftHanded = true;
                        YUR_Log.Log("Found Oculus Controller Left");
                        break;
                    }
                }
            }
            else if (YUR.Yur.platform == VRUiKits.Utils.VRPlatform.VIVE_STEAM2)  /// If the platform is Steam VR 2, find the preferred controller.
            {
                YUR_Log.Log("Locating Steam VR 2 Controllers...");
                for (int i = 0; i < children; i++)
                {
                    Debug.Log("Found Controller: " + Camera.transform.parent.GetChild(i).gameObject.name);
                    if (Camera.transform.parent.GetChild(i).gameObject.name.Contains("right"))
                    {
                        var temppp = YUR.Yur.YURAssetBundle.LoadAsset<GameObject>("Steam2UIKitPointer");
                        UIPointerKit = Instantiate(temppp, YUR.Yur.Camera.transform.parent.GetChild(i).gameObject.transform);
                        // UIPointerKit = (GameObject)Instantiate(Resources.Load("Steam2UIKitPointer"), Camera.transform.parent.GetChild(i).gameObject.transform);
                        instantiatedUIKit = true;
                        leftHanded = false;
                        YUR_Log.Log("Found Steam Controller right");
                        break;
                    }
                    if (Camera.transform.parent.GetChild(i).gameObject.name.Contains("left"))
                    {
                        var temppp = YUR.Yur.YURAssetBundle.LoadAsset<GameObject>("Steam2UIKitPointer");
                        UIPointerKit = Instantiate(temppp, YUR.Yur.Camera.transform.parent.GetChild(i).gameObject.transform);
                        // UIPointerKit = (GameObject)Instantiate(Resources.Load("Steam2UIKitPointer"), Camera.transform.parent.GetChild(i).gameObject.transform);
                        instantiatedUIKit = true;
                        leftHanded = true;
                        YUR_Log.Log("Found Steam Controller Left");
                        break;
                    }

                }
            }

            LaserIM.platform = YUR.Yur.platform;
            LaserIM.pointer = (leftHanded ? VRUiKits.Utils.Pointer.LeftHand : VRUiKits.Utils.Pointer.RightHand);

            if (!instantiatedUIKit) // If UIPointerKit not previously instantiated
            {
                var temppp = YUR.Yur.YURAssetBundle.LoadAsset<GameObject>("UIKitPointer");
                UIPointerKit = Instantiate(temppp);
                //UIPointerKit = (GameObject)Instantiate(Resources.Load("UIKitPointer"));
            }


            //YUR_Log.Log("Checking for event system...");
            //if (YUR.Yur.eventSystem == null) // If there is no event system, create a new gameobject and attach an Event System Component
            //{
            //    YUR_Log.Log("None found, create event system...");
            //    //EventSystem = new GameObject("EventSystem");

            //    //yield return YUR.Yur.eventSystem = EventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            //}
            //else
            //{
            //    YUR_Log.Log("Found event System");
            //    EventSystem = YUR.Yur.eventSystem.gameObject;
            //}

            //if(EventSystem.gameObject.GetComponent<VRUiKits.Utils.VREventSystemHelper>())
            //{
            //    YUR_Log.Log("VR Event System Helper already added to game object");
            //}
            //else
            //{
            //    YUR_Log.Log("Adding Event System Helper and Input Module");
            //    EventSystem.gameObject.AddComponent<VRUiKits.Utils.VREventSystemHelper>();
            //}


        }

        public void Update() ///  Used for testing
        {

            if (Input.GetKey("a"))
            {
                PresentYURGUI(YUR.Yur.Settings.UseYURInteractionSystem); // Press the "a" key to present YUR Dash
            }
            if(Input.GetKey("x")) // Press the "x" key to close YUR Dash
            {
                DeactivateYURDash();
            }
            if (Input.GetKey("l")) // Login to a test user account
            {
                Login.Email_Password("ethan@yur.fit", "123654e");
            }
            if(Input.GetKey("w")) // Start a workout
            {
                Workouts.Workout.StartingWorkout();
            }
            if(Input.GetKey("e")) // End a workout with a specified string
            {
                Workouts.Workout.EndingWorkout("Unity Example Scene");
            }
        }

        public enum Screens
        {
            MainMenu, InitialLogin
        }

        private string YUR_UserManager_Bad_Login(string response)
        {
            PresentYURGUI();
            return response;
        }

        private string Successful_Login_PrepareScreenSystem(string response)
        {
            PresentYURGUI();
            return response;
        }

    }
}