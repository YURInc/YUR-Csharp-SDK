using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace YUR.SDK.Unity.UserData 
{
    public class Biometrics
    {
        //private bool SET = true;

        public enum Sexs
        {
            Female, Male, Unspecified
        }

        private void NotifyPropertyChanged(string propertyname = "UserBiometrics")
        {
            YUR_Main.main.User_Manager.CurrentUser.PropertyChangedInMyClass(this, new PropertyChangedEventArgs(propertyname));
        }

        [SerializeField]
        private int age;
        [SerializeField]
        private List<string> birthdate;
        
        public Unit customary;
        [SerializeField]
        private object gamertag;
        [SerializeField]
        private long last_modified;
        [SerializeField]
        public Unit metric;
        [SerializeField]
        private bool metric_units;
        [SerializeField]
        private string name;
        [SerializeField]
        private string sex;

        public int Age { get { return age; } set { if (value != age) { age = value; NotifyPropertyChanged(); } } }
        
        public List<string> BirthDate { get { return birthdate; } set { if (value != birthdate) { birthdate = value; NotifyPropertyChanged(); } } }
        
        public object GamerTag { get { return gamertag; } set { if (value != gamertag) { gamertag = value; NotifyPropertyChanged(); } } }
        
        public long Last_modified { get { return last_modified; } set { if (value != last_modified) { last_modified = value; NotifyPropertyChanged(); } } }
        
        public bool Metric_Units { get { return metric_units; } set { if (value != metric_units) { metric_units = value; NotifyPropertyChanged(); } } }
        
        public string Name { get { return name; } set { if (value != name) { name = value; NotifyPropertyChanged(); } } }
        
        public string Sex { get { return sex; } set { if (value != sex) { sex = value; NotifyPropertyChanged(); } } }
    }

    [Serializable]
    public class Unit
    {

        private void NotifyPropertyChanged(string propertyname = "UserBiometrics")
        {
            //Console.WriteLine(Plugin.modLog + "User Biometrics property changed");

            YUR_Main.main.User_Manager.CurrentUser.PropertyChangedInMyClass(this, new PropertyChangedEventArgs(propertyname));
        }

        [SerializeField]
        private float height;
        [SerializeField]
        private int weight;

        
        public float Height { get { return height; } set { if (value != height) { height = value; NotifyPropertyChanged(); } } }
        
        public int Weight { get { return weight; } set { if (value != weight) { weight = value; NotifyPropertyChanged(); } } }
    }

}
