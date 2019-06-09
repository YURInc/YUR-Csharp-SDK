
using UnityEngine;
using UnityEngine.EventSystems;


namespace VRUiKits.Utils
{
    [RequireComponent(typeof(VREventSystemHelper))]
    public class LaserInputModule : UIKitInputModule
    {
        public VRPlatform platform;
        public Pointer pointer = Pointer.LeftHand;
        /*** Define trigger key to fire events for different platforms ***/

        /********** Gaze ***********/
        public GameObject currentTarget;
        float currentClickTime;

        /*********************/
        public static LaserInputModule instance { get { return _instance; } set { _instance = value; } }
        private static LaserInputModule _instance = null;

        public Camera helperCamera;
        public UIKitPointer controller;

        // Support variables
        public bool triggerPressed = false;
        public bool triggerPressedLastFrame = false;
        public PointerEventData pointerEventData;
        public Vector3 lastRaycastHitPoint;
        public float pressedDistance;  // Distance the laser travelled while pressed.

        protected override void Awake()
        {
            base.Awake();
                

            if (_instance != null)
            {
                Debug.LogWarning("Trying to instantiate multiple UIKitLaserInputModule.");
                DestroyImmediate(this.gameObject);
            }

            _instance = this;

            //if (platform == VRPlatform.OCULUS && OculusLaserInputModule.instance == null)
            //    _instance = gameObject.AddComponent<OculusLaserInputModule>();
            //else if (platform == VRPlatform.VIVE_STEAM2 && Steam2LaserInputModule.instance == null)
            //    _instance = gameObject.AddComponent<Steam2LaserInputModule>();
        }

        protected override void Start()
        {
            base.Start();

            // Create a helper camera that will be used for raycasts
            helperCamera = new GameObject("Helper Camera").AddComponent<Camera>();
            // Add physics raycaster for 3d objects
            helperCamera.gameObject.AddComponent<PhysicsRaycaster>();
            helperCamera.clearFlags = CameraClearFlags.Nothing;
            helperCamera.nearClipPlane = 0.01f;
            helperCamera.enabled = false;
            // Assign all the canvases with the helper camera;
            Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                canvas.worldCamera = helperCamera;
            }
        }

        public void SetController(UIKitPointer _controller)
        {
            controller = _controller;
        }

        public void RemoveController(UIKitPointer _controller)
        {
            if (null != controller && controller == _controller)
            {
                controller = null;
            }
        }
        // Update helper camera position
        public void UpdateHelperCamera()
        {
            helperCamera.transform.position = controller.transform.position;
            helperCamera.transform.rotation = controller.transform.rotation;
        }

        public void ProcessGazePointer()
        {
            SendUpdateEventToSelectedObject();

            PointerEventData eventData = GetPointerEventData();
            ProcessMove(eventData);

            if (null != eventData.pointerEnter)
            {
                GameObject handler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(eventData.pointerEnter);
                if (currentTarget != handler)
                {
                    currentTarget = handler;
                    currentClickTime = Time.realtimeSinceStartup + delayTimeInSeconds + gazeTimeInSeconds;
                    RaiseGazeChangeEvent(currentTarget);
                }

                if (null != currentTarget && Time.realtimeSinceStartup > currentClickTime)
                {
                    // find a press handler
                    ExecuteEvents.ExecuteHierarchy(currentTarget, eventData, ExecuteEvents.pointerDownHandler);
                    // search for a click handler
                    ExecuteEvents.ExecuteHierarchy(currentTarget, eventData, ExecuteEvents.pointerClickHandler);
                    currentTarget = null;
                    RaiseGazeChangeEvent(currentTarget);
                }
            }
            else
            {
                currentTarget = null;
                RaiseGazeChangeEvent(currentTarget);
            }
        }

        public void ProcessLaserPointer()
        {
           
            SendUpdateEventToSelectedObject();
           
            PointerEventData eventData = GetPointerEventData();
            ProcessPress(eventData);
            ProcessMove(eventData);
            if (triggerPressed)
            {
                ProcessDrag(eventData);
                if (!Mathf.Approximately(eventData.scrollDelta.sqrMagnitude, 0.0f))
                {
                    var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.pointerCurrentRaycast.gameObject);
                    ExecuteEvents.ExecuteHierarchy(scrollHandler, eventData, ExecuteEvents.scrollHandler);
                }
            }

            triggerPressedLastFrame = triggerPressed;
        }

        public void ProcessPress(PointerEventData eventData)
        {
            var currentOverGo = eventData.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (TriggerPressedThisFrame())
            {
                eventData.eligibleForClick = true;
                eventData.delta = Vector2.zero;
                eventData.dragging = false;
                eventData.useDragThreshold = true;
                eventData.pressPosition = eventData.position;
                eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;
                pressedDistance = 0;

                if (eventData.pointerEnter != currentOverGo)
                {
                    // send a pointer enter to the touched element if it isn't the one to select...
                    HandlePointerExitAndEnter(eventData, currentOverGo);
                    eventData.pointerEnter = currentOverGo;
                }

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                float time = Time.unscaledTime;

                if (newPressed == eventData.lastPress)
                {
                    var diffTime = time - eventData.clickTime;
                    if (diffTime < 0.3f)
                        ++eventData.clickCount;
                    else
                        eventData.clickCount = 1;

                    eventData.clickTime = time;
                }
                else
                {
                    eventData.clickCount = 1;
                }

                eventData.pointerPress = newPressed;
                eventData.rawPointerPress = currentOverGo;

                eventData.clickTime = time;

                // Save the drag handler as well
                eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (eventData.pointerDrag != null)
                    ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
            }

            // PointerUp notification
            if (TriggerReleasedThisFrame())
            {
                ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

                // see if we button up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (eventData.pointerPress == pointerUpHandler && eventData.eligibleForClick)
                {
                    ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerClickHandler);
                }
                else if (eventData.pointerDrag != null && eventData.dragging)
                {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.dropHandler);
                }

                eventData.eligibleForClick = false;
                eventData.pointerPress = null;
                eventData.rawPointerPress = null;
                pressedDistance = 0;

                if (eventData.pointerDrag != null && eventData.dragging)
                {
                    ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
                }
                eventData.dragging = false;
                eventData.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                ExecuteEvents.ExecuteHierarchy(eventData.pointerEnter, eventData, ExecuteEvents.pointerExitHandler);
                eventData.pointerEnter = null;
            }
        }

        public PointerEventData GetPointerEventData()
        {
            if (null == pointerEventData)
            {
                pointerEventData = new PointerEventData(eventSystem);
            }
            pointerEventData.Reset();
            pointerEventData.position = new Vector2(helperCamera.pixelWidth / 2,
                helperCamera.pixelHeight / 2);

            pointerEventData.scrollDelta = Vector2.zero;

            eventSystem.RaycastAll(pointerEventData, m_RaycastResultCache);
            RaycastResult currentRaycast = FindFirstRaycast(m_RaycastResultCache);
            pointerEventData.pointerCurrentRaycast = currentRaycast;

            // Delta is used to define if the cursor was moved.
            // It will be used for drag threshold calculation, which we'll calculate angle in degrees
            // between the last and the current raycasts.
            Ray ray = new Ray(helperCamera.transform.position, helperCamera.transform.forward);
            Vector3 hitPoint = ray.GetPoint(currentRaycast.distance);
            pointerEventData.delta = new Vector2(Vector3.Angle(hitPoint, lastRaycastHitPoint), 0);
            lastRaycastHitPoint = hitPoint;

            m_RaycastResultCache.Clear();
            return pointerEventData;
        }

        public bool TriggerReleasedThisFrame()
        {
            return (triggerPressedLastFrame && !triggerPressed);
        }

        public bool TriggerPressedThisFrame()
        {
            return (!triggerPressedLastFrame && triggerPressed);
        }

        // Copied from StandaloneInputModule
        public bool SendUpdateEventToSelectedObject()
        {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        protected void ClearSelection()
        {
            var baseEventData = GetBaseEventData();
            eventSystem.SetSelectedGameObject(null, baseEventData);
        }

        // Copied from PointerInputModule
        public bool ShouldStartDrag(float threshold, bool useDragThreshold)
        {
            if (!useDragThreshold)
                return true;
            return pressedDistance >= threshold;
        }

        // Copied from PointerInputModule
        protected virtual void ProcessMove(PointerEventData pointerEvent)
        {
            var targetGO = (Cursor.lockState == CursorLockMode.Locked ? null : pointerEvent.pointerCurrentRaycast.gameObject);
            HandlePointerExitAndEnter(pointerEvent, targetGO);
        }

        // Modiefied from PointerInputModule
        public void ProcessDrag(PointerEventData eventData)
        {
            // If pointer is not moving or if a button is not pressed (or pressed control did not return drag handler), do nothing
            if (!eventData.IsPointerMoving() || eventData.pointerDrag == null)
                return;

            // We are eligible for drag. If drag did not start yet, add drag distance
            if (!eventData.dragging)
            {
                pressedDistance += eventData.delta.x;

                if (ShouldStartDrag(eventSystem.pixelDragThreshold, eventData.useDragThreshold))
                {
                    ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.beginDragHandler);
                    eventData.dragging = true;
                }
            }

            // Drag notification
            if (eventData.dragging)
            {
                // Before doing drag we should cancel any pointer down state
                // And clear selection!
                if (eventData.pointerPress != eventData.pointerDrag)
                {
                    ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;
                    eventData.rawPointerPress = null;
                }
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.dragHandler);
            }
        }
    }

    public enum VRPlatform
    {
        NONE = 0,
        OCULUS = 1,
        VIVE = 2,
        VIVE_STEAM2 = 3
    }

    public enum Pointer
    {
        RightHand,
        LeftHand,
        Eye
    }
}
