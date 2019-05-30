using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VRUiKits.Utils
{
    [RequireComponent(typeof(VREventSystemHelper))]
    public class OculusLaserInputModule : LaserInputModule
    {
        public static OculusLaserInputModule occinst;
        /*** Define trigger key to fire events for different platforms ***/
        OVRInput.Controller activeController;
        OVRCameraRig oculusRig;
        public OVRInput.Button trigger = OVRInput.Button.PrimaryIndexTrigger;
        public Transform TargetControllerTransform
        {
            get
            {
                if (pointer == Pointer.LeftHand)
                {
                    return oculusRig.leftHandAnchor;
                }
                else if (pointer == Pointer.RightHand)
                {
                    return oculusRig.rightHandAnchor;
                }
                else
                {
                    return oculusRig.centerEyeAnchor;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            YUR.SDK.Unity.YUR_Log.Log("Awaking Oculus Laser Input Module");
            if(occinst != null)
            {
                DestroyImmediate(this.gameObject);
            }
            occinst = this;
        }

        protected override void Start()
        {
            base.Start();
            SetupOculus();
        }

        public override void Process()
        {
            if (null != controller)
            {
                UpdateHelperCamera();
                if (pointer == Pointer.Eye)
                {
                    ProcessGazePointer();
                }
                else
                {
                    CheckTriggerStatus();
                    ProcessLaserPointer();
                }
            }
        }

        public void CheckTriggerStatus()
        {
            //Debug.Log("<b>Check trigger status: </b>");
            /*** Define trigger key to fire events for different platforms ***/
            activeController = OVRInput.GetActiveController();
            // Check if Oculus Rift is being used, then we need to check if button is pressed on target pointer.
            if (activeController == OVRInput.Controller.Touch ||
            activeController == OVRInput.Controller.LTouch || activeController == OVRInput.Controller.RTouch)
            {
                if (pointer == Pointer.LeftHand)
                {
                    triggerPressed = OVRInput.Get(trigger, OVRInput.Controller.LTouch);
                    //Debug.Log("Trigger pressed Left? : " + triggerPressed);
                }
                else if (pointer == Pointer.RightHand)
                {
                    triggerPressed = OVRInput.Get(trigger, OVRInput.Controller.RTouch);
                    //Debug.Log("Trigger pressed right? : " + triggerPressed);
                }
            }
            else
            {
                //triggerPressed = OVRInput.Get(trigger);
            }
        }

        // Modified from StandaloneInputModule
        public override void ActivateModule()
        {
            base.ActivateModule();

            var toSelect = eventSystem.currentSelectedGameObject;
            if (toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        //// Copied from StandaloneInputModule
        public override void DeactivateModule()
        {
            base.DeactivateModule();
            ClearSelection();
        }

        void SetupOculus()
        {
            if (null != OVRManager.instance)
            {
                oculusRig = OVRManager.instance.GetComponent<OVRCameraRig>();
            }

            if (null == oculusRig)
            {
                oculusRig = Object.FindObjectOfType<OVRCameraRig>();
            }

            if (null == oculusRig)
            {
                Debug.LogError("Please import Oculus Utilities and put OVRCameraRig prefab into your scene");
            }
        }

    }
}
