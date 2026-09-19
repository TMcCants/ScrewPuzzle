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
        private static Sprite neutralScrewSprite;
        private static Sprite fiveSlotTraySprite;
        private static Sprite buttonPlateSprite;
        private static bool neutralScrewSpriteLoaded;
        private static bool fiveSlotTraySpriteLoaded;
        private static bool buttonPlateSpriteLoaded;
        private static Font bodyFont;
        private static Font displayFont;
        private static Font accentFont;

        private static readonly Color WarmCream = new Color(0.95f, 0.89f, 0.76f);
        private static readonly Color WarmAmber = new Color(0.89f, 0.66f, 0.29f);
        private static readonly Color MutedCream = new Color(0.74f, 0.69f, 0.60f);

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

            if (Object.FindFirstObjectByType<AudioListener>() == null)
            {
                camera.gameObject.AddComponent<AudioListener>();
            }

            return camera;
        }

        public static void BuildBackground(Color backgroundColor)
        {
            Texture2D workshopTexture = Resources.Load<Texture2D>("Art/Workshop_Background");

            if (workshopTexture != null)
            {
                const float backgroundHeight = 15f;
                float pixelsPerUnit = workshopTexture.height / backgroundHeight;
                Sprite workshopSprite = Sprite.Create(
                    workshopTexture,
                    new Rect(0f, 0f, workshopTexture.width, workshopTexture.height),
                    new Vector2(0.5f, 0.5f),
                    pixelsPerUnit);

                GameObject backgroundObject = new GameObject("Workshop Background");
                backgroundObject.transform.position = new Vector3(0f, 0f, 2f);
                SpriteRenderer backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
                backgroundRenderer.sprite = workshopSprite;
                backgroundRenderer.sortingOrder = -10;
                return;
            }

            // Retain a plain-color fallback so a missing art resource cannot break a level.
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
            Sprite traySprite = capacity == 5 ? GetFiveSlotTraySprite() : null;

            if (traySprite != null)
            {
                Transform trayVisual = CreateSpriteObject(
                    "Tray Background",
                    trayRoot,
                    Vector3.zero,
                    traySprite,
                    Color.white,
                    1);
                trayVisual.localScale = Vector3.one * 7.95f;
            }
            else
            {
                CreateRectangle(
                    "Tray Background",
                    trayRoot,
                    Vector3.zero,
                    new Vector2((capacity * 1.3f) + 0.8f, 1.25f),
                    new Color(0.16f, 0.16f, 0.19f),
                    1);
            }

            Transform[] slots = new Transform[capacity];
            float firstSlotX = -((capacity - 1) * 1.3f) / 2f;

            for (int index = 0; index < slots.Length; index++)
            {
                float x = firstSlotX + (index * 1.3f);
                if (traySprite != null)
                {
                    slots[index] = new GameObject("Tray Slot " + (index + 1)).transform;
                    slots[index].SetParent(trayRoot, false);
                    slots[index].localPosition = new Vector3(x, 0.14f, 0f);
                }
                else
                {
                    slots[index] = CreateCircle(
                        "Tray Slot " + (index + 1),
                        trayRoot,
                        new Vector3(x, 0f, 0f),
                        0.85f,
                        new Color(0.28f, 0.28f, 0.32f),
                        2);
                }
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
            Sprite screwSprite = GetNeutralScrewSprite();

            for (int index = 0; index < screwDefinitions.Length; index++)
            {
                ScrewDefinition definition = screwDefinitions[index];
                Transform screwVisual;

                if (screwSprite != null)
                {
                    screwVisual = CreateSpriteObject(
                        definition.ColorId + " Screw " + (index + 1),
                        objectRoot,
                        definition.Position,
                        screwSprite,
                        ColorFor(definition.ColorId),
                        10);
                    screwVisual.localScale = Vector3.one * 0.92f;
                }
                else
                {
                    screwVisual = CreateCircle(
                        definition.ColorId + " Screw " + (index + 1),
                        objectRoot,
                        definition.Position,
                        0.62f,
                        ColorFor(definition.ColorId),
                        10);
                }

                CircleCollider2D collider = screwVisual.gameObject.AddComponent<CircleCollider2D>();

                if (screwSprite != null)
                {
                    collider.radius = 0.34f;
                }

                ScrewDependency dependency = screwVisual.gameObject.AddComponent<ScrewDependency>();
                Screw screw = screwVisual.gameObject.AddComponent<Screw>();
                screw.Configure(definition.ColorId, dependency, trayManager, gameManager);
                screws[index] = screw;

                if (screwSprite == null)
                {
                    CreateRectangle(
                        "Screw Slot",
                        screwVisual,
                        Vector3.zero,
                        new Vector2(0.34f, 0.08f),
                        new Color(0.18f, 0.16f, 0.14f),
                        11);
                }
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
                60,
                TextAnchor.MiddleCenter,
                WarmAmber);
            StyleTitle(title);
            SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -75f), new Vector2(900f, 80f));

            Text levelName = CreateText(
                "Level Name",
                canvas.transform,
                level.LevelName.ToUpperInvariant(),
                38,
                TextAnchor.MiddleCenter,
                WarmCream);
            StyleAccentHeading(levelName);
            SetRect(levelName.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -145f), new Vector2(900f, 60f));

            Text instruction = CreateText(
                "Instructions",
                canvas.transform,
                "Tap open screws. Match " + level.MatchSize + " of one color before the tray fills.",
                28,
                TextAnchor.MiddleCenter,
                WarmCream);
            SetRect(instruction.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -210f), new Vector2(980f, 70f));

            Button soundButton = CreateButton("Sound Button", canvas.transform, "SOUND: ON");
            SetRect(
                soundButton.GetComponent<RectTransform>(),
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-125f, -290f),
                new Vector2(220f, 64f));
            StyleSecondaryButton(soundButton);
            Text soundLabel = soundButton.GetComponentInChildren<Text>();
            UpdateSoundButtonLabel(soundLabel);
            soundButton.onClick.AddListener(() =>
            {
                FeedbackAudio.ToggleSound();
                UpdateSoundButtonLabel(soundLabel);
            });

            Text status = CreateText(
                "Status",
                canvas.transform,
                "Cleared 0 / " + level.Screws.Length,
                32,
                TextAnchor.MiddleCenter,
                WarmCream);
            status.fontStyle = FontStyle.Bold;
            SetRect(status.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 220f), new Vector2(700f, 70f));

            Button restartButton = CreateButton("Restart Button", canvas.transform, "RESTART");
            SetRect(restartButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 110f), new Vector2(360f, 94f));
            restartButton.onClick.AddListener(gameManager.RestartLevel);

            GameObject overlay = new GameObject("Result Overlay");
            overlay.transform.SetParent(canvas.transform, false);
            Image overlayImage = overlay.AddComponent<Image>();
            overlayImage.color = new Color(0.055f, 0.04f, 0.03f, 0.95f);
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
                WarmAmber);
            StyleAccentHeading(resultTitle);
            SetRect(resultTitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 100f), new Vector2(900f, 260f));

            Button resultActionButton = CreateButton("Result Action Button", overlay.transform, "PLAY AGAIN");
            SetRect(resultActionButton.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -120f), new Vector2(420f, 112f));
            Text resultActionLabel = resultActionButton.GetComponentInChildren<Text>();

            return new PrototypeUiReferences(
                status,
                overlay,
                resultTitle,
                resultActionButton,
                resultActionLabel);
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

        public static Transform CreateResourceSprite(
            string objectName,
            Transform parent,
            Vector3 localPosition,
            string resourcePath,
            Vector2 size,
            Color color,
            int sortingOrder)
        {
            Sprite sprite = LoadResourceSprite(resourcePath);

            if (sprite == null)
            {
                return null;
            }

            Transform spriteObject = CreateSpriteObject(
                objectName,
                parent,
                localPosition,
                sprite,
                color,
                sortingOrder);
            Vector2 spriteSize = sprite.bounds.size;
            spriteObject.localScale = new Vector3(
                size.x / spriteSize.x,
                size.y / spriteSize.y,
                1f);
            return spriteObject;
        }

        private static Transform CreateSpriteObject(
            string objectName,
            Transform parent,
            Vector3 localPosition,
            Sprite sprite,
            Color color,
            int sortingOrder)
        {
            GameObject spriteObject = new GameObject(objectName);
            spriteObject.transform.SetParent(parent, false);
            spriteObject.transform.localPosition = localPosition;
            SpriteRenderer renderer = spriteObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return spriteObject.transform;
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

        public static Button CreateButton(string objectName, Transform parent, string label)
        {
            GameObject buttonObject = new GameObject(objectName);
            buttonObject.transform.SetParent(parent, false);
            Image image = buttonObject.AddComponent<Image>();
            Sprite plateSprite = GetButtonPlateSprite();

            if (plateSprite != null)
            {
                image.sprite = plateSprite;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
            }
            else
            {
                image.color = new Color(0.27f, 0.20f, 0.15f);
            }

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.96f, 0.84f);
            colors.pressedColor = new Color(0.72f, 0.68f, 0.62f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.46f, 0.44f, 0.42f, 0.78f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;

            Text text = CreateText("Label", buttonObject.transform, label, 30, TextAnchor.MiddleCenter, WarmCream);
            text.font = GetAccentFont();
            text.fontStyle = FontStyle.Normal;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 20;
            text.resizeTextMaxSize = 34;
            AddTextShadow(text, new Vector2(2f, -2f));
            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(18f, 8f);
            textRect.offsetMax = new Vector2(-18f, -8f);
            return button;
        }

        public static Text CreateText(
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
            text.font = GetBodyFont();
            text.text = value;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.lineSpacing = 1f;
            return text;
        }

        public static void StyleTitle(Text text)
        {
            if (text == null)
            {
                return;
            }

            text.font = GetDisplayFont();
            text.fontStyle = FontStyle.Normal;
            text.color = WarmAmber;
            AddTextShadow(text, new Vector2(3f, -3f));
        }

        public static void StyleAccentHeading(Text text)
        {
            if (text == null)
            {
                return;
            }

            text.font = GetAccentFont();
            text.fontStyle = FontStyle.Normal;
            AddTextShadow(text, new Vector2(2f, -2f));
        }

        public static void StyleSecondaryButton(Button button)
        {
            if (button == null)
            {
                return;
            }

            Image image = button.GetComponent<Image>();

            if (image != null)
            {
                image.color = new Color(0.72f, 0.72f, 0.72f, 0.90f);
            }

            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.72f, 0.72f, 0.72f, 0.90f);
            colors.highlightedColor = new Color(0.82f, 0.80f, 0.74f, 0.95f);
            colors.pressedColor = new Color(0.54f, 0.52f, 0.48f, 0.92f);
            colors.selectedColor = colors.normalColor;
            colors.disabledColor = new Color(0.38f, 0.37f, 0.35f, 0.70f);
            button.colors = colors;

            Text label = button.GetComponentInChildren<Text>();

            if (label != null)
            {
                label.font = GetBodyFont();
                label.fontStyle = FontStyle.Bold;
                label.fontSize = 22;
                label.resizeTextMaxSize = 22;
                label.color = MutedCream;
            }
        }

        private static void AddTextShadow(Text text, Vector2 distance)
        {
            if (text.GetComponent<Shadow>() != null)
            {
                return;
            }

            Shadow shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0.05f, 0.025f, 0.015f, 0.88f);
            shadow.effectDistance = distance;
            shadow.useGraphicAlpha = true;
        }

        private static void UpdateSoundButtonLabel(Text label)
        {
            if (label != null)
            {
                label.text = FeedbackAudio.IsSoundEnabled ? "SOUND: ON" : "SOUND: OFF";
            }
        }

        public static void SetRect(
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

        private static Sprite GetNeutralScrewSprite()
        {
            if (!neutralScrewSpriteLoaded)
            {
                neutralScrewSpriteLoaded = true;
                neutralScrewSprite = LoadResourceSprite("Art/Hardware/Screw_Head_Neutral");
            }

            return neutralScrewSprite;
        }

        private static Sprite GetFiveSlotTraySprite()
        {
            if (!fiveSlotTraySpriteLoaded)
            {
                fiveSlotTraySpriteLoaded = true;
                fiveSlotTraySprite = LoadResourceSprite("Art/Hardware/Five_Slot_Tray");
            }

            return fiveSlotTraySprite;
        }

        private static Sprite GetButtonPlateSprite()
        {
            if (!buttonPlateSpriteLoaded)
            {
                buttonPlateSpriteLoaded = true;
                Texture2D texture = Resources.Load<Texture2D>("Art/UI/Button_Plate");

                if (texture != null)
                {
                    buttonPlateSprite = Sprite.Create(
                        texture,
                        new Rect(0f, 0f, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        100f,
                        0,
                        SpriteMeshType.FullRect,
                        new Vector4(82f, 82f, 82f, 82f));
                }
            }

            return buttonPlateSprite;
        }

        private static Font GetBodyFont()
        {
            if (bodyFont == null)
            {
                bodyFont = Resources.Load<Font>("Fonts/DejaVuSans");

                if (bodyFont == null)
                {
                    bodyFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
            }

            return bodyFont;
        }

        private static Font GetDisplayFont()
        {
            if (displayFont == null)
            {
                displayFont = Resources.Load<Font>("Fonts/DejaVuSans-Bold");

                if (displayFont == null)
                {
                    displayFont = GetBodyFont();
                }
            }

            return displayFont;
        }

        private static Font GetAccentFont()
        {
            if (accentFont == null)
            {
                accentFont = Resources.Load<Font>("Fonts/DejaVuSerif-Bold");

                if (accentFont == null)
                {
                    accentFont = GetDisplayFont();
                }
            }

            return accentFont;
        }

        private static Sprite LoadResourceSprite(string resourcePath)
        {
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);

            if (texture == null)
            {
                return null;
            }

            return Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                texture.width);
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
        public Button ResultActionButton { get; }
        public Text ResultActionLabel { get; }

        public PrototypeUiReferences(
            Text statusText,
            GameObject resultOverlay,
            Text resultTitle,
            Button resultActionButton,
            Text resultActionLabel)
        {
            StatusText = statusText;
            ResultOverlay = resultOverlay;
            ResultTitle = resultTitle;
            ResultActionButton = resultActionButton;
            ResultActionLabel = resultActionLabel;
        }
    }
}
