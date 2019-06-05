using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRUiKits.Utils
{
    public class Steam2LaserInputDirect
    {
        Steam2LaserInputDirect(GameObject gameObject, out LaserInputModule laserInputModule)
        {
            laserInputModule = gameObject.AddComponent<VRUiKits.Utils.Steam2LaserInputModule>();
        }
    }
}
