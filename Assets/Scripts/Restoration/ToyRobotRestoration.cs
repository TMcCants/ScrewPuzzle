using System;
using System.Collections;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Loosens robot service panels during play, then reassembles and activates the robot on a win.
    /// Limbs remain attached throughout the restoration.
    /// </summary>
    public sealed class ToyRobotRestoration : RestorationController
    {
        [Serializable]
        public sealed class RobotPart
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

        [SerializeField] private Transform robotRoot;
        [SerializeField] private Transform headRoot;
        [SerializeField] private Transform waveArm;
        [SerializeField] private SpriteRenderer[] eyes;
        [SerializeField] private SpriteRenderer chestLight;
        [SerializeField] private RobotPart[] parts;

        private Quaternion originalHeadRotation;
        private Quaternion originalArmRotation;

        private void Start()
        {
            CacheOriginalTransforms();
        }

        public override void HandleScrewRemoved(Screw removedScrew)
        {
            foreach (RobotPart part in parts)
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
            Transform newRobotRoot,
            Transform newHeadRoot,
            Transform newWaveArm,
            SpriteRenderer[] newEyes,
            SpriteRenderer newChestLight,
            RobotPart[] newParts)
        {
            robotRoot = newRobotRoot;
            headRoot = newHeadRoot;
            waveArm = newWaveArm;
            eyes = newEyes;
            chestLight = newChestLight;
            parts = newParts;
            CacheOriginalTransforms();
        }

        private void CacheOriginalTransforms()
        {
            if (headRoot != null)
            {
                originalHeadRotation = headRoot.localRotation;
            }

            if (waveArm != null)
            {
                originalArmRotation = waveArm.localRotation;
            }

            if (parts == null)
            {
                return;
            }

            foreach (RobotPart part in parts)
            {
                if (part.visual == null)
                {
                    continue;
                }

                part.originalLocalPosition = part.visual.localPosition;
                part.originalLocalRotation = part.visual.localRotation;
            }
        }

        private bool AreAllHoldingScrewsRemoved(RobotPart part)
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

        private IEnumerator ReleasePart(RobotPart part)
        {
            if (part.visual == null)
            {
                yield break;
            }

            Vector3 startPosition = part.visual.localPosition;
            Quaternion startRotation = part.visual.localRotation;
            Vector3 targetPosition = part.originalLocalPosition + part.releasedOffset;
            Quaternion targetRotation = part.originalLocalRotation
                * Quaternion.Euler(0f, 0f, part.releasedRotation);
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

                foreach (RobotPart part in parts)
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

            foreach (RobotPart part in parts)
            {
                if (part.visual != null)
                {
                    part.visual.localPosition = part.originalLocalPosition;
                    part.visual.localRotation = part.originalLocalRotation;
                }
            }

            yield return PlayRobotActivation();

            if (onFinished != null)
            {
                onFinished();
            }
        }

        private IEnumerator PlayRobotActivation()
        {
            Color eyeOff = new Color(0.22f, 0.18f, 0.10f);
            Color eyeOn = new Color(1f, 0.72f, 0.18f);
            Color chestOff = chestLight == null ? eyeOff : chestLight.color;

            SetEyeColor(eyeOff);

            for (int flicker = 0; flicker < 3; flicker++)
            {
                SetEyeColor(eyeOn);
                yield return new WaitForSeconds(0.08f);
                SetEyeColor(eyeOff);
                yield return new WaitForSeconds(0.06f);
            }

            SetEyeColor(eyeOn);

            if (headRoot != null)
            {
                yield return RotateTransform(
                    headRoot,
                    originalHeadRotation,
                    originalHeadRotation * Quaternion.Euler(0f, 0f, -7f),
                    0.2f);
                yield return RotateTransform(
                    headRoot,
                    headRoot.localRotation,
                    originalHeadRotation,
                    0.16f);
            }

            if (waveArm != null)
            {
                Quaternion raisedRotation = originalArmRotation * Quaternion.Euler(0f, 0f, 34f);
                yield return RotateTransform(waveArm, originalArmRotation, raisedRotation, 0.18f);

                for (int wave = 0; wave < 2; wave++)
                {
                    Quaternion waveOut = raisedRotation * Quaternion.Euler(0f, 0f, -9f);
                    yield return RotateTransform(waveArm, waveArm.localRotation, waveOut, 0.09f);
                    yield return RotateTransform(waveArm, waveArm.localRotation, raisedRotation, 0.09f);
                }

                yield return RotateTransform(
                    waveArm,
                    waveArm.localRotation,
                    originalArmRotation,
                    0.2f);
            }

            if (chestLight != null)
            {
                chestLight.color = eyeOn;
                yield return new WaitForSeconds(0.12f);
                chestLight.color = chestOff;
                yield return new WaitForSeconds(0.08f);
                chestLight.color = eyeOn;
            }

            if (robotRoot != null)
            {
                Vector3 originalScale = robotRoot.localScale;
                float elapsed = 0f;
                const float duration = 0.28f;

                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float progress = Mathf.Clamp01(elapsed / duration);
                    float proudPulse = Mathf.Sin(progress * Mathf.PI) * 0.035f;
                    robotRoot.localScale = originalScale * (1f + proudPulse);
                    yield return null;
                }

                robotRoot.localScale = originalScale;
            }

            yield return new WaitForSeconds(0.22f);
        }

        private IEnumerator RotateTransform(
            Transform target,
            Quaternion startRotation,
            Quaternion targetRotation,
            float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                target.localRotation = Quaternion.Slerp(startRotation, targetRotation, progress);
                yield return null;
            }

            target.localRotation = targetRotation;
        }

        private void SetEyeColor(Color color)
        {
            if (eyes == null)
            {
                return;
            }

            foreach (SpriteRenderer eye in eyes)
            {
                if (eye != null)
                {
                    eye.color = color;
                }
            }
        }
    }
}
