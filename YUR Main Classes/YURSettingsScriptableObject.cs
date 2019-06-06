using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YURSettingsScriptableObject : ScriptableObject
{
    public string CameraTag = "MainCamera";
    public bool EditorSetupCompleted = false;
    public string GameID;
    public bool AutomaticUpdates = true;
    public bool AutomaticallySignInUser = true;

    public bool UseYURsInteractionSystem = true;

    public bool SceneBasedWorkoutSystem = false;

    public VRUiKits.Utils.VRPlatform platform = VRUiKits.Utils.VRPlatform.NONE;

    public string LeftControllerButton;
    public string RightControllerButton;

    public bool debugging = true;
    public bool ErrorDebugging = true;
    public bool ServerDebugging = false;
    public bool WriteDebuggingToFile = false;

    //////
    public bool CustomYURGUI = false;

    public string BackgroundsList;
}
