using System.ComponentModel;
using UnityEngine;

namespace YUR.SDK.Unity.UserData
{
    public class GameData
    {

        private void NotifyPropertyChanged(string propertyname = "UserCalories")
        {
            YUR_Main.main.User_Manager.CurrentUser.PropertyChangedInMyClass(this, new PropertyChangedEventArgs(propertyname));
        }

        [SerializeField]
        private int all_calories;
        [SerializeField]
        private long time_played;
        [SerializeField]
        private long last_played;
        [SerializeField]
        private long last_modified;
        [SerializeField]
        private int todays_calories;
        [SerializeField]
        private int weekly_calories;
        [SerializeField]
        private string gameID;


        
        public int Calories_All { get { return all_calories; } set { if (value != all_calories) { all_calories = value; NotifyPropertyChanged(); } } }
        
        public long Time_Played { get { return time_played; } set { if (value != time_played) { time_played = value; NotifyPropertyChanged(); } } }
        
        public long Last_Played { get { return last_played; } set { if (value != last_played) { last_played = value; NotifyPropertyChanged(); } } }
        
        public long Last_Modified { get { return last_modified; } set { if (value != last_modified) { last_modified = value; NotifyPropertyChanged(); } } }
        
        public int Calories_Today { get { return todays_calories; } set { if (value != todays_calories) { todays_calories = value; NotifyPropertyChanged(); } } }
        
        public int Calories_Weekly { get { return weekly_calories; } set { if (value != weekly_calories) { weekly_calories = value; NotifyPropertyChanged(); } } }
        
        public string Game_Identifier { get { return gameID; } set { if (value != gameID) { gameID = value; NotifyPropertyChanged(); } } }

    }
}
