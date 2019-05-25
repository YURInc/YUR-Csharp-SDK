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

        public void Awake()
        {
            calorieCounter = this;
            Calories.StartCounting += StartCounting;
            Calories.EndCounting += StopCounting;
            Debug.Log("Calorie Counter is initialized");

            if (FollowCamera)
            {
                /// Create UI Asset
                if(CaloriesDisplay == null)
                    CaloriesDisplay = UI.CreateUI.CreateCanvas(Calories.CalorieCounter.CalorieDisplayObject);
                if(CalorieCountDisplay == null)
                    CalorieCountDisplay = UI.CreateUI.CreateTMP_Text(CaloriesDisplay, "0", Calories.CalorieCounter.CalorieDisplayObject.transform.position, new Vector2(100, 50));

                Vector3 labelPosition = CalorieCountDisplay.transform.position;
                labelPosition.y += 4;

                if(CalorieLabel == null)
                    CalorieLabel = UI.CreateUI.CreateTMP_Text(CaloriesDisplay, "Calories", labelPosition, new Vector2(100, 50));
                Vector3 YUR_Position = Vector3.zero;
                Vector3 YUR_Direction = Vector3.zero;
                if (YUR.Yur.CalorieDisplay)
                {
                    Debug.Log("Calorie Display found, setting position and rotation!");
                    Calories.CalorieCounter.transform.SetParent(YUR.Yur.CalorieDisplay.transform);
                    Calories.CalorieCounter.transform.localPosition = YUR.Yur.CalorieDisplayPositionOffset;
                    Calories.CalorieCounter.transform.localEulerAngles = YUR.Yur.CalorieDisplayRotationOffset;
                    Calories.CalorieCounter.CalorieDisplayObject.transform.SetParent(Calories.CalorieCounter.transform);
                    Calories.CalorieCounter.CalorieDisplayObject.transform.localPosition = Vector3.zero;
                    Calories.CalorieCounter.CalorieDisplayObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }
                else
                {
                    YUR_Position = YUR.Yur.Camera.transform.position;
                    YUR_Direction = YUR.Yur.Camera.transform.forward;
                    YUR_Direction.y = 0;

                    Calories.CalorieCounter.CalorieDisplayObject.transform.SetPositionAndRotation(
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





        /// <summary>
        /// Determine users settings and begin counting calories
        /// </summary>
        public void StartCounting()
        {
            Debug.Log("Starting to count calories");

            Vector3 YUR_Position = Vector3.zero;
            Vector3 YUR_Direction = Vector3.zero;
            if (YUR.Yur.CalorieDisplay)
            {
                Debug.Log("Calorie Display is set transform");
                Calories.CalorieCounter.transform.localPosition = YUR.Yur.CalorieDisplayPositionOffset;
                Calories.CalorieCounter.transform.localEulerAngles = YUR.Yur.CalorieDisplayRotationOffset;
            }
            else
            {
                YUR_Position = YUR.Yur.Camera.transform.position;
                YUR_Direction = YUR.Yur.Camera.transform.forward;
                YUR_Direction.y = 0;

                Calories.CalorieCounter.CalorieDisplayObject.transform.SetPositionAndRotation(
                    new Vector3(YUR_Position.x + (YUR_Direction.x * 5),
                    YUR_Position.y, YUR_Position.z + (YUR_Direction.z * 5)),
                    Quaternion.LookRotation(YUR_Direction));
            }

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
            Calories.CalorieCounter.totalCaloriesBurnt = 0;
            gameObject.SetActive(true);
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
            StopAllCoroutines();
            gameObject.SetActive(false);
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
                    Calories.CalorieCounter.totalCaloriesBurnt += lastBurn;
                    CalorieCountDisplay.text = Calories.CalorieCounter.totalCaloriesBurnt.ToString(decimalPlace);
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