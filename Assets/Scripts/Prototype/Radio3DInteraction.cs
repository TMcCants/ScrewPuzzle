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
        public bool AllowPitch { get; set; }
        public bool ExplainLockedScrews { get; set; }
        public event System.Action<float> Rotated;
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
            var screw = PickVisibleScrew(point);
            return screw != null && screw.IsAccessible ? screw : null;
        }

        public Radio3DScrew PickVisibleScrew(Vector2 point)
        {
            if (View == null) return null;
            // The nearest solid surface wins: the cabinet blocks screws on its far side.
            if (!Physics.Raycast(View.ScreenPointToRay(point), out RaycastHit hit, 100f,
                ~0, QueryTriggerInteraction.Ignore)) return null;
            Radio3DScrew screw = hit.collider.GetComponentInParent<Radio3DScrew>();
            if (screw != null && !screw.IsAccessible && !IsHeadVisible(screw)) return null;
            return screw;
        }

        private bool IsHeadVisible(Radio3DScrew screw)
        {
            // The generous tap proxy can protrude through a covering panel. A lock badge
            // must refer to a visible head, not reveal a hidden screw through that panel.
            Transform visual = screw.transform.Find("Head Visual");
            if (visual == null) return true;
            Vector3 target = visual.TransformPoint(Vector3.up * .8f);
            Vector3 direction = target - View.transform.position;
            foreach (var hit in Physics.RaycastAll(View.transform.position, direction.normalized,
                direction.magnitude, ~0, QueryTriggerInteraction.Ignore))
                if (hit.collider.GetComponentInParent<Radio3DScrew>() != screw) return false;
            return true;
        }

        public Radio3DScrew FindHint()
        {
            if (!puzzle.CanInteract) return null;
            CancelPointer();
            var candidates = new List<Radio3DScrew>();
            foreach (var screw in screws)
            {
                if (screw.IsRemoved || !screw.IsAccessible) continue;
                bool matches = System.Array.Exists(puzzle.Trays, t => t.IsOpen && t.HasColor && t.Color == screw.ColorId && t.Count < Radio3DPuzzle.Capacity);
                if (matches) candidates.Add(screw);
            }
            Physics.SyncTransforms();
            foreach (var screw in candidates)
                if (PickScrew(View.WorldToScreenPoint(screw.transform.position)) == screw) return screw;
            Quaternion original = Radio.rotation;
            foreach (var screw in candidates)
            {
                Vector3 normal = Radio.InverseTransformDirection(screw.transform.up);
                float yaw = Vector3.SignedAngle(Vector3.ProjectOnPlane(normal, Vector3.up), Vector3.back, Vector3.up);
                foreach (float offset in new[] { 0f, -15f, 15f, -30f, 30f })
                {
                    Radio.rotation = AllowPitch
                        ? Quaternion.AngleAxis(offset, View.transform.up) * Quaternion.FromToRotation(normal, -View.transform.forward)
                        : Quaternion.Euler(0, yaw + offset, 0);
                    Physics.SyncTransforms();
                    if (PickScrew(View.WorldToScreenPoint(screw.transform.position)) == screw) return screw;
                }
                Radio.rotation = original;
            }
            Physics.SyncTransforms();
            return null;
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
            pressedScrew = ExplainLockedScrews ? PickVisibleScrew(point) : PickScrew(point);
        }

        public void MovePointer(Vector2 point)
        {
            if (!tracking) return;
            if (!IsDragging && Vector2.Distance(start, point) >= DragThreshold) IsDragging = true;
            if (IsDragging)
            {
                float degrees = -(point.x - previous.x) * 300f / Mathf.Max(1, Screen.width);
                Radio.Rotate(Vector3.up, degrees, Space.World);
                float pitch = AllowPitch ? (point.y - previous.y) * 300f / Mathf.Max(1, Screen.height) : 0;
                // Camera-relative pitch stays intuitive even after turning the toy around.
                // Quaternions allow continuous rotation through the top and underside.
                if (AllowPitch) Radio.Rotate(View.transform.right, pitch, Space.World);
                Rotated?.Invoke(Mathf.Abs(degrees) + Mathf.Abs(pitch));
            }
            previous = point;
        }

        public void EndPointer(Vector2 point)
        {
            if (!tracking) return;
            MovePointer(point); // Account for a final movement arriving on the release frame.
            if (!IsDragging && !IsOverUi(point) && pressedScrew != null
                && pressedScrew == (ExplainLockedScrews ? PickVisibleScrew(point) : PickScrew(point)) && pressedScrew.gameObject.activeSelf)
            {
                GetComponent<WorkshopHint>()?.Dismiss();
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

        public void ResetView()
        {
            CancelPointer();
            Radio.localRotation = initialRotation;
            Rotated?.Invoke(0);
        }

        public void ResetExperiment()
        {
            ResetView();
            puzzle.Restart();
        }

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) { CancelPointer(); waitForRelease = true; }
        }

        private void OnDisable() { CancelPointer(); }
    }
}
