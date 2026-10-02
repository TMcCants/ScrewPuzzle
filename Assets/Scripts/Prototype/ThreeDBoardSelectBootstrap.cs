using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    public sealed class ThreeDBoardSelectBootstrap : MonoBehaviour
    {
        private RectTransform layout;
        private Text sound;
        private RectTransform heading, subtitle, soundControl, footer;
        private readonly List<RectTransform> cards = new List<RectTransform>();
        private readonly List<RectTransform> shadows = new List<RectTransform>();
        private RectTransform viewport, content;
        private ScrollRect scroll;
        private int previousColumns;
        private Sprite rounded;
        private Texture2D roundedTexture;
        private RawImage backdrop;
        private RectTransform headerBacking, footerBacking;
        public bool IsWideLayout { get; private set; }

        private void Awake()
        {
            var camera = new GameObject("Board Select Camera").AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.88f, 0.83f, 0.74f);
            camera.gameObject.AddComponent<AudioListener>();
            var canvas = new GameObject("Board Select UI", typeof(Canvas), typeof(GraphicRaycaster)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            backdrop = new GameObject("Toy Store Background", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
            backdrop.transform.SetParent(canvas.transform, false);
            backdrop.rectTransform.anchorMin = Vector2.zero;
            backdrop.rectTransform.anchorMax = Vector2.one;
            backdrop.rectTransform.offsetMin = backdrop.rectTransform.offsetMax = Vector2.zero;
            backdrop.texture = Resources.Load<Texture2D>("Art/Workshop/Toy-Shop-Background");
            backdrop.raycastTarget = false;
            var wash = new GameObject("Background Softening", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            wash.transform.SetParent(canvas.transform, false);
            wash.rectTransform.anchorMin = Vector2.zero;
            wash.rectTransform.anchorMax = Vector2.one;
            wash.rectTransform.offsetMin = wash.rectTransform.offsetMax = Vector2.zero;
            wash.color = new Color(0.94f, 0.87f, 0.74f, 0.22f);
            wash.raycastTarget = false;
            layout = new GameObject("Board Select Layout", typeof(RectTransform)).GetComponent<RectTransform>();
            layout.SetParent(canvas.transform, false);
            if (FindFirstObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            CreateRoundedSprite();
            headerBacking = Panel("Title Backing", layout, Vector2.zero, Vector2.one, new Color(0.98f, 0.95f, 0.88f, 0.92f)).rectTransform;
            heading = Label(layout, "THE TOY SHOP", Vector2.zero, new Vector2(680, 90), 54).rectTransform;
            subtitle = Label(layout, "Pick a project. Make it come apart.", Vector2.zero, new Vector2(900, 70), 28).rectTransform;
            var soundButton = Panel("Sound", layout, Vector2.zero, new Vector2(180, 64), new Color(0.78f, 0.72f, 0.61f));
            soundControl = soundButton.rectTransform;
            soundButton.raycastTarget = true;
            soundButton.gameObject.AddComponent<Button>().onClick.AddListener(() => FeedbackAudio.ToggleSound());
            sound = Label(soundControl, "", Vector2.zero, new Vector2(170, 60), 23);
            viewport = new GameObject("Board Grid", typeof(RectTransform), typeof(Image), typeof(RectMask2D), typeof(ScrollRect)).GetComponent<RectTransform>();
            viewport.SetParent(layout, false);
            viewport.GetComponent<Image>().color = Color.clear;
            content = new GameObject("Board Grid Content", typeof(RectTransform)).GetComponent<RectTransform>();
            content.SetParent(viewport, false);
            content.anchorMin = content.anchorMax = new Vector2(0.5f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            scroll = viewport.GetComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 35;
            MakeCard(ThreeDBoardProgress.Radio, "Radio", "Art/Workshop/Shelf-Radio", new Color(0.57f, 0.32f, 0.15f), ThreeDBoardNavigation.OpenRadio);
            MakeCard(ThreeDBoardProgress.ToyCar, "Toy Car", "Art/Workshop/Shelf-Car", new Color(0.10f, 0.39f, 0.39f), ThreeDBoardNavigation.OpenToyCar);
            footerBacking = Panel("Footer Backing", layout, Vector2.zero, new Vector2(800, 65), new Color(0.98f, 0.95f, 0.88f, 0.92f)).rectTransform;
            footer = Label(layout, "Choose a board to start a fresh puzzle", Vector2.zero, new Vector2(900, 65), 25).rectTransform;
            RefreshLayout(Screen.safeArea);
        }

        private void CreateRoundedSprite()
        {
            roundedTexture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            roundedTexture.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                {
                    Vector2 q = new Vector2(Mathf.Abs(x - 31.5f) - 16, Mathf.Abs(y - 31.5f) - 16);
                    float distance = new Vector2(Mathf.Max(q.x, 0), Mathf.Max(q.y, 0)).magnitude + Mathf.Min(Mathf.Max(q.x, q.y), 0) - 15;
                    roundedTexture.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(0.5f - distance)));
                }
            roundedTexture.Apply();
            rounded = Sprite.Create(roundedTexture, new Rect(0, 0, 64, 64), Vector2.one * 0.5f, 100, 0, SpriteMeshType.FullRect, new Vector4(20, 20, 20, 20));
        }

        private void MakeCard(string boardId, string name, string resource, Color accent, UnityEngine.Events.UnityAction action)
        {
            shadows.Add(Panel(name + " Shadow", content, Vector2.zero, Vector2.one, new Color(0.25f, 0.16f, 0.09f, 0.18f)).rectTransform);
            var card = Panel("Play " + name, content, Vector2.zero, Vector2.one, new Color(0.98f, 0.95f, 0.88f));
            cards.Add(card.rectTransform);
            card.raycastTarget = true;
            card.gameObject.AddComponent<Button>().onClick.AddListener(action);
            Label(card.transform, name.ToUpperInvariant(), new Vector2(0, 130), new Vector2(320, 44), 32);
            bool completed = ThreeDBoardProgress.IsCompleted(boardId);
            if (completed)
            {
                var badge = Label(card.transform, "COMPLETED", new Vector2(0, 98), new Vector2(300, 24), 18);
                badge.gameObject.name = name + " Completed";
                badge.color = new Color(0.12f, 0.38f, 0.28f);
            }

            // The soft grounding shadow and shelf sit behind the actual rendered game model.
            Panel("Object Shadow", card.transform, new Vector2(0, -53), new Vector2(200, 14), new Color(0.25f, 0.17f, 0.10f, 0.13f)).raycastTarget = false;
            Panel("Shelf Shadow", card.transform, new Vector2(0, -71), new Vector2(270, 16), new Color(0.26f, 0.15f, 0.06f, 0.17f)).raycastTarget = false;
            Panel("Wood Shelf", card.transform, new Vector2(0, -62), new Vector2(290, 16), new Color(0.58f, 0.37f, 0.19f)).raycastTarget = false;
            var image = new GameObject(name + " Preview", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
            image.transform.SetParent(card.transform, false);
            image.texture = Resources.Load<Texture2D>(resource);
            image.raycastTarget = false;

            PrototypeLevelBuilder.SetRect(image.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(0, 12), new Vector2(320, 160));
            var play = Panel("Start " + name, card.transform, new Vector2(0, -116), new Vector2(240, 72), accent);
            play.raycastTarget = true;
            play.gameObject.AddComponent<Button>().onClick.AddListener(action);
            var label = Label(play.transform, completed ? "REPLAY  →" : "PLAY  →", Vector2.zero, new Vector2(230, 68), 27);
            label.color = new Color(1f, 0.97f, 0.88f);
        }

        private Image Panel(string name, Transform parent, Vector2 position, Vector2 size, Color color)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            PrototypeLevelBuilder.SetRect(obj.GetComponent<RectTransform>(), Vector2.one * 0.5f, Vector2.one * 0.5f, position, size);
            var image = obj.GetComponent<Image>();
            image.sprite = rounded;
            image.type = Image.Type.Sliced;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Text Label(Transform parent, string value, Vector2 position, Vector2 size, int fontSize)
        {
            var text = PrototypeLevelBuilder.CreateText("Label", parent, value, fontSize, TextAnchor.MiddleCenter, new Color(0.25f, 0.18f, 0.12f));
            text.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(text.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, position, size);
            return text;
        }

        public void RefreshLayout(Rect safe)
        {
            IsWideLayout = safe.width >= safe.height * 1.1f;
            Vector2 design = IsWideLayout ? new Vector2(1440, 900) : new Vector2(900, 1560);
            layout.sizeDelta = design;
            layout.position = safe.center;
            layout.localScale = Vector3.one * Mathf.Min(safe.width / design.x, safe.height / design.y);
            headerBacking.anchoredPosition = new Vector2(0, IsWideLayout ? 330 : 650);
            headerBacking.sizeDelta = new Vector2(IsWideLayout ? 1360 : 840, 180);
            heading.anchoredPosition = IsWideLayout ? new Vector2(-330, 357) : new Vector2(-90, 660);
            subtitle.anchoredPosition = IsWideLayout ? new Vector2(-220, 285) : new Vector2(0, 588);
            soundControl.anchoredPosition = IsWideLayout ? new Vector2(590, 356) : new Vector2(325, 665);
            footer.anchoredPosition = new Vector2(0, IsWideLayout ? -377 : -620);
            footerBacking.anchoredPosition = footer.anchoredPosition;
            if (backdrop.texture != null)
            {
                Rect area = backdrop.rectTransform.rect;
                float screenAspect = area.width / Mathf.Max(1f, area.height);
                float artAspect = (float)backdrop.texture.width / backdrop.texture.height;
                float width = Mathf.Min(1f, screenAspect / artAspect);
                float height = Mathf.Min(1f, artAspect / screenAspect);
                backdrop.uvRect = new Rect((1f - width) / 2f, (1f - height) / 2f, width, height);
            }
            int columns = IsWideLayout ? 3 : 2;
            viewport.anchoredPosition = new Vector2(0, IsWideLayout ? -40 : -10);
            viewport.sizeDelta = new Vector2(IsWideLayout ? 1180 : 780, IsWideLayout ? 500 : 920);
            int rows = Mathf.CeilToInt((float)cards.Count / columns);
            float contentHeight = Mathf.Max(viewport.sizeDelta.y, rows * 366f + 18f);
            content.sizeDelta = new Vector2(viewport.sizeDelta.x, contentHeight);
            scroll.vertical = contentHeight > viewport.sizeDelta.y;
            if (previousColumns != columns)
            {
                content.anchoredPosition = Vector2.zero;
                scroll.StopMovement();
                previousColumns = columns;
            }
            int usedColumns = Mathf.Min(columns, cards.Count);
            for (int i = 0; i < cards.Count; i++)
            {
                Vector2 position = new Vector2((i % columns - (usedColumns - 1) / 2f) * 376f,
                    contentHeight / 2f - 183f - (i / columns) * 366f);
                cards[i].anchoredPosition = position;
                cards[i].sizeDelta = new Vector2(340, 330);
                shadows[i].anchoredPosition = position + new Vector2(0, -7);
                shadows[i].sizeDelta = cards[i].sizeDelta;
            }
            sound.text = FeedbackAudio.IsSoundEnabled ? "SOUND ON" : "SOUND OFF";
        }

        private void Update() { RefreshLayout(Screen.safeArea); }
        private void OnDestroy()
        {
            if (rounded != null) Destroy(rounded);
            if (roundedTexture != null) Destroy(roundedTexture);
        }
    }
}

