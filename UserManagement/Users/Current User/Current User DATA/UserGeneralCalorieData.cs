using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace YUR.SDK.Unity.UserData
{
    public class GeneralCalorieData
    {
        private void NotifyPropertyChanged(string propertyname = "UserCalories")
        {
            YUR_Main.main.User_Manager.CurrentUser.PropertyChangedInMyClass(this, new PropertyChangedEventArgs(propertyname));
        }

        [SerializeField]
        private int all_calories;
        [SerializeField]
        private string last_game;
        [SerializeField]
        private long last_modified;
        [SerializeField]
        private long last_played;
        [SerializeField]
        private long last_resetDay;
        [SerializeField]
        private long last_resetWeek;
        [SerializeField]
        private long time_played;
        [SerializeField]
        private int todays_calories;
        [SerializeField]
        private long weekly_calories;

        
        public int Calories_All { get { return all_calories; } set { if (value != all_calories) { all_calories = value; NotifyPropertyChanged(); } } }
        
        public string Last_game { get { return last_game; } set { if (value != last_game) { last_game = value; NotifyPropertyChanged(); } } }
        
        public long Last_modified { get { return last_modified; } set { if (value != last_modified) { last_modified = value; NotifyPropertyChanged(); } } }
        
        public long Last_played { get { return last_played; } set { if (value != last_played) { last_played = value; NotifyPropertyChanged(); } } }
        
        public long Last_resetDay { get { return last_resetDay; } set { if (value != last_resetDay) { last_resetDay = value; NotifyPropertyChanged(); } } }
        
        public long Last_resetWeek { get { return last_resetWeek; } set { if (value != last_resetWeek) { last_resetWeek = value; NotifyPropertyChanged(); } } }
        
        public long Time_played { get { return time_played; } set { if (value != time_played) { time_played = value; NotifyPropertyChanged(); } } }
        
        public int Calories_Today { get { return todays_calories; } set { if (value != todays_calories) { todays_calories = value; NotifyPropertyChanged(); } } }
        
        public long Calories_Weekly { get { return weekly_calories; } set { if (value != weekly_calories) { weekly_calories = value; NotifyPropertyChanged(); } } }
    }
}
