/// Not Completed Script
/// Intended purpose of add to game object that would be enabled on the start of a workout
///     and disabled at the end of the workout. Untested but may work.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YUR.SDK.Unity
{
    //[AddComponentMenu("YUR/Workout Start On Enable")]
    public class WorkoutStartOnEnable : MonoBehaviour
    {
        public static WorkoutStartOnEnable instance;
        public string Identifier = "YUR_Example_Scene";

        public void OnEnable()
        {
            instance = this;
            Workouts.Workout.StartingWorkout();
        }

        public void OnDisable()
        {
            Workouts.Workout.EndingWorkout(Identifier);
        }

    }
}
