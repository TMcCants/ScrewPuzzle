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

            Transform chassis = PrototypeLevelBuilder.CreateResourceSprite(
                "Car Chassis",
                carRoot,
                new Vector3(0f, -0.05f, 0f),
                "Art/ToyCar/ToyCar_Body_OpenDoor",
                new Vector2(7.1f, 3.39f),
                Color.white,
                2);

            bool hasProductionChassis = chassis != null;

            if (!hasProductionChassis)
            {
                chassis = PrototypeLevelBuilder.CreateRectangle(
                    "Car Chassis",
                    carRoot,
                    new Vector3(0f, -0.15f, 0f),
                    new Vector2(6.6f, 2.05f),
                    carBodyColor,
                    2);
            }

            Transform roof = new GameObject("Car Roof").transform;
            roof.SetParent(carRoot, false);
            roof.localPosition = new Vector3(-0.6f, 0.8f, 0f);
            Transform roofVisual = PrototypeLevelBuilder.CreateResourceSprite(
                "Roof Assembly",
                roof,
                Vector3.zero,
                "Art/ToyCar/ToyCar_Roof",
                new Vector2(3.65f, 1.82f),
                Color.white,
                3);

            if (roofVisual == null)
            {
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
            }

            Transform door = PrototypeLevelBuilder.CreateResourceSprite(
                "Car Door",
                carRoot,
                new Vector3(-0.1f, -0.15f, 0f),
                "Art/ToyCar/ToyCar_Door",
                new Vector2(1.85f, 0.9f),
                Color.white,
                3);

            if (door == null)
            {
                door = PrototypeLevelBuilder.CreateRectangle(
                    "Car Door",
                    carRoot,
                    new Vector3(-0.1f, -0.15f, 0f),
                    new Vector2(1.85f, 0.9f),
                    carBodyColor,
                    3);
            }

            Transform hood = PrototypeLevelBuilder.CreateResourceSprite(
                "Car Hood",
                carRoot,
                new Vector3(1.45f, 0.25f, 0f),
                "Art/ToyCar/ToyCar_Hood",
                new Vector2(3.15f, 0.76f),
                Color.white,
                3);

            if (hood == null)
            {
                hood = PrototypeLevelBuilder.CreateRectangle(
                    "Car Hood",
                    carRoot,
                    new Vector3(2.35f, 0.5f, 0f),
                    new Vector2(1.9f, 0.72f),
                    carAccentColor,
                    3);
            }

            Transform wheelAssembly = new GameObject("Wheel Assembly").transform;
            wheelAssembly.SetParent(carRoot, false);
            CreateWheel(wheelAssembly, "Rear Wheel", new Vector3(-1.95f, -0.7f, 0f));
            CreateWheel(wheelAssembly, "Front Wheel", new Vector3(2.55f, -0.7f, 0f));

            SpriteRenderer upperHeadlight = CreateHeadlight(
                "Upper Headlight",
                carRoot,
                new Vector3(2.2f, 0.33f, 0f),
                0.38f);
            SpriteRenderer lowerHeadlight = CreateHeadlight(
                "Lower Headlight",
                carRoot,
                new Vector3(2.6f, 0.33f, 0f),
                0.31f);

            if (!hasProductionChassis)
            {
                PrototypeLevelBuilder.CreateRectangle(
                    "Front Bumper",
                    carRoot,
                    new Vector3(3.45f, -0.72f, 0f),
                    new Vector2(0.5f, 0.28f),
                    new Color(0.45f, 0.47f, 0.50f),
                    3);
            }

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
                MakeCarPart("Door", door, new Screw[] { screws[3], screws[4], screws[5] }, new Vector3(0.08f, -0.55f, 0f), -8f),
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
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(-0.75f, 0.15f, 0f)),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(-0.05f, 0.05f, 0f), 1),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(0.65f, -0.05f, 0f), 2),
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
            Transform wheel = PrototypeLevelBuilder.CreateResourceSprite(
                wheelName,
                parent,
                position,
                "Art/ToyCar/ToyCar_Wheel",
                new Vector2(1.42f, 1.42f),
                Color.white,
                3);

            if (wheel == null)
            {
                wheel = PrototypeLevelBuilder.CreateCircle(
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
        }

        private SpriteRenderer CreateHeadlight(
            string headlightName,
            Transform parent,
            Vector3 position,
            float size)
        {
            Color unlitColor = new Color(0.36f, 0.31f, 0.16f);
            Transform headlight = PrototypeLevelBuilder.CreateResourceSprite(
                headlightName,
                parent,
                position,
                "Art/ToyCar/ToyCar_Headlight",
                new Vector2(size, size),
                unlitColor,
                5);

            if (headlight == null)
            {
                headlight = PrototypeLevelBuilder.CreateCircle(
                    headlightName,
                    parent,
                    position,
                    size,
                    unlitColor,
                    5);
            }

            return headlight.GetComponent<SpriteRenderer>();
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
