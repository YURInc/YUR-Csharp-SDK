using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VRUiKits.Utils
{
    public class OculusAddLaserComponent : UnityEngine.MonoBehaviour
    {
        void Awake()
        {
            this.gameObject.AddComponent<OculusLaserInputModule>();
            Destroy(this);
        }
    }
}
