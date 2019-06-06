using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRUiKits.Utils
{
    public class Steam2LaserInputDirect
    {
        public Steam2LaserInputDirect(ref GameObject gameObject, out LaserInputModule laserInputModule)
        {
            laserInputModule = gameObject.AddComponent<VRUiKits.Utils.Steam2LaserInputModule>();
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
