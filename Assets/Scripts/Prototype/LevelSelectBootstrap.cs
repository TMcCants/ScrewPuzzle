using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>
    /// Builds the temporary level-selection screen for the V0.2 alpha.
    /// </summary>
    public sealed class LevelSelectBootstrap : MonoBehaviour
    {
        private readonly Color backgroundColor = new Color(0.06f, 0.05f, 0.08f);

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

            Text title = PrototypeLevelBuilder.CreateText(
                "Title",
                canvas.transform,
                "SCREWPUZZLE",
                70,
                TextAnchor.MiddleCenter,
                new Color(0.89f, 0.66f, 0.29f));
            PrototypeLevelBuilder.StyleTitle(title);
            PrototypeLevelBuilder.SetRect(
                title.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -125f),
                new Vector2(900f, 100f));

            Text subtitle = PrototypeLevelBuilder.CreateText(
                "Subtitle",
                canvas.transform,
                "SELECT A LEVEL",
                36,
                TextAnchor.MiddleCenter,
                new Color(0.95f, 0.89f, 0.76f));
            PrototypeLevelBuilder.StyleAccentHeading(subtitle);
            PrototypeLevelBuilder.SetRect(
                subtitle.rectTransform,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -225f),
                new Vector2(900f, 70f));

            Button radioButton = PrototypeLevelBuilder.CreateButton(
                "Radio Level Button",
                canvas.transform,
                "LEVEL 1\nVINTAGE RADIO");
            PrototypeLevelBuilder.SetRect(
                radioButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 170f),
                new Vector2(720f, 170f));
            radioButton.onClick.AddListener(() => LoadLevel("Level01_Radio"));

            bool toyCarUnlocked = ProgressManager.IsLevelUnlocked(2);
            string toyCarLabel = toyCarUnlocked
                ? "LEVEL 2\nTOY CAR"
                : "LEVEL 2\nLOCKED";

            Button toyCarButton = PrototypeLevelBuilder.CreateButton(
                "Toy Car Level Button",
                canvas.transform,
                toyCarLabel);
            PrototypeLevelBuilder.SetRect(
                toyCarButton.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -60f),
                new Vector2(720f, 170f));
            toyCarButton.interactable = toyCarUnlocked;

            if (toyCarUnlocked)
            {
                toyCarButton.onClick.AddListener(() => LoadLevel("Level02_ToyCar"));
            }

            string progressMessage = toyCarUnlocked
                ? "Toy Car unlocked"
                : "Restore the radio to unlock Level 2";
            Text progress = PrototypeLevelBuilder.CreateText(
                "Progress Message",
                canvas.transform,
                progressMessage,
                30,
                TextAnchor.MiddleCenter,
                new Color(0.86f, 0.81f, 0.71f));
            PrototypeLevelBuilder.SetRect(
                progress.rectTransform,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -205f),
                new Vector2(900f, 80f));

            Text saveNote = PrototypeLevelBuilder.CreateText(
                "Save Note",
                canvas.transform,
                "Progress saves on this device.",
                24,
                TextAnchor.MiddleCenter,
                new Color(0.66f, 0.62f, 0.55f));
            PrototypeLevelBuilder.SetRect(
                saveNote.rectTransform,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 100f),
                new Vector2(900f, 60f));
        }

        private void LoadLevel(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
