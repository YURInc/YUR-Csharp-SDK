using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YURSettingsScriptableObject : ScriptableObject
{
    public string CameraTag = "MainCamera";
    public bool EditorSetupCompleted = false;
    public string GameID;
    public bool AutomaticallySignInUser = true;
    public VRUiKits.Utils.VRPlatform platform = VRUiKits.Utils.VRPlatform.NONE;

#if !UIKIT_VIVE_STEAM_2
    public string LeftControllerButton;
    public string RightControllerButton;
#endif
    public bool debugging = true;
    public bool ErrorDebugging = true;
    public bool ServerDebugging = false;
    public bool WriteDebuggingToFile = false;

    //////
    public bool CustomYURGUI = false;  
}
