using System.Collections;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>A plate stays attached until all of its own screws have been collected.</summary>
    public sealed class Radio3DPlate : MonoBehaviour
    {
        public bool IsReleased { get; private set; }
        public bool IsAnimating { get; private set; }
        private Radio3DScrew[] fasteners;
        private Collider[] colliders;
        private Vector3 origin;
        private Quaternion orientation;
        private Vector3 scale;
        private Vector3 outward;

        public void Configure(Radio3DScrew[] screws, Vector3 normal)
        {
            fasteners = screws;
            outward = normal;
            origin = transform.localPosition;
            orientation = transform.localRotation;
            scale = transform.localScale;
            colliders = GetComponentsInChildren<Collider>();
        }

        // The puzzle owns this coroutine so Restart cancels tray and plate motion together.
        public IEnumerator ReleaseIfReady()
        {
            if (IsReleased || fasteners == null || fasteners.Length == 0) yield break;
            foreach (Radio3DScrew screw in fasteners)
                if (!screw.IsRemoved) yield break;

            IsReleased = IsAnimating = true;
            foreach (Collider collider in colliders) collider.enabled = false;
            const float duration = 0.65f;
            for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                float t = Mathf.Clamp01(elapsed / duration);
                transform.localPosition = origin + outward * (0.75f * t) + Vector3.down * (1.3f * t * t);
                transform.localRotation = orientation * Quaternion.Euler(25f * t, 0f, 30f * t);
                transform.localScale = scale * (1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.55f, 1f, t)));
                yield return null;
            }
            IsAnimating = false;
            gameObject.SetActive(false);
        }

        public void Restore()
        {
            transform.localPosition = origin;
            transform.localRotation = orientation;
            transform.localScale = scale;
            foreach (Collider collider in colliders) collider.enabled = true;
            IsReleased = IsAnimating = false;
            gameObject.SetActive(true);
        }
    }
}
