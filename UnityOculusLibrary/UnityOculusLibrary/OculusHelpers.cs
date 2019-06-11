using UnityEngine;
using System.Collections.Generic;
using OVR;
using UnityEngine.Scripting;
[assembly: Preserve]

namespace UnityOculusLibrary
{
    public class OculusHelpers
    {
        public static GameObject GetPlayer()
        {
            Debug.Log("Attempting to get gameobject for YUR player");
            return OVRManager.instance.GetComponent<OVRCameraRig>().gameObject;
        }

        public static bool IsOVRManager()
        {
            Debug.Log("Checking to see if OVRManager is availble");
            if (OVRManager.instance != null && !ReferenceEquals(OVRManager.instance, null))
            {
                Debug.Log("OVRManager is not null and is referenced");
                return true;
            }
            else
            {
                Debug.Log("OVRManager is null and is not referenced");
                return false;
            }
                
        }

        public static GameObject GetHand(bool lefthand)
        {
            if(lefthand)
            {
                return OVRManager.instance.GetComponent<OVRCameraRig>().leftHandAnchor.gameObject;
            }
            else
            {
                return OVRManager.instance.GetComponent<OVRCameraRig>().rightHandAnchor.gameObject;
            }
            
        }
        
    }
}
