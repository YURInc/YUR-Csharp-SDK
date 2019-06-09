using UnityEngine;
using YUR.SDK.Unity.Systems.Interops;

namespace YUR.SDK.Unity.Systems.Tracking
{
    public class Calories : MonoBehaviour
    {
        public static float Biometric_Adjustment(UserData.Biometrics.Sexs sex, float height_centimeters, float weight_kilograms, int age)
        {
            return CalorieTracking.BioAdjust((int)sex, height_centimeters, weight_kilograms, age);
        }

        public static float Calculate_Calories_Velocities(
            Vector3 head_Velocities,
            Vector3 leftHand_Velocities,
            Vector3 rightHand_Velocities,
            float timeStep,
            float weight_kilograms,
            float biometric_adjustment,
            bool units_kilograms = true,
            float FixedUpdate_FrameRate = 90)
        {
            return CalorieTracking.OnePassCalculation(
                head_Velocities.x,
                head_Velocities.y,
                head_Velocities.z,
                leftHand_Velocities.x,
                leftHand_Velocities.y,
                leftHand_Velocities.z,
                rightHand_Velocities.x,
                rightHand_Velocities.y,
                rightHand_Velocities.z,
                timeStep,
                weight_kilograms,
                biometric_adjustment,
                units_kilograms,
                FixedUpdate_FrameRate);
        }
    }
}

namespace YUR.SDK.Unity.Tracking
{
    public class Calories : MonoBehaviour
    {
        public delegate void CalorieStatus();
        public static event CalorieStatus StartCounting;
        public static event CalorieStatus EndCounting;

        public static Calories CalorieCounter;
        public float totalCaloriesBurnt = 0;
        public GameObject CalorieDisplayObject;

        public static void StartCalorieCounter()
        {
            StartCounting?.Invoke();           
            
            return;
        }

        void Awake()
        {
            CalorieCounter = this;
            CalorieDisplayObject = new GameObject("YUR Calorie Canvas");

            DontDestroyOnLoad(CalorieDisplayObject);
            DontDestroyOnLoad(CalorieCounter);
        }

        public static float EndCalorieCounter()
        {
            EndCounting?.Invoke();
            return CalorieCounter.totalCaloriesBurnt;
        }

    }
}