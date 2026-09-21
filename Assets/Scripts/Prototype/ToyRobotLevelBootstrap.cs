using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Builds the V0.6 generated-shape toy-robot prototype and connects it to shared gameplay.
    /// Final production art can replace these shapes without changing the puzzle definition.
    /// </summary>
    public sealed class ToyRobotLevelBootstrap : MonoBehaviour
    {
        private readonly Color backgroundColor = new Color(0.07f, 0.06f, 0.05f);
        private readonly Color bodyColor = new Color(0.30f, 0.38f, 0.36f);
        private readonly Color panelColor = new Color(0.42f, 0.48f, 0.43f);
        private readonly Color brassColor = new Color(0.70f, 0.49f, 0.23f);
        private readonly Color jointColor = new Color(0.14f, 0.13f, 0.12f);
        private readonly Color unlitColor = new Color(0.22f, 0.18f, 0.10f);

        private void Awake()
        {
            BuildLevel();
        }

        private void BuildLevel()
        {
            LevelDefinition level = CreateToyRobotLevelDefinition();
            PrototypeLevelBuilder.BuildCamera(backgroundColor);
            PrototypeLevelBuilder.BuildBackground(backgroundColor);

            GameObject systems = new GameObject("Game Systems");
            GameManager gameManager = systems.AddComponent<GameManager>();
            TrayManager trayManager = new GameObject("Tray Manager").AddComponent<TrayManager>();
            trayManager.transform.SetParent(systems.transform);
            ToyRobotRestoration restoration =
                new GameObject("Toy Robot Restoration").AddComponent<ToyRobotRestoration>();
            restoration.transform.SetParent(systems.transform);

            Transform robotRoot = new GameObject("Toy Robot").transform;
            robotRoot.position = new Vector3(0f, 0.45f, 0f);

            BuildStaticRobotStructure(robotRoot);

            Transform headRoot = new GameObject("Head Assembly").transform;
            headRoot.SetParent(robotRoot, false);
            headRoot.localPosition = new Vector3(0f, 2.05f, 0f);

            Transform headHousing = PrototypeLevelBuilder.CreateRectangle(
                "Head Housing",
                headRoot,
                Vector3.zero,
                new Vector2(2.35f, 1.45f),
                panelColor,
                4);
            PrototypeLevelBuilder.CreateRectangle(
                "Head Inner Panel",
                headRoot,
                new Vector3(0f, -0.05f, 0f),
                new Vector2(1.82f, 0.86f),
                jointColor,
                5);

            SpriteRenderer leftEye = PrototypeLevelBuilder.CreateCircle(
                "Left Eye",
                headRoot,
                new Vector3(-0.47f, 0.08f, 0f),
                0.30f,
                unlitColor,
                6).GetComponent<SpriteRenderer>();
            SpriteRenderer rightEye = PrototypeLevelBuilder.CreateCircle(
                "Right Eye",
                headRoot,
                new Vector3(0.47f, 0.08f, 0f),
                0.30f,
                unlitColor,
                6).GetComponent<SpriteRenderer>();

            Transform chestPlate = PrototypeLevelBuilder.CreateRectangle(
                "Chest Access Plate",
                robotRoot,
                new Vector3(0f, 0.55f, 0f),
                new Vector2(2.55f, 1.75f),
                panelColor,
                4);
            PrototypeLevelBuilder.CreateRectangle(
                "Chest Inset",
                chestPlate,
                new Vector3(0f, -0.05f, 0f),
                new Vector2(1.55f, 0.82f),
                jointColor,
                5);
            SpriteRenderer chestLight = PrototypeLevelBuilder.CreateCircle(
                "Chest Light",
                chestPlate,
                new Vector3(0f, 0.05f, 0f),
                0.34f,
                unlitColor,
                6).GetComponent<SpriteRenderer>();

            Transform lowerTorsoPanel = PrototypeLevelBuilder.CreateRectangle(
                "Lower Torso Service Panel",
                robotRoot,
                new Vector3(0f, -0.85f, 0f),
                new Vector2(2.18f, 0.95f),
                panelColor,
                4);
            PrototypeLevelBuilder.CreateRectangle(
                "Lower Torso Seam",
                lowerTorsoPanel,
                new Vector3(0f, 0f, 0f),
                new Vector2(1.48f, 0.14f),
                brassColor,
                5);

            Transform waveArm = BuildRightArm(robotRoot);
            Transform forearmCasing = PrototypeLevelBuilder.CreateRectangle(
                "Right Forearm Casing",
                waveArm,
                new Vector3(0f, -0.98f, 0f),
                new Vector2(0.78f, 1.38f),
                panelColor,
                4);

            Transform[] traySlots = PrototypeLevelBuilder.BuildTray(level.TrayCapacity);
            PrototypeUiReferences ui = PrototypeLevelBuilder.BuildUi(gameManager, level);
            Screw[] screws = PrototypeLevelBuilder.BuildScrews(
                robotRoot,
                trayManager,
                gameManager,
                level.Screws);

            ToyRobotRestoration.RobotPart[] robotParts =
            {
                MakeRobotPart(
                    "Head Housing",
                    headHousing,
                    new Screw[] { screws[0], screws[1], screws[2] },
                    new Vector3(0.10f, 0.14f, 0f),
                    -6f),
                MakeRobotPart(
                    "Chest Access Plate",
                    chestPlate,
                    new Screw[] { screws[3], screws[4], screws[5] },
                    new Vector3(0f, -0.34f, 0f),
                    -5f),
                MakeRobotPart(
                    "Lower Torso Service Panel",
                    lowerTorsoPanel,
                    new Screw[] { screws[6], screws[7], screws[8] },
                    new Vector3(-0.12f, -0.25f, 0f),
                    4f),
                MakeRobotPart(
                    "Right Forearm Casing",
                    forearmCasing,
                    new Screw[] { screws[9], screws[10], screws[11] },
                    new Vector3(0.16f, -0.16f, 0f),
                    7f)
            };

            restoration.Configure(
                robotRoot,
                headRoot,
                waveArm,
                new SpriteRenderer[] { leftEye, rightEye },
                chestLight,
                robotParts);
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

        private void BuildStaticRobotStructure(Transform robotRoot)
        {
            PrototypeLevelBuilder.CreateRectangle(
                "Torso Base",
                robotRoot,
                new Vector3(0f, 0.15f, 0f),
                new Vector2(2.95f, 3.25f),
                bodyColor,
                2);
            PrototypeLevelBuilder.CreateRectangle(
                "Neck",
                robotRoot,
                new Vector3(0f, 1.38f, 0f),
                new Vector2(0.62f, 0.55f),
                brassColor,
                3);
            PrototypeLevelBuilder.CreateRectangle(
                "Hip Assembly",
                robotRoot,
                new Vector3(0f, -1.52f, 0f),
                new Vector2(2.25f, 0.55f),
                brassColor,
                3);

            BuildLeg(robotRoot, "Left Leg", -0.68f);
            BuildLeg(robotRoot, "Right Leg", 0.68f);
            BuildLeftArm(robotRoot);
        }

        private void BuildLeg(Transform parent, string name, float xPosition)
        {
            Transform leg = new GameObject(name).transform;
            leg.SetParent(parent, false);
            leg.localPosition = new Vector3(xPosition, -2.15f, 0f);

            PrototypeLevelBuilder.CreateRectangle(
                "Leg Casing",
                leg,
                Vector3.zero,
                new Vector2(0.82f, 1.35f),
                bodyColor,
                2);
            PrototypeLevelBuilder.CreateCircle(
                "Knee Joint",
                leg,
                new Vector3(0f, 0.28f, 0f),
                0.34f,
                brassColor,
                3);
            PrototypeLevelBuilder.CreateRectangle(
                "Foot",
                leg,
                new Vector3(0f, -0.78f, 0f),
                new Vector2(1.05f, 0.38f),
                jointColor,
                3);
        }

        private void BuildLeftArm(Transform parent)
        {
            Transform arm = new GameObject("Left Arm").transform;
            arm.SetParent(parent, false);
            arm.localPosition = new Vector3(-1.72f, 0.72f, 0f);

            PrototypeLevelBuilder.CreateCircle(
                "Left Shoulder",
                arm,
                Vector3.zero,
                0.68f,
                brassColor,
                3);
            PrototypeLevelBuilder.CreateRectangle(
                "Left Upper Arm",
                arm,
                new Vector3(0f, -0.45f, 0f),
                new Vector2(0.78f, 1.18f),
                bodyColor,
                3);
            PrototypeLevelBuilder.CreateCircle(
                "Left Elbow",
                arm,
                new Vector3(0f, -1.02f, 0f),
                0.42f,
                jointColor,
                3);
            PrototypeLevelBuilder.CreateRectangle(
                "Left Forearm",
                arm,
                new Vector3(0f, -1.62f, 0f),
                new Vector2(0.72f, 1.05f),
                bodyColor,
                3);
        }

        private Transform BuildRightArm(Transform parent)
        {
            Transform arm = new GameObject("Right Wave Arm").transform;
            arm.SetParent(parent, false);
            arm.localPosition = new Vector3(1.72f, 0.72f, 0f);

            PrototypeLevelBuilder.CreateCircle(
                "Right Shoulder",
                arm,
                Vector3.zero,
                0.68f,
                brassColor,
                3);
            PrototypeLevelBuilder.CreateRectangle(
                "Right Upper Arm",
                arm,
                new Vector3(0f, -0.38f, 0f),
                new Vector2(0.78f, 1.05f),
                bodyColor,
                3);
            PrototypeLevelBuilder.CreateCircle(
                "Right Elbow",
                arm,
                new Vector3(0f, -0.82f, 0f),
                0.42f,
                jointColor,
                3);
            PrototypeLevelBuilder.CreateRectangle(
                "Right Hand",
                arm,
                new Vector3(0f, -1.78f, 0f),
                new Vector2(0.86f, 0.44f),
                brassColor,
                3);
            return arm;
        }

        private LevelDefinition CreateToyRobotLevelDefinition()
        {
            ScrewDefinition[] screws =
            {
                new ScrewDefinition(ScrewColorId.Red, new Vector3(-0.72f, 2.38f, 0f)),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(0.72f, 2.38f, 0f)),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(0f, 1.72f, 0f)),

                new ScrewDefinition(ScrewColorId.Blue, new Vector3(-0.82f, 0.88f, 0f)),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(0.82f, 0.88f, 0f), 0),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(0f, 0.18f, 0f), 1),

                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(-0.68f, -0.62f, 0f)),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(0.68f, -0.62f, 0f), 1),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(0f, -1.18f, 0f), 2),

                new ScrewDefinition(ScrewColorId.Red, new Vector3(1.72f, 0.42f, 0f), 3, 6),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(1.72f, -0.18f, 0f), 4, 7),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(1.72f, -0.78f, 0f), 5, 8)
            };

            return new LevelDefinition(
                3,
                "Toy Robot",
                "LevelSelect",
                "LEVEL SELECT",
                "Toy robot restored!",
                "RESTORED!\nSystems online.",
                5,
                3,
                screws);
        }

        private ToyRobotRestoration.RobotPart MakeRobotPart(
            string partName,
            Transform visual,
            Screw[] holdingScrews,
            Vector3 releasedOffset,
            float releasedRotation)
        {
            return new ToyRobotRestoration.RobotPart
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
