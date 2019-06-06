using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YURSettingsScriptableObject : ScriptableObject
{
    [Header("Editor Settings")]
    public string CameraTag = "MainCamera";
    public VRUiKits.Utils.VRPlatform platform = VRUiKits.Utils.VRPlatform.NONE;
    public string LeftControllerButton;
    public string RightControllerButton;
    [HideInInspector]
    public bool EditorSetupCompleted = false;

    [Header("Debugging Options")]
    public bool debugging = true;
    public bool ErrorDebugging = true;
    public bool ServerDebugging = false;
    public bool WriteDebuggingToFile = false;

    [Header("Game Settings")]
    public string GameID;
    public bool AutomaticUpdates = true;
    public bool AutomaticallySignInUser = true;
    public bool UseYURInteractionSystem = true;
    
    /// TODO : Custom GUI needs reimplemented
    [HideInInspector]
    public bool CustomYURGUI = false;
    /// TODO : Background system needs to be finished
    [HideInInspector]
    public string BackgroundsList;

    [Header("Scene Based Workouts")]
    public bool SceneBasedWorkouts;
    public string[] StartWorkoutScenes;
    public string[] EndWorkoutScenes;
}
