using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Builds the prototype radio and connects it to the shared puzzle systems.
    /// Radio-specific shapes and restoration parts stay here.
    /// </summary>
    public sealed class RadioLevelBootstrap : MonoBehaviour
    {
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
            LevelDefinition level = CreateRadioLevelDefinition();
            PrototypeLevelBuilder.BuildCamera(backgroundColor);
            PrototypeLevelBuilder.BuildBackground(backgroundColor);

            GameObject systems = new GameObject("Game Systems");
            GameManager gameManager = systems.AddComponent<GameManager>();
            TrayManager trayManager = new GameObject("Tray Manager").AddComponent<TrayManager>();
            trayManager.transform.SetParent(systems.transform);
            RadioRestoration radioRestoration = new GameObject("Radio Restoration").AddComponent<RadioRestoration>();
            radioRestoration.transform.SetParent(systems.transform);

            Transform radioRoot = new GameObject("Vintage Radio").transform;
            radioRoot.position = new Vector3(0f, 0.65f, 0f);

            Transform backPanel = PrototypeLevelBuilder.CreateResourceSprite(
                "Radio Case",
                radioRoot,
                Vector3.zero,
                "Art/Radio/Radio_Case",
                new Vector2(6.8f, 4.3f),
                Color.white,
                1);

            if (backPanel == null)
            {
                backPanel = PrototypeLevelBuilder.CreateRectangle(
                    "Radio Case",
                    radioRoot,
                    Vector3.zero,
                    new Vector2(6.8f, 4.3f),
                    radioBrown,
                    1);
            }

            Transform faceplate = new GameObject("Faceplate").transform;
            faceplate.SetParent(radioRoot, false);
            faceplate.localPosition = new Vector3(0f, 0.15f, 0f);
            Transform faceplateVisual = PrototypeLevelBuilder.CreateResourceSprite(
                "Faceplate Visual",
                faceplate,
                Vector3.zero,
                "Art/Radio/Radio_Faceplate",
                new Vector2(6.15f, 3.35f),
                Color.white,
                2);

            if (faceplateVisual == null)
            {
                PrototypeLevelBuilder.CreateRectangle(
                    "Faceplate Visual",
                    faceplate,
                    Vector3.zero,
                    new Vector2(6.15f, 3.35f),
                    radioTan,
                    2);
            }

            Transform speakerGrille = PrototypeLevelBuilder.CreateResourceSprite(
                "Speaker Grille",
                radioRoot,
                new Vector3(-1.65f, 0.15f, 0f),
                "Art/Radio/Radio_Speaker_Grille",
                new Vector2(2.35f, 2.45f),
                Color.white,
                3);

            if (speakerGrille == null)
            {
                speakerGrille = new GameObject("Speaker Grille").transform;
                speakerGrille.SetParent(radioRoot, false);
                speakerGrille.localPosition = new Vector3(-1.65f, 0.15f, 0f);
                PrototypeLevelBuilder.CreateRectangle(
                    "Grille Background",
                    speakerGrille,
                    Vector3.zero,
                    new Vector2(2.35f, 2.45f),
                    darkMetal,
                    3);
                CreateSpeakerLines(speakerGrille);
            }

            Transform display = PrototypeLevelBuilder.CreateResourceSprite(
                "Radio Display",
                faceplate,
                new Vector3(1.45f, 0.45f, 0f),
                "Art/Radio/Radio_Display",
                new Vector2(2.15f, 0.65f),
                Color.white,
                4);

            if (display == null)
            {
                PrototypeLevelBuilder.CreateRectangle(
                    "Radio Display",
                    faceplate,
                    new Vector3(1.45f, 0.45f, 0f),
                    new Vector2(2.15f, 0.65f),
                    new Color(0.15f, 0.12f, 0.09f),
                    4);
            }

            SpriteRenderer displayGlow = PrototypeLevelBuilder.CreateRectangle(
                "Radio Display Glow",
                faceplate,
                new Vector3(1.45f, 0.45f, 0f),
                new Vector2(0.07f, 0.46f),
                new Color(0.26f, 0.20f, 0.12f),
                5).GetComponent<SpriteRenderer>();

            CreateDial(faceplate, "Left Dial", new Vector3(0.65f, -0.75f, 0f));
            CreateDial(faceplate, "Right Dial", new Vector3(2.2f, -0.75f, 0f));

            Transform[] traySlots = PrototypeLevelBuilder.BuildTray(level.TrayCapacity);
            PrototypeUiReferences ui = PrototypeLevelBuilder.BuildUi(gameManager, level);
            Screw[] screws = PrototypeLevelBuilder.BuildScrews(
                radioRoot,
                trayManager,
                gameManager,
                level.Screws);

            RadioRestoration.RadioPart[] radioParts =
            {
                MakeRadioPart("Faceplate", faceplate, new Screw[] { screws[0], screws[1], screws[2] }, new Vector3(0f, -0.22f, 0f), -2f),
                MakeRadioPart("Speaker grille", speakerGrille, new Screw[] { screws[3], screws[4], screws[5] }, new Vector3(-0.18f, 0f, 0f), 3f),
                MakeRadioPart("Radio case", backPanel, new Screw[] { screws[6], screws[7], screws[8] }, new Vector3(0f, 0.14f, 0f), 1.5f)
            };

            radioRestoration.Configure(radioRoot, displayGlow, radioParts);
            trayManager.Configure(traySlots, level.MatchSize, gameManager);
            gameManager.Configure(
                trayManager,
                radioRestoration,
                ui.StatusText,
                ui.ResultOverlay,
                ui.ResultTitle,
                ui.ResultActionButton,
                ui.ResultActionLabel,
                screws.Length,
                level.LevelNumber,
                level.NextSceneName,
                level.WinButtonLabel,
                level.RestoredStatusMessage,
                level.RestoredResultMessage);
        }

        private LevelDefinition CreateRadioLevelDefinition()
        {
            // Blocker numbers refer to positions in this same screw array.
            ScrewDefinition[] screws =
            {
                new ScrewDefinition(ScrewColorId.Red, new Vector3(-2.75f, 1.55f, 0f)),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(0f, 1.55f, 0f)),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(2.75f, 1.55f, 0f)),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(-2.75f, 0.1f, 0f)),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(0f, 0.1f, 0f), 1),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(2.75f, 0.1f, 0f), 2),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(-2.75f, -1.45f, 0f)),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(0f, -1.45f, 0f), 4),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(2.75f, -1.45f, 0f), 3)
            };

            return new LevelDefinition(
                1,
                "Vintage Radio",
                "Level02_ToyCar",
                "NEXT LEVEL",
                "Radio restored!",
                "RESTORED!\nThe radio is alive.",
                5,
                3,
                screws);
        }

        private void CreateSpeakerLines(Transform grille)
        {
            for (int index = 0; index < 7; index++)
            {
                float y = -0.85f + (index * 0.28f);
                PrototypeLevelBuilder.CreateRectangle(
                    "Speaker Line " + index,
                    grille,
                    new Vector3(0f, y, 0f),
                    new Vector2(1.85f, 0.06f),
                    new Color(0.32f, 0.30f, 0.27f),
                    4);
            }
        }

        private void CreateDial(Transform faceplate, string dialName, Vector3 localPosition)
        {
            Transform dial = PrototypeLevelBuilder.CreateResourceSprite(
                dialName,
                faceplate,
                localPosition,
                "Art/Radio/Radio_Knob",
                new Vector2(0.78f, 0.78f),
                Color.white,
                4);

            if (dial == null)
            {
                PrototypeLevelBuilder.CreateCircle(
                    dialName,
                    faceplate,
                    localPosition,
                    0.72f,
                    darkMetal,
                    4);
            }
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
    }
}
