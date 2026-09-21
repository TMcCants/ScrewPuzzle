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
                new Vector2(0f, 385f),
                new Vector2(860f, 390f));
            AddCardLabels(radioCard.transform, "LEVEL 1", "VINTAGE RADIO", "AVAILABLE", false);
            radioCard.onClick.AddListener(() => LoadLevel("Level01_Radio"));

            bool toyCarUnlocked = ProgressManager.IsLevelUnlocked(2);
            Button toyCarCard = CreateCardSurface("Toy Car Level Card", canvas.transform, toyCarUnlocked);
            PrototypeLevelBuilder.SetRect(
                toyCarCard.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -55f),
                new Vector2(860f, 390f));

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

            bool robotUnlocked = ProgressManager.IsLevelUnlocked(3);
            Button robotCard = CreateCardSurface("Toy Robot Level Card", canvas.transform, robotUnlocked);
            PrototypeLevelBuilder.SetRect(
                robotCard.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -495f),
                new Vector2(860f, 390f));

            AddCardLabels(
                robotCard.transform,
                "LEVEL 3",
                "TOY ROBOT",
                robotUnlocked ? "AVAILABLE" : "LOCKED",
                !robotUnlocked);

            if (robotUnlocked)
            {
                robotCard.onClick.AddListener(() => LoadLevel("Level03_ToyRobot"));
            }

            string progressMessage;

            if (robotUnlocked)
            {
                progressMessage = "All restorations are ready";
            }
            else if (toyCarUnlocked)
            {
                progressMessage = "Restore the Toy Car to unlock the Toy Robot";
            }
            else
            {
                progressMessage = "Restore the radio to unlock the Toy Car";
            }
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
                new Vector2(0f, -735f),
                new Vector2(940f, 58f));

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
                new Vector2(0f, 38f),
                new Vector2(900f, 54f));
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

        private void AddCardLabels(Transform parent, string levelNumber, string levelName, string status, bool locked)
        {
            Text level = CreateCardText(
                "Level Number", parent, levelNumber, 27, TextAnchor.MiddleLeft,
                warmAmber, new Vector2(-255f, 142f), new Vector2(260f, 46f));
            level.fontStyle = FontStyle.Bold;

            Text statusText = CreateCardText(
                "Level Status", parent, status, 24, TextAnchor.MiddleRight,
                locked ? mutedCream : warmAmber,
                new Vector2(255f, 142f), new Vector2(280f, 46f));
            statusText.fontStyle = FontStyle.Bold;

            Text name = CreateCardText(
                "Level Name", parent, levelName, 54, TextAnchor.MiddleCenter,
                locked ? mutedCream : warmCream,
                new Vector2(0f, 0f),
                new Vector2(720f, 96f));
            PrototypeLevelBuilder.StyleAccentHeading(name);
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
