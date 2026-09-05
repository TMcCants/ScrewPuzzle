using System;
using System.Collections;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Connects puzzle progress to the radio's visible response.
    /// Each part loosens when all screws assigned to it leave the radio.
    /// </summary>
    public sealed class RadioRestoration : MonoBehaviour
    {
        [Serializable]
        public sealed class RadioPart
        {
            public string name;
            public Transform visual;
            public Screw[] holdingScrews;
            public Vector3 releasedOffset;
            public float releasedRotation;

            [NonSerialized] public Vector3 originalLocalPosition;
            [NonSerialized] public Quaternion originalLocalRotation;
            [NonSerialized] public bool isReleased;
        }

        [SerializeField] private Transform radioRoot;
        [SerializeField] private SpriteRenderer displayGlow;
        [SerializeField] private RadioPart[] parts;

        private void Start()
        {
            CacheOriginalPartPositions();
        }

        public void HandleScrewRemoved(Screw removedScrew)
        {
            foreach (RadioPart part in parts)
            {
                if (!part.isReleased && AreAllHoldingScrewsRemoved(part))
                {
                    part.isReleased = true;
                    StartCoroutine(ReleasePart(part));
                }
            }
        }

        public void PlayFinalRestoration(Action onFinished)
        {
            StartCoroutine(FinalRestorationRoutine(onFinished));
        }

        public void Configure(
            Transform newRadioRoot,
            SpriteRenderer newDisplayGlow,
            RadioPart[] newParts)
        {
            radioRoot = newRadioRoot;
            displayGlow = newDisplayGlow;
            parts = newParts;
            CacheOriginalPartPositions();
        }

        private void CacheOriginalPartPositions()
        {
            if (parts == null)
            {
                return;
            }

            foreach (RadioPart part in parts)
            {
                if (part.visual == null)
                {
                    continue;
                }

                part.originalLocalPosition = part.visual.localPosition;
                part.originalLocalRotation = part.visual.localRotation;
            }
        }

        private bool AreAllHoldingScrewsRemoved(RadioPart part)
        {
            if (part.holdingScrews == null || part.holdingScrews.Length == 0)
            {
                return false;
            }

            foreach (Screw screw in part.holdingScrews)
            {
                if (screw != null && !screw.IsRemovedFromObject)
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerator ReleasePart(RadioPart part)
        {
            Vector3 startPosition = part.visual.localPosition;
            Quaternion startRotation = part.visual.localRotation;
            Vector3 targetPosition = part.originalLocalPosition + part.releasedOffset;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, part.releasedRotation);
            float elapsed = 0f;
            const float duration = 0.35f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                part.visual.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                part.visual.localRotation = Quaternion.Slerp(startRotation, targetRotation, progress);
                yield return null;
            }
        }

        private IEnumerator FinalRestorationRoutine(Action onFinished)
        {
            const float restoreDuration = 0.65f;
            float elapsed = 0f;

            while (elapsed < restoreDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, elapsed / restoreDuration);

                foreach (RadioPart part in parts)
                {
                    if (part.visual == null)
                    {
                        continue;
                    }

                    part.visual.localPosition = Vector3.Lerp(
                        part.visual.localPosition,
                        part.originalLocalPosition,
                        progress);
                    part.visual.localRotation = Quaternion.Slerp(
                        part.visual.localRotation,
                        part.originalLocalRotation,
                        progress);
                }

                yield return null;
            }

            foreach (RadioPart part in parts)
            {
                if (part.visual != null)
                {
                    part.visual.localPosition = part.originalLocalPosition;
                    part.visual.localRotation = part.originalLocalRotation;
                }
            }

            yield return PulseRadio();

            if (onFinished != null)
            {
                onFinished();
            }
        }

        private IEnumerator PulseRadio()
        {
            Vector3 originalScale = radioRoot.localScale;
            Color originalGlowColor = displayGlow.color;
            float elapsed = 0f;
            const float duration = 1.1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float pulse = Mathf.Sin((elapsed / duration) * Mathf.PI * 4f) * 0.04f;
                radioRoot.localScale = originalScale * (1f + pulse);

                float brightness = 0.55f + Mathf.Abs(Mathf.Sin(elapsed * 9f)) * 0.45f;
                displayGlow.color = Color.Lerp(originalGlowColor, new Color(1f, 0.85f, 0.25f), brightness);
                yield return null;
            }

            radioRoot.localScale = originalScale;
            displayGlow.color = new Color(1f, 0.78f, 0.18f);
        }
    }
}
