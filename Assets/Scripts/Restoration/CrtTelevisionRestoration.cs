using System;
using System.Collections;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>Reseats the television parts and warms the CRT screen into a steady picture.</summary>
    public sealed class CrtTelevisionRestoration : RestorationController
    {
        public sealed class TelevisionPart
        {
            public Transform Visual;
            public Screw[] HoldingScrews;
            public Vector3 ReleasedOffset;
            public float ReleasedRotation;
            public Vector3 OriginalPosition;
            public Quaternion OriginalRotation;
            public bool Released;
        }

        private TelevisionPart[] parts;
        private SpriteRenderer screen;
        private SpriteRenderer lamp;

        public void Configure(SpriteRenderer newScreen, SpriteRenderer newLamp, TelevisionPart[] newParts)
        {
            screen = newScreen;
            lamp = newLamp;
            parts = newParts;
            foreach (TelevisionPart part in parts)
            {
                part.OriginalPosition = part.Visual.localPosition;
                part.OriginalRotation = part.Visual.localRotation;
            }
        }

        public override void HandleScrewRemoved(Screw removedScrew)
        {
            foreach (TelevisionPart part in parts)
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

        private IEnumerator MovePart(TelevisionPart part, Vector3 position, Quaternion rotation, float duration)
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
            foreach (TelevisionPart part in parts)
                yield return MovePart(part, part.OriginalPosition, part.OriginalRotation, 0.18f);
            Color off = screen.color;
            Color on = new Color(0.48f, 0.72f, 0.61f);
            lamp.color = new Color(1f, 0.7f, 0.22f);
            // A short, localized warm-up, followed by a steady screen.
            screen.color = Color.Lerp(off, on, 0.45f);
            yield return new WaitForSeconds(0.15f);
            screen.color = off;
            yield return new WaitForSeconds(0.12f);
            screen.color = Color.Lerp(off, on, 0.7f);
            yield return new WaitForSeconds(0.18f);
            screen.color = Color.Lerp(off, on, 0.3f);
            yield return new WaitForSeconds(0.12f);
            screen.color = on;
            yield return new WaitForSeconds(0.4f);
            onFinished?.Invoke();
        }
    }
}
