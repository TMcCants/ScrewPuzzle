using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScrewPuzzle
{
    /// <summary>3D workshop board with completion separate from production puzzle progression.</summary>
    public class Radio3DPrototypeBootstrap : MonoBehaviour
    {
        private readonly List<Material> materials = new List<Material>();
        protected readonly List<Mesh> modelMeshes = new List<Mesh>();
        private Radio3DInteraction interaction;
        private Text status;
        private Text soundLabel;
        private Text musicLabel;
        private Text instructions;
        private WorkshopHint hint;
        private Radio3DPuzzle puzzle;
        private Camera view;
        private RectTransform layout;
        private readonly Text[] trayLabels = new Text[4];
        private readonly Image[,] holes = new Image[4, 3];
        private readonly Image[] rims = new Image[4];
        private readonly Button[] unlocks = new Button[4];
        private Sprite circle;
        private Texture2D circleTexture;
        private WorkshopTrayPresentation trayPresentation;
        private int replayVariant;

        protected void Awake()
        {
            WorkshopMusic.EnsurePlaying();
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
            var sequence = (ScrewColorId[])TraySequence.Clone();
            if(BoardId == ThreeDBoardProgress.Radio || BoardId == ThreeDBoardProgress.ToyCar || BoardId == ThreeDBoardProgress.ToyRobot)
            {
                replayVariant=WorkshopReplayLayout.Select(BoardId);
                if(replayVariant!=0)
                {
                    var colors=new[] { Material(TrayColor(ScrewColorId.Red)), Material(TrayColor(ScrewColorId.Blue)), Material(TrayColor(ScrewColorId.Yellow)) };
                    foreach(var screw in screws)
                    {
                        var color=WorkshopReplayLayout.Map(screw.ColorId,replayVariant);
                        screw.Recolor(color,colors[(int)color]);
                    }
                    for(int i=0;i<sequence.Length;i++) sequence[i]=WorkshopReplayLayout.Map(sequence[i],replayVariant);
                }
            }
            puzzle = gameObject.AddComponent<Radio3DPuzzle>();
            puzzle.Configure(camera, screws.ToArray(), plates, sequence);
            puzzle.Completed += () => ThreeDBoardProgress.MarkCompleted(BoardId);
            interaction = gameObject.AddComponent<Radio3DInteraction>();
            interaction.Configure(camera, radio, screws.ToArray());
            interaction.AllowPitch = AllowPitch;
            BuildUi();
            hint = gameObject.AddComponent<WorkshopHint>();
            hint.Configure(layout, puzzle, interaction);
            gameObject.AddComponent<WorkshopTutorial>().Configure(layout, instructions, puzzle, interaction, screws.ToArray());
            gameObject.AddComponent<WorkshopCompletion>().Configure(layout, puzzle, BoardId, WinTitle, () => ThreeDBoardNavigation.Replay(BoardId));
            puzzle.EnableResume(BoardId, LayoutSeed);
        }

        protected virtual string BoardTitle => "THE RADIO WORKSHOP";
        protected virtual bool AllowPitch => true;
        protected virtual bool StyledTrays => true;
        protected virtual float ScrewVisualScale => .56f;
        protected virtual void DecorateBoardUi(RectTransform parent)
        {
            if (BoardId != ThreeDBoardProgress.Radio) return;
            var panel = new GameObject("Music Shop Header Backing", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1080, 440);
            rect.anchoredPosition = new Vector2(0, 740);
            var image = panel.GetComponent<Image>();
            image.color = new Color(.08f, .12f, .15f, .84f);
            image.raycastTarget = false;
        }
        protected virtual int LayoutSeed => replayVariant;
        protected virtual string BoardId => ThreeDBoardProgress.Radio;
        protected virtual ScrewColorId[] TraySequence => new[] { ScrewColorId.Red, ScrewColorId.Blue, ScrewColorId.Yellow, ScrewColorId.Red, ScrewColorId.Blue, ScrewColorId.Yellow };
        protected virtual float ModelVerticalOffset => 180f;
        protected virtual string WinTitle => "RADIO CLEARED!";

        protected virtual void BuildModel(out Transform radio, out List<Radio3DScrew> screws, out Radio3DPlate[] plates)
        {
            Camera.main.gameObject.AddComponent<WorkshopAirportBackdrop>().Configure("Art/Workshop/Music-Shop-Background", "Music Shop Background");
            radio = new GameObject("Rotating Radio").transform;
            Material wood = Material(new Color(0.035f, 0.48f, 0.48f));
            Material brass = Material(new Color(0.95f, 0.87f, 0.68f));
            Material dark = Material(new Color(0.11f, 0.13f, 0.12f));
            // A hollow frame leaves an actual cavity as the assemblies come away.
            foreach (float x in new[] { -1.48f, 1.48f })
                RoundedBody("Chassis Rail", radio, new Vector3(x, 0, 0.15f), new Vector3(0.12f, 1.9f, 1.1f), 0.05f, dark);
            RoundedBody("Top Frame", radio, new Vector3(0f, 1.14f, 0f), new Vector3(3.5f, 0.24f, 1.85f), 0.1f, wood);
            RoundedBody("Bottom Frame", radio, new Vector3(0f, -1.14f, 0f), new Vector3(3.5f, 0.24f, 1.85f), 0.1f, wood);
            RoundedBody("Battery Cradle", radio, new Vector3(0, -0.67f, 0.30f), new Vector3(1.8f, 0.46f, 0.5f), 0.08f, dark);
            foreach (float x in new[] { -0.45f, 0.45f })
            {
                Transform battery = Shape("Toy Battery", PrimitiveType.Cylinder, radio, new Vector3(x, -0.63f, 0.26f), new Vector3(0.28f, 0.36f, 0.28f), brass);
                battery.localRotation = Quaternion.Euler(0, 0, 90);
            }
            Material circuit = Material(new Color(0.12f, 0.34f, 0.23f));
            plates = new Radio3DPlate[6];
            string[] plateNames = { "Front Plate Assembly", "Rear Plate Assembly", "Left Plate Assembly", "Right Plate Assembly", "Inner Front Plate Assembly", "Circuit Assembly" };
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
            RoundedBody("Carry Handle", radio, new Vector3(0, 1.82f, 0), new Vector3(2.25f, 0.23f, 0.30f), 0.11f, wood);
            Shape("Left Panel", PrimitiveType.Cube, plates[2].transform, new Vector3(-1.72f, 0f, 0f),
                new Vector3(0.08f, 2.1f, 1.6f), wood);
            Shape("Right Panel", PrimitiveType.Cube, plates[3].transform, new Vector3(1.72f, 0f, 0f),
                new Vector3(0.08f, 2.1f, 1.6f), wood);
            RoundedBody("Left Top Mount", plates[2].transform, new Vector3(-1.53f, 1.28f, .55f),
                new Vector3(.48f, .10f, .52f), .045f, brass);
            RoundedBody("Right Underside Mount", plates[3].transform, new Vector3(1.53f, -1.28f, .55f),
                new Vector3(.48f, .10f, .52f), .045f, brass);
            // The speaker baffle, cone and tuning mechanism are one removable assembly.
            RoundedBody("Inner Front Plate", plates[4].transform, new Vector3(0, 0, -0.62f), new Vector3(2.8f, 1.8f, 0.06f), 0.025f, brass);
            Transform driver = new GameObject("Speaker Driver").transform;
            driver.SetParent(plates[4].transform, false);
            Disc("Driver Frame", driver, new Vector3(-0.65f, -0.22f, -0.685f), 1.18f, 0.025f, dark);
            Disc("Speaker Cone", driver, new Vector3(-0.65f, -0.22f, -0.735f), 0.95f, 0.025f, Material(new Color(0.30f, 0.32f, 0.30f)));
            Shape("Speaker Dust Cap", PrimitiveType.Sphere, driver, new Vector3(-0.65f, -0.22f, -0.77f), new Vector3(0.36f, 0.36f, 0.09f), dark);
            RoundedBody("Tuner Housing", plates[4].transform, new Vector3(0.7f, 0.15f, -0.71f), new Vector3(0.65f, 0.52f, 0.10f), 0.045f, wood);
            for (int i = 0; i < 5; i++)
                Shape("Tuning Coil", PrimitiveType.Cylinder, plates[4].transform, new Vector3(0.47f + i * 0.11f, 0.18f, -0.80f), new Vector3(0.05f, 0.16f, 0.05f), brass);
            RoundedBody("Circuit Board", plates[5].transform, new Vector3(0, 0, -0.36f), new Vector3(2.65f, 1.65f, 0.06f), 0.025f, circuit);
            RoundedBody("Receiver Chip", plates[5].transform, new Vector3(-0.48f, 0.05f, -0.425f), new Vector3(0.7f, 0.62f, 0.07f), 0.03f, dark);
            for (int i = 0; i < 3; i++)
            {
                RoundedBody("Circuit Trace", plates[5].transform, new Vector3(0.63f, (i - 1) * 0.24f, -0.405f), new Vector3(0.65f, 0.04f, 0.018f), 0.008f, brass);
                Shape("Capacitor", PrimitiveType.Cylinder, plates[5].transform, new Vector3(0.62f + i * 0.16f, 0.15f, -0.445f), new Vector3(0.1f, 0.13f, 0.1f), dark);
            }
            Material[] colors = { Material(new Color(0.9f, 0.12f, 0.08f)),
                Material(new Color(0.05f, 0.4f, 0.95f)), Material(new Color(1f, 0.7f, 0.04f)) };
            screws = new List<Radio3DScrew>();
            for (int index = 0; index < 3; index++)
            {
                float x = (index - 1) * 1.1f;
                float y = (index - 1) * 0.7f;
                screws.Add(Screw("Front " + index, radio, new Vector3(x, 0.72f, -1.02f), Vector3.back, colors[index], dark));
                screws.Add(Screw("Back " + index, radio, new Vector3(x, 0.72f, 1.02f), Vector3.forward, colors[index], dark));
                screws.Add(Screw("Left " + index, radio, index == 2 ? new Vector3(-1.53f, 1.35f, .55f) : new Vector3(-1.82f, y, 0f),
                    index == 2 ? Vector3.up : Vector3.left, colors[index], dark));
                screws.Add(Screw("Right " + index, radio, index == 0 ? new Vector3(1.53f, -1.35f, .55f) : new Vector3(1.82f, y, 0f),
                    index == 0 ? Vector3.down : Vector3.right, colors[0], dark));
            }
            for (int index = 0; index < screws.Count; index++)
                screws[index].Initialize(index % 4 == 3 ? ScrewColorId.Red : (ScrewColorId)(index / 4));
            Vector3[] normals = { Vector3.back, Vector3.forward, Vector3.left, Vector3.right };
            for (int index = 0; index < normals.Length; index++)
                plates[index].Configure(new[] { screws[index], screws[index + 4], screws[index + 8] }, normals[index]);
            var innerScrews = new Radio3DScrew[3];
            for (int index = 0; index < innerScrews.Length; index++)
            {
                innerScrews[index] = Screw("Inner " + index, radio, new Vector3(index == 0 ? -1.08f : index == 1 ? 0.35f : 1.1f, index == 2 ? -0.58f : 0.65f, -0.71f), Vector3.back, colors[1], dark);
                innerScrews[index].Initialize(ScrewColorId.Blue, plates[0]);
                screws.Add(innerScrews[index]);
            }
            plates[4].Configure(innerScrews, Vector3.back);
            var circuitScrews = new Radio3DScrew[3];
            for (int index = 0; index < 3; index++)
            {
                circuitScrews[index] = Screw("Circuit " + index, radio,
                    new Vector3(index == 2 ? 0 : index == 0 ? -1.02f : 1.02f, index == 2 ? -0.58f : 0.59f, -0.48f), Vector3.back, colors[2], dark);
                circuitScrews[index].Initialize(ScrewColorId.Yellow, plates[4]);
                screws.Add(circuitScrews[index]);
            }
            plates[5].Configure(circuitScrews, Vector3.back);
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
            RefineScrewVisuals(screw);
            return screw;
        }

        private Mesh recessedHead;

        private void RefineScrewVisuals(Radio3DScrew screw)
        {
            // Original colliders stay in place; only rendered geometry changes.
            Transform head = screw.transform.Find("Head");
            head.GetComponent<MeshRenderer>().enabled = false;
            screw.transform.Find("Slot").GetComponent<MeshRenderer>().enabled = false;
            if (recessedHead == null)
            {
                recessedHead = CreateRecessedHead();
                modelMeshes.Add(recessedHead);
            }
            var visual = new GameObject("Head Visual", typeof(MeshFilter), typeof(MeshRenderer));
            visual.transform.SetParent(screw.transform, false);
            visual.transform.localPosition = -Vector3.up * .04f;
            visual.transform.localScale = head.localScale * ScrewVisualScale;
            visual.GetComponent<MeshFilter>().sharedMesh = recessedHead;
            visual.GetComponent<MeshRenderer>().sharedMaterial = head.GetComponent<MeshRenderer>().sharedMaterial;
        }

        private static Mesh CreateRecessedHead()
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var angles = new List<float>();
            const float arm = .34f;
            const float halfWidth = .065f;
            // Include exact cross corners so the recess has straight, crisp arms.
            for (int i = 0; i < 96; i++) angles.Add(i * Mathf.PI * 2 / 96);
            float corner = Mathf.Atan(halfWidth / arm);
            for (int quadrant = 0; quadrant < 4; quadrant++)
            {
                float start = quadrant * Mathf.PI / 2;
                angles.Add(start + corner);
                angles.Add(start + Mathf.PI / 4);
                angles.Add(start + Mathf.PI / 2 - corner);
            }
            angles.Sort();
            for (int i = angles.Count - 1; i > 0; i--)
                if (Mathf.Abs(angles[i] - angles[i - 1]) < .00001f) angles.RemoveAt(i);

            for (int i = 0; i < angles.Count; i++)
            {
                float a = angles[i], b = i + 1 == angles.Count ? Mathf.PI * 2 : angles[i + 1];
                Vector3 da = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a));
                Vector3 db = new Vector3(Mathf.Cos(b), 0, Mathf.Sin(b));
                float ra = CrossRadius(a, arm, halfWidth), rb = CrossRadius(b, arm, halfWidth);
                Vector3 lipA = da * ra + Vector3.up, lipB = db * rb + Vector3.up;
                Vector3 floorA = da * (ra * .72f) + Vector3.up * .15f;
                Vector3 floorB = db * (rb * .72f) + Vector3.up * .15f;
                Vector3 topA = da * .47f + Vector3.up, topB = db * .47f + Vector3.up;
                Vector3 rimA = da * .5f + Vector3.up * .8f, rimB = db * .5f + Vector3.up * .8f;
                Vector3 bottomA = da * .5f - Vector3.up, bottomB = db * .5f - Vector3.up;
                AddQuad(vertices, triangles, lipA, lipB, topB, topA, Vector3.up);
                AddQuad(vertices, triangles, lipA, floorA, floorB, lipB, -(da + db));
                AddTriangle(vertices, triangles, Vector3.up * .15f, floorA, floorB, Vector3.up);
                AddQuad(vertices, triangles, topA, topB, rimB, rimA, da + db + Vector3.up);
                AddQuad(vertices, triangles, rimA, rimB, bottomB, bottomA, da + db);
                AddTriangle(vertices, triangles, -Vector3.up, bottomA, bottomB, Vector3.down);
            }
            var mesh = new Mesh { name = "Workshop recessed Phillips head" };
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static float CrossRadius(float angle, float arm, float width)
        {
            float x = Mathf.Max(.00001f, Mathf.Abs(Mathf.Cos(angle)));
            float z = Mathf.Max(.00001f, Mathf.Abs(Mathf.Sin(angle)));
            return Mathf.Max(Mathf.Min(arm / x, width / z), Mathf.Min(width / x, arm / z));
        }

        private static void AddQuad(List<Vector3> vertices, List<int> indices, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal)
        {
            AddTriangle(vertices, indices, a, b, c, normal);
            AddTriangle(vertices, indices, a, c, d, normal);
        }

        private static void AddTriangle(List<Vector3> vertices, List<int> indices, Vector3 a, Vector3 b, Vector3 c, Vector3 normal)
        {
            if (Vector3.Cross(b - a, c - a).sqrMagnitude < .0000000001f) return;
            int start = vertices.Count;
            vertices.Add(a); vertices.Add(b); vertices.Add(c);
            indices.Add(start);
            bool forward = Vector3.Dot(Vector3.Cross(b - a, c - a), normal) > 0;
            indices.Add(start + (forward ? 1 : 2));
            indices.Add(start + (forward ? 2 : 1));
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
            DecorateBoardUi(layout);
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
            instructions = Label(layout, AllowPitch ? "Drag sideways to turn • Up/down to tilt\nMatch screws to their trays" :
                "Drag to rotate • Match screws to their trays\nRemove all 3 screws to release a plate", 32, 0.87f);
            var sound = Panel("Sound", layout, new Vector2(-330, 593), new Vector2(310, 64), new Color(0.22f, 0.23f, 0.25f));
            sound.gameObject.AddComponent<Button>().onClick.AddListener(() => FeedbackAudio.ToggleSound());
            soundLabel = Label(sound.transform, "", 28, 0.5f);
            soundLabel.rectTransform.sizeDelta = new Vector2(310, 60);
            var music = Panel("Music", layout, new Vector2(0, 593), new Vector2(310, 64), new Color(0.22f, 0.23f, 0.25f));
            music.gameObject.AddComponent<Button>().onClick.AddListener(WorkshopMusic.Toggle);
            musicLabel = Label(music.transform, "", 28, 0.5f);
            musicLabel.rectTransform.sizeDelta = new Vector2(300, 60);
            var hintButton = Panel("Hint", layout, new Vector2(330, 593), new Vector2(310, 64), new Color(0.13f, 0.39f, 0.36f));
            hintButton.gameObject.AddComponent<Button>().onClick.AddListener(() => hint.ShowHint());
            Label(hintButton.transform, "HINT", 28, 0.5f).rectTransform.sizeDelta = new Vector2(310, 60);
            if (AllowPitch)
            {
                var resetView = Panel("Reset View", layout, new Vector2(0, -150), new Vector2(300, 64), new Color(.13f, .23f, .29f));
                resetView.gameObject.AddComponent<Button>().onClick.AddListener(interaction.ResetView);
                Label(resetView.transform, "RESET VIEW", 26, .5f).rectTransform.sizeDelta = new Vector2(290, 60);
            }
            status = Label(layout, "", 30, 0.115f);
            if (StyledTrays)
            {
                trayPresentation = gameObject.AddComponent<WorkshopTrayPresentation>();
                trayPresentation.Configure(layout, puzzle, interaction);
            }
            for (int i = 0; !StyledTrays && i < 4; i++)
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
            if (!StyledTrays) puzzle.HoleScreenPosition = (tray, hole) => RectTransformUtility.WorldToScreenPoint(null, holes[tray, hole].transform.position);
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
            musicLabel.text = WorkshopMusic.IsEnabled ? "MUSIC: ON" : "MUSIC: OFF";
            soundLabel.text = FeedbackAudio.IsSoundEnabled ? "SFX: ON" : "SFX: OFF";
            float scale = Mathf.Min(safe.width / 1080f, safe.height / 1920f);
            layout.position = safe.center;
            layout.localScale = Vector3.one * scale;
            view.fieldOfView = Mathf.Max(65f, 2f * Mathf.Atan(2.6f / (9f * view.aspect)) * Mathf.Rad2Deg);
            interaction.Radio.position = view.ScreenToWorldPoint(new Vector3(safe.center.x, safe.center.y + ModelVerticalOffset * scale, 9f));
            status.text = puzzle.State == Radio3DPuzzle.PuzzleState.Won ? WinTitle + "  •  All " + puzzle.TotalPlateCount + " plates removed" :
                string.IsNullOrEmpty(puzzle.Message) ? "Cleared " + puzzle.ClearedCount + " / " + interaction.TotalCount + "  •  Plates " + puzzle.ReleasedPlateCount + " / " + puzzle.TotalPlateCount : puzzle.Message;
            if (hint != null && !string.IsNullOrEmpty(hint.Message) && puzzle.CanInteract) status.text = hint.Message;
            for (int i = 0; !StyledTrays && i < 4; i++)
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
