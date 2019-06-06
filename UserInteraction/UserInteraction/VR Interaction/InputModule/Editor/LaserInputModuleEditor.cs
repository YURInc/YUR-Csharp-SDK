using UnityEngine;
using UnityEditor;

namespace VRUiKits.Utils
{
    [CustomEditor(typeof(LaserInputModule))]
    public class LaserInputModuleEditor : Editor
    {
        string[] uikitPlatformDefines = new string[] {
            "UIKIT_OCULUS", "UIKIT_VIVE", "UIKIT_VIVE_STEAM_2"
        };

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LaserInputModule _target = (LaserInputModule)target;
            if (_target.pointer == Pointer.Eye)
            {
                _target.gazeTimeInSeconds = EditorGUILayout.FloatField("Gaze Time In Seconds", _target.gazeTimeInSeconds);
                _target.delayTimeInSeconds = EditorGUILayout.FloatField("Delay Time In Seconds", _target.delayTimeInSeconds);
            }

            if (GUILayout.Button("Setup Input Module"))
            {
                switch (_target.platform)
                {
                    case VRPlatform.OCULUS:
                        SetPlatformCustomDefine("UIKIT_OCULUS");
                        break;
                    case VRPlatform.VIVE:
                        SetPlatformCustomDefine("UIKIT_VIVE");
                        break;
                    case VRPlatform.VIVE_STEAM2:
                        SetPlatformCustomDefine("UIKIT_VIVE_STEAM_2");
                        break;
                    case VRPlatform.NONE:
                        SetPlatformCustomDefine("");
                        break;
                }
            }
        }

        void SetPlatformCustomDefine(string define)
        {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Remove uikit platform defines
            foreach (string item in uikitPlatformDefines)
            {
                if (defines.Contains(item))
                {
                    if (defines.Contains((";" + item)))
                    {
                        defines = defines.Replace((";" + item), "");
                    }
                    else
                    {
                        defines = defines.Replace(item, "");
                    }
                }
            }

            if (define != "" && !defines.Contains(define))
            {
                defines += ";" + define;
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }
    }
}
