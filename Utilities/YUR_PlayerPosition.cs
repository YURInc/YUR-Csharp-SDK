using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace YUR.SDK.Unity.Utilities
{
    public class YUR_PlayerPosition
    {

        public static void GetHandGameObject(ref GameObject Hand, bool isLeftHand)
        {
            if(YUR.Yur.platform == VRUiKits.Utils.VRPlatform.OCULUS)
            {
                Hand = (isLeftHand ? UnityOculusLibrary.OculusHelpers.GetHand(true) : UnityOculusLibrary.OculusHelpers.GetHand(false));
            }
        }
        public static void GetCameraObject(ref GameObject Camera)
        {
            try
            {
                if(YUR.Yur.platform == VRUiKits.Utils.VRPlatform.OCULUS)
                {
                    var oculusGobject = UnityOculusLibrary.OculusHelpers.GetPlayer();
                    if(oculusGobject != null)
                    {
                        Camera =  oculusGobject;
                        return;
                    }
                }
                var gobject = GameObject.FindGameObjectsWithTag(YUR.Yur.Settings.CameraTag);
                if(gobject.Length < 0)
                {
                    YUR_Log.Log("Unable to find any objects with tag");
                    return;
                }
                foreach (var obj in gobject)
                {
                    if (obj.name.Contains("eye"))
                    {
                        Camera = obj;
                        return;
                    }
                }
                
            }
            catch (System.Exception e)
            {
                YUR_Log.Warning("Error while getting camera: " + e);
                //var gobject = GameObject.FindGameObjectsWithTag(YUR.Yur.Settings.CameraTag);
                //foreach (var obj in gobject)
                //{
                //    if (obj.name.Contains("eye"))
                //    {
                //        return obj;
                //    }
                //}
            }
            YUR_Log.Warning("Was unable to locate a valid camera tagged " + YUR.Yur.Settings.CameraTag);
            return;
        }


    }
}
