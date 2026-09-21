using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Builds the V0.6 production toy-robot level and connects it to shared gameplay.
    /// Generated shapes remain as a safe fallback if an art resource is missing.
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

            Transform headRoot;
            Transform headHousing;
            Transform chestPlate;
            Transform lowerTorsoPanel;
            Transform leftArm;
            Transform waveArm;
            Transform forearmCasing;
            SpriteRenderer leftEye;
            SpriteRenderer rightEye;
            SpriteRenderer chestLight;

            Transform productionBase = PrototypeLevelBuilder.CreateResourceSprite(
                "Robot Base",
                robotRoot,
                Vector3.zero,
                "Art/ToyRobot/ToyRobot_Base",
                new Vector2(4.1f, 6.15f),
                Color.white,
                2);

            if (productionBase != null)
            {
                headRoot = new GameObject("Head Assembly").transform;
                headRoot.SetParent(robotRoot, false);
                headRoot.localPosition = new Vector3(0f, 2.33f, 0f);

                PrototypeLevelBuilder.CreateResourceSprite(
                    "Head Structure",
                    headRoot,
                    Vector3.zero,
                    "Art/ToyRobot/Head_Structure",
                    new Vector2(1.46f, 1f),
                    Color.white,
                    3);

                headHousing = PrototypeLevelBuilder.CreateResourceSprite(
                    "Head Housing",
                    headRoot,
                    new Vector3(0f, -0.12f, 0f),
                    "Art/ToyRobot/Head_Housing",
                    new Vector2(1.08f, 0.72f),
                    Color.white,
                    4);

                leftEye = PrototypeLevelBuilder.CreateCircle(
                    "Left Eye Glow",
                    headRoot,
                    new Vector3(-0.25f, -0.11f, 0f),
                    0.15f,
                    unlitColor,
                    6).GetComponent<SpriteRenderer>();
                rightEye = PrototypeLevelBuilder.CreateCircle(
                    "Right Eye Glow",
                    headRoot,
                    new Vector3(0.25f, -0.11f, 0f),
                    0.15f,
                    unlitColor,
                    6).GetComponent<SpriteRenderer>();

                chestPlate = PrototypeLevelBuilder.CreateResourceSprite(
                    "Chest Access Plate",
                    robotRoot,
                    new Vector3(0f, 1.07f, 0f),
                    "Art/ToyRobot/Chest_Access_Plate",
                    new Vector2(1f, 0.84f),
                    Color.white,
                    4);
                chestLight = PrototypeLevelBuilder.CreateCircle(
                    "Chest Light Glow",
                    robotRoot,
                    new Vector3(0f, 1.07f, 0f),
                    0.17f,
                    unlitColor,
                    6).GetComponent<SpriteRenderer>();

                lowerTorsoPanel = PrototypeLevelBuilder.CreateResourceSprite(
                    "Lower Torso Service Panel",
                    robotRoot,
                    new Vector3(0f, 0.10f, 0f),
                    "Art/ToyRobot/Lower_Torso_Panel",
                    new Vector2(0.76f, 0.52f),
                    Color.white,
                    4);

                leftArm = new GameObject("Left Arm Assembly").transform;
                leftArm.SetParent(robotRoot, false);
                leftArm.localPosition = new Vector3(-0.85f, 1.35f, 0f);
                PrototypeLevelBuilder.CreateResourceSprite(
                    "Left Arm Structure",
                    leftArm,
                    new Vector3(-0.16f, -0.92f, 0f),
                    "Art/ToyRobot/Left_Arm",
                    new Vector2(0.92f, 2.68f),
                    Color.white,
                    3);

                waveArm = new GameObject("Right Wave Arm").transform;
                waveArm.SetParent(robotRoot, false);
                waveArm.localPosition = new Vector3(0.85f, 1.35f, 0f);
                PrototypeLevelBuilder.CreateResourceSprite(
                    "Right Arm Structure",
                    waveArm,
                    new Vector3(0.16f, -0.92f, 0f),
                    "Art/ToyRobot/Right_Arm",
                    new Vector2(0.92f, 2.68f),
                    Color.white,
                    3);

                forearmCasing = PrototypeLevelBuilder.CreateResourceSprite(
                    "Right Forearm Casing",
                    waveArm,
                    new Vector3(0.34f, -1.16f, 0f),
                    "Art/ToyRobot/Right_Forearm_Casing",
                    new Vector2(0.36f, 0.60f),
                    Color.white,
                    4);
            }
            else
            {
                BuildStaticRobotStructure(robotRoot);

                headRoot = new GameObject("Head Assembly").transform;
                headRoot.SetParent(robotRoot, false);
                headRoot.localPosition = new Vector3(0f, 2.05f, 0f);

                headHousing = PrototypeLevelBuilder.CreateRectangle(
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

                leftEye = PrototypeLevelBuilder.CreateCircle(
                    "Left Eye",
                    headRoot,
                    new Vector3(-0.47f, 0.08f, 0f),
                    0.30f,
                    unlitColor,
                    6).GetComponent<SpriteRenderer>();
                rightEye = PrototypeLevelBuilder.CreateCircle(
                    "Right Eye",
                    headRoot,
                    new Vector3(0.47f, 0.08f, 0f),
                    0.30f,
                    unlitColor,
                    6).GetComponent<SpriteRenderer>();

                chestPlate = PrototypeLevelBuilder.CreateRectangle(
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
                chestLight = PrototypeLevelBuilder.CreateCircle(
                    "Chest Light",
                    chestPlate,
                    new Vector3(0f, 0.05f, 0f),
                    0.34f,
                    unlitColor,
                    6).GetComponent<SpriteRenderer>();

                lowerTorsoPanel = PrototypeLevelBuilder.CreateRectangle(
                    "Lower Torso Service Panel",
                    robotRoot,
                    new Vector3(0f, -0.85f, 0f),
                    new Vector2(2.18f, 0.95f),
                    panelColor,
                    4);
                PrototypeLevelBuilder.CreateRectangle(
                    "Lower Torso Seam",
                    lowerTorsoPanel,
                    Vector3.zero,
                    new Vector2(1.48f, 0.14f),
                    brassColor,
                    5);

                leftArm = lowerTorsoPanel;
                waveArm = BuildRightArm(robotRoot);
                forearmCasing = PrototypeLevelBuilder.CreateRectangle(
                    "Right Forearm Casing",
                    waveArm,
                    new Vector3(0f, -0.98f, 0f),
                    new Vector2(0.78f, 1.38f),
                    panelColor,
                    4);
            }

            Transform[] traySlots = PrototypeLevelBuilder.BuildTray(level.TrayCapacity);
            PrototypeUiReferences ui = PrototypeLevelBuilder.BuildUi(gameManager, level);
            Screw[] screws = PrototypeLevelBuilder.BuildScrews(
                robotRoot,
                trayManager,
                gameManager,
                level.Screws);

            if (productionBase != null)
            {
                foreach (Screw screw in screws)
                {
                    screw.transform.localScale *= 0.78f;
                    CircleCollider2D collider = screw.GetComponent<CircleCollider2D>();
                    if (collider != null)
                    {
                        collider.radius = 0.44f;
                    }
                }
            }

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
                    "Left Arm Assembly",
                    leftArm,
                    new Screw[] { screws[6], screws[7], screws[8] },
                    new Vector3(-0.12f, -0.10f, 0f),
                    -5f),
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
                new ScrewDefinition(ScrewColorId.Red, new Vector3(-0.50f, 2.38f, 0f)),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(0.50f, 2.38f, 0f)),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(0f, 1.95f, 0f)),

                new ScrewDefinition(ScrewColorId.Blue, new Vector3(-0.46f, 1.27f, 0f)),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(0.46f, 1.27f, 0f), 0),
                new ScrewDefinition(ScrewColorId.Blue, new Vector3(0f, 0.78f, 0f), 1),

                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(-1.18f, 0.62f, 0f)),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(-1.20f, 0.18f, 0f), 1),
                new ScrewDefinition(ScrewColorId.Yellow, new Vector3(-1.20f, -0.27f, 0f), 2),

                new ScrewDefinition(ScrewColorId.Red, new Vector3(1.18f, 0.62f, 0f), 3, 6),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(1.20f, 0.18f, 0f), 4, 7),
                new ScrewDefinition(ScrewColorId.Red, new Vector3(1.20f, -0.27f, 0f), 5, 8)
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
