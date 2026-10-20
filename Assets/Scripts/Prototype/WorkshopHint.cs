using UnityEngine;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    public sealed class WorkshopHint : MonoBehaviour
    {
        public Radio3DScrew Target { get; private set; }
        public string Message { get; private set; }
        private Radio3DInteraction input;
        private Radio3DPuzzle puzzle;
        private RectTransform layout;
        private Image ring;
        private Button button;
        private Texture2D texture;
        private Sprite sprite;
        private float until;

        public void Configure(RectTransform parent, Radio3DPuzzle board, Radio3DInteraction interaction)
        {
            layout = parent; puzzle = board; input = interaction;
            button = parent.Find("Hint").GetComponent<Button>();
            texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), Vector2.one * 31.5f);
                    texture.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(31 - distance) * Mathf.Clamp01(distance - 26)));
                }
            texture.Apply();
            sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), Vector2.one * 0.5f);
            ring = new GameObject("Hint Ring", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            ring.transform.SetParent(parent, false);
            ring.sprite = sprite; ring.color = new Color(1f, 0.94f, 0.58f); ring.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(ring.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.zero, new Vector2(110, 110));
            Clear();
            puzzle.Restarted += Clear;
            input.Rotated += OnRotate;
        }
        public void ShowHint()
        {
            if (!puzzle.CanInteract) return;
            Target = input.FindHint();
            until = Time.unscaledTime + 5f;
            Message = Target == null ? "No matching screw is available right now." : "Tap the highlighted " + Target.ColorId.ToString().ToLowerInvariant() + " screw.";
            RefreshRing();
        }
        private void LateUpdate()
        {
            button.interactable = puzzle.CanInteract;
            if (!puzzle.CanInteract || Time.unscaledTime >= until || (Target != null && Target.IsRemoved)) Clear();
            else RefreshRing();
        }
        private void RefreshRing()
        {
            bool visible = Target != null && !Target.IsRemoved;
            ring.gameObject.SetActive(visible);
            if (!visible) return;
            var canvas = layout.GetComponentInParent<Canvas>();
            Vector2 point;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(layout, input.View.WorldToScreenPoint(Target.transform.position),
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out point);
            ring.rectTransform.anchoredPosition = point;
            ring.rectTransform.localScale = Vector3.one * (1f + 0.08f * Mathf.Sin(Time.unscaledTime * 7f));
        }
        private void OnRotate(float degrees) { if (degrees > 0) Clear(); }
        public void Dismiss() { Clear(); }
        private void Clear() { Target = null; Message = ""; if (ring != null) ring.gameObject.SetActive(false); }
        private void OnDestroy()
        {
            if (puzzle != null) puzzle.Restarted -= Clear;
            if (input != null) input.Rotated -= OnRotate;
            if (sprite != null) Destroy(sprite);
            if (texture != null) Destroy(texture);
        }
    }
}
