using UnityEngine;
using System.Collections.Generic;
using OVR;

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
            if (OVRManager.instance.gameObject != null && !ReferenceEquals(OVRManager.instance.gameObject, null))
                return true;
            else
                return false;
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
