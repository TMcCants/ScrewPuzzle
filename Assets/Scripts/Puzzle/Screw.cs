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
            StartCoroutine(MoveRoutine(targetPosition));
        }

        public void MoveToAnotherTraySlot(Vector3 targetPosition)
        {
            StartCoroutine(MoveRoutine(targetPosition));
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

        private IEnumerator MoveRoutine(Vector3 targetPosition)
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

        }

        private IEnumerator ClearRoutine()
        {
            Vector3 startScale = transform.localScale;
            float elapsed = 0f;
            const float clearDuration = 0.2f;

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
