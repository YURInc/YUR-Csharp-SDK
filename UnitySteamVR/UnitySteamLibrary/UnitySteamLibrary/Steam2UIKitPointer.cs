using UnityEngine;

using Valve.VR;
using Valve.VR.InteractionSystem;

namespace VRUiKits.Utils
{
    public partial class Steam2UIKitPointer : UIKitPointer
    {
        //Pointer temp; // Used to detect if the pointer has changed in the input module.

        void Start()
        {
            if (null == LaserInputModule.instance)
            {
                return;
            }

            Steam2LaserInputModule.steam2inst.SetController(this);
        }

        // Change pointer when player clicks trigger on the other pointer.
        [SerializeField]
        bool allowAutoSwitchHand = false;

        void Update()
        {
            bool isEyePointer = Steam2LaserInputModule.steam2inst.pointer == Pointer.Eye;
            if (temp != Steam2LaserInputModule.steam2inst.pointer)
            {
                gazePointer.SetActive(isEyePointer);
                laser.SetActive(!isEyePointer);

                SetPointer(Steam2LaserInputModule.steam2inst.pointer);
                temp = Steam2LaserInputModule.steam2inst.pointer;
            }


            if (allowAutoSwitchHand)
            {
                if (Steam2LaserInputModule.steam2inst.triggerAction.GetStateDown(SteamVR_Input_Sources.RightHand)
                && Steam2LaserInputModule.steam2inst.pointer != Pointer.RightHand)
                {
                    SetPointer(Pointer.RightHand);
                }
                else if (Steam2LaserInputModule.steam2inst.triggerAction.GetStateDown(SteamVR_Input_Sources.LeftHand)
                && Steam2LaserInputModule.steam2inst.pointer != Pointer.LeftHand)
                {
                    SetPointer(Pointer.LeftHand);
                }
            }
        }
        void SetPointer(Pointer targetPointer)
        {
            if (null != LaserInputModule.instance)
            {
                LaserInputModule.instance.pointer = targetPointer;
                transform.SetParent(Steam2LaserInputModule.steam2inst.TargetControllerTransform);
                ResetTransform(transform);
            }
        }

        void ResetTransform(Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = Vector3.one;
        }

        void OnDestroy()
        {
            if (null != LaserInputModule.instance)
            {
                LaserInputModule.instance.RemoveController(this);
            }
        }
    }
}
