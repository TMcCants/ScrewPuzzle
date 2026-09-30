using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>Isolated interaction experiment; does not read or write puzzle progression.</summary>
    public sealed class Radio3DPrototypeBootstrap : MonoBehaviour
    {
        private readonly List<Material> materials = new List<Material>();
        private Radio3DInteraction interaction;
        private Text status;
        private Radio3DPuzzle puzzle;
        private Camera view;
        private RectTransform layout;
        private readonly Text[] trayLabels = new Text[4];
        private readonly Image[,] holes = new Image[4, 3];
        private readonly Image[] rims = new Image[4];
        private readonly Button[] unlocks = new Button[4];
        private Sprite circle;
        private Texture2D circleTexture;

        private void Awake()
        {
            Camera camera = new GameObject("3D Camera").AddComponent<Camera>();
            view = camera;
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

            Transform radio = new GameObject("Rotating Radio").transform;
            Material wood = Material(new Color(0.38f, 0.19f, 0.08f));
            Material brass = Material(new Color(0.7f, 0.49f, 0.22f));
            Material dark = Material(new Color(0.11f, 0.13f, 0.12f));
            Shape("Inner Chassis", PrimitiveType.Cube, radio, Vector3.zero, new Vector3(2.95f, 1.9f, 1.35f), dark);
            Shape("Top Frame", PrimitiveType.Cube, radio, new Vector3(0f, 1.14f, 0f), new Vector3(3.4f, 0.12f, 1.8f), wood);
            Shape("Bottom Frame", PrimitiveType.Cube, radio, new Vector3(0f, -1.14f, 0f), new Vector3(3.4f, 0.12f, 1.8f), wood);
            Material circuit = Material(new Color(0.12f, 0.34f, 0.23f));
            Shape("Circuit Board", PrimitiveType.Cube, radio, new Vector3(0f, 0f, -0.71f), new Vector3(2.7f, 1.7f, 0.06f), circuit);
            Shape("Receiver Module", PrimitiveType.Cube, radio, new Vector3(-0.65f, 0f, -0.78f), new Vector3(0.7f, 1.1f, 0.08f), brass);
            for (int index = 0; index < 3; index++)
                Shape("Circuit Trace " + index, PrimitiveType.Cube, radio, new Vector3(0.55f, (index - 1) * 0.42f, -0.76f),
                    new Vector3(0.95f, 0.06f, 0.04f), brass);

            Radio3DPlate[] plates = new Radio3DPlate[4];
            string[] plateNames = { "Front Plate Assembly", "Rear Plate Assembly", "Left Plate Assembly", "Right Plate Assembly" };
            for (int index = 0; index < plates.Length; index++)
            {
                plates[index] = new GameObject(plateNames[index]).AddComponent<Radio3DPlate>();
                plates[index].transform.SetParent(radio, false);
            }
            Shape("Front Plate", PrimitiveType.Cube, plates[0].transform, new Vector3(0f, 0f, -0.92f),
                new Vector3(3.15f, 2.15f, 0.08f), brass);
            Shape("Rear Plate", PrimitiveType.Cube, plates[1].transform, new Vector3(0f, 0f, 0.92f),
                new Vector3(3.15f, 2.15f, 0.08f), dark);
            Shape("Speaker", PrimitiveType.Cube, plates[0].transform, new Vector3(-0.7f, -0.25f, -0.99f),
                new Vector3(1.1f, 1f, 0.05f), dark);
            Shape("Tuning Display", PrimitiveType.Cube, plates[0].transform, new Vector3(0.65f, -0.15f, -0.99f),
                new Vector3(1.1f, 0.5f, 0.05f), dark);
            Shape("Left Panel", PrimitiveType.Cube, plates[2].transform, new Vector3(-1.72f, 0f, 0f),
                new Vector3(0.08f, 2.1f, 1.6f), brass);
            Shape("Right Panel", PrimitiveType.Cube, plates[3].transform, new Vector3(1.72f, 0f, 0f),
                new Vector3(0.08f, 2.1f, 1.6f), brass);

            Material[] colors = { Material(new Color(0.9f, 0.12f, 0.08f)),
                Material(new Color(0.05f, 0.4f, 0.95f)), Material(new Color(1f, 0.7f, 0.04f)) };
            var screws = new List<Radio3DScrew>();
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
            for (int index = 0; index < plates.Length; index++)
                plates[index].Configure(new[] { screws[index], screws[index + 4], screws[index + 8] }, normals[index]);
            puzzle = gameObject.AddComponent<Radio3DPuzzle>();
            puzzle.Configure(camera, screws.ToArray(), plates);
            interaction = gameObject.AddComponent<Radio3DInteraction>();
            interaction.Configure(camera, radio, screws.ToArray());
            BuildUi();
        }

        private Material Material(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline == null)
                shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.color = color;
            materials.Add(material);
            return material;
        }

        private Transform Shape(string name, PrimitiveType type, Transform parent,
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

        private Radio3DScrew Screw(string name, Transform radio, Vector3 position, Vector3 normal,
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
            Label(layout, "THE RADIO WORKSHOP", 48, 0.94f);
            Label(layout, "Drag to rotate • Match screws to their trays\nRemove all 3 screws to release a plate", 32, 0.87f);
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
            var reset = Panel("Restart", layout, new Vector2(0, -861), new Vector2(420, 92), new Color(0.42f, 0.27f, 0.12f));
            reset.gameObject.AddComponent<Button>().onClick.AddListener(interaction.ResetExperiment);
            Label(reset.transform, "RESTART", 32, 0.5f).rectTransform.sizeDelta = new Vector2(400, 80);
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

        private void Update()
        {
            Rect safe = Screen.safeArea;
            float scale = Mathf.Min(safe.width / 1080f, safe.height / 1920f);
            layout.position = safe.center;
            layout.localScale = Vector3.one * scale;
            view.fieldOfView = Mathf.Max(65f, 2f * Mathf.Atan(2.6f / (9f * view.aspect)) * Mathf.Rad2Deg);
            interaction.Radio.position = view.ScreenToWorldPoint(new Vector3(safe.center.x, safe.center.y + 250f * scale, 9f));
            status.text = puzzle.State == Radio3DPuzzle.PuzzleState.Won ? "RADIO CLEARED!  •  All 4 plates removed" :
                string.IsNullOrEmpty(puzzle.Message) ? "Cleared " + puzzle.ClearedCount + " / 12  •  Plates " + puzzle.ReleasedPlateCount + " / 4" : puzzle.Message;
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
        private void OnDestroy()
        {
            if (circle != null) Destroy(circle);
            if (circleTexture != null) Destroy(circleTexture);
            foreach (Material material in materials) if (material != null) Destroy(material);
        }
    }
}


