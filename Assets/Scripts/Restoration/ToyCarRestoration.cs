using System;
using System.Collections;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Loosens toy-car parts during play, then reassembles and starts the car on a win.
    /// </summary>
    public sealed class ToyCarRestoration : RestorationController
    {
        [Serializable]
        public sealed class CarPart
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

        [SerializeField] private Transform carRoot;
        [SerializeField] private SpriteRenderer[] headlights;
        [SerializeField] private CarPart[] parts;

        private void Start()
        {
            CacheOriginalPartPositions();
        }

        public override void HandleScrewRemoved(Screw removedScrew)
        {
            foreach (CarPart part in parts)
            {
                if (!part.isReleased && AreAllHoldingScrewsRemoved(part))
                {
                    part.isReleased = true;
                    StartCoroutine(ReleasePart(part));
                }
            }
        }

        public override void PlayFinalRestoration(Action onFinished)
        {
            StartCoroutine(FinalRestorationRoutine(onFinished));
        }

        public void Configure(
            Transform newCarRoot,
            SpriteRenderer[] newHeadlights,
            CarPart[] newParts)
        {
            carRoot = newCarRoot;
            headlights = newHeadlights;
            parts = newParts;
            CacheOriginalPartPositions();
        }

        private void CacheOriginalPartPositions()
        {
            if (parts == null)
            {
                return;
            }

            foreach (CarPart part in parts)
            {
                if (part.visual == null)
                {
                    continue;
                }

                part.originalLocalPosition = part.visual.localPosition;
                part.originalLocalRotation = part.visual.localRotation;
            }
        }

        private bool AreAllHoldingScrewsRemoved(CarPart part)
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

        private IEnumerator ReleasePart(CarPart part)
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

                foreach (CarPart part in parts)
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

            foreach (CarPart part in parts)
            {
                if (part.visual != null)
                {
                    part.visual.localPosition = part.originalLocalPosition;
                    part.visual.localRotation = part.originalLocalRotation;
                }
            }

            yield return PlayCarStartEffect();

            if (onFinished != null)
            {
                onFinished();
            }
        }

        private IEnumerator PlayCarStartEffect()
        {
            Color brightHeadlight = new Color(1f, 0.88f, 0.35f);

            for (int flash = 0; flash < 3; flash++)
            {
                SetHeadlightColor(brightHeadlight);
                yield return new WaitForSeconds(0.09f);
                SetHeadlightColor(new Color(0.36f, 0.31f, 0.16f));
                yield return new WaitForSeconds(0.07f);
            }

            SetHeadlightColor(brightHeadlight);

            if (carRoot != null)
            {
                Vector3 startPosition = carRoot.localPosition;
                float elapsed = 0f;
                const float driveDuration = 0.75f;

                while (elapsed < driveDuration)
                {
                    elapsed += Time.deltaTime;
                    float progress = Mathf.Clamp01(elapsed / driveDuration);
                    float driveOffset = Mathf.Sin(progress * Mathf.PI) * 0.85f;
                    carRoot.localPosition = startPosition + (Vector3.right * driveOffset);
                    yield return null;
                }

                carRoot.localPosition = startPosition;
            }

            yield return new WaitForSeconds(0.2f);
        }

        private void SetHeadlightColor(Color color)
        {
            if (headlights == null)
            {
                return;
            }

            foreach (SpriteRenderer headlight in headlights)
            {
                if (headlight != null)
                {
                    headlight.color = color;
                }
            }
        }
    }
}
