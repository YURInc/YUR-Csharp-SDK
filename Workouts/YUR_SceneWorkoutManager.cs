using UnityEngine;
using UnityEngine.SceneManagement;

namespace YUR.SDK.Unity.Workouts
{
    public class YUR_SceneWorkoutManager : MonoBehaviour
    {
        public static YUR_SceneWorkoutManager instance;

        private string WorkoutIdentifier = string.Empty;
        private Scene CurrentScene;

        /// <summary>
        /// Set the identifier the user will see when they have completed a workout
        /// </summary>
        /// <param name="identifier">What the user will see when they are looking at their workout data. For Example, the name of a song, an intense moment in the game.</param>
        public void SetWorkoutIdentifier(string identifier)
        {
            CurrentScene = SceneManager.GetActiveScene();
            WorkoutIdentifier = identifier;
        }

        public void Awake()
        {
            if (instance == null)
                instance = this;

            if(!YUR.Yur.Settings.SceneBasedWorkouts)
            {
                YUR_Log.Log("Scene Based Workouts are disabled");
                return;
            }

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(Scene LoadedScene, LoadSceneMode arg1)
        {
            /// Check if the identifier applys to the scene in which it was applied to.
            if (CurrentScene.name != LoadedScene.name)
            {
                WorkoutIdentifier = string.Empty;
            }

            for (int i = 0; i < YUR.Yur.Settings.StartWorkoutScenes.Length; i++)
            {
                if (LoadedScene.name == YUR.Yur.Settings.StartWorkoutScenes[i])
                {
                    Workout.StartingWorkout();
                }
                else if(LoadedScene.name == YUR.Yur.Settings.EndWorkoutScenes[i])
                {

                    Workout.EndingWorkout((string.IsNullOrEmpty(WorkoutIdentifier) ? LoadedScene.name : WorkoutIdentifier));
                    WorkoutIdentifier = string.Empty;
                }
            }
        }
    }
}
