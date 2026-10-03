using UnityEngine;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>Non-blocking first-play coaching shared by all workshop boards.</summary>
    public sealed class WorkshopTutorial : MonoBehaviour
    {
        public const string PreferenceKey = "ScrewPuzzle.Workshop3D.Tutorial.v1";
        public static bool HasFinished => PlayerPrefs.GetInt(PreferenceKey, 0) == 1;
        public int Step { get; private set; }
        public bool IsActive { get; private set; }
        private Radio3DPuzzle puzzle;
        private Radio3DInteraction interaction;
        private Radio3DScrew[] screws;
        private Text instructions;
        private GameObject skip;
        private string usualInstructions;
        private float rotation;

        public void Configure(RectTransform layout, Text label, Radio3DPuzzle board, Radio3DInteraction input, Radio3DScrew[] targets)
        {
            puzzle = board; interaction = input; screws = targets; instructions = label;
            usualInstructions = label.text;
            skip = new GameObject("Skip Tutorial", typeof(RectTransform), typeof(Image), typeof(Button));
            skip.transform.SetParent(layout, false);
            PrototypeLevelBuilder.SetRect(skip.GetComponent<RectTransform>(), Vector2.one * 0.5f, Vector2.one * 0.5f, new Vector2(445, 710), new Vector2(130, 70));
            skip.GetComponent<Image>().color = new Color(0.28f, 0.28f, 0.25f);
            skip.GetComponent<Button>().onClick.AddListener(Finish);
            var caption = PrototypeLevelBuilder.CreateText("Label", skip.transform, "SKIP", 26, TextAnchor.MiddleCenter, new Color(0.98f, 0.92f, 0.79f));
            caption.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(caption.rectTransform, Vector2.one * 0.5f, Vector2.one * 0.5f, Vector2.zero, new Vector2(130, 70));
            interaction.Rotated += OnRotated;
            puzzle.Restarted += RestartLesson;
            puzzle.Completed += Finish;
            IsActive = !HasFinished;
            Refresh();
        }

        private void OnRotated(float degrees)
        {
            if (!IsActive || Step != 0) return;
            rotation += degrees;
            if (rotation >= 20f) { Step = 1; Refresh(); }
        }
        private void Update()
        {
            if (!IsActive || puzzle.IsBusy) return;
            if (Step == 1 && puzzle.ClearedCount >= 3) { Step = 2; Refresh(); }
            if (Step != 2) return;
            foreach (var screw in screws)
                if (screw.IsInnerLayer && screw.IsRemoved) { Finish(); return; }
            Refresh();
        }
        private void RestartLesson()
        {
            if (!IsActive) return;
            rotation = 0; Step = 0; Refresh();
        }
        private void Finish()
        {
            if (!IsActive) return;
            PlayerPrefs.SetInt(PreferenceKey, 1);
            PlayerPrefs.Save();
            IsActive = false;
            Refresh();
        }
        private void Refresh()
        {
            skip.SetActive(IsActive);
            instructions.rectTransform.anchoredPosition = new Vector2(IsActive ? -90 : 0, 0);
            instructions.rectTransform.sizeDelta = new Vector2(IsActive ? 800 : 1000, 140);
            if (!IsActive) { instructions.text = usualInstructions; return; }
            if (Step == 0) instructions.text = "1 / 3  •  TURN THE TOY\nDrag left or right to find screws on every side.";
            else if (Step == 1) instructions.text = "2 / 3  •  MATCH A TRAY\nTap screws matching an open tray. Collect 3 to clear it.";
            else
            {
                bool revealed = false;
                foreach (var screw in screws) if (screw.IsInnerLayer && screw.IsAccessible) revealed = true;
                instructions.text = revealed ? "3 / 3  •  A HIDDEN LAYER!\nFind a matching tray and remove an inner screw." :
                    "3 / 3  •  LOOK INSIDE\nClear each plate's 3 screws to uncover the hidden layer.";
            }
        }
        private void OnDestroy()
        {
            if (interaction != null) interaction.Rotated -= OnRotated;
            if (puzzle != null) { puzzle.Restarted -= RestartLesson; puzzle.Completed -= Finish; }
        }
    }
}
