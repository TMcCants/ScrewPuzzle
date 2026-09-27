using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>Builds the layered production camera; unscaled assemblies preserve repair motion.</summary>
    public sealed class OldCameraLevelBootstrap : MonoBehaviour
    {
        public static LevelDefinition CreateLevelDefinition()
        {
            return new LevelDefinition(4, "Old Camera", "LevelSelect", "LEVEL SELECT",
                "Old camera restored!", "RESTORED!\nReady for another memory.", 5, 3,
                new[]
                {
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(-1.5f, 1.65f, 0f)),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(0f, 1.65f, 0f)),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(1.5f, 1.65f, 0f)),
                    new ScrewDefinition(ScrewColorId.Blue, new Vector3(-1.8f, 0.7f, 0f)),
                    new ScrewDefinition(ScrewColorId.Blue, new Vector3(-1.8f, -0.15f, 0f), 0),
                    new ScrewDefinition(ScrewColorId.Blue, new Vector3(-1.8f, -1f, 0f), 1),
                    new ScrewDefinition(ScrewColorId.Yellow, new Vector3(1.8f, 0.7f, 0f)),
                    new ScrewDefinition(ScrewColorId.Yellow, new Vector3(1.8f, -0.15f, 0f), 1),
                    new ScrewDefinition(ScrewColorId.Yellow, new Vector3(1.8f, -1f, 0f), 2),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(-0.8f, -0.9f, 0f), 3, 6),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(0f, 0.65f, 0f), 4, 7),
                    new ScrewDefinition(ScrewColorId.Red, new Vector3(0.8f, -0.9f, 0f), 5, 8)
                });
        }

        private Texture2D cameraAtlas;
        private readonly System.Collections.Generic.List<Sprite> ownedSprites =
            new System.Collections.Generic.List<Sprite>();

        private void Awake()
        {
            LevelDefinition level = CreateLevelDefinition();
            Color charcoal = new Color(0.12f, 0.10f, 0.09f);
            Color brass = new Color(0.66f, 0.46f, 0.25f);
            Color leather = new Color(0.24f, 0.16f, 0.11f);
            PrototypeLevelBuilder.BuildCamera(charcoal);
            PrototypeLevelBuilder.BuildBackground(charcoal);
            Transform systems = new GameObject("Game Systems").transform;
            GameManager game = systems.gameObject.AddComponent<GameManager>();
            TrayManager tray = systems.gameObject.AddComponent<TrayManager>();
            OldCameraRestoration restoration = systems.gameObject.AddComponent<OldCameraRestoration>();
            Transform root = Assembly("Old Camera", null, new Vector3(0f, 0.7f, 0f));
            cameraAtlas = Resources.Load<Texture2D>("Art/OldCamera/Camera_Atlas");
            if (cameraAtlas == null)
            {
                Rect("Body Rim", root, Vector3.zero, new Vector2(4.9f, 3.4f), brass, 2);
                Rect("Leather Body", root, Vector3.zero, new Vector2(4.65f, 3.15f), charcoal, 3);

            }
            else
            {
                AtlasPart("Camera Body Art", root, Vector3.zero, new Vector2(4.9f, 3.4f),
                    new Rect(13f, 145f, 630f, 378f), 2);
            }

            Transform top = Assembly("Flash Housing", root, new Vector3(0f, 1.55f, 0f));
            if (cameraAtlas == null)
            {
                Rect("Top Plate", top, Vector3.zero, new Vector2(4.7f, 0.65f), brass, 4);
                Rect("Viewfinder", top, new Vector3(0f, 0.5f, 0f), new Vector2(1.1f, 0.45f), charcoal, 5);
                Rect("Flash Rim", top, new Vector3(1.35f, 0.7f, 0f), new Vector2(1.2f, 0.7f), brass, 4);
            }
            else
            {
                AtlasPart("Flash Housing Art", top, new Vector3(0f, 0.35f, 0f),
                    new Vector2(4.7f, 1.5f), new Rect(647f, 286f, 592f, 192f), 4);
            }
            Vector3 flashPosition = cameraAtlas == null
                ? new Vector3(1.35f, 0.7f, 0f) : new Vector3(1.06f, 0.60f, 0f);
            SpriteRenderer flash = Rect("Flash Glass", top, flashPosition,
                new Vector2(0.85f, 0.5f), new Color(0.40f, 0.37f, 0.28f, cameraAtlas == null ? 1f : 0f), 5).GetComponent<SpriteRenderer>();

            Transform left = Assembly("Film Door", root, new Vector3(-1.75f, -0.15f, 0f));
            if (cameraAtlas == null)
                Rect("Film Door Leather", left, Vector3.zero, new Vector2(0.95f, 2.55f), leather, 4);
            else
                AtlasPart("Film Door Art", left, Vector3.zero, new Vector2(0.95f, 2.55f),
                    new Rect(224f, 649f, 178f, 510f), 4);
            Transform right = Assembly("Grip Panel", root, new Vector3(1.75f, -0.15f, 0f));
            if (cameraAtlas == null)
                Rect("Grip Leather", right, Vector3.zero, new Vector2(0.95f, 2.55f), leather, 4);
            else
                AtlasPart("Grip Art", right, Vector3.zero, new Vector2(0.95f, 2.55f),
                    new Rect(224f, 649f, 178f, 510f), 4);
            Transform lens = Assembly("Lens Assembly", root, new Vector3(0f, -0.25f, 0f));
            if (cameraAtlas == null)
            {
                PrototypeLevelBuilder.CreateCircle("Lens Brass Ring", lens, Vector3.zero, 2.65f, brass, 4);
                PrototypeLevelBuilder.CreateCircle("Lens Barrel", lens, Vector3.zero, 2.25f, charcoal, 5);
                PrototypeLevelBuilder.CreateCircle("Lens Glass", lens, Vector3.zero, 1.75f,
                new Color(0.12f, 0.23f, 0.24f), 6);
                PrototypeLevelBuilder.CreateCircle("Lens Reflection", lens, new Vector3(-0.35f, 0.35f, 0f),
                0.36f, new Color(0.46f, 0.58f, 0.52f), 7);

            }
            else
            {
                AtlasPart("Lens Art", lens, Vector3.zero, new Vector2(2.65f, 2.65f),
                    new Rect(703f, 694f, 455f, 450f), 4);
            }

            Transform[] slots = PrototypeLevelBuilder.BuildTray(level.TrayCapacity);
            PrototypeUiReferences ui = PrototypeLevelBuilder.BuildUi(game, level);
            Screw[] screws = PrototypeLevelBuilder.BuildScrews(root, tray, game, level.Screws);
            restoration.Configure(lens, flash, new[]
            {
                Part(top, screws, 0, new Vector3(0f, 0.22f, 0f), -4f),
                Part(left, screws, 3, new Vector3(-0.2f, -0.1f, 0f), -6f),
                Part(right, screws, 6, new Vector3(0.2f, -0.1f, 0f), 6f),
                Part(lens, screws, 9, new Vector3(0f, -0.2f, 0f), -8f)
            });
            tray.Configure(slots, level.MatchSize, game);
            game.Configure(tray, restoration, ui.StatusText, ui.ResultOverlay, ui.ResultTitle,
                ui.ResultActionButton, ui.ResultActionLabel, screws.Length, level.LevelNumber,
                level.NextSceneName, level.WinButtonLabel, level.RestoredStatusMessage,
                level.RestoredResultMessage);
        }

        // Atlas rectangles use the source image's top-left origin (1254 x 1254).
        // Sprite.Create uses a bottom-left origin; normalized scaling also supports import resizing.
        private void AtlasPart(string name, Transform parent, Vector3 position,
            Vector2 size, Rect source, int order)
        {
            float sx = cameraAtlas.width / 1254f;
            float sy = cameraAtlas.height / 1254f;
            Rect rect = new Rect(source.x * sx, (1254f - source.yMax) * sy,
                source.width * sx, source.height * sy);
            Sprite sprite = Sprite.Create(cameraAtlas, rect, new Vector2(0.5f, 0.5f),
                100f, 0, SpriteMeshType.FullRect);
            ownedSprites.Add(sprite);
            Transform visual = Assembly(name, parent, position);
            SpriteRenderer renderer = visual.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            visual.localScale = new Vector3(size.x / sprite.bounds.size.x,
                size.y / sprite.bounds.size.y, 1f);
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

        private static Transform Rect(string name, Transform parent, Vector3 position,
            Vector2 size, Color color, int order)
        {
            return PrototypeLevelBuilder.CreateRectangle(name, parent, position, size, color, order);
        }

        private static OldCameraRestoration.CameraPart Part(Transform visual, Screw[] screws,
            int start, Vector3 offset, float rotation)
        {
            return new OldCameraRestoration.CameraPart
            {
                Visual = visual,
                HoldingScrews = new[] { screws[start], screws[start + 1], screws[start + 2] },
                ReleasedOffset = offset,
                ReleasedRotation = rotation
            };
        }
    }
}
