using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace YUR.SDK.Unity.UI
{
    public class StatsViewer : MonoBehaviour
    {
        
        public TextMeshProUGUI GameName;
        [Header("Today")]
        public Slider DailyProgressBar;
        public TextMeshProUGUI DisplayDailyCalories;

        [Header("Weekly")]
        public Slider WeeklyProgressBar;
        public TextMeshProUGUI DisplayWeeklyCalories;

        [Header("All Time")]
        public TextMeshProUGUI DisplayAllTimeCalories;

        [Header("Other Stats")]
        public TextMeshProUGUI TimePlayedOutput;

        [Header("Goals")]
        public int DailyCalorieGoal = 350;
        public int WeeklyCalorieGoal = 3500;
        
        public void GetValues(ref UserData.GameData UserDataClass)
        {
            if(UserDataClass == null)
            {
                DisplayDailyCalories.text = "0";
                DailyProgressBar.value = 0;
                DisplayWeeklyCalories.text = "0";
                WeeklyProgressBar.value = 0;
                DisplayAllTimeCalories.text = "0";
                TimePlayedOutput.text = "0 miliseconds";
                return;
            }
            DisplayDailyCalories.text = UserDataClass.Calories_Today.ToString();
            DailyProgressBar.value = (float)((decimal)UserDataClass.Calories_Today / DailyCalorieGoal);

            DisplayWeeklyCalories.text = UserDataClass.Calories_Weekly.ToString();
            WeeklyProgressBar.value = (float)((decimal)UserDataClass.Calories_Weekly / WeeklyCalorieGoal);

            DisplayAllTimeCalories.text = UserDataClass.Calories_All.ToString();
            TimePlayedOutput.text = TimePlayed(ref UserDataClass);
        }

        public void GetValues(ref UserData.GeneralCalorieData UserDataClass)
        {
            DisplayDailyCalories.text = UserDataClass.Calories_Today.ToString();
            DailyProgressBar.value = (float)((decimal)UserDataClass.Calories_Today / DailyCalorieGoal);

            DisplayWeeklyCalories.text = UserDataClass.Calories_Weekly.ToString();
            WeeklyProgressBar.value = (float)((decimal)UserDataClass.Calories_Weekly / WeeklyCalorieGoal);

            DisplayAllTimeCalories.text = UserDataClass.Calories_All.ToString();
            TimePlayedOutput.text = TimePlayed(ref UserDataClass);
        }

        public string TimePlayed(ref UserData.GameData UserDataClass)
        {
            YUR_Log.Log("Time Played: " + UserDataClass.Time_Played);
            TimeSpan btime = TimeSpan.FromSeconds(UserDataClass.Time_Played);
            YUR_Log.Log("Time Span: " + btime);
            int Days = btime.Days;
            int Weeks = 0;

            WeekCalculate(ref Days, ref Weeks);

            string ret = (Weeks > 0 ? Weeks + "w " : "" ) + (Days > 0 ? Days + "d " : "") + btime.Hours.ToString() + "h " + btime.Minutes.ToString() + "m " + btime.Seconds.ToString() + "s";
            return ret;
        }

        public string TimePlayed(ref UserData.GeneralCalorieData UserDataClass)
        {
            TimeSpan btime = TimeSpan.FromSeconds(UserDataClass.Time_played);

            int Days = btime.Days;
            int Weeks = 0;

            WeekCalculate(ref Days, ref Weeks);

            string ret = (Weeks > 0 ? Weeks + "w " : "") + (Days > 0 ? Days + "d " : "") + btime.Hours.ToString() + "h " + btime.Minutes.ToString() + "m " + btime.Seconds.ToString() + "s";
            return ret;
        }

        /// <summary>
        /// Use recursion to determine number of weeks
        /// </summary>
        /// <param name="Days"></param>
        /// <param name="Weeks"></param>
        public void WeekCalculate(ref int Days, ref int Weeks)
        {
            if (Days >= 7)
            {
                Days = Days - 7;
                Weeks++;
                WeekCalculate(ref Days, ref Weeks);
            }
            else
            {
                return;
            }
        }


    }
}
