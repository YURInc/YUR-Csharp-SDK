using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace VRUiKits.Utils
{
    [RequireComponent(typeof(VREventSystemHelper))]
    public partial class Steam2LaserInputModule : LaserInputModule
    {
        //public VRPlatform platform;
        //public Pointer pointer = Pointer.LeftHand;
        /*** Define trigger key to fire events for different platforms ***/

        public SteamVR_Action_Boolean triggerAction;
        [SerializeField]
        SteamVR_PlayArea steamVRPlayArea;
        Transform controllerLeft, controllerRight, centerEye;
        public Transform TargetControllerTransform
        {
            get
            {
                if (pointer == Pointer.LeftHand)
                {
                    return controllerLeft;
                }
                else if (pointer == Pointer.RightHand)
                {
                    return controllerRight;
                }
                else
                {
                    return centerEye;
                }
            }
        }

        public static Steam2LaserInputModule steam2inst = null;

        protected override void Awake()
        {
            base.Awake();

            if (steam2inst != null)
            {
                Debug.LogWarning("Trying to instantiate multiple UIKitLaserInputModule.");
                DestroyImmediate(this.gameObject);
            }

            steam2inst = this;
        }

        protected override void Start()
        {
            base.Start();
            SetupVive2Controllers();
            if (null == triggerAction)
            {
                Debug.LogError("No trigger action assigned");
                return;
            }
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

        void CheckTriggerStatus()
        {

            // Using the action system
            if (pointer == Pointer.LeftHand)
            {
                triggerPressed = triggerAction.GetState(SteamVR_Input_Sources.LeftHand);
            }
            else if (pointer == Pointer.RightHand)
            {
                triggerPressed = triggerAction.GetState(SteamVR_Input_Sources.RightHand);
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

        // Copied from StandaloneInputModule
        public override void DeactivateModule()
        {
            base.DeactivateModule();
            ClearSelection();
        }

        void SetupVive2Controllers()
        {
            if (null == steamVRPlayArea)
            {
                steamVRPlayArea = FindObjectOfType<SteamVR_PlayArea>();
            }

            if (null != steamVRPlayArea)
            {
                foreach (SteamVR_Behaviour_Pose pose in steamVRPlayArea.GetComponentsInChildren<SteamVR_Behaviour_Pose>(true))
                {
                    if (pose.inputSource == SteamVR_Input_Sources.RightHand)
                    {
                        controllerRight = pose.transform;
                    }
                    else if (pose.inputSource == SteamVR_Input_Sources.LeftHand)
                    {
                        controllerLeft = pose.transform;
                    }
                }

                centerEye = steamVRPlayArea.GetComponentInChildren<Camera>(true).transform;
            }
            else
            {
                Debug.LogError("Please import SteamVR Plugin and put [CameraRig] prefab into your scene");
            }
        }
    }
}
