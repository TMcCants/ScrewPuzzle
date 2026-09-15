using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Builds the prototype toy-car level and connects it to the shared puzzle systems.
    /// </summary>
    public sealed class ToyCarLevelBootstrap : MonoBehaviour
    {
        private readonly Color backgroundColor = new Color(0.07f, 0.08f, 0.10f);
        private readonly Color carBodyColor = new Color(0.08f, 0.56f, 0.62f);
        private readonly Color carAccentColor = new Color(0.12f, 0.72f, 0.76f);
        private readonly Color windowColor = new Color(0.10f, 0.20f, 0.28f);
        private readonly Color tireColor = new Color(0.08f, 0.08f, 0.09f);

        private void Awake()
        {
            BuildLevel();
        }

        private void BuildLevel()
        {
            LevelDefinition level = CreateToyCarLevelDefinition();
            PrototypeLevelBuilder.BuildCamera(backgroundColor);
            PrototypeLevelBuilder.BuildBackground(backgroundColor);

            GameObject systems = new GameObject("Game Systems");
            GameManager gameManager = systems.AddComponent<GameManager>();
            TrayManager trayManager = new GameObject("Tray Manager").AddComponent<TrayManager>();
            trayManager.transform.SetParent(systems.transform);
            ToyCarRestoration restoration = new GameObject("Toy Car Restoration").AddComponent<ToyCarRestoration>();
            restoration.transform.SetParent(systems.transform);

            Transform carRoot = new GameObject("Toy Car").transform;
            carRoot.position = new Vector3(0f, 0.65f, 0f);

            PrototypeLevelBuilder.CreateRectangle(
                "Car Chassis",
                carRoot,
                new Vector3(0f, -0.15f, 0f),
                new Vector2(6.6f, 2.05f),
                carBodyColor,
                2);

            Transform roof = new GameObject("Car Roof").transform;
            roof.SetParent(carRoot, false);
            roof.localPosition = new Vector3(-0.65f, 1.25f, 0f);
            PrototypeLevelBuilder.CreateRectangle(
                "Roof Body",
                roof,
                Vector3.zero,
                new Vector2(3.65f, 1.45f),
                carAccentColor,
                3);

            PrototypeLevelBuilder.CreateRectangle(
                "Rear Window",
                roof,
                new Vector3(-0.8f, 0f, 0f),
                new Vector2(0.95f, 0.75f),
                windowColor,
                4);
            PrototypeLevelBuilder.CreateRectangle(
                "Front Window",
                roof,
                new Vector3(0.58f, 0f, 0f),
                new Vector2(1.15f, 0.75f),
                windowColor,
                4);

            Transform hood = PrototypeLevelBuilder.CreateRectangle(
                "Car Hood",
                carRoot,
                new Vector3(2.35f, 0.5f, 0f),
                new Vector2(1.9f, 0.72f),
                carAccentColor,
                3);

            Transform wheelAssembly = new GameObject("Wheel Assembly").transform;
            wheelAssembly.SetParent(carRoot, false);
            CreateWheel(wheelAssembly, "Rear Wheel", new Vector3(-2.25f, -1.2f, 0f));
            CreateWheel(wheelAssembly, "Front Wheel", new Vector3(2.25f, -1.2f, 0f));

            SpriteRenderer upperHeadlight = PrototypeLevelBuilder.CreateCircle(
                "Upper Headlight",
                carRoot,
                new Vector3(3.1f, 0.32f, 0f),
                0.34f,
                new Color(0.36f, 0.31f, 0.16f),
                5).GetComponent<SpriteRenderer>();
            SpriteRenderer lowerHeadlight = PrototypeLevelBuilder.CreateCircle(
                "Lower Headlight",
                carRoot,
                new Vector3(3.1f, -0.28f, 0f),
                0.28f,
                new Color(0.36f, 0.31f, 0.16f),
                5).GetComponent<SpriteRenderer>();

            PrototypeLevelBuilder.CreateRectangle(
                "Front Bumper",
                carRoot,
                new Vector3(3.45f, -0.72f, 0f),
                new Vector2(0.5f, 0.28f),
                new Color(0.45f, 0.47f, 0.50f),
                3);

            Transform[] traySlots = PrototypeLevelBuilder.BuildTray(level.TrayCapacity);
            PrototypeUiReferences ui = PrototypeLevelBuilder.BuildUi(gameManager, level);
            Screw[] screws = PrototypeLevelBuilder.BuildScrews(
                carRoot,
                trayManager,
                gameManager,
                level.Screws);

            ToyCarRestoration.CarPart[] carParts =
            {
                MakeCarPart("Roof", roof, new Screw[] { screws[0], screws[1], screws[2] }, new Vector3(0f, 0.2f, 0f), -3f),
                MakeCarPart("Hood", hood, new Screw[] { screws[3], screws[4], screws[5] }, new Vector3(0.2f, 0f, 0f), 3f),
                MakeCarPart("Wheels", wheelAssembly, new Screw[] { screws[6], screws[7], screws[8] }, new Vector3(0f, -0.18f, 0f), 1.5f)
            };

            restoration.Configure(
                carRoot,
                new SpriteRenderer[] { upperHeadlight, lowerHeadlight },
                carParts);
            trayManager.Configure(traySlots, level.MatchSize, gameManager);
            gameManager.Configure(
                trayManager,
                restoration,
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

        private LevelDefinition CreateToyCarLevelDefinition()
        {
            ScrewDefinition[] screws =
            {
                new ScrewDefinition(ScrewColorId.Red, new Vector3(-1.75f, 1.55f, 0f)),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(-0.6f, 1.65f, 0f)),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(0.55f, 1.55f, 0f)),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(-2.5f, 0.3f, 0f)),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(0f, 0.15f, 0f), 1),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(2.3f, 0.35f, 0f), 2),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(-2.45f, -0.9f, 0f)),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(0f, -0.85f, 0f), 4),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(2.45f, -0.9f, 0f), 3)
            };

            return new LevelDefinition(
                2,
                "Toy Car",
                "LevelSelect",
                "LEVEL SELECT",
                "Toy car restored!",
                "RESTORED!\nReady to roll.",
                5,
                3,
                screws);
        }

        private void CreateWheel(Transform parent, string wheelName, Vector3 position)
        {
            Transform wheel = PrototypeLevelBuilder.CreateCircle(
                wheelName,
                parent,
                position,
                1.35f,
                tireColor,
                3);
            PrototypeLevelBuilder.CreateCircle(
                "Wheel Hub",
                wheel,
                Vector3.zero,
                0.48f,
                new Color(0.48f, 0.50f, 0.53f),
                4);
        }

        private ToyCarRestoration.CarPart MakeCarPart(
            string partName,
            Transform visual,
            Screw[] holdingScrews,
            Vector3 releasedOffset,
            float releasedRotation)
        {
            return new ToyCarRestoration.CarPart
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
