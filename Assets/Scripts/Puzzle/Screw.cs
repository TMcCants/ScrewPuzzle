using System.Collections;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Owns the state and movement of one screw.
    /// It does not decide matches, wins, losses, or restoration.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public sealed class Screw : MonoBehaviour
    {
        [Header("Screw identity")]
        [SerializeField] private ScrewColorId colorId;

        [Header("Scene references")]
        [SerializeField] private ScrewDependency dependency;
        [SerializeField] private TrayManager trayManager;
        [SerializeField] private GameManager gameManager;

        [Header("Movement")]
        [SerializeField, Min(0.05f)] private float moveDuration = 0.25f;

        private Collider2D screwCollider;
        private SpriteRenderer screwRenderer;
        private Color normalColor;
        private bool isMoving;
        private Coroutine blockedFeedbackRoutine;

        public ScrewColorId ColorId { get { return colorId; } }
        public bool IsRemovedFromObject { get; private set; }

        private void Awake()
        {
            screwCollider = GetComponent<Collider2D>();
            screwRenderer = GetComponent<SpriteRenderer>();

            if (screwRenderer != null)
            {
                normalColor = screwRenderer.color;
            }
        }

        private void Update()
        {
            if (IsRemovedFromObject || screwRenderer == null)
            {
                return;
            }

            bool isBlocked = dependency != null && !dependency.AreAllBlockersRemoved();
            Color displayColor = normalColor;
            displayColor.a = isBlocked ? 0.4f : 1f;
            screwRenderer.color = displayColor;
        }

        private void OnMouseDown()
        {
            TrySelect();
        }

        public void TrySelect()
        {
            if (IsRemovedFromObject || isMoving || gameManager == null || trayManager == null)
            {
                return;
            }

            if (!gameManager.CanAcceptInput)
            {
                return;
            }

            if (dependency != null && !dependency.AreAllBlockersRemoved())
            {
                PlayBlockedFeedback();
                FeedbackAudio.PlayBlocked();
                gameManager.ShowTemporaryMessage("That screw is still blocked.");
                return;
            }

            trayManager.TryAddScrew(this);
        }

        public void MoveToTray(Vector3 targetPosition)
        {
            IsRemovedFromObject = true;
            isMoving = true;

            if (screwCollider != null)
            {
                screwCollider.enabled = false;
            }

            gameManager.NotifyScrewRemovedFromObject(this);
            StartCoroutine(MoveRoutine(targetPosition, true));
        }

        public void MoveToAnotherTraySlot(Vector3 targetPosition)
        {
            StartCoroutine(MoveRoutine(targetPosition, false));
        }

        public void ClearFromTray()
        {
            StopAllCoroutines();
            StartCoroutine(ClearRoutine());
        }

        public void Configure(
            ScrewColorId newColorId,
            ScrewDependency newDependency,
            TrayManager newTrayManager,
            GameManager newGameManager)
        {
            colorId = newColorId;
            dependency = newDependency;
            trayManager = newTrayManager;
            gameManager = newGameManager;
        }

        private IEnumerator MoveRoutine(Vector3 targetPosition, bool playArrivalPulse)
        {
            Vector3 startPosition = transform.position;
            float elapsed = 0f;

            while (elapsed < moveDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / moveDuration);
                float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
                transform.position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
                yield return null;
            }

            transform.position = targetPosition;
            isMoving = false;

            if (playArrivalPulse)
            {
                yield return ArrivalPulseRoutine();
            }
        }

        private IEnumerator ArrivalPulseRoutine()
        {
            Vector3 originalScale = transform.localScale;
            float elapsed = 0f;
            const float duration = 0.12f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float pulse = Mathf.Sin(progress * Mathf.PI) * 0.1f;
                transform.localScale = originalScale * (1f + pulse);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        private void PlayBlockedFeedback()
        {
            // Ignore repeated taps until the current shake finishes.
            // This prevents multiple coroutines from fighting over the screw's position.
            if (blockedFeedbackRoutine == null)
            {
                blockedFeedbackRoutine = StartCoroutine(BlockedFeedbackRoutine());
            }
        }

        private IEnumerator BlockedFeedbackRoutine()
        {
            Vector3 startPosition = transform.localPosition;
            float elapsed = 0f;
            const float duration = 0.28f;
            const float shakeDistance = 0.14f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float strength = 1f - progress;
                float horizontalShake = Mathf.Sin(progress * Mathf.PI * 4f)
                    * shakeDistance
                    * strength;

                transform.localPosition = startPosition + (Vector3.right * horizontalShake);
                yield return null;
            }

            transform.localPosition = startPosition;
            blockedFeedbackRoutine = null;
        }

        private IEnumerator ClearRoutine()
        {
            Vector3 startScale = transform.localScale;
            Color startColor = screwRenderer == null ? normalColor : screwRenderer.color;
            float elapsed = 0f;
            const float glowDuration = 0.09f;

            while (elapsed < glowDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / glowDuration);
                float pulse = Mathf.Sin(progress * Mathf.PI);
                transform.localScale = startScale * (1f + (pulse * 0.16f));

                if (screwRenderer != null)
                {
                    screwRenderer.color = Color.Lerp(startColor, Color.white, pulse * 0.65f);
                }

                yield return null;
            }

            transform.localScale = startScale;

            if (screwRenderer != null)
            {
                screwRenderer.color = startColor;
            }

            elapsed = 0f;
            const float clearDuration = 0.16f;

            while (elapsed < clearDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / clearDuration);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
