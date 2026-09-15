using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>
    /// Creates the temporary shapes, tray, UI, and screws shared by prototype levels.
    /// Object-specific bootstraps remain responsible for building their own object visuals.
    /// </summary>
    public static class PrototypeLevelBuilder
    {
        private static Sprite squareSprite;
        private static Sprite circleSprite;

        public static Camera BuildCamera(Color backgroundColor)
        {
            Camera camera = Camera.main;

            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 6.8f;
            camera.transform.position = new Vector3(0f, 0f, -10f);
            camera.backgroundColor = backgroundColor;
            camera.clearFlags = CameraClearFlags.SolidColor;
            return camera;
        }

        public static void BuildBackground(Color backgroundColor)
        {
            Transform background = CreateRectangle(
                "Workshop Background",
                null,
                new Vector3(0f, 0f, 2f),
                new Vector2(12.5f, 15f),
                backgroundColor,
                -10);
            background.position = new Vector3(0f, 0f, 2f);
        }

        public static Transform[] BuildTray(int capacity)
        {
            Transform trayRoot = new GameObject("Holding Tray").transform;
            trayRoot.position = new Vector3(0f, -4.2f, 0f);
            CreateRectangle(
                "Tray Background",
                trayRoot,
                Vector3.zero,
                new Vector2((capacity * 1.3f) + 0.8f, 1.25f),
                new Color(0.16f, 0.16f, 0.19f),
                1);

            Transform[] slots = new Transform[capacity];
            float firstSlotX = -((capacity - 1) * 1.3f) / 2f;

            for (int index = 0; index < slots.Length; index++)
            {
                float x = firstSlotX + (index * 1.3f);
                slots[index] = CreateCircle(
                    "Tray Slot " + (index + 1),
                    trayRoot,
                    new Vector3(x, 0f, 0f),
                    0.85f,
                    new Color(0.28f, 0.28f, 0.32f),
                    2);
            }

            return slots;
        }

        public static Screw[] BuildScrews(
            Transform objectRoot,
            TrayManager trayManager,
            GameManager gameManager,
            ScrewDefinition[] screwDefinitions)
        {
            Screw[] screws = new Screw[screwDefinitions.Length];

            for (int index = 0; index < screwDefinitions.Length; index++)
            {
                ScrewDefinition definition = screwDefinitions[index];
                Transform screwVisual = CreateCircle(
                    definition.ColorId + " Screw " + (index + 1),
                    objectRoot,
                    definition.Position,
                    0.62f,
                    ColorFor(definition.ColorId),
                    10);

                screwVisual.gameObject.AddComponent<CircleCollider2D>();
                ScrewDependency dependency = screwVisual.gameObject.AddComponent<ScrewDependency>();
                Screw screw = screwVisual.gameObject.AddComponent<Screw>();
                screw.Configure(definition.ColorId, dependency, trayManager, gameManager);
                screws[index] = screw;

                CreateRectangle(
                    "Screw Slot",
                    screwVisual,
                    Vector3.zero,
                    new Vector2(0.34f, 0.08f),
                    new Color(0.18f, 0.16f, 0.14f),
                    11);
            }

            ConfigureBlockers(screws, screwDefinitions);
            return screws;
        }

        public static PrototypeUiReferences BuildUi(
            GameManager gameManager,
            LevelDefinition level)
        {
            GameObject canvasObject = new GameObject("UI");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;
            canvasObject.AddComponent<GraphicRaycaster>();

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }

            Text title = CreateText(
                "Title",
                canvas.transform,
                "SCREWPUZZLE",
                56,
                TextAnchor.MiddleCenter,
                new Color(1f, 0.82f, 0.38f));
            SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -75f), new Vector2(900f, 80f));

            Text levelName = CreateText(
                "Level Name",
                canvas.transform,
                level.LevelName.ToUpperInvariant(),
                34,
                TextAnchor.MiddleCenter,
                Color.white);
            SetRect(levelName.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -145f), new Vector2(900f, 60f));

            Text instruction = CreateText(
                "Instructions",
                canvas.transform,
                "Tap open screws. Match " + level.MatchSize + " of one color before the tray fills.",
                28,
                TextAnchor.MiddleCenter,
                Color.white);
            SetRect(instruction.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -210f), new Vector2(980f, 70f));

            Text status = CreateText(
                "Status",
                canvas.transform,
                "Cleared 0 / " + level.Screws.Length,
                34,
                TextAnchor.MiddleCenter,
                Color.white);
            SetRect(status.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 220f), new Vector2(700f, 70f));

            Button restartButton = CreateButton("Restart Button", canvas.transform, "RESTART");
            SetRect(restartButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 110f), new Vector2(330f, 90f));
            restartButton.onClick.AddListener(gameManager.RestartLevel);

            GameObject overlay = new GameObject("Result Overlay");
            overlay.transform.SetParent(canvas.transform, false);
            Image overlayImage = overlay.AddComponent<Image>();
            overlayImage.color = new Color(0.04f, 0.03f, 0.05f, 0.94f);
            RectTransform overlayRect = overlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Text resultTitle = CreateText(
                "Result Title",
                overlay.transform,
                "RESTORED!",
                62,
                TextAnchor.MiddleCenter,
                new Color(1f, 0.82f, 0.38f));
            SetRect(resultTitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 100f), new Vector2(900f, 260f));

            Button overlayRestart = CreateButton("Overlay Restart Button", overlay.transform, "PLAY AGAIN");
            SetRect(overlayRestart.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -120f), new Vector2(400f, 110f));
            overlayRestart.onClick.AddListener(gameManager.RestartLevel);

            return new PrototypeUiReferences(status, overlay, resultTitle);
        }

        public static Transform CreateRectangle(
            string objectName,
            Transform parent,
            Vector3 localPosition,
            Vector2 size,
            Color color,
            int sortingOrder)
        {
            GameObject rectangle = new GameObject(objectName);
            rectangle.transform.SetParent(parent, false);
            rectangle.transform.localPosition = localPosition;
            rectangle.transform.localScale = new Vector3(size.x, size.y, 1f);
            SpriteRenderer renderer = rectangle.AddComponent<SpriteRenderer>();
            renderer.sprite = GetSquareSprite();
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return rectangle.transform;
        }

        public static Transform CreateCircle(
            string objectName,
            Transform parent,
            Vector3 localPosition,
            float size,
            Color color,
            int sortingOrder)
        {
            GameObject circle = new GameObject(objectName);
            circle.transform.SetParent(parent, false);
            circle.transform.localPosition = localPosition;
            circle.transform.localScale = Vector3.one * size;
            SpriteRenderer renderer = circle.AddComponent<SpriteRenderer>();
            renderer.sprite = GetCircleSprite();
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return circle.transform;
        }

        private static void ConfigureBlockers(
            Screw[] screws,
            ScrewDefinition[] screwDefinitions)
        {
            for (int screwIndex = 0; screwIndex < screws.Length; screwIndex++)
            {
                List<Screw> blockers = new List<Screw>();

                foreach (int blockerIndex in screwDefinitions[screwIndex].BlockerIndexes)
                {
                    if (blockerIndex < 0 || blockerIndex >= screws.Length)
                    {
                        Debug.LogError(
                            "Screw " + screwIndex + " has invalid blocker index " + blockerIndex + ".");
                        continue;
                    }

                    blockers.Add(screws[blockerIndex]);
                }

                screws[screwIndex]
                    .GetComponent<ScrewDependency>()
                    .Configure(blockers.ToArray());
            }
        }

        private static Button CreateButton(string objectName, Transform parent, string label)
        {
            GameObject buttonObject = new GameObject(objectName);
            buttonObject.transform.SetParent(parent, false);
            Image image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.72f, 0.38f, 0.12f);
            Button button = buttonObject.AddComponent<Button>();

            Text text = CreateText("Label", buttonObject.transform, label, 30, TextAnchor.MiddleCenter, Color.white);
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            return button;
        }

        private static Text CreateText(
            string objectName,
            Transform parent,
            string value,
            int fontSize,
            TextAnchor alignment,
            Color color)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent, false);
            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            return text;
        }

        private static void SetRect(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 size)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        private static Sprite GetSquareSprite()
        {
            if (squareSprite == null)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.name = "Runtime Square";
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
                squareSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            }

            return squareSprite;
        }

        private static Sprite GetCircleSprite()
        {
            if (circleSprite == null)
            {
                const int size = 64;
                Texture2D texture = new Texture2D(size, size);
                texture.name = "Runtime Circle";
                Vector2 center = new Vector2((size - 1) / 2f, (size - 1) / 2f);
                float radius = size / 2f;

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), center);
                        texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
                    }
                }

                texture.Apply();
                circleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
            }

            return circleSprite;
        }

        private static Color ColorFor(ScrewColorId colorId)
        {
            switch (colorId)
            {
                case ScrewColorId.Red:
                    return new Color(0.90f, 0.20f, 0.16f);
                case ScrewColorId.Blue:
                    return new Color(0.16f, 0.48f, 0.92f);
                case ScrewColorId.Yellow:
                    return new Color(1f, 0.75f, 0.10f);
                default:
                    return Color.white;
            }
        }
    }

    public sealed class PrototypeUiReferences
    {
        public Text StatusText { get; }
        public GameObject ResultOverlay { get; }
        public Text ResultTitle { get; }

        public PrototypeUiReferences(
            Text statusText,
            GameObject resultOverlay,
            Text resultTitle)
        {
            StatusText = statusText;
            ResultOverlay = resultOverlay;
            ResultTitle = resultTitle;
        }
    }
}
