using UnityEngine;
using System.Collections;

namespace YUR.SDK.Unity.Workouts
{
    public partial class Workout : MonoBehaviour
    {
        public static void StartingWorkout()
        {
            if (workout.workoutInProgress || Login.Status == Login.StatusType.Logged_Out)
            {
                YUR_Log.Error("Workout is either already in progress or there is no user logged in!");
                return;
            }
            StartWorkout?.Invoke();
            Tracking.Calories.StartCalorieCounter();
            System.TimeSpan t = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
            workout.Calories = 0;
            workout.Start_Time = (long)t.TotalSeconds;
            return;
        }

        public static void EndingWorkout(string identifier)
        {
            workout.EndActiveWorkout(identifier);
        }

        public void EndActiveWorkout(string identifier)
        {
            StartCoroutine(EndWorkoutRoutine(identifier));
        }

        public IEnumerator EndWorkoutRoutine(string Identifier)
        {
            if (!workout.workoutInProgress)
            {
                YUR_Log.Error("There is no workout currently in progress to end!");
                yield break;
            }
            int Calories;
            yield return Calories = (int)Mathf.Floor(Tracking.Calories.EndCalorieCounter());            
            YUR_Main.main.User_Manager.CurrentUser.Data_Current_Game.Calories_All += Calories;
            YUR_Main.main.User_Manager.CurrentUser.Data_Current_Game.Calories_Weekly += Calories;
            YUR_Main.main.User_Manager.CurrentUser.Data_Current_Game.Calories_Today += Calories;
            YUR_Main.main.User_Manager.CurrentUser.Data_General_Calories.Calories_All += Calories;
            YUR_Main.main.User_Manager.CurrentUser.Data_General_Calories.Calories_Weekly += Calories;
            YUR_Main.main.User_Manager.CurrentUser.Data_General_Calories.Calories_Today += Calories;
            YUR_Main.main.User_Manager.CurrentUser.Data_Current_Session_Calories += Calories;
            YUR_Main.main.User_Manager.CurrentUser.SaveAll();

            

            EndWorkout?.Invoke();
            System.TimeSpan t;
            long duration;
            yield return t = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
            yield return workout.End_Time = (long)(t.TotalSeconds - workout.Time_Duration_Paused);

            yield return duration = (workout.End_Time - workout.Start_Time);
            YUR_Main.main.User_Manager.CurrentUser.Data_Current_Game.Time_Played += duration;
            YUR_Main.main.User_Manager.CurrentUser.Data_General_Calories.Time_played += duration;

            yield return workout.Calories += (int)(Calories - workout.Paused_Calories);
            yield return workout.Identifier = Identifier;
            StartCoroutine(workout.UploadWorkout());
            yield break;

        }

        public static void PausingWorkout(int Calories)
        {
            PauseWorkout?.Invoke(Calories);
            System.TimeSpan t = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
            workout.Time_Of_Pause = (long)t.TotalSeconds;
            workout.Calories += Calories;
        }

        public static void ResumingWorkout(int Calories)
        {
            ResumeWorkout?.Invoke(Calories);
            System.TimeSpan t = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
            workout.Time_Duration_Paused = (long)t.TotalSeconds - workout.Time_Of_Pause;
            workout.Paused_Calories = Calories - workout.Calories;
        }
    }

}
