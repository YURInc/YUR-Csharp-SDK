using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YUR.SDK.Unity.Workouts
{
    [AddComponentMenu("YUR/Workout Controller",0)]
    public class WorkoutController : MonoBehaviour
    {
        [Tooltip("The name of the workout or a way to identify it")]
        public string WorkoutIdentifier;
        public void StartWorkout()
        {
            Workout.StartingWorkout();
        }

        public void EndWorkout(string identifier)
        {
            Workout.EndingWorkout(identifier);
        }

        public void EndWorkout()
        {
            Workout.EndingWorkout(WorkoutIdentifier);
        }

    }
}
