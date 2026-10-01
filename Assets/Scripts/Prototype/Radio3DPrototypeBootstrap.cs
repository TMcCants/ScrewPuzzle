using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>Isolated interaction experiment; does not read or write puzzle progression.</summary>
    public class Radio3DPrototypeBootstrap : MonoBehaviour
    {
        private readonly List<Material> materials = new List<Material>();
        private readonly List<Mesh> modelMeshes = new List<Mesh>();
        private Radio3DInteraction interaction;
        private Text status;
        private Text soundLabel;
        private Radio3DPuzzle puzzle;
        private Camera view;
        private RectTransform layout;
        private readonly Text[] trayLabels = new Text[4];
        private readonly Image[,] holes = new Image[4, 3];
        private readonly Image[] rims = new Image[4];
        private readonly Button[] unlocks = new Button[4];
        private Sprite circle;
        private Texture2D circleTexture;

        protected void Awake()
        {
            Camera camera = new GameObject("3D Camera").AddComponent<Camera>();
            view = camera;
            if (FindFirstObjectByType<AudioListener>() == null) camera.gameObject.AddComponent<AudioListener>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0f, 0.6f, -9f);
            camera.transform.LookAt(Vector3.zero);
            camera.fieldOfView = 42f;
            camera.backgroundColor = new Color(0.09f, 0.075f, 0.06f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            Light light = new GameObject("Workshop Key Light").AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.25f;
            light.transform.rotation = Quaternion.Euler(35f, -35f, 0f);

            BuildModel(out Transform radio, out List<Radio3DScrew> screws, out Radio3DPlate[] plates);
            puzzle = gameObject.AddComponent<Radio3DPuzzle>();
            puzzle.Configure(camera, screws.ToArray(), plates);
            interaction = gameObject.AddComponent<Radio3DInteraction>();
            interaction.Configure(camera, radio, screws.ToArray());
            BuildUi();
        }

        protected virtual string BoardTitle => "THE RADIO WORKSHOP";
        protected virtual float ModelVerticalOffset => 180f;
        protected virtual string WinTitle => "RADIO CLEARED!";

        protected virtual void BuildModel(out Transform radio, out List<Radio3DScrew> screws, out Radio3DPlate[] plates)
        {
            radio = new GameObject("Rotating Radio").transform;
            Material wood = Material(new Color(0.035f, 0.48f, 0.48f));
            Material brass = Material(new Color(0.95f, 0.87f, 0.68f));
            Material dark = Material(new Color(0.11f, 0.13f, 0.12f));
            Shape("Inner Chassis", PrimitiveType.Cube, radio, Vector3.zero, new Vector3(2.95f, 1.9f, 1.35f), dark);
            RoundedBody("Top Frame", radio, new Vector3(0f, 1.14f, 0f), new Vector3(3.5f, 0.24f, 1.85f), 0.1f, wood);
            RoundedBody("Bottom Frame", radio, new Vector3(0f, -1.14f, 0f), new Vector3(3.5f, 0.24f, 1.85f), 0.1f, wood);
            Material circuit = Material(new Color(0.12f, 0.34f, 0.23f));
            Shape("Circuit Board", PrimitiveType.Cube, radio, new Vector3(0f, 0f, -0.71f), new Vector3(2.7f, 1.7f, 0.06f), circuit);
            Shape("Receiver Module", PrimitiveType.Cube, radio, new Vector3(-0.65f, 0f, -0.75f), new Vector3(0.7f, 1.1f, 0.04f), brass);
            for (int index = 0; index < 3; index++)
                Shape("Circuit Trace " + index, PrimitiveType.Cube, radio, new Vector3(0.55f, (index - 1) * 0.42f, -0.76f),
                    new Vector3(0.95f, 0.06f, 0.04f), brass);

            plates = new Radio3DPlate[5];
            string[] plateNames = { "Front Plate Assembly", "Rear Plate Assembly", "Left Plate Assembly", "Right Plate Assembly", "Inner Front Plate Assembly" };
            for (int index = 0; index < plates.Length; index++)
            {
                plates[index] = new GameObject(plateNames[index]).AddComponent<Radio3DPlate>();
                plates[index].transform.SetParent(radio, false);
            }
            RoundedBody("Front Plate", plates[0].transform, new Vector3(0f, 0f, -0.92f),
                new Vector3(3.15f, 2.15f, 0.08f), 0.035f, wood);
            RoundedBody("Rear Plate", plates[1].transform, new Vector3(0f, 0f, 0.92f),
                new Vector3(3.15f, 2.15f, 0.08f), 0.035f, wood);
            // Front decorations belong to the removable face, not the permanent chassis.
            Transform speaker = new GameObject("Speaker").transform;
            speaker.SetParent(plates[0].transform, false);
            Disc("Speaker Rim", speaker, new Vector3(-0.7f, -0.26f, -1.005f), 1.28f, 0.035f, brass);
            Disc("Speaker Fabric", speaker, new Vector3(-0.7f, -0.26f, -1.045f), 1.10f, 0.018f, dark);
            for (int i = -4; i <= 4; i++)
            {
                float y = i * 0.115f;
                float width = 2f * Mathf.Sqrt(0.52f * 0.52f - y * y);
                RoundedBody("Grille Slat", speaker, new Vector3(-0.7f, -0.26f + y, -1.075f), new Vector3(width, 0.045f, 0.035f), 0.017f, brass);
            }
            RoundedBody("Tuning Display", plates[0].transform, new Vector3(0.67f, 0.23f, -1.01f), new Vector3(1.18f, 0.40f, 0.09f), 0.044f, brass);
            RoundedBody("Tuning Glass", plates[0].transform, new Vector3(0.67f, 0.23f, -1.068f), new Vector3(1.02f, 0.26f, 0.025f), 0.012f, Material(new Color(0.06f, 0.18f, 0.23f)));
            for (int i = 0; i < 9; i++)
                Shape("Tuning Tick", PrimitiveType.Cube, plates[0].transform, new Vector3(0.26f + i * 0.1f, 0.25f, -1.087f), new Vector3(0.014f, i % 2 == 0 ? 0.12f : 0.065f, 0.008f), brass);
            Shape("Tuning Needle", PrimitiveType.Cube, plates[0].transform, new Vector3(0.81f, 0.23f, -1.10f), new Vector3(0.025f, 0.2f, 0.01f), Material(new Color(0.95f, 0.39f, 0.16f)));
            for (int i = 0; i < 2; i++)
            {
                float x = 0.33f + i * 0.7f;
                Disc("Knob Bezel", plates[0].transform, new Vector3(x, -0.49f, -1.01f), 0.55f, 0.035f, dark);
                Disc("Cream Knob", plates[0].transform, new Vector3(x, -0.49f, -1.10f), 0.46f, 0.09f, brass);
                RoundedBody("Knob Pointer", plates[0].transform, new Vector3(x, -0.38f, -1.198f), new Vector3(0.035f, 0.12f, 0.025f), 0.012f, wood);
            }
            foreach (float x in new[] { -1.03f, 1.03f })
            {
                RoundedBody("Handle Mount", radio, new Vector3(x, 1.31f, 0), new Vector3(0.27f, 0.28f, 0.42f), 0.1f, brass);
                RoundedBody("Handle Upright", radio, new Vector3(x, 1.58f, 0), new Vector3(0.20f, 0.56f, 0.28f), 0.09f, wood);
                RoundedBody("Rubber Foot", radio, new Vector3(x, -1.31f, 0), new Vector3(0.52f, 0.18f, 0.85f), 0.08f, dark);
            }
            RoundedBody("Carry Handle", radio, new Vector3(0, 1.82f, 0), new Vector3(2.25f, 0.23f, 0.30f), 0.11f, wood);            Shape("Left Panel", PrimitiveType.Cube, plates[2].transform, new Vector3(-1.72f, 0f, 0f),
                new Vector3(0.08f, 2.1f, 1.6f), wood);
            Shape("Right Panel", PrimitiveType.Cube, plates[3].transform, new Vector3(1.72f, 0f, 0f),
                new Vector3(0.08f, 2.1f, 1.6f), wood);
            Shape("Inner Front Plate", PrimitiveType.Cube, plates[4].transform, new Vector3(0f, 0f, -0.79f),
                new Vector3(2.7f, 1.7f, 0.04f), Material(new Color(0.45f, 0.57f, 0.61f)));

            Material[] colors = { Material(new Color(0.9f, 0.12f, 0.08f)),
                Material(new Color(0.05f, 0.4f, 0.95f)), Material(new Color(1f, 0.7f, 0.04f)) };
            screws = new List<Radio3DScrew>();
            for (int index = 0; index < 3; index++)
            {
                float x = (index - 1) * 1.1f;
                float y = (index - 1) * 0.7f;
                screws.Add(Screw("Front " + index, radio, new Vector3(x, 0.72f, -1.02f), Vector3.back, colors[index], dark));
                screws.Add(Screw("Back " + index, radio, new Vector3(x, 0.72f, 1.02f), Vector3.forward, colors[index], dark));
                screws.Add(Screw("Left " + index, radio, new Vector3(-1.82f, y, 0f), Vector3.left, colors[index], dark));
                screws.Add(Screw("Right " + index, radio, new Vector3(1.82f, y, 0f), Vector3.right, colors[0], dark));
            }
            for (int index = 0; index < screws.Count; index++)
                screws[index].Initialize(index % 4 == 3 ? ScrewColorId.Red : (ScrewColorId)(index / 4));
            Vector3[] normals = { Vector3.back, Vector3.forward, Vector3.left, Vector3.right };
            for (int index = 0; index < normals.Length; index++)
                plates[index].Configure(new[] { screws[index], screws[index + 4], screws[index + 8] }, normals[index]);
            var innerScrews = new Radio3DScrew[3];
            for (int index = 0; index < innerScrews.Length; index++)
            {
                innerScrews[index] = Screw("Inner " + index, radio, new Vector3((index - 1) * 0.85f, 0.45f, -0.84f), Vector3.back, colors[1], dark);
                innerScrews[index].Initialize(ScrewColorId.Blue, plates[0]);
                screws.Add(innerScrews[index]);
            }
            plates[4].Configure(innerScrews, Vector3.back);
        }

        private void Disc(string name, Transform parent, Vector3 position, float diameter, float depth, Material material)
        {
            Transform disc = Shape(name, PrimitiveType.Cylinder, parent, position, new Vector3(diameter, depth, diameter), material);
            disc.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }

        protected Transform RoundedBody(string name, Transform parent, Vector3 position, Vector3 size, float radius, Material material)
        {
            var obj = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider));
            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = position;
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var triangles = new List<int>();
            Vector3 half = size * 0.5f;
            Vector3 inner = half - Vector3.one * radius;
            const int steps = 12;
            foreach (Vector3 n in new[] { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back })
            {
                Vector3 u = Vector3.Cross(n, Mathf.Abs(n.y) > 0.5f ? Vector3.forward : Vector3.up);
                Vector3 v = Vector3.Cross(n, u);
                int start = vertices.Count;
                for (int y = 0; y <= steps; y++)
                    for (int x = 0; x <= steps; x++)
                    {
                        Vector3 p = Vector3.Scale(n + u * (2f * x / steps - 1f) + v * (2f * y / steps - 1f), half);
                        Vector3 core = new Vector3(Mathf.Clamp(p.x, -inner.x, inner.x), Mathf.Clamp(p.y, -inner.y, inner.y), Mathf.Clamp(p.z, -inner.z, inner.z));
                        Vector3 normal = (p - core).normalized;
                        vertices.Add(core + normal * radius);
                        normals.Add(normal);
                        if (x < steps && y < steps)
                        {
                            int a = start + y * (steps + 1) + x;
                            triangles.AddRange(new[] { a, a + 1, a + steps + 1, a + 1, a + steps + 2, a + steps + 1 });
                        }
                    }
            }
            var mesh = new Mesh { name = name + " Rounded Mesh" };
            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateBounds();
            modelMeshes.Add(mesh);
            obj.GetComponent<MeshFilter>().sharedMesh = mesh;
            obj.GetComponent<MeshRenderer>().sharedMaterial = material;
            obj.GetComponent<BoxCollider>().size = size;
            return obj.transform;
        }

        protected Material Material(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline == null)
                shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.color = color;
            materials.Add(material);
            return material;
        }

        protected Transform Shape(string name, PrimitiveType type, Transform parent,
            Vector3 position, Vector3 scale, Material material)
        {
            GameObject shape = GameObject.CreatePrimitive(type);
            shape.name = name;
            shape.transform.SetParent(parent, false);
            shape.transform.localPosition = position;
            shape.transform.localScale = scale;
            shape.GetComponent<Renderer>().sharedMaterial = material;
            return shape.transform;
        }

        protected Radio3DScrew Screw(string name, Transform radio, Vector3 position, Vector3 normal,
            Material material, Material slotMaterial)
        {
            Transform mount = new GameObject(name).transform;
            mount.SetParent(radio, false);
            mount.localPosition = position;
            mount.localRotation = Quaternion.FromToRotation(Vector3.up, normal);
            Radio3DScrew screw = mount.gameObject.AddComponent<Radio3DScrew>();
            Shape("Head", PrimitiveType.Cylinder, mount, Vector3.zero, new Vector3(0.42f, 0.055f, 0.42f), material);
            Shape("Slot", PrimitiveType.Cube, mount, new Vector3(0f, 0.058f, 0f),
                new Vector3(0.29f, 0.015f, 0.055f), slotMaterial);
            return screw;
        }

        private Image Panel(string name, Transform parent, Vector2 position, Vector2 size, Color color)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            PrototypeLevelBuilder.SetRect(obj.GetComponent<RectTransform>(), Vector2.one * 0.5f,
                Vector2.one * 0.5f, position, size);
            var image = obj.GetComponent<Image>();
            image.color = color;
            return image;
        }

        private void BuildUi()
        {
            var canvasObject = new GameObject("3D Experiment UI");
            canvasObject.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();
            layout = new GameObject("Portrait Layout", typeof(RectTransform)).GetComponent<RectTransform>();
            layout.SetParent(canvasObject.transform, false);
            layout.sizeDelta = new Vector2(1080, 1920);
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                var events = new GameObject("EventSystem");
                events.AddComponent<EventSystem>();
                events.AddComponent<StandaloneInputModule>();
            }
            circleTexture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 64; x++)
                    circleTexture.SetPixel(x, y, new Color(1, 1, 1,
                        Mathf.Clamp01(31.5f - Vector2.Distance(new Vector2(x, y), new Vector2(31.5f, 31.5f)))));
            circleTexture.Apply();
            circle = Sprite.Create(circleTexture, new Rect(0, 0, 64, 64), Vector2.one * 0.5f);
            Label(layout, BoardTitle, 48, 0.94f);
            Label(layout, "Drag to rotate • Match screws to their trays\nRemove all 3 screws to release a plate", 32, 0.87f);
            var sound = Panel("Sound", layout, new Vector2(0, 593), new Vector2(320, 64), new Color(0.22f, 0.23f, 0.25f));
            sound.gameObject.AddComponent<Button>().onClick.AddListener(() => FeedbackAudio.ToggleSound());
            soundLabel = Label(sound.transform, "", 28, 0.5f);
            soundLabel.rectTransform.sizeDelta = new Vector2(310, 60);
            status = Label(layout, "", 30, 0.115f);
            for (int i = 0; i < 4; i++)
            {
                int index = i;
                Vector2 position = new Vector2(i % 2 == 0 ? -248 : 248, i < 2 ? -365 : -605);
                rims[i] = Panel("Tray " + (i + 1), layout, position, new Vector2(460, 214), Color.gray);
                var card = Panel("Inset", rims[i].transform, Vector2.zero, new Vector2(448, 202), new Color(0.18f, 0.19f, 0.21f));
                trayLabels[i] = Label(card.transform, "", 30, 0.82f);
                trayLabels[i].rectTransform.sizeDelta = new Vector2(430, 60);
                for (int h = 0; h < 3; h++)
                {
                    var rim = Panel("Hole rim", card.transform, new Vector2((h - 1) * 125, -12), new Vector2(80, 80), new Color(0.32f, 0.34f, 0.36f));
                    rim.sprite = circle;
                    holes[i, h] = Panel("Hole", rim.transform, Vector2.zero, new Vector2(64, 64), Color.black);
                    holes[i, h].sprite = circle;
                }
                if (i >= 2)
                {
                    var image = Panel("Test unlock", card.transform, new Vector2(0, -78), new Vector2(390, 44), new Color(0.39f, 0.30f, 0.18f));
                    unlocks[i] = image.gameObject.AddComponent<Button>();
                    unlocks[i].onClick.AddListener(() => puzzle.UnlockTestTray(index));
                    var text = Label(image.transform, "TEST UNLOCK", 26, 0.5f);
                    text.rectTransform.sizeDelta = new Vector2(380, 42);
                }
            }
            var reset = Panel("Restart", layout, new Vector2(225, -861), new Vector2(400, 92), new Color(0.42f, 0.27f, 0.12f));
            reset.gameObject.AddComponent<Button>().onClick.AddListener(interaction.ResetExperiment);
            Label(reset.transform, "RESTART", 32, 0.5f).rectTransform.sizeDelta = new Vector2(400, 80);
            var boards = Panel("Boards", layout, new Vector2(-225, -861), new Vector2(400, 92), new Color(0.22f, 0.23f, 0.25f));
            boards.gameObject.AddComponent<Button>().onClick.AddListener(ThreeDBoardNavigation.OpenSelector);
            Label(boards.transform, "BOARDS", 32, 0.5f).rectTransform.sizeDelta = new Vector2(400, 80);
            puzzle.HoleScreenPosition = (tray, hole) => RectTransformUtility.WorldToScreenPoint(null, holes[tray, hole].transform.position);
            Update();
        }
        private Text Label(Transform parent, string value, int size, float height)
        {
            Text label = PrototypeLevelBuilder.CreateText("Label", parent, value, size,
                TextAnchor.MiddleCenter, new Color(0.95f, 0.87f, 0.7f));
            label.raycastTarget = false;
            PrototypeLevelBuilder.SetRect(label.rectTransform, new Vector2(0.5f, height),
                new Vector2(0.5f, height), Vector2.zero, new Vector2(1000f, 140f));
            return label;
        }

        private static Color TrayColor(ScrewColorId color)
        {
            return color == ScrewColorId.Red ? new Color(0.95f, 0.26f, 0.22f) :
                color == ScrewColorId.Blue ? new Color(0.23f, 0.58f, 1f) : new Color(1f, 0.79f, 0.19f);
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ThreeDBoardNavigation.OpenSelector();
                return;
            }
            Rect safe = Screen.safeArea;
            soundLabel.text = FeedbackAudio.IsSoundEnabled ? "SOUND: ON" : "SOUND: OFF";
            float scale = Mathf.Min(safe.width / 1080f, safe.height / 1920f);
            layout.position = safe.center;
            layout.localScale = Vector3.one * scale;
            view.fieldOfView = Mathf.Max(65f, 2f * Mathf.Atan(2.6f / (9f * view.aspect)) * Mathf.Rad2Deg);
            interaction.Radio.position = view.ScreenToWorldPoint(new Vector3(safe.center.x, safe.center.y + ModelVerticalOffset * scale, 9f));
            status.text = puzzle.State == Radio3DPuzzle.PuzzleState.Won ? WinTitle + "  •  All " + puzzle.TotalPlateCount + " plates removed" :
                string.IsNullOrEmpty(puzzle.Message) ? "Cleared " + puzzle.ClearedCount + " / " + interaction.TotalCount + "  •  Plates " + puzzle.ReleasedPlateCount + " / " + puzzle.TotalPlateCount : puzzle.Message;
            for (int i = 0; i < 4; i++)
            {
                var tray = puzzle.Trays[i];
                Color color = tray.IsOpen && tray.HasColor ? TrayColor(tray.Color) : new Color(0.38f, 0.39f, 0.42f);
                rims[i].color = color;
                trayLabels[i].text = !tray.IsOpen ? "LOCKED • BONUS TRAY" : !tray.HasColor ? "COMPLETE" : tray.Color.ToString().ToUpperInvariant() + "  " + tray.Count + " / 3";
                for (int h = 0; h < 3; h++) holes[i, h].color = h < tray.Count ? color : new Color(0.07f, 0.08f, 0.10f);
                if (unlocks[i] != null)
                {
                    unlocks[i].gameObject.SetActive(!tray.IsOpen);
                    unlocks[i].interactable = puzzle.CanInteract;
                }
            }
        }
        protected void OnDestroy()
        {
            if (circle != null) Destroy(circle);
            if (circleTexture != null) Destroy(circleTexture);
            foreach (Material material in materials) if (material != null) Destroy(material);
            foreach (Mesh mesh in modelMeshes) if (mesh != null) Destroy(mesh);
        }
    }
}


