using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>
    /// Builds the workshop level-selection screen and reflects local unlock progress.
    /// </summary>
    public sealed class LevelSelectBootstrap : MonoBehaviour
    {
        private readonly Color backgroundColor = new Color(0.06f, 0.05f, 0.08f);
        private readonly Color warmAmber = new Color(0.89f, 0.66f, 0.29f);
        private readonly Color warmCream = new Color(0.95f, 0.89f, 0.76f);
        private readonly Color mutedCream = new Color(0.76f, 0.71f, 0.63f);
        private Sprite levelCardSprite;

        private void Awake()
        {
            BuildLevelSelect();
        }

        private void BuildLevelSelect()
        {
            PrototypeLevelBuilder.BuildCamera(backgroundColor);
            PrototypeLevelBuilder.BuildBackground(backgroundColor);

            GameObject canvasObject = new GameObject("Level Select UI");
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

            BuildHeading(canvas.transform);

            Button radioCard = CreateCardSurface("Radio Level Card", canvas.transform, true);
            PrototypeLevelBuilder.SetRect(
                radioCard.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 285f),
                new Vector2(860f, 510f));
            CreateRadioThumbnail(radioCard.transform, Color.white);
            AddCardLabels(radioCard.transform, "LEVEL 1", "VINTAGE RADIO", "AVAILABLE", false);
            radioCard.onClick.AddListener(() => LoadLevel("Level01_Radio"));

            bool toyCarUnlocked = ProgressManager.IsLevelUnlocked(2);
            Button toyCarCard = CreateCardSurface("Toy Car Level Card", canvas.transform, toyCarUnlocked);
            PrototypeLevelBuilder.SetRect(
                toyCarCard.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -275f),
                new Vector2(860f, 510f));

            Color carTint = toyCarUnlocked
                ? Color.white
                : new Color(0.43f, 0.43f, 0.43f, 0.62f);
            CreateToyCarThumbnail(toyCarCard.transform, carTint);
            AddCardLabels(
                toyCarCard.transform,
                "LEVEL 2",
                "TOY CAR",
                toyCarUnlocked ? "AVAILABLE" : "LOCKED",
                !toyCarUnlocked);

            if (toyCarUnlocked)
            {
                toyCarCard.onClick.AddListener(() => LoadLevel("Level02_ToyCar"));
            }

            string progressMessage = toyCarUnlocked
                ? "Both restorations are ready"
                : "Restore the radio to unlock the Toy Car";
            Text progress = PrototypeLevelBuilder.CreateText(
                "Progress Message",
                canvas.transform,
                progressMessage,
                28,
                TextAnchor.MiddleCenter,
                warmCream);
            progress.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(
                progress.rectTransform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -615f),
                new Vector2(940f, 70f));

            Text saveNote = PrototypeLevelBuilder.CreateText(
                "Save Note",
                canvas.transform,
                "Progress saves on this device.",
                24,
                TextAnchor.MiddleCenter,
                new Color(0.66f, 0.62f, 0.55f));
            saveNote.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(
                saveNote.rectTransform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 82f),
                new Vector2(900f, 60f));
        }

        private void BuildHeading(Transform canvas)
        {
            Text title = PrototypeLevelBuilder.CreateText(
                "Title",
                canvas,
                "SCREWPUZZLE",
                70,
                TextAnchor.MiddleCenter,
                warmAmber);
            PrototypeLevelBuilder.StyleTitle(title);
            title.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(
                title.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -105f),
                new Vector2(900f, 100f));

            Text subtitle = PrototypeLevelBuilder.CreateText(
                "Subtitle",
                canvas,
                "SELECT A RESTORATION",
                34,
                TextAnchor.MiddleCenter,
                warmCream);
            PrototypeLevelBuilder.StyleAccentHeading(subtitle);
            subtitle.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(
                subtitle.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -190f),
                new Vector2(900f, 70f));
        }

        private Button CreateCardSurface(string objectName, Transform parent, bool interactable)
        {
            GameObject cardObject = new GameObject(objectName);
            cardObject.transform.SetParent(parent, false);
            Image image = cardObject.AddComponent<Image>();
            image.sprite = GetLevelCardSprite();
            image.type = Image.Type.Simple;
            image.color = Color.white;

            if (image.sprite == null)
            {
                image.color = new Color(0.26f, 0.16f, 0.10f, 0.98f);
            }

            Button button = cardObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;
            button.interactable = interactable;

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.95f, 0.80f);
            colors.pressedColor = new Color(0.72f, 0.66f, 0.56f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.43f, 0.43f, 0.43f, 0.82f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
            return button;
        }

        private void CreateRadioThumbnail(Transform parent, Color tint)
        {
            CreateResourceImage(
                "Radio Case", parent, "Art/Radio/Radio_Case",
                new Vector2(25f, 0f), new Vector2(500f, 274f), tint);
            CreateResourceImage(
                "Radio Faceplate", parent, "Art/Radio/Radio_Faceplate",
                new Vector2(25f, 10f), new Vector2(452f, 246f), tint);
            CreateResourceImage(
                "Radio Grille", parent, "Art/Radio/Radio_Speaker_Grille",
                new Vector2(-96f, 10f), new Vector2(173f, 180f), tint);
            CreateResourceImage(
                "Radio Display", parent, "Art/Radio/Radio_Display",
                new Vector2(132f, 43f), new Vector2(158f, 48f), tint);
            CreateResourceImage(
                "Left Radio Knob", parent, "Art/Radio/Radio_Knob",
                new Vector2(73f, -45f), new Vector2(58f, 58f), tint);
            CreateResourceImage(
                "Right Radio Knob", parent, "Art/Radio/Radio_Knob",
                new Vector2(187f, -45f), new Vector2(58f, 58f), tint);
        }

        private void CreateToyCarThumbnail(Transform parent, Color tint)
        {
            CreateResourceImage(
                "Toy Car Body", parent, "Art/ToyCar/ToyCar_Body",
                new Vector2(10f, -8f), new Vector2(570f, 272f), tint);
            CreateResourceImage(
                "Toy Car Roof", parent, "Art/ToyCar/ToyCar_Roof",
                new Vector2(-38f, 56f), new Vector2(293f, 146f), tint);
            CreateResourceImage(
                "Toy Car Hood", parent, "Art/ToyCar/ToyCar_Hood",
                new Vector2(159f, -8f), new Vector2(188f, 61f), tint);
            CreateResourceImage(
                "Rear Wheel", parent, "Art/ToyCar/ToyCar_Wheel",
                new Vector2(-147f, -64f), new Vector2(114f, 114f), tint);
            CreateResourceImage(
                "Front Wheel", parent, "Art/ToyCar/ToyCar_Wheel",
                new Vector2(215f, -64f), new Vector2(114f, 114f), tint);
            CreateResourceImage(
                "Upper Headlight", parent, "Art/ToyCar/ToyCar_Headlight",
                new Vector2(206f, 2f), new Vector2(31f, 31f), tint);
            CreateResourceImage(
                "Lower Headlight", parent, "Art/ToyCar/ToyCar_Headlight",
                new Vector2(238f, 2f), new Vector2(25f, 25f), tint);
        }

        private void AddCardLabels(Transform parent, string levelNumber, string levelName, string status, bool locked)
        {
            Text level = CreateCardText(
                "Level Number", parent, levelNumber, 27, TextAnchor.MiddleLeft,
                warmAmber, new Vector2(-255f, 170f), new Vector2(260f, 50f));
            level.fontStyle = FontStyle.Bold;

            Text statusText = CreateCardText(
                "Level Status", parent, status, 24, TextAnchor.MiddleRight,
                locked ? mutedCream : warmAmber,
                new Vector2(255f, 170f), new Vector2(280f, 50f));
            statusText.fontStyle = FontStyle.Bold;

            Text name = CreateCardText(
                "Level Name", parent, levelName, 34, TextAnchor.MiddleCenter,
                warmCream, new Vector2(0f, -170f), new Vector2(650f, 58f));
            PrototypeLevelBuilder.StyleAccentHeading(name);

            if (locked)
            {
                Text lockLabel = CreateCardText(
                    "Locked Label", parent, "LOCKED", 48, TextAnchor.MiddleCenter,
                    warmCream, new Vector2(0f, 5f), new Vector2(500f, 90f));
                lockLabel.fontStyle = FontStyle.Bold;
            }
        }

        private Text CreateCardText(
            string objectName,
            Transform parent,
            string value,
            int fontSize,
            TextAnchor alignment,
            Color color,
            Vector2 position,
            Vector2 size)
        {
            Text text = PrototypeLevelBuilder.CreateText(objectName, parent, value, fontSize, alignment, color);
            text.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(
                text.rectTransform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                position,
                size);
            return text;
        }

        private RawImage CreateResourceImage(
            string objectName,
            Transform parent,
            string resourcePath,
            Vector2 position,
            Vector2 size,
            Color tint)
        {
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);

            if (texture == null)
            {
                return null;
            }

            GameObject imageObject = new GameObject(objectName);
            imageObject.transform.SetParent(parent, false);
            RawImage image = imageObject.AddComponent<RawImage>();
            image.texture = texture;
            image.color = tint;
            image.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(
                image.rectTransform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                position,
                size);
            return image;
        }

        private Sprite GetLevelCardSprite()
        {
            if (levelCardSprite != null)
            {
                return levelCardSprite;
            }

            Texture2D texture = Resources.Load<Texture2D>("Art/UI/Level_Select_Card");

            if (texture != null)
            {
                levelCardSprite = Sprite.Create(
                    texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    texture.width);
            }

            return levelCardSprite;
        }

        private void LoadLevel(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
