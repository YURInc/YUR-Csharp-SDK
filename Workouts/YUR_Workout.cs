using UnityEngine;
using System.Collections;

namespace YUR.SDK.Unity.Workouts
{

    public partial class Workout : MonoBehaviour
    {
        /// <summary>
        /// Server response event
        /// </summary>
        /// <returns>Reason for failure</returns>
        public delegate string Workout_Upload(string response);
        event Workout_Upload Success;
        event Workout_Upload Failure;

        public bool workoutInProgress = false;

        public delegate void Workout_Status();
        public static event Workout_Status StartWorkout;
        public static event Workout_Status EndWorkout;
        public delegate void Workout_Pause(int calories_burned);
        public static event Workout_Pause PauseWorkout;
        public static event Workout_Pause ResumeWorkout;

        /// <summary>
        /// Instance, set me up like new!
        /// </summary>        
        public static Workout workout;


        /// <summary>
        /// Create new workout and signal whether to start it
        /// </summary>
        /// <param name="GameID">Use the same Game ID for every workout instance</param>
        /// <param name="start">True to start the workout on creation of the object</param>
        public void SetupWorkout(string GameID, bool start = true)
        {
            if(workout != null)
            {
                if(workout.workoutInProgress)
                {
                    YUR_Log.Log("Workout already in progress, will not setup new workout!");
                    return;
                }
            }

            if(workout == null || !workout)
                workout = this;
            DontDestroyOnLoad(workout);


            gameID = GameID;

            StartWorkout += Workout_StartWorkout;
            EndWorkout += Workout_EndWorkout;
            PauseWorkout += Workout_PauseWorkout;
            ResumeWorkout += Workout_ResumeWorkout;

            this.Success += new Workout_Upload(this.SuccessFull_Upload);
            if (start)
            {
                StartingWorkout();
            }
        }

        void Awake()
        {
            SetupWorkout(YUR_Main.main.game_ID, false);

        }

        private void Workout_ResumeWorkout(int calories_burned)
        {
            YUR_Log.Log("[Workout | Resume] Calories Burned while paused: " + calories_burned);
            return;
        }

        private void Workout_PauseWorkout(int calories_burned)
        {
            
            YUR_Log.Log("[Workout | Pause] Calories Burned up until paused: " + calories_burned);
            return;
        }

        private void Workout_EndWorkout()
        {
            workoutInProgress = false;
            YUR_Log.Log("[Workout | End] Ending Workout" +
                "\n Calories Burned: " + calories +
                "\n Identifier: " + identifier +
                "\n GameID: " + gameID);
            return;
        }

        private void Workout_StartWorkout()
        {
            workoutInProgress = true;
            YUR_Log.Log("[Workout | Start] Starting workout");
            return;
        }

        internal string SuccessFull_Upload(string response)
        {
            YUR_Log.Log("[Workout | Upload]" + response);
            return response;
        }

        internal string FailureTo_Upload(string response)
        {
            YUR_Log.Error("[Workout | Upload]" + response);
            return response;
        }

        internal IEnumerator UploadWorkout()
        {
            YUR_Log.Server_Log("Uploading workout");
            string response;
            yield return response = Systems.Interops.Workouts.UploadWorkout(Utilities.YUR_Conversions.ConvertObjectToString(this), YUR_Main.main.User_Manager.CurrentUser.loginCredentials.IDtoken);
            if (response.StartsWith("--1"))
            {
                Failure?.Invoke(response);
                yield break;
            }
            Success?.Invoke(response);
            yield break;
        }

        [SerializeField]
        private int calories;
        [SerializeField]
        private long end;
        [SerializeField]
        private string gameID;
        [SerializeField]
        private string identifier;
        [SerializeField]
        private long start;

        
        internal long Time_Of_Pause = 0;

        
        internal long Time_Duration_Paused = 0;

        
        internal int Paused_Calories = 0;

        
        public int Calories
        {
            get {  return calories;}
            set { calories = value; }
        }
        
        public long End_Time
        {
            get { return end; }
            set { end = value; }
        }
        
        public string GameID
        {
            get { return gameID; }
            set { gameID = value; }
        }
        
        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }
        
        public long Start_Time
        {
            get { return start; }
            set { start = value; }
        }
     }
}
