using UnityEngine;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    public sealed class WorkshopCompletion : MonoBehaviour
    {
        private Radio3DPuzzle puzzle;
        private GameObject overlay;
        private RectTransform card;
        private readonly RectTransform[] confetti = new RectTransform[24];
        private readonly Vector3[] corners = new Vector3[4];
        private float elapsed;
        public bool IsVisible => overlay != null && overlay.activeSelf;

        public void Configure(RectTransform layout, Radio3DPuzzle board, string boardId, string title, System.Action replay)
        {
            puzzle = board;
            overlay = Box("Completion Overlay", layout, Vector2.zero, new Vector2(1080, 1920), new Color(0.04f, 0.03f, 0.02f, 0.85f)).gameObject;
            card = Box("Completion Card", overlay.transform, Vector2.zero, new Vector2(900, 780), new Color(0.98f, 0.94f, 0.84f)).rectTransform;
            Text(card, "WELL DONE!", 64, 275, new Color(0.15f, 0.4f, 0.34f));
            Text(card, title, 38, 175, new Color(0.3f, 0.2f, 0.12f));
            Text(card, "Every screw sorted. Every layer discovered.", 28, 95, new Color(0.3f, 0.2f, 0.12f));
            AddButton("Next Toy", "NEXT TOY  →", -30, new Color(0.1f, 0.4f, 0.36f), () => ThreeDBoardNavigation.OpenNext(boardId));
            AddButton("Back to Shop", "BACK TO SHOP", -150, new Color(0.55f, 0.32f, 0.17f), ThreeDBoardNavigation.OpenSelector);
            AddButton("Replay Toy", "PLAY AGAIN", -270, new Color(0.37f, 0.36f, 0.32f), () => { replay(); overlay.SetActive(false); });
            for (int i = 0; i < confetti.Length; i++)
            {
                Color color = i % 3 == 0 ? new Color(1f, 0.72f, 0.18f) : i % 3 == 1 ? new Color(0.9f, 0.3f, 0.2f) : new Color(0.15f, 0.7f, 0.62f);
                var piece = Box("Confetti", overlay.transform, Vector2.zero, new Vector2(14, 25), color);
                piece.raycastTarget = false;
                confetti[i] = piece.rectTransform;
            }
            overlay.SetActive(false);
            puzzle.Completed += Show;
            puzzle.Restarted += Hide;
        }

        private void Hide() { overlay.SetActive(false); }

        private void Show()
        {
            elapsed = 0;
            overlay.SetActive(true);
            card.localScale = Vector3.one * 0.85f;
            foreach (var piece in confetti) piece.gameObject.SetActive(true);
            Animate();
            GetComponent<Radio3DFeedback>().PlayCompletion();
        }

        private void LateUpdate()
        {
            if (!IsVisible) return;
            if (puzzle.State != Radio3DPuzzle.PuzzleState.Won) { overlay.SetActive(false); return; }
            elapsed += Time.unscaledDeltaTime;
            Animate();
        }

        private void Animate()
        {
            var backdrop = overlay.GetComponent<RectTransform>();
            var canvas = overlay.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            // Cover the full canvas even when the safe-area layout is a narrow portrait column.
            canvas.GetWorldCorners(corners);
            Vector2 extent = Vector2.zero;
            foreach (var corner in corners)
            {
                Vector3 point = backdrop.InverseTransformPoint(corner);
                extent = Vector2.Max(extent, new Vector2(Mathf.Abs(point.x), Mathf.Abs(point.y)));
            }
            backdrop.sizeDelta = extent * 2f;
            card.localScale = Vector3.one * Mathf.Lerp(0.85f, 1f, Mathf.SmoothStep(0, 1, elapsed / 0.3f));
            for (int i = 0; i < confetti.Length; i++)
            {
                confetti[i].gameObject.SetActive(elapsed < 2.5f);
                confetti[i].anchoredPosition = new Vector2(-490 + (i * 137 % 980), 680 - elapsed * (360 + i % 5 * 45));
                confetti[i].localRotation = Quaternion.Euler(0, 0, i * 31 + elapsed * (i % 2 == 0 ? 160 : -160));
            }
        }

        private void AddButton(string name, string caption, float y, Color color, UnityEngine.Events.UnityAction action)
        {
            var image = Box(name, card, new Vector2(0, y), new Vector2(680, 92), color);
            image.gameObject.AddComponent<Button>().onClick.AddListener(action);
            Text(image.transform, caption, 34, 0, new Color(1f, 0.97f, 0.88f));
        }
        private static Image Box(string name, Transform parent, Vector2 position, Vector2 size, Color color)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            PrototypeLevelBuilder.SetRect(obj.GetComponent<RectTransform>(), Vector2.one * 0.5f, Vector2.one * 0.5f, position, size);
            var image = obj.GetComponent<Image>(); image.color = color; return image;
        }
        private static void Text(Transform parent, string caption, int size, float y, Color color)
        {
            var text = PrototypeLevelBuilder.CreateText("Label", parent, caption, size, TextAnchor.MiddleCenter, color);
            text.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(text.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0, y), new Vector2(850, 90));
        }
        private void OnDestroy()
        {
            if (puzzle == null) return;
            puzzle.Completed -= Show;
            puzzle.Restarted -= Hide;
        }
    }
}
