using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ScrewPuzzle
{
    /// <summary>One pointer owns each gesture. Selection happens only on a stationary release.</summary>
    public sealed class Radio3DInteraction : MonoBehaviour
    {
        public Transform Radio { get; private set; }
        public Camera View { get; private set; }
        public int RemovedCount { get { return puzzle.RemovedCount; } }
        public int TotalCount { get; private set; }
        public bool IsDragging { get; private set; }
        public float DragThreshold { get { return Mathf.Max(12f, Mathf.Min(Screen.width, Screen.height) * 0.015f); } }

        private Radio3DPuzzle puzzle;
        private bool tracking;
        private bool waitForRelease;
        private int fingerId;
        private Vector2 start;
        private Vector2 previous;
        private Radio3DScrew pressedScrew;
        private Quaternion initialRotation;
        private Radio3DScrew[] screws;
        private readonly List<RaycastResult> uiHits = new List<RaycastResult>();

        public void Configure(Camera camera, Transform radio, Radio3DScrew[] targets)
        {
            puzzle = GetComponent<Radio3DPuzzle>();
            View = camera;
            Radio = radio;
            screws = targets;
            TotalCount = targets.Length;
            initialRotation = radio.localRotation;
        }

        private void Update()
        {
            if (Input.touchCount > 1)
            {
                CancelPointer();
                waitForRelease = true;
                return;
            }
            if (waitForRelease)
            {
                if (Input.touchCount == 0 && !Input.GetMouseButton(0)) waitForRelease = false;
                return;
            }
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    fingerId = touch.fingerId;
                    BeginPointer(touch.position);
                }
                else if (touch.fingerId == fingerId)
                {
                    if (touch.phase == TouchPhase.Canceled) CancelPointer();
                    else if (touch.phase == TouchPhase.Ended) EndPointer(touch.position);
                    else MovePointer(touch.position);
                }
                return; // Never also process the simulated mouse for this touch.
            }
            if (Input.GetMouseButtonDown(0)) BeginPointer(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0)) EndPointer(Input.mousePosition);
            else if (Input.GetMouseButton(0)) MovePointer(Input.mousePosition);
        }

        public Radio3DScrew PickScrew(Vector2 point)
        {
            if (View == null) return null;
            // The nearest solid surface wins: the cabinet blocks screws on its far side.
            if (!Physics.Raycast(View.ScreenPointToRay(point), out RaycastHit hit, 100f,
                ~0, QueryTriggerInteraction.Ignore)) return null;
            Radio3DScrew screw = hit.collider.GetComponentInParent<Radio3DScrew>();
            return screw != null && screw.IsAccessible ? screw : null;
        }

        private bool IsOverUi(Vector2 point)
        {
            if (EventSystem.current == null) return false;
            uiHits.Clear();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = point }, uiHits);
            return uiHits.Count > 0;
        }

        public void BeginPointer(Vector2 point)
        {
            CancelPointer();
            if (Radio == null || !puzzle.CanInteract || IsOverUi(point)) return;
            tracking = true;
            start = previous = point;
            pressedScrew = PickScrew(point);
        }

        public void MovePointer(Vector2 point)
        {
            if (!tracking) return;
            if (!IsDragging && Vector2.Distance(start, point) >= DragThreshold) IsDragging = true;
            if (IsDragging)
                Radio.Rotate(Vector3.up, -(point.x - previous.x) * 300f / Mathf.Max(1, Screen.width), Space.World);
            previous = point;
        }

        public void EndPointer(Vector2 point)
        {
            if (!tracking) return;
            MovePointer(point); // Account for a final movement arriving on the release frame.
            if (!IsDragging && !IsOverUi(point) && pressedScrew != null
                && pressedScrew == PickScrew(point) && pressedScrew.gameObject.activeSelf)
            {
                puzzle.TryAdd(pressedScrew);
            }
            CancelPointer();
        }

        public void CancelPointer()
        {
            tracking = false;
            IsDragging = false;
            pressedScrew = null;
        }

        public void ResetExperiment()
        {
            CancelPointer();
            Radio.localRotation = initialRotation;
            puzzle.Restart();
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) { CancelPointer(); waitForRelease = true; }
        }

        private void OnDisable() { CancelPointer(); }
    }
}
