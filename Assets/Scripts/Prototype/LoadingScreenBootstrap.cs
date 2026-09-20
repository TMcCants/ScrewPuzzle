using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>
    /// Builds the branded startup screen and opens the level-selection scene.
    /// </summary>
    public sealed class LoadingScreenBootstrap : MonoBehaviour
    {
        private const float MinimumDisplaySeconds = 3f;

        private readonly Color warmCream = new Color(0.95f, 0.89f, 0.76f);
        private readonly Color warmAmber = new Color(0.89f, 0.66f, 0.29f);
        private readonly Color[] dotColors =
        {
            new Color(0.90f, 0.23f, 0.20f),
            new Color(0.09f, 0.54f, 0.90f),
            new Color(0.96f, 0.73f, 0.09f)
        };

        private RectTransform iconRect;
        private Image[] loadingDots;

        private void Awake()
        {
            BuildScreen();
            StartCoroutine(OpenLevelSelect());
        }

        private void Update()
        {
            float time = Time.unscaledTime;

            if (iconRect != null)
            {
                float iconPulse = 1f + (Mathf.Sin(time * 2.2f) * 0.012f);
                iconRect.localScale = Vector3.one * iconPulse;
            }

            if (loadingDots == null)
            {
                return;
            }

            for (int index = 0; index < loadingDots.Length; index++)
            {
                Image dot = loadingDots[index];
                float wave = (Mathf.Sin((time * 4.2f) - (index * 1.3f)) + 1f) * 0.5f;
                float scale = Mathf.Lerp(0.78f, 1.18f, wave);
                dot.rectTransform.localScale = Vector3.one * scale;

                Color color = dotColors[index];
                color.a = Mathf.Lerp(0.45f, 1f, wave);
                dot.color = color;
            }
        }

        private void BuildScreen()
        {
            GameObject cameraObject = new GameObject("Loading Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.07f, 0.06f);

            GameObject canvasObject = new GameObject("Loading Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            CreateBackground(canvasObject.transform);
            CreateBranding(canvasObject.transform);
        }

        private void CreateBackground(Transform parent)
        {
            Image background = CreateImage("Workshop Background", parent, Color.white);
            Stretch(background.rectTransform);

            Texture2D backgroundTexture = Resources.Load<Texture2D>("Art/Workshop_Background");

            if (backgroundTexture != null)
            {
                background.sprite = CreateSprite(backgroundTexture);
            }
            else
            {
                background.color = new Color(0.08f, 0.07f, 0.06f);
            }

            Image veil = CreateImage("Warm Dark Veil", parent, new Color(0.05f, 0.025f, 0.015f, 0.48f));
            Stretch(veil.rectTransform);
        }

        private void CreateBranding(Transform parent)
        {
            Image icon = CreateImage("App Icon", parent, Color.white);
            iconRect = icon.rectTransform;
            SetRect(iconRect, new Vector2(0.5f, 0.5f), new Vector2(0f, 170f), new Vector2(560f, 560f));
            icon.preserveAspect = true;

            Texture2D iconTexture = Resources.Load<Texture2D>("Art/Branding/App_Icon");

            if (iconTexture != null)
            {
                icon.sprite = CreateSprite(iconTexture);
            }

            Text title = PrototypeLevelBuilder.CreateText(
                "Game Title",
                parent,
                "SCREWPUZZLE",
                76,
                TextAnchor.MiddleCenter,
                warmAmber);
            PrototypeLevelBuilder.StyleTitle(title);
            SetRect(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, -265f), new Vector2(920f, 120f));

            Text studio = PrototypeLevelBuilder.CreateText(
                "Studio Credit",
                parent,
                "A LUNYX INTERACTIVE STUDIOS GAME",
                25,
                TextAnchor.MiddleCenter,
                warmCream);
            studio.fontStyle = FontStyle.Bold;
            SetRect(studio.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, -355f), new Vector2(900f, 64f));

            Text loading = PrototypeLevelBuilder.CreateText(
                "Loading Label",
                parent,
                "RESTORING...",
                24,
                TextAnchor.MiddleCenter,
                new Color(0.76f, 0.71f, 0.63f));
            SetRect(loading.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, -500f), new Vector2(420f, 54f));

            Sprite dotSprite = CreateCircleSprite();
            loadingDots = new Image[dotColors.Length];

            for (int index = 0; index < loadingDots.Length; index++)
            {
                Image dot = CreateImage("Loading Dot " + (index + 1), parent, dotColors[index]);
                dot.sprite = dotSprite;
                SetRect(
                    dot.rectTransform,
                    new Vector2(0.5f, 0.5f),
                    new Vector2((index - 1) * 48f, -560f),
                    new Vector2(24f, 24f));
                loadingDots[index] = dot;
            }
        }

        private IEnumerator OpenLevelSelect()
        {
            yield return null;

            float elapsed = 0f;
            AsyncOperation load = SceneManager.LoadSceneAsync("LevelSelect");

            if (load == null)
            {
                yield break;
            }

            load.allowSceneActivation = false;

            while (elapsed < MinimumDisplaySeconds || load.progress < 0.9f)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            load.allowSceneActivation = true;
        }

        private static Image CreateImage(string objectName, Transform parent, Color color)
        {
            GameObject imageObject = new GameObject(objectName);
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static Sprite CreateSprite(Texture2D texture)
        {
            return Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
        }

        private static Sprite CreateCircleSprite()
        {
            const int size = 32;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "Loading Dot";
            float radius = (size - 2f) * 0.5f;
            Vector2 center = new Vector2((size - 1f) * 0.5f, (size - 1f) * 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01(radius - distance + 0.75f);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SetRect(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
    }
}
