using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>Builds the layered CRT television with generated geometry as a resource fallback.</summary>
    public sealed class CrtTelevisionLevelBootstrap : MonoBehaviour
    {
        public static LevelDefinition CreateLevelDefinition()
        {
            return new LevelDefinition(5, "CRT Television", "LevelSelect", "LEVEL SELECT",
                "Television restored!", "RESTORED!\nBack on the air.", 5, 3, new[]
                {
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(-1.6f, 1.9f, 0f)),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(0f, 1.9f, 0f)),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(1.6f, 1.9f, 0f)),
                    new ScrewDefinition(ScrewColorId.Blue, new Vector3(-1.9f, 1f, 0f)),
                    new ScrewDefinition(ScrewColorId.Blue, new Vector3(-0.6f, 1f, 0f), 0),
                    new ScrewDefinition(ScrewColorId.Blue, new Vector3(0.6f, 1f, 0f), 1),
                    new ScrewDefinition(ScrewColorId.Yellow, new Vector3(1.8f, 0.9f, 0f)),
                    new ScrewDefinition(ScrewColorId.Yellow, new Vector3(1.8f, 0f, 0f), 1),
                    new ScrewDefinition(ScrewColorId.Yellow, new Vector3(1.8f, -0.9f, 0f), 2),
                    new ScrewDefinition(ScrewColorId.Blue, new Vector3(-1.8f, -1.6f, 0f), 3, 6),
                    new ScrewDefinition(ScrewColorId.Blue, new Vector3(-0.7f, -1.6f, 0f), 4, 7),
                    new ScrewDefinition(ScrewColorId.Blue, new Vector3(0.4f, -1.6f, 0f), 5, 8),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(-1.8f, -0.65f, 0f), 9),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(-0.7f, -0.65f, 0f), 10),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(0.4f, -0.65f, 0f), 11)
                });
        }

        private Texture2D televisionAtlas;
        private readonly System.Collections.Generic.List<Sprite> ownedSprites =
            new System.Collections.Generic.List<Sprite>();

        private void Awake()
        {
            LevelDefinition level = CreateLevelDefinition();
            Color dark = new Color(0.13f, 0.11f, 0.09f);
            Color walnut = new Color(0.34f, 0.19f, 0.10f);
            Color brass = new Color(0.68f, 0.47f, 0.24f);
            PrototypeLevelBuilder.BuildCamera(dark);
            PrototypeLevelBuilder.BuildBackground(dark);
            GameObject systems = new GameObject("Game Systems");
            GameManager game = systems.AddComponent<GameManager>();
            TrayManager tray = systems.AddComponent<TrayManager>();
            CrtTelevisionRestoration restoration = systems.AddComponent<CrtTelevisionRestoration>();
            Transform root = Assembly("CRT Television", null, new Vector3(0f, 0.55f, 0f));
            Rect("Walnut Cabinet", root, Vector3.zero, new Vector2(5.1f, 4.3f), walnut, 2);
            Rect("Inner Cabinet", root, Vector3.zero, new Vector2(4.8f, 4f), dark, 3);
            Rect("Left Foot", root, new Vector3(-1.6f, -2.3f, 0f), new Vector2(0.6f, 0.4f), brass, 2);
            Rect("Right Foot", root, new Vector3(1.6f, -2.3f, 0f), new Vector2(0.6f, 0.4f), brass, 2);

            Transform antenna = Assembly("Antenna Rail", root, new Vector3(0f, 1.9f, 0f));
            Rect("Top Rail", antenna, Vector3.zero, new Vector2(4.7f, 0.5f), brass, 4);
            Transform leftAntenna = Rect("Left Antenna", antenna, new Vector3(-0.6f, 0.6f, 0f),
                new Vector2(0.09f, 1.2f), brass, 3);
            leftAntenna.localRotation = Quaternion.Euler(0f, 0f, 35f);
            Transform rightAntenna = Rect("Right Antenna", antenna, new Vector3(0.6f, 0.6f, 0f),
                new Vector2(0.09f, 1.2f), brass, 3);
            rightAntenna.localRotation = Quaternion.Euler(0f, 0f, -35f);

            Transform bezel = Assembly("Screen Bezel", root, new Vector3(-0.65f, 0.25f, 0f));
            Rect("Screen Rim", bezel, Vector3.zero, new Vector2(3.45f, 2.4f), brass, 4);
            SpriteRenderer screen = Rect("CRT Screen", bezel, Vector3.zero,
                new Vector2(3.05f, 1.8f), new Color(0.12f, 0.19f, 0.17f), 5).GetComponent<SpriteRenderer>();
            for (int row = 0; row < 9; row++)
                Rect("Scan Line " + row, bezel, new Vector3(0f, -0.7f + row * 0.175f, 0f),
                    new Vector2(2.9f, 0.025f), new Color(0.05f, 0.08f, 0.07f, 0.18f), 6);

            Transform controls = Assembly("Control Panel", root, new Vector3(1.8f, 0f, 0f));
            Rect("Control Plate", controls, Vector3.zero, new Vector2(0.95f, 3f), walnut, 4);
            for (int knob = 0; knob < 2; knob++)
                PrototypeLevelBuilder.CreateCircle("Tuning Dial " + knob, controls,
                    new Vector3(0f, 0.45f - knob * 0.9f, 0f), 0.55f, brass, 5);
            Transform speaker = Assembly("Speaker Grille", root, new Vector3(-0.7f, -1.6f, 0f));
            Rect("Speaker Plate", speaker, Vector3.zero, new Vector2(3.5f, 0.7f), walnut, 4);
            for (int slot = 0; slot < 10; slot++)
                Rect("Speaker Slot " + slot, speaker, new Vector3(-1.4f + slot * 0.31f, 0f, 0f),
                    new Vector2(0.08f, 0.42f), dark, 5);
            Transform power = Assembly("Power Module", root, new Vector3(-0.7f, -0.65f, 0f));
            Rect("Power Trim", power, Vector3.zero, new Vector2(3.2f, 0.45f), walnut, 7);
            SpriteRenderer lamp = PrototypeLevelBuilder.CreateCircle("Power Lamp", controls,
                new Vector3(0f, -1.25f, 0f), 0.12f, dark, 6).GetComponent<SpriteRenderer>();

            televisionAtlas = Resources.Load<Texture2D>("Art/CrtTelevision/Television_Atlas");
            if (televisionAtlas != null)
            {
                foreach (SpriteRenderer fallback in root.GetComponentsInChildren<SpriteRenderer>())
                    fallback.enabled = false;
                screen.gameObject.name = "CRT Screen Fallback";
                lamp.enabled = true;
                AtlasPart("Television Cabinet Art", root, new Vector3(0f, -0.1f, 0f),
                    new Vector2(5.1f, 4.7f), new Rect(46f, 96f, 357f, 367f), 2);
                AtlasPart("Antenna Rail Art", antenna, new Vector3(0f, 0.55f, 0f),
                    new Vector2(4.7f, 1.6f), new Rect(490f, 120f, 524f, 333f), 4);
                screen = AtlasPart("CRT Screen", bezel, Vector3.zero,
                    new Vector2(3.45f, 2.4f), new Rect(1097f, 135f, 389f, 312f), 4)
                    .GetComponent<SpriteRenderer>();
                screen.color = new Color(0.25f, 0.35f, 0.30f);
                AtlasPart("Control Panel Art", controls, Vector3.zero,
                    new Vector2(0.95f, 3f), new Rect(100f, 546f, 141f, 373f), 4);
                AtlasPart("Speaker Grille Art", speaker, Vector3.zero,
                    new Vector2(3.5f, 0.7f), new Rect(326f, 647f, 557f, 191f), 4);
                AtlasPart("Power Module Art", power, Vector3.zero,
                    new Vector2(3.2f, 0.45f), new Rect(925f, 694f, 572f, 107f), 7);
            }

            Transform[] slots = PrototypeLevelBuilder.BuildTray(level.TrayCapacity);
            PrototypeUiReferences ui = PrototypeLevelBuilder.BuildUi(game, level);
            Screw[] screws = PrototypeLevelBuilder.BuildScrews(root, tray, game, level.Screws);
            restoration.Configure(screen, lamp, new[]
            {
                Part(antenna, screws, 0, new Vector3(0f, 0.15f, 0f), -3f),
                Part(bezel, screws, 3, new Vector3(-0.1f, 0f, 0f), -4f),
                Part(controls, screws, 6, new Vector3(0.14f, 0f, 0f), 5f),
                Part(speaker, screws, 9, new Vector3(0f, -0.14f, 0f), -3f),
                Part(power, screws, 12, new Vector3(0f, -0.1f, 0f), 4f)
            });
            tray.Configure(slots, level.MatchSize, game);
            game.Configure(tray, restoration, ui.StatusText, ui.ResultOverlay, ui.ResultTitle,
                ui.ResultActionButton, ui.ResultActionLabel, screws.Length, level.LevelNumber,
                level.NextSceneName, level.WinButtonLabel, level.RestoredStatusMessage, level.RestoredResultMessage);
        }

        // Source pixel rectangles use a top-left origin on the 1536 x 1024 atlas.
        private Transform AtlasPart(string name, Transform parent, Vector3 position,
            Vector2 size, Rect source, int order)
        {
            float sx = televisionAtlas.width / 1536f;
            float sy = televisionAtlas.height / 1024f;
            Rect rect = new Rect(source.x * sx, (1024f - source.yMax) * sy,
                source.width * sx, source.height * sy);
            Sprite sprite = Sprite.Create(televisionAtlas, rect, new Vector2(0.5f, 0.5f),
                100f, 0, SpriteMeshType.FullRect);
            ownedSprites.Add(sprite);
            Transform visual = Assembly(name, parent, position);
            SpriteRenderer renderer = visual.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            visual.localScale = new Vector3(size.x / sprite.bounds.size.x,
                size.y / sprite.bounds.size.y, 1f);
            return visual;
        }

        private void OnDestroy()
        {
            foreach (Sprite sprite in ownedSprites)
                if (sprite != null) Destroy(sprite);
        }

        private static Transform Assembly(string name, Transform parent, Vector3 position)
        {
            Transform result = new GameObject(name).transform;
            result.SetParent(parent, false);
            result.localPosition = position;
            return result;
        }

        private static Transform Rect(string name, Transform parent, Vector3 position, Vector2 size,
            Color color, int order)
        {
            return PrototypeLevelBuilder.CreateRectangle(name, parent, position, size, color, order);
        }

        private static CrtTelevisionRestoration.TelevisionPart Part(Transform visual, Screw[] screws,
            int start, Vector3 offset, float rotation)
        {
            return new CrtTelevisionRestoration.TelevisionPart
            {
                Visual = visual, HoldingScrews = new[] { screws[start], screws[start + 1], screws[start + 2] },
                ReleasedOffset = offset, ReleasedRotation = rotation
            };
        }
    }
}
