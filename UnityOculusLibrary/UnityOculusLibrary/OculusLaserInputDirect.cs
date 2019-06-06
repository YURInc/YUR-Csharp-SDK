using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRUiKits.Utils
{
    public class OculusLaserInputDirect
    {
        public OculusLaserInputDirect(ref GameObject gameObject, out LaserInputModule laserInputModule)
        {
            laserInputModule = gameObject.AddComponent<VRUiKits.Utils.OculusLaserInputModule>();
        }
    }

    //public class Steam2LaserPointerDirect
    //{
    //    public Steam2LaserPointerDirect(ref GameObject gameObject, out UIKitPointer uIKitPointer)
    //    {
    //        uIKitPointer = gameObject.AddComponent<VRUiKits.Utils.Steam2UIKitPointer>();
    //    }
    //}

}
