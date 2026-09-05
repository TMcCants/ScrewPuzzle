using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>
    /// Builds the V0.1 radio level from simple shapes when the scene starts.
    /// This keeps the repository immediately playable without requiring art assets.
    /// Replace this builder with hand-authored prefabs later; gameplay scripts do not depend on it.
    /// </summary>
    public sealed class RadioLevelBootstrap : MonoBehaviour
    {
        private static Sprite squareSprite;
        private static Sprite circleSprite;

        private readonly Color backgroundColor = new Color(0.08f, 0.07f, 0.09f);
        private readonly Color radioBrown = new Color(0.35f, 0.18f, 0.10f);
        private readonly Color radioTan = new Color(0.75f, 0.52f, 0.28f);
        private readonly Color darkMetal = new Color(0.12f, 0.13f, 0.15f);

        private void Awake()
        {
            BuildLevel();
        }

        private void BuildLevel()
        {
            Camera mainCamera = BuildCamera();
            BuildBackground(mainCamera);

            GameObject systems = new GameObject("Game Systems");
            GameManager gameManager = systems.AddComponent<GameManager>();
            TrayManager trayManager = new GameObject("Tray Manager").AddComponent<TrayManager>();
            trayManager.transform.SetParent(systems.transform);
            RadioRestoration radioRestoration = new GameObject("Radio Restoration").AddComponent<RadioRestoration>();
            radioRestoration.transform.SetParent(systems.transform);

            Transform radioRoot = new GameObject("Vintage Radio").transform;
            radioRoot.position = new Vector3(0f, 0.65f, 0f);

            Transform backPanel = CreateRectangle(
                "Radio Case",
                radioRoot,
                Vector3.zero,
                new Vector2(6.8f, 4.3f),
                radioBrown,
                1);

            Transform faceplate = CreateRectangle(
                "Faceplate",
                radioRoot,
                new Vector3(0f, 0.15f, 0f),
                new Vector2(6.15f, 3.35f),
                radioTan,
                2);

            Transform speakerGrille = new GameObject("Speaker Grille").transform;
            speakerGrille.SetParent(radioRoot, false);
            speakerGrille.localPosition = new Vector3(-1.65f, 0.15f, 0f);
            CreateRectangle(
                "Grille Background",
                speakerGrille,
                Vector3.zero,
                new Vector2(2.35f, 2.45f),
                darkMetal,
                3);

            CreateSpeakerLines(speakerGrille);

            SpriteRenderer displayGlow = CreateRectangle(
                "Radio Display",
                radioRoot,
                new Vector3(1.45f, 0.6f, 0f),
                new Vector2(2.15f, 0.65f),
                new Color(0.26f, 0.20f, 0.12f),
                4).GetComponent<SpriteRenderer>();

            CreateCircle("Left Dial", radioRoot, new Vector3(0.65f, -0.6f, 0f), 0.72f, darkMetal, 4);
            CreateCircle("Right Dial", radioRoot, new Vector3(2.2f, -0.6f, 0f), 0.72f, darkMetal, 4);

            Transform[] traySlots = BuildTray();
            UiReferences ui = BuildUi(gameManager);

            Screw[] screws = BuildScrews(radioRoot, trayManager, gameManager);
            ConfigureBlockers(screws);

            RadioRestoration.RadioPart[] radioParts = new RadioRestoration.RadioPart[]
            {
                MakeRadioPart("Faceplate", faceplate, new Screw[] { screws[0], screws[1], screws[2] }, new Vector3(0f, -0.22f, 0f), -2f),
                MakeRadioPart("Speaker grille", speakerGrille, new Screw[] { screws[3], screws[4], screws[5] }, new Vector3(-0.18f, 0f, 0f), 3f),
                MakeRadioPart("Radio case", backPanel, new Screw[] { screws[6], screws[7], screws[8] }, new Vector3(0f, 0.14f, 0f), 1.5f)
            };

            radioRestoration.Configure(radioRoot, displayGlow, radioParts);
            trayManager.Configure(traySlots, 3, gameManager);
            gameManager.Configure(
                trayManager,
                radioRestoration,
                ui.statusText,
                ui.resultOverlay,
                ui.resultTitle,
                screws.Length);
        }

        private Camera BuildCamera()
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

        private void BuildBackground(Camera camera)
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

        private Transform[] BuildTray()
        {
            Transform trayRoot = new GameObject("Holding Tray").transform;
            trayRoot.position = new Vector3(0f, -4.2f, 0f);
            CreateRectangle(
                "Tray Background",
                trayRoot,
                Vector3.zero,
                new Vector2(7.3f, 1.25f),
                new Color(0.16f, 0.16f, 0.19f),
                1);

            Transform[] slots = new Transform[5];

            for (int index = 0; index < slots.Length; index++)
            {
                float x = -2.6f + (index * 1.3f);
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

        private Screw[] BuildScrews(
            Transform radioRoot,
            TrayManager trayManager,
            GameManager gameManager)
        {
            Vector3[] positions =
            {
                new Vector3(-2.75f, 1.55f, 0f),
                new Vector3(0f, 1.55f, 0f),
                new Vector3(2.75f, 1.55f, 0f),
                new Vector3(-2.75f, 0.1f, 0f),
                new Vector3(0f, 0.1f, 0f),
                new Vector3(2.75f, 0.1f, 0f),
                new Vector3(-2.75f, -1.45f, 0f),
                new Vector3(0f, -1.45f, 0f),
                new Vector3(2.75f, -1.45f, 0f)
            };

            ScrewColorId[] colors =
            {
                ScrewColorId.Red,
                ScrewColorId.Red,
                ScrewColorId.Red,
                ScrewColorId.Blue,
                ScrewColorId.Blue,
                ScrewColorId.Blue,
                ScrewColorId.Yellow,
                ScrewColorId.Yellow,
                ScrewColorId.Yellow
            };

            Screw[] screws = new Screw[positions.Length];

            for (int index = 0; index < positions.Length; index++)
            {
                Transform screwVisual = CreateCircle(
                    colors[index] + " Screw " + (index + 1),
                    radioRoot,
                    positions[index],
                    0.62f,
                    ColorFor(colors[index]),
                    10);

                screwVisual.gameObject.AddComponent<CircleCollider2D>();
                ScrewDependency dependency = screwVisual.gameObject.AddComponent<ScrewDependency>();
                Screw screw = screwVisual.gameObject.AddComponent<Screw>();
                screw.Configure(colors[index], dependency, trayManager, gameManager);
                screws[index] = screw;

                CreateRectangle(
                    "Screw Slot",
                    screwVisual,
                    Vector3.zero,
                    new Vector2(0.34f, 0.08f),
                    new Color(0.18f, 0.16f, 0.14f),
                    11);
            }

            return screws;
        }

        private void ConfigureBlockers(Screw[] screws)
        {
            // Five screws begin open, so a careless mixed-color sequence can fill the tray.
            // A safe route is red set, blue set, then yellow set.
            SetBlockers(screws[0]);
            SetBlockers(screws[1]);
            SetBlockers(screws[2]);
            SetBlockers(screws[3]);
            SetBlockers(screws[4], screws[1]);
            SetBlockers(screws[5], screws[2]);
            SetBlockers(screws[6]);
            SetBlockers(screws[7], screws[4]);
            SetBlockers(screws[8], screws[3]);
        }

        private void SetBlockers(Screw screw, params Screw[] blockers)
        {
            screw.GetComponent<ScrewDependency>().Configure(blockers);
        }

        private RadioRestoration.RadioPart MakeRadioPart(
            string partName,
            Transform visual,
            Screw[] holdingScrews,
            Vector3 releasedOffset,
            float releasedRotation)
        {
            return new RadioRestoration.RadioPart
            {
                name = partName,
                visual = visual,
                holdingScrews = holdingScrews,
                releasedOffset = releasedOffset,
                releasedRotation = releasedRotation
            };
        }

        private void CreateSpeakerLines(Transform grille)
        {
            for (int index = 0; index < 7; index++)
            {
                float y = -0.85f + (index * 0.28f);
                CreateRectangle(
                    "Speaker Line " + index,
                    grille,
                    new Vector3(0f, y, 0f),
                    new Vector2(1.85f, 0.06f),
                    new Color(0.32f, 0.30f, 0.27f),
                    4);
            }
        }

        private UiReferences BuildUi(GameManager gameManager)
        {
            GameObject canvasObject = new GameObject("UI");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;
            canvasObject.AddComponent<GraphicRaycaster>();

            if (FindFirstObjectByType<EventSystem>() == null)
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
            SetRect(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -85f), new Vector2(900f, 90f));

            Text instruction = CreateText(
                "Instructions",
                canvas.transform,
                "Tap open screws. Match 3 colors before the tray fills.",
                30,
                TextAnchor.MiddleCenter,
                Color.white);
            SetRect(instruction.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -165f), new Vector2(980f, 80f));

            Text status = CreateText(
                "Status",
                canvas.transform,
                "Cleared 0 / 9",
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

            return new UiReferences
            {
                statusText = status,
                resultOverlay = overlay,
                resultTitle = resultTitle
            };
        }

        private Button CreateButton(string objectName, Transform parent, string label)
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

        private Text CreateText(
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

        private void SetRect(
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

        private Transform CreateRectangle(
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

        private Transform CreateCircle(
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

        private Sprite GetSquareSprite()
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

        private Sprite GetCircleSprite()
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

        private Color ColorFor(ScrewColorId colorId)
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

        private sealed class UiReferences
        {
            public Text statusText;
            public GameObject resultOverlay;
            public Text resultTitle;
        }
    }
}
