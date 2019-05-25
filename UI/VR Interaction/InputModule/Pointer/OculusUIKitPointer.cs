using UnityEngine;

namespace VRUiKits.Utils
{
    public class OculusUIKitPointer : UIKitPointer
    {
         void Start()
         {
            if (null == OculusLaserInputModule.occinst)
            {
                return;
            }

            OculusLaserInputModule.occinst.SetController(this);
         }


        OVRInput.Controller activeController;

        // Change pointer when player clicks trigger on the other pointer.
        [SerializeField]
        bool allowAutoSwitchHand = false;
        void Update()
        {

            bool isEyePointer = OculusLaserInputModule.occinst.pointer == Pointer.Eye;
            if (temp != OculusLaserInputModule.occinst.pointer)
            {
                gazePointer.SetActive(isEyePointer);
                laser.SetActive(!isEyePointer);
                SetPointer(OculusLaserInputModule.occinst.pointer);
                temp = OculusLaserInputModule.occinst.pointer;
            }
            activeController = OVRInput.GetActiveController();
            /********* Oculus Go and Gear VR **********/
            if (activeController == OVRInput.Controller.LTrackedRemote
            && OculusLaserInputModule.occinst.pointer != Pointer.LeftHand)
            {
                SetPointer(Pointer.LeftHand);
            }
            else if (activeController == OVRInput.Controller.RTrackedRemote
            && OculusLaserInputModule.occinst.pointer != Pointer.RightHand)
            {
                SetPointer(Pointer.RightHand);
            }
            if (allowAutoSwitchHand)
            {
                /********* Oculus Rift **********/
                if (OVRInput.GetDown(OculusLaserInputModule.occinst.trigger, OVRInput.Controller.RTouch)
                && OculusLaserInputModule.occinst.pointer != Pointer.RightHand)
                {
                    SetPointer(Pointer.RightHand);
                }
                else if (OVRInput.GetDown(OculusLaserInputModule.occinst.trigger, OVRInput.Controller.LTouch)
                && OculusLaserInputModule.occinst.pointer != Pointer.LeftHand)
                {
                    SetPointer(Pointer.LeftHand);
                }
            }
        }
        void SetPointer(Pointer targetPointer)
        {
            if (null != OculusLaserInputModule.instance)
            {
                OculusLaserInputModule.occinst.pointer = targetPointer;
                transform.SetParent(OculusLaserInputModule.occinst.TargetControllerTransform);
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
            if (null != OculusLaserInputModule.occinst)
            {
                OculusLaserInputModule.occinst.RemoveController(this);
            }
        }
    }
}
