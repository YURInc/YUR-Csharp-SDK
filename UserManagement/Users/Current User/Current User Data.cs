using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using YUR.SDK.Unity.UserData;

namespace YUR.SDK.Unity
{
    public partial class YUR_CurrentUser : MonoBehaviour
    {
        public Biometrics Data_Biometrics = new Biometrics();
        public GeneralCalorieData Data_General_Calories = new GeneralCalorieData();
        public GameData Data_Current_Game = new GameData();
        public int Data_Current_Session_Calories;

        internal bool ALTERED_userBiometrics { get; set; }
        internal bool ALTERED_userCalories { get; set; }
        internal bool ALTERED_userGameData { get; set; }

        internal bool BiometricDATA_Acquired = false;
        internal bool GeneralCalorieDATA_Acquired = false;
        internal bool GameCalorieDATA_Acquired = false;
        internal bool ProfileDATA_Acquired = false;

        void Awake()
        {
            DontDestroyOnLoad(this);
            ALTERED_userBiometrics = false;
            ALTERED_userCalories = false;
        }

        internal void Compare_Dates()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            long currentDate = (long)t.TotalSeconds;

            long lWeekChange_Gen = Data_General_Calories.Last_resetWeek;
            long lDateChange_Gen = Data_General_Calories.Last_resetDay;
            long lPlayDate_Gen = Data_General_Calories.Last_played;

            if ((currentDate - lDateChange_Gen) >= 86400 || (currentDate - lDateChange_Gen) > 64800)
            {
                Data_General_Calories.Calories_Today = 0;
                Data_Current_Game.Calories_Today = 0;
                Data_General_Calories.Last_resetDay = currentDate;
            }

            Data_General_Calories.Last_played = currentDate;
            Data_Current_Game.Last_Played = currentDate;

            if ((currentDate - lWeekChange_Gen) >= 604800)
            {
                Data_General_Calories.Calories_Weekly = 0;
                Data_Current_Game.Calories_Weekly = 0;
                Data_General_Calories.Last_resetWeek = currentDate;
            }


        }

        internal void PropertyChangedInMyClass(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserBiometrics")
            {
               ALTERED_userBiometrics = true;
            }
            if (e.PropertyName == "UserCalories")
            {
                ALTERED_userCalories = true;
            }
            if (e.PropertyName == "UserGameData")
                ALTERED_userGameData = true;
        }

        IEnumerator SavingAll(bool force)
        {
            Save(Data_Biometrics, force);
            yield return new WaitForFixedUpdate();
            Save(Data_General_Calories, force);
            yield return new WaitForFixedUpdate();
            Save(Data_Current_Game, force);
            yield break;
        }
        public void SaveAll(bool force = false)
        {
            StartCoroutine(SavingAll(force));
        }

        public void Save<T>(T UserClass, bool force = false)
        {
            try
            {
                if (!force)
                {

                    if (UserClass is GeneralCalorieData)
                    {
                        if (!ALTERED_userCalories || !UserDataCheck(Data_General_Calories))
                        {
                            return;
                        }


                    }
                    if (UserClass is Biometrics)
                    {
                        if (!ALTERED_userBiometrics || !UserDataCheck(Data_Biometrics))
                        {
                            return;
                        }

                    }

                    if (UserClass is GameData)
                    {
                        if (!ALTERED_userCalories || !UserDataCheck(Data_Current_Game))
                        {
                            return;
                        }

                    }
                }

                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                long currentDate = (long)t.TotalSeconds;

                if (UserClass is GeneralCalorieData)
                {
                    Data_General_Calories.Last_modified = currentDate;
                    UserManagement.YUR_UserManager.YUR_Users.SavingData(Data_General_Calories);
                }
                else if (UserClass is Biometrics)
                {
                    Data_Biometrics.Last_modified = currentDate;
                    UserManagement.YUR_UserManager.YUR_Users.SavingData(Data_Biometrics);
                }
                else if (UserClass is GameData)
                {
                    Data_Current_Game.Last_Modified = currentDate;
                    UserManagement.YUR_UserManager.YUR_Users.SavingData(Data_Current_Game);
                }
                else if (UserClass is YUR_CurrentUser)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                YUR_Log.Log("Exception[User|Save<T>] " + e);
            }

        }

        internal bool UserDataCheck(Biometrics userBio)
        {

            Int32.TryParse(Data_Biometrics.BirthDate[0], out int year);
            if (Data_Biometrics.metric.Weight == 0 && Data_Biometrics.customary.Weight == 0)
            {
               
                if (year >= 1900)
                {
                    ALTERED_userBiometrics = false;
                    return false;
                }
            }
            return true;
        }

        internal bool UserDataCheck(GeneralCalorieData userCals)
        {
            if (Data_General_Calories.Time_played > 30 && Data_General_Calories.Calories_All <= 1)
            {
                Data_General_Calories.Time_played = 0;
                ALTERED_userCalories = false;
                return false;
            }
            return true;
        }

        internal bool UserDataCheck(GameData userGame)
        {
            if (Data_Current_Game.Time_Played > 30 && Data_Current_Game.Calories_All <= 1)
            {
                Data_Current_Game.Time_Played = 0;
                ALTERED_userCalories = false;
                return false;
            }
            return true;
        }

        public bool UserDataCheck()
        {
            var genCal = UserDataCheck(Data_General_Calories);
            var bio = UserDataCheck(Data_Biometrics);
            var gameCal = UserDataCheck(Data_Current_Game);

            if (!genCal || !bio || !gameCal)
            {
                return false;
            }
            return true;
        }


        internal void LOCAL_Update_UserBiometrics(string DATA)
        { 
            Utilities.YUR_Conversions.ConvertStringToObjectOverwrite(DATA, ref Data_Biometrics);
            ALTERED_userBiometrics = false;
            BiometricDATA_Acquired = true;

        }

        internal void LOCAL_Update_UserCalories(string DATA)
        {
            Utilities.YUR_Conversions.ConvertStringToObjectOverwrite(DATA, ref Data_General_Calories);
            ALTERED_userCalories = false;
            GeneralCalorieDATA_Acquired = true;
        }

        internal void LOCAL_Update_UserGame(string DATA)
        {
            Utilities.YUR_Conversions.ConvertStringToObjectOverwrite(DATA, ref Data_Current_Game);
            ALTERED_userCalories = false;
            GameCalorieDATA_Acquired = true;
        }

        internal enum DataType
        {
            all, biometrics, general_calories, game_calories
        }
    }
}
