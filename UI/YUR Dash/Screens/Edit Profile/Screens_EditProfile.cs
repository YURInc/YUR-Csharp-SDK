using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace YUR.SDK.Unity.UI
{
    public class Screens_EditProfile : YURScreenController
    {
        public static Screens_EditProfile inst;

        public TextMeshProUGUI Title;

        public YUROptionsManager Units;
        public YUROptionsManager Sex;

        public YUR_CurrentUser User { get; set; }

        public WeightScript Weight;
        public HeightScript Height;

        public BirthdateScript BirthDate;

        public void Awake()
        {
            inst = this;
            Finished += Screens_EditProfile_Finished;
        }

        public void OnEnable()
        {
            StartCoroutine(Setup());
        }

        private void Screens_EditProfile_BackButtonPressed()
        {

            Year_EndEdit();
            Month_EndEdit();
            Day_EndEdit();

            UpdateAge();

            if (User.Data_Biometrics.Metric_Units)
            {
                int newValue;
                if(int.TryParse(Weight.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text, out newValue))
                {
                    User.Data_Biometrics.metric.Weight = newValue;
                }

                if (int.TryParse(Height.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text, out newValue))
                {
                    User.Data_Biometrics.metric.Height = newValue / 100;
                }
            }
            else
            {
                int newValue;
                if (int.TryParse(Weight.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text, out newValue))
                {
                    User.Data_Biometrics.customary.Weight = newValue;
                }

                if (int.TryParse(Height.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text, out newValue))
                {
                    User.Data_Biometrics.customary.Height = newValue;
                }
            }

            User.SaveAll();
        }

        void UpdateAge()
        {
            int year; int month; int day;
            int.TryParse(User.Data_Biometrics.BirthDate[0], out year);
            int.TryParse(User.Data_Biometrics.BirthDate[1], out month);
            int.TryParse(User.Data_Biometrics.BirthDate[2], out day);

            if (year != 1900)
            {
                System.DateTime bday = new System.DateTime(year: year, month: month, day: day);
                System.TimeSpan timeAlive = System.DateTime.UtcNow - bday;
                int age = (int)timeAlive.TotalHours / 8766;
                User.Data_Biometrics.Age = age;
                BirthDate.AgeDisplay.text = age + " years";
            }
            else
            {
                BirthDate.AgeDisplay.text = "Invalid Birth Date";
            }

        }


        private void Day_EndEdit()
        {
            int day;
            if (int.TryParse(BirthDate.Day.InputField.text, out day))
            {
                int month;
                int year;
                Month_EndEdit();
                Year_EndEdit();
                int.TryParse(BirthDate.Month.InputField.text, out month);
                int.TryParse(BirthDate.Year.InputField.text, out year);
                Debug.Log("Month: " + month);
                Debug.Log("Year: " + year);
                if (day <= System.DateTime.DaysInMonth(year, month))
                {
                    User.Data_Biometrics.BirthDate[2] = day.ToString();
                }
                else
                {
                    BirthDate.Day.InputField.text = User.Data_Biometrics.BirthDate[2];
                }
            }
            else
            {
                BirthDate.Day.InputField.text = User.Data_Biometrics.BirthDate[2];
            }
        }

        
        void Month_EndEdit()
        {
            int month;
            if (int.TryParse(BirthDate.Month.InputField.text, out month))
            {
                if (month <= 12)
                    User.Data_Biometrics.BirthDate[1] = month.ToString();
                else
                {
                    BirthDate.Month.InputField.text = User.Data_Biometrics.BirthDate[1];
                }
                    
            }
            else
            {
                BirthDate.Month.InputField.text = User.Data_Biometrics.BirthDate[1];
            }
        }


        void Year_EndEdit()
        {
            int year;
            int.TryParse(BirthDate.Year.InputField.text, out year);
            if (year > 1899 && year < 2012)
            {
                User.Data_Biometrics.BirthDate[0] = year.ToString();
            }
            else
            {
                int byear;
                int.TryParse(User.Data_Biometrics.BirthDate[0], out byear);
                if (byear < 1899 || byear > 2013)
                {
                    Debug.Log("Birth year not set!");
                    year = 1900;
                    User.Data_Biometrics.BirthDate[0] = year.ToString();
                    BirthDate.Year.InputField.text = year.ToString();
                }
                else
                    BirthDate.Year.InputField.text = User.Data_Biometrics.BirthDate[0];
            }
        }

        private void Screens_EditProfile_Finished()
        {
            BackButtonPressed += Screens_EditProfile_BackButtonPressed;
            CloseButtonPressed += Screens_EditProfile_BackButtonPressed;
        }


        IEnumerator Setup() {
            YUR_Log.Log("Setting up Profile");
            yield return User = UserManagement.YUR_UserManager.YUR_Users.CurrentUser;
            YUR_Log.Log("Retrieved the user manager");
            Title.text = User.Data_Biometrics.Name + "'s Profile";
            UpdateAge();

            /// BirthDate
            ////////////////////////////////////
            BirthDate.Day.PlaceHolder.text = "Day";
            BirthDate.Day.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = User.Data_Biometrics.BirthDate[2];

            BirthDate.Month.PlaceHolder.text = "Month";
            BirthDate.Month.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = User.Data_Biometrics.BirthDate[1];

            BirthDate.Year.PlaceHolder.text = "Year";
            BirthDate.Year.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = User.Data_Biometrics.BirthDate[0];
            


            /// Units
            ////////////////////////////////////
            if (User.Data_Biometrics.Metric_Units)
            {
                Units.selectedValue = "Metric";
                /// Weight
                Weight.PlaceHolder.text = "kgs";
                Weight.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = User.Data_Biometrics.metric.Weight.ToString();
                Weight.Label.text = "Weight: (in kilograms)";
                /// Height
                Height.PlaceHolder.text = "cm";
                Height.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = (User.Data_Biometrics.metric.Height * 100).ToString();
                Height.Label.text = "Height: (in centimeters)";
            }
            else
            {
          
                Units.selectedValue = "Customary";
                /// Weight
                Weight.PlaceHolder.text = "lbs";
                Weight.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = User.Data_Biometrics.customary.Weight.ToString();
                Weight.Label.text = "Weight: (in pounds)";
                /// Height
                Height.PlaceHolder.text = "in";
                Height.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = User.Data_Biometrics.customary.Height.ToString();
                Height.Label.text = "Height: (in inches)";

            }
            /// Sex
            ///////////////////////////////////////
            if (User.Data_Biometrics.Sex == "male")
            {
                Sex.selectedValue = "Male";
            }
            else if (User.Data_Biometrics.Sex == "female")
            {
                Sex.selectedValue = "Female";
            }
            else
            {
                Sex.selectedValue = "Unspecified";
            }

            Units.NextPressed += Units_PrevPressed;
            Units.PrevPressed += Units_PrevPressed;

            Sex.NextPressed += Sex_NextPressed;
            Sex.PrevPressed += Sex_PrevPressed;

            YURScreenCoordinator.ScreenCoordinator.Keyboard.SetActive(true, KeyboardCanvas.KeyboardStyle.KeyboardNumPad);
            yield break;
        }

        private void Sex_PrevPressed(string value)
        {
            if(value == "Male")
            {
                Sex.selectedValue = "Female";
                User.Data_Biometrics.Sex = "female";

            }
            else if(value == "Female")
            {
                Sex.selectedValue = "Unspecified";
                User.Data_Biometrics.Sex = "unspecified";
            }
            else
            {
                Sex.selectedValue = "Male";
                User.Data_Biometrics.Sex = "male";
            }
        }

        private void Sex_NextPressed(string value)
        {
            if (value == "Male")
            {
                Sex.selectedValue = "Unspecified";
                User.Data_Biometrics.Sex = "unspecified";

            }
            else if (value == "Female")
            {
                Sex.selectedValue = "Male";
                User.Data_Biometrics.Sex = "male";
            }
            else
            {
                Sex.selectedValue = "Female";
                User.Data_Biometrics.Sex = "female";
            }
        }

        private void Units_PrevPressed(string value)
        {
            /// New Value is Metric
            if (value == "Metric")
            {
                int valToStore;
                if(int.TryParse(Weight.Weight.text, out valToStore))
                {
                    User.Data_Biometrics.metric.Weight = valToStore;
                }
                if(int.TryParse(Height.Height.text, out valToStore))
                {
                    User.Data_Biometrics.metric.Height = (valToStore / 100);
                }
                
                User.Data_Biometrics.Metric_Units = false;
                Units.selectedValue = "Customary";
                /// Weight
                Weight.PlaceHolder.text = "lbs";
                Weight.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = User.Data_Biometrics.customary.Weight.ToString();
                Weight.Label.text = "Weight: (in pounds)";
                /// Height
                Height.PlaceHolder.text = "in";
                Height.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = User.Data_Biometrics.customary.Height.ToString();
                Height.Label.text = "Height: (in inches)";
            }
            else /// New Value is Customary
            {
                int valToStore;
                if (int.TryParse(Weight.Weight.text, out valToStore))
                {
                    User.Data_Biometrics.customary.Weight = valToStore;
                }
                if (int.TryParse(Height.Height.text, out valToStore))
                {
                    User.Data_Biometrics.customary.Height = valToStore;
                }

                User.Data_Biometrics.Metric_Units = true;
                Units.selectedValue = "Metric";
                /// Weight
                Weight.PlaceHolder.text = "kgs";
                Weight.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = User.Data_Biometrics.metric.Weight.ToString();
                Weight.Label.text = "Weight: (in kilograms)";
                /// Height
                Height.PlaceHolder.text = "cm";
                Height.gameObject.GetComponent<VRUiKits.Utils.UIKitInputField>().text = (User.Data_Biometrics.metric.Height * 100).ToString();
                Height.Label.text = "Height: (in centimeters)";
            }
        }

    }
}