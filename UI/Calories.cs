using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using TMPro;
using YUR.SDK.Unity.Tracking;

namespace YUR.SDK.Unity
{
    public class CalorieCounter : MonoBehaviour
    {
        public delegate void Finished();
        public event Finished Completed;
        public static CalorieCounter calorieCounter;

        public TMP_Text CalorieCountDisplay;
        public TMP_Text CalorieLabel;
        public Canvas CaloriesDisplay;

        public bool isSetup = false;
        
        public bool FollowCamera = true;
        [HideInInspector]
        public enum Decimals
        {
            Tenths, Hundreths, Thousandths
        }

        private string decimalPlace = "n2";


        List<XRNodeState> nodeStates = new List<XRNodeState>();
        Vector3 HMDvelocity;
        Vector3 LHCvelocity;
        Vector3 RHCvelocity;
        float WeightInKilograms;
        UserData.Biometrics.Sexs Sexs;
        float BioAdjustment;

        bool IsRunning = false;

        //public bool CustomSetup(Decimals DecimalPlace, float Label_FontSize, Color Label_Color, float Counter_FontSize, Color Counter_Color)
        //{
        //    CalorieCountDisplay.fontSize = Counter_FontSize;
        //    CalorieCountDisplay.color = Counter_Color;
        //}
        public void OnDestroy()
        {
            Debug.Log("CalorieCounter destroyed with " + transform.childCount + " children");
            Debug.Break();
        }

        public void Awake()
        {
            if(calorieCounter == null)
                calorieCounter = this;
            
            Calories.StartCounting += StartCounting;
            Calories.EndCounting += StopCounting;
            Debug.Log("Calorie Counter is initialized");

            if (FollowCamera)
            {
                /// Create UI Asset
                if(CaloriesDisplay == null)
                    CaloriesDisplay = UI.CreateUI.CreateCanvas(Calories.calories.CalorieDisplayObject);
                if(CalorieCountDisplay == null)
                    CalorieCountDisplay = UI.CreateUI.CreateTMP_Text(CaloriesDisplay, "0", Calories.calories.CalorieDisplayObject.transform.position, new Vector2(100, 50));

                Vector3 labelPosition = CalorieCountDisplay.transform.position;
                labelPosition.y += 4;

                if(CalorieLabel == null)
                    CalorieLabel = UI.CreateUI.CreateTMP_Text(CaloriesDisplay, "Calories", labelPosition, new Vector2(100, 50));
                Vector3 YUR_Position = Vector3.zero;
                Vector3 YUR_Direction = Vector3.zero;
                if (YUR.Yur.CalorieDisplay)
                {
                    Debug.Log("Calorie Display found, setting position and rotation!");
                    Calories.calories.transform.SetParent(YUR.Yur.CalorieDisplay.transform);
                    Calories.calories.transform.localPosition = YUR.Yur.CalorieDisplayPositionOffset;
                    Calories.calories.transform.localEulerAngles = YUR.Yur.CalorieDisplayRotationOffset;
                    Calories.calories.CalorieDisplayObject.transform.SetParent(Calories.calories.transform);
                    Calories.calories.CalorieDisplayObject.transform.localPosition = Vector3.zero;
                    Calories.calories.CalorieDisplayObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }
                else
                {

                    YUR_Position = YUR.Yur.Camera.transform.position;
                    YUR_Direction = YUR.Yur.Camera.transform.forward;
                    YUR_Direction.y = 0;

                    Calories.calories.CalorieDisplayObject.transform.SetPositionAndRotation(
                        new Vector3(YUR_Position.x + (YUR_Direction.x * 5),
                        YUR_Position.y, YUR_Position.z + (YUR_Direction.z * 5)),
                        Quaternion.LookRotation(YUR_Direction));
                }

                gameObject.SetActive(false);
            }
        }
        public Decimals DecimalPlace
        {
            get
            {
                if (decimalPlace == "n1")
                {
                    return Decimals.Tenths;
                }
                else if (decimalPlace == "n2")
                {
                    return Decimals.Hundreths;
                }
                else
                {
                    return Decimals.Thousandths;
                }
            }
            set
            {
                if (value == Decimals.Tenths)
                    decimalPlace = "n1";
                else if (value == Decimals.Hundreths)
                    decimalPlace = "n2";
                else
                    decimalPlace = "n3";
            }
        }

        public GameObject newgobject;

        void Update()
        {
            if(newgobject != null)
            {
                YUR.Yur.CalorieDisplay.transform.SetPositionAndRotation(newgobject.transform.position, newgobject.transform.rotation);
            }
        }

        public IEnumerator WaitForOVRManager(bool Start)
        {
            if (Start)
            {
                // TODO !!!! Steam version!?
                while (!UnityOculusLibrary.OculusHelpers.IsOVRManager())
                {
                    for (int i = 0; i < 20; i++)
                    {
                        yield return new WaitForFixedUpdate();
                    }
                }

                newgobject = new GameObject("[YUR] Reference Object");
                var ParentImmortal = newgobject.AddComponent<Utilities.ParentGameObjectImmortality>();
                newgobject.transform.SetParent(UnityOculusLibrary.OculusHelpers.GetPlayer().transform);
               
                PositionDisplay();
                yield break;
            }
        }


        public void PositionDisplay()
        {
            Vector3 YUR_Position = Vector3.zero;
            Vector3 YUR_Direction = Vector3.zero;
            if (YUR.Yur.CalorieDisplay)
            {
                Debug.Log("Calorie Display is set transform");
                Calories.calories.transform.localPosition = YUR.Yur.CalorieDisplayPositionOffset;
                Calories.calories.transform.localEulerAngles = YUR.Yur.CalorieDisplayRotationOffset;
            }
            else
            {
                YUR_Position = YUR.Yur.Camera.transform.position;
                YUR_Direction = YUR.Yur.Camera.transform.forward;
                YUR_Direction.y = 0;

                Calories.calories.CalorieDisplayObject.transform.SetPositionAndRotation(
                    new Vector3(YUR_Position.x + (YUR_Direction.x * 5),
                    YUR_Position.y, YUR_Position.z + (YUR_Direction.z * 5)),
                    Quaternion.LookRotation(YUR_Direction));
            }
        }

        /// <summary>
        /// Determine users settings and begin counting calories
        /// </summary>
        public void StartCounting()
        {


            string UsersSex = UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_Biometrics.Sex;
            if (UsersSex == "male")
                Sexs = UserData.Biometrics.Sexs.Male;
            else if (UsersSex == "female")
                Sexs = UserData.Biometrics.Sexs.Female;
            else
                Sexs = UserData.Biometrics.Sexs.Unspecified;

            float Height;
            if (UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_Biometrics.Metric_Units)
            {
                WeightInKilograms = UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_Biometrics.metric.Weight;
                Height = UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_Biometrics.metric.Height * 100;
            }
            else
            {
                WeightInKilograms = UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_Biometrics.customary.Weight * 0.45359f;   // Convert weight pounds to kilograms
                Height = UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_Biometrics.customary.Height * 2.54f;                 // Convert height inches to centimeters
            }

            BioAdjustment = Systems.Tracking.Calories.Biometric_Adjustment(Sexs, Height, WeightInKilograms, UserManagement.YUR_UserManager.YUR_Users.CurrentUser.Data_Biometrics.Age);

            CalorieCountDisplay.text = "0";
            Calories.calories.totalCaloriesBurnt = 0;
            gameObject.SetActive(true);
            if (YUR.Yur.platform == VRUiKits.Utils.VRPlatform.OCULUS)
            {
                 StartCoroutine(WaitForOVRManager(true));
            }

            Debug.Log("Starting to count calories");


            isSetup = true;
            Completed?.Invoke();
        }

        /// <summary>
        /// Stop Counting all calories and return total Calories Burned
        /// </summary>
        /// <returns></returns>
        public void StopCounting()
        {
            Debug.Log("Stopping calorie counter!");
            isSetup = false;
            StopCoroutine(CountCoroutine());
            
        }

        public void FixedUpdate()
        {
            InputTracking.GetNodeStates(nodeStates);
            if(isSetup && !IsRunning)
            {
                YUR_Log.Log("Starting Calorie counter coroutine!");
                StartCoroutine(CountCoroutine());
            }
                
        }

        IEnumerator CountCoroutine()
        {
            while (true)
            {
                //YUR_Log.Log("Calories: " + Calories.calories.totalCaloriesBurnt);
                IsRunning = true;
                foreach (XRNodeState ns in nodeStates)
                {
                    if (ns.nodeType == XRNode.Head)
                    {
                        ns.TryGetVelocity(out HMDvelocity);
                    }
                    if (ns.nodeType == XRNode.LeftHand)
                    {
                        ns.TryGetVelocity(out LHCvelocity);
                    }
                    if (ns.nodeType == XRNode.RightHand)
                    {
                        ns.TryGetVelocity(out RHCvelocity);
                    }
                }

                float lastBurn = Systems.Tracking.Calories.Calculate_Calories_Velocities(HMDvelocity, LHCvelocity, RHCvelocity, 1, WeightInKilograms, BioAdjustment, FixedUpdate_FrameRate: YUR_Main.main.TimeStep);

                if (lastBurn > 0)
                {
                    Calories.calories.totalCaloriesBurnt += lastBurn;
                    CalorieCountDisplay.text = Calories.calories.totalCaloriesBurnt.ToString(decimalPlace);
                }

                for (float duration = 1; duration > 0; duration--)
                {
                    yield return new WaitForFixedUpdate();
                }

                IsRunning = false;
            }
        }
    }
}