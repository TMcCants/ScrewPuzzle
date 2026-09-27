using System;
using System.Collections;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>Reseats the camera parts, focuses the lens, then fires one local flash.</summary>
    public sealed class OldCameraRestoration : RestorationController
    {
        public sealed class CameraPart
        {
            public Transform Visual;
            public Screw[] HoldingScrews;
            public Vector3 ReleasedOffset;
            public float ReleasedRotation;
            public Vector3 OriginalPosition;
            public Quaternion OriginalRotation;
            public bool Released;
        }

        private CameraPart[] parts;
        private Transform lens;
        private SpriteRenderer flash;

        public void Configure(Transform newLens, SpriteRenderer newFlash, CameraPart[] newParts)
        {
            lens = newLens;
            flash = newFlash;
            parts = newParts;
            foreach (CameraPart part in parts)
            {
                part.OriginalPosition = part.Visual.localPosition;
                part.OriginalRotation = part.Visual.localRotation;
            }
        }

        public override void HandleScrewRemoved(Screw removedScrew)
        {
            foreach (CameraPart part in parts)
            {
                if (part.Released) continue;
                bool ready = true;
                foreach (Screw screw in part.HoldingScrews)
                    if (screw != null && !screw.IsRemovedFromObject) ready = false;
                if (!ready) continue;
                part.Released = true;
                StartCoroutine(MovePart(part, part.OriginalPosition + part.ReleasedOffset,
                    part.OriginalRotation * Quaternion.Euler(0f, 0f, part.ReleasedRotation), 0.35f));
            }
        }

        public override void PlayFinalRestoration(Action onFinished)
        {
            StopAllCoroutines();
            StartCoroutine(Restore(onFinished));
        }

        private IEnumerator MovePart(CameraPart part, Vector3 position, Quaternion rotation, float duration)
        {
            Vector3 start = part.Visual.localPosition;
            Quaternion startRotation = part.Visual.localRotation;
            for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
            {
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                part.Visual.localPosition = Vector3.Lerp(start, position, t);
                part.Visual.localRotation = Quaternion.Slerp(startRotation, rotation, t);
                yield return null;
            }
            part.Visual.localPosition = position;
            part.Visual.localRotation = rotation;
        }

        private IEnumerator Restore(Action onFinished)
        {
            foreach (CameraPart part in parts)
                yield return MovePart(part, part.OriginalPosition, part.OriginalRotation, 0.18f);
            Vector3 scale = lens.localScale;
            for (float elapsed = 0f; elapsed < 0.4f; elapsed += Time.deltaTime)
            {
                lens.localScale = scale * (1f + Mathf.Sin(elapsed / 0.4f * Mathf.PI) * 0.09f);
                yield return null;
            }
            lens.localScale = scale;
            Color idle = flash.color;
            flash.color = new Color(1f, 0.96f, 0.78f);
            FeedbackAudio.PlaySelected();
            yield return new WaitForSeconds(0.12f);
            flash.color = idle;
            yield return new WaitForSeconds(0.35f);
            onFinished?.Invoke();
        }
    }
}
