using System.Collections.Generic;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>A second shape using the same tray, depth, input and feedback rules as the radio.</summary>
    public sealed class ToyCar3DPrototypeBootstrap : Radio3DPrototypeBootstrap
    {
        protected override string BoardTitle => "THE RACE CAR WORKSHOP";
        protected override ScrewColorId[] TraySequence => new[] { ScrewColorId.Red, ScrewColorId.Blue, ScrewColorId.Red, ScrewColorId.Blue, ScrewColorId.Yellow };
        protected override string BoardId => ThreeDBoardProgress.ToyCar;
        protected override float ModelVerticalOffset => 210f;
        protected override string WinTitle => "RACE CAR CLEARED!";

        protected override void BuildModel(out Transform radio, out List<Radio3DScrew> screws, out Radio3DPlate[] plates)
        {
            Camera.main.gameObject.AddComponent<WorkshopAirportBackdrop>().Configure("Art/Workshop/Racetrack-Background", "Racetrack Background");
            radio = new GameObject("Rotating Toy Car").transform;
            Material paint = Finish(new Color(0.88f, 0.12f, 0.075f), .12f, .52f);
            Material dark = Finish(new Color(0.045f, 0.065f, 0.085f), .02f, .22f);
            Material silver = Finish(new Color(0.62f, 0.72f, 0.76f), .55f, .50f);
            Material glass = Finish(new Color(0.07f, 0.26f, 0.36f), .22f, .65f);
            Material cream = Finish(new Color(0.98f, 0.93f, 0.77f), .04f, .40f);
            Material redLamp = Material(new Color(0.8f, 0.07f, 0.035f));
            Material lamp = Material(new Color(1f, 0.86f, 0.48f));
            RoundedBody("Car Chassis", radio, new Vector3(0, -0.43f, 0), new Vector3(3.9f, 0.18f, 1.3f), 0.07f, dark);
            foreach (float axle in new[] { -1.35f, 1.35f })
            {
                Transform shaft = Shape("Axle", PrimitiveType.Cylinder, radio, new Vector3(axle, -0.65f, 0), new Vector3(0.15f, 0.95f, 0.15f), silver);
                shaft.localRotation = Quaternion.Euler(90, 0, 0);
            }
            RoundedBody("Cabin", radio, new Vector3(-0.25f, .79f, 0), new Vector3(1.78f, .72f, 1.42f), .29f, glass);
            RoundedBody("Race Roof", radio, new Vector3(-.30f, 1.12f, 0), new Vector3(1.28f, .16f, 1.22f), .075f, paint);
            foreach (float stripe in new[] { -.22f, .22f })
                RoundedBody("Roof Racing Stripe", radio, new Vector3(-.30f, 1.205f, stripe), new Vector3(1.08f, .018f, .19f), .008f, cream);
            foreach (float side in new[] { -1f, 1f })
            {
                RoundedBody("Window Pillar", radio, new Vector3(-.45f, .85f, side * .712f), new Vector3(.07f, .40f, .035f), .015f, paint);
                foreach (float axle in new[] { -1.35f, 1.35f })
                {
                    Transform wheel = Shape("Wheel", PrimitiveType.Cylinder, radio, new Vector3(axle, -0.65f, side * 0.91f), new Vector3(.96f, 0.17f, .96f), dark);
                    wheel.localRotation = Quaternion.Euler(90, 0, 0);
                    Transform hub = Shape("Wheel Hub", PrimitiveType.Cylinder, radio, new Vector3(axle, -0.65f, side * 1.07f), new Vector3(0.44f, 0.025f, 0.44f), silver);
                    hub.localRotation = Quaternion.Euler(90, 0, 0);
                    Transform rim = Shape("Cream Wheel Rim", PrimitiveType.Cylinder, radio, new Vector3(axle, -0.65f, side * 1.075f), new Vector3(0.65f, 0.025f, 0.65f), cream);
                    rim.localRotation = Quaternion.Euler(90, 0, 0);
                    hub.localPosition = new Vector3(axle, -0.65f, side * 1.11f);
                    for (int spoke = 0; spoke < 5; spoke++)
                    {
                        float angle = spoke * Mathf.PI * 2 / 5f;
                        var blade = RoundedBody("Race Wheel Spoke", radio,
                            new Vector3(axle + Mathf.Cos(angle) * .14f, -.65f + Mathf.Sin(angle) * .14f, side * 1.145f),
                            new Vector3(.26f, .075f, .025f), .012f, dark);
                        blade.localRotation = Quaternion.Euler(0, 0, spoke * 72);
                    }
                }
            }
            Transform hood = RoundedBody("Hood", radio, new Vector3(1.38f, 0.55f, 0), new Vector3(1.25f, 0.20f, 1.7f), 0.09f, paint);
            hood.localRotation = Quaternion.Euler(0, 0, -6);
            Transform trunk = RoundedBody("Trunk", radio, new Vector3(-1.59f, 0.55f, 0), new Vector3(0.8f, 0.20f, 1.7f), 0.09f, paint);
            plates = new Radio3DPlate[5];
            string[] names = { "Near Side Assembly", "Far Side Assembly", "Rear Bumper Assembly", "Nose Assembly", "Inner Side Assembly" };
            for (int i = 0; i < plates.Length; i++)
            {
                plates[i] = new GameObject(names[i]).AddComponent<Radio3DPlate>();
                plates[i].transform.SetParent(radio, false);
            }
            hood.SetParent(plates[3].transform, false);
            trunk.SetParent(plates[2].transform, false);
            foreach (float stripe in new[] { -.22f, .22f })
            {
                var hoodStripe = RoundedBody("Hood Racing Stripe", plates[3].transform, new Vector3(1.38f, .655f, stripe),
                    new Vector3(1.03f, .018f, .19f), .008f, cream);
                hoodStripe.localRotation = Quaternion.Euler(0, 0, -6);
                RoundedBody("Trunk Racing Stripe", plates[2].transform, new Vector3(-1.59f, .655f, stripe),
                    new Vector3(.60f, .018f, .19f), .008f, cream);
                RoundedBody("Nose Racing Stripe", plates[3].transform, new Vector3(2.107f, -.02f, stripe),
                    new Vector3(.018f, .55f, .19f), .008f, cream);
            }
            foreach (float side in new[] { -.57f, .57f })
                RoundedBody("Spoiler Support", plates[2].transform, new Vector3(-1.67f, .76f, side),
                    new Vector3(.11f, .34f, .10f), .045f, dark);
            RoundedBody("Rear Spoiler", plates[2].transform, new Vector3(-1.67f, .97f, 0),
                new Vector3(.42f, .13f, 2.02f), .06f, cream);
            foreach (float side in new[] { -1f, 1f })
                RoundedBody("Spoiler End Cap", plates[2].transform, new Vector3(-1.67f, 1.01f, side * .99f),
                    new Vector3(.49f, .24f, .09f), .04f, paint);
            RoundedBody("Rear Underside Mount", plates[2].transform, new Vector3(-1.10f, -.56f, 0),
                new Vector3(1.70f, .10f, .62f), .045f, cream);
            // Contrasting seats make the vertical fasteners legible on the painted hood
            // and belly. They belong to the same assemblies as the screws they surround.
            foreach (float side in new[] { -.52f, .52f })
            {
                var seat = Shape("Hood Screw Seat", PrimitiveType.Cylinder, plates[3].transform,
                    hood.localPosition + hood.localRotation * new Vector3(0, .112f, side),
                    new Vector3(.36f, .012f, .36f), cream);
                seat.localRotation = hood.localRotation;
                seat.GetComponent<Collider>().enabled = false;
                Destroy(seat.GetComponent<Collider>());
            }
            foreach (float x in new[] { -1.65f, -.55f })
            {
                var seat = Shape("Underside Screw Seat", PrimitiveType.Cylinder, plates[2].transform,
                    new Vector3(x, -.622f, 0), new Vector3(.36f, .012f, .36f), silver);
                seat.GetComponent<Collider>().enabled = false;
                Destroy(seat.GetComponent<Collider>());
            }
            RaceSide("Near Body Panel", plates[0].transform, -.86f, paint);
            RaceSide("Far Body Panel", plates[1].transform, .86f, paint);
            for (int face = 0; face < 2; face++)
            {
                float side = face == 0 ? -1f : 1f;
                RoundedBody("Cream Sill", plates[face].transform, new Vector3(0, -0.27f, side * 0.935f), new Vector3(1.65f, 0.09f, 0.045f), 0.02f, cream);
                RoundedBody("Side Racing Stripe", plates[face].transform, new Vector3(-.60f, -.13f, side * .94f),
                    new Vector3(.66f, .09f, .028f), .012f, cream);
                var roundel = Shape("Race Number Roundel", PrimitiveType.Cylinder, plates[face].transform,
                    new Vector3(.57f, .04f, side * .945f), new Vector3(.43f, .009f, .43f), cream);
                roundel.localRotation = Quaternion.Euler(90, 0, 0);
                RoundedBody("Race Number Seven Top", plates[face].transform, new Vector3(.57f, .13f, side * .96f),
                    new Vector3(.18f, .035f, .016f), .007f, dark);
                var seven = RoundedBody("Race Number Seven Stem", plates[face].transform, new Vector3(side < 0 ? .58f : .56f, .035f, side * .96f),
                    new Vector3(.035f, .20f, .016f), .007f, dark);
                seven.localRotation = Quaternion.Euler(0, 0, side < 0 ? -22 : 22);
            }
            for (int i = 2; i < 4; i++)
            {
                float end = i == 2 ? -1f : 1f;
                RoundedBody("End Panel", plates[i].transform, new Vector3(end * 2.04f, i == 3 ? -.02f : .08f, 0),
                    new Vector3(0.12f, i == 3 ? .72f : .92f, 1.7f), 0.055f, paint);
                RoundedBody("Bumper", plates[i].transform, new Vector3(end * 2.12f, -0.32f, 0), new Vector3(0.22f, 0.19f, 1.86f), 0.085f, cream);
                foreach (float side in new[] { -1f, 1f })
                    RoundedBody("Lamp", plates[i].transform, new Vector3(end * 2.10f, i == 3 ? .22f : .32f, side * 0.6f), new Vector3(0.09f, 0.16f, 0.3f), 0.04f, i == 2 ? redLamp : lamp);
                RoundedBody("End Grille", plates[i].transform, new Vector3(end * 2.115f, i == 3 ? .23f : .33f, 0), new Vector3(0.04f, 0.14f, 0.60f), 0.018f, dark);
                for (int bar = -2; bar <= 2; bar++)
                    Shape("Grille Bar", PrimitiveType.Cube, plates[i].transform, new Vector3(end * 2.14f, i == 3 ? .23f : .33f, bar * 0.1f), new Vector3(0.015f, 0.11f, 0.025f), silver);
            }
            Shape("Inner Side Plate", PrimitiveType.Cube, plates[4].transform, new Vector3(0, 0.08f, -0.73f), new Vector3(2.7f, 0.88f, 0.04f), silver);
            Material copper = Material(new Color(0.68f, 0.43f, 0.16f));
            // The blue-fastened motor carrier removes as one assembly.
            Transform motor = Shape("Motor", PrimitiveType.Cylinder, plates[4].transform,
                new Vector3(0, -0.15f, -0.60f), new Vector3(0.36f, 0.62f, 0.36f), copper);
            motor.localRotation = Quaternion.Euler(0, 0, 90);
            foreach (float end in new[] { -1f, 1f })
            {
                Transform cap = Shape("Motor End Cap", PrimitiveType.Cylinder, plates[4].transform,
                    new Vector3(end * 0.62f, -0.15f, -0.60f), new Vector3(0.38f, 0.06f, 0.38f), dark);
                cap.localRotation = Quaternion.Euler(0, 0, 90);
            }
            RoundedBody("Motor Inspection Window", plates[4].transform, new Vector3(0, -0.16f, -0.765f),
                new Vector3(1.05f, 0.24f, 0.025f), 0.012f, dark);
            for (int winding = -3; winding <= 3; winding++)
                Shape("Copper Winding", PrimitiveType.Cube, plates[4].transform,
                    new Vector3(winding * 0.12f, -0.16f, -0.79f), new Vector3(0.055f, 0.18f, 0.035f), copper);
            RoundedBody("Battery Holder", radio, new Vector3(0, -0.23f, 0.20f), new Vector3(1.9f, 0.32f, 0.55f), 0.08f, dark);
            Transform battery = Shape("Cream Battery", PrimitiveType.Cylinder, radio, new Vector3(0, -0.06f, 0.20f), new Vector3(0.26f, 0.75f, 0.26f), cream);
            battery.localRotation = Quaternion.Euler(0, 0, 90);
            Material[] colors = { Material(new Color(0.9f, 0.12f, 0.08f)), Material(new Color(0.05f, 0.4f, 0.95f)), Material(new Color(1f, 0.7f, 0.04f)) };
            Vector3[] normals = { Vector3.back, Vector3.forward, Vector3.left, Vector3.right, Vector3.back };
            string[] prefixes = { "Near", "Far", "Rear", "Nose", "Inner" };
            int[,] pattern = { { 0, 1, 1 }, { 1, 2, 0 }, { 2, 0, 1 }, { 0, 0, 1 }, { 0, 1, 2 } };
            screws = new List<Radio3DScrew>();
            for (int face = 0; face < 5; face++)
            {
                var fasteners = new Radio3DScrew[3];
                for (int i = 0; i < 3; i++)
                {
                    int color = pattern[face, i];
                    Vector3 position = face < 2 ? new Vector3((i - 1) * 1.15f, i == 1 ? 0.24f : 0.04f, face == 0 ? -0.98f : 0.98f) :
                        face < 4 ? new Vector3(face == 2 ? -2.16f : 2.16f, 0.02f, (i - 1) * 0.53f) :
                        new Vector3((i - 1) * 0.85f, 0.14f, -0.79f);
                    Vector3 normal = normals[face];
                    if (face == 3 && i > 0)
                    {
                        position = hood.localPosition + hood.localRotation * new Vector3(0, .16f, i == 1 ? -.52f : .52f);
                        normal = hood.localRotation * Vector3.up;
                    }
                    if (face == 2 && i > 0)
                    {
                        position = new Vector3(i == 1 ? -1.65f : -.55f, -.68f, 0);
                        normal = Vector3.down;
                    }
                    fasteners[i] = Screw(prefixes[face] + " " + i, radio, position, normal, colors[color], dark);
                    fasteners[i].Initialize((ScrewColorId)color, face == 4 ? plates[0] : null);
                    screws.Add(fasteners[i]);
                }
                plates[face].Configure(fasteners, normals[face]);
            }
            radio.localRotation = Quaternion.Euler(0, 20, 0);
        }

        private void RaceSide(string name, Transform parent, float z, Material paint)
        {
            // A beveled strip follows the tire openings rather than hiding them behind a box.
            const int steps = 96;
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            for (int i = 0; i <= steps; i++)
            {
                float x = Mathf.Lerp(-1.95f, 1.95f, i / (float)steps);
                float high = x > 1.5f ? Mathf.Lerp(.50f, .37f, (x - 1.5f) / .45f) : .50f;
                float low = -.38f;
                foreach (float axle in new[] { -1.35f, 1.35f })
                {
                    float d = Mathf.Abs(x - axle);
                    if (d < .57f) low = Mathf.Max(low, -.65f + Mathf.Sqrt(.57f * .57f - d * d));
                }
                vertices.AddRange(new[] {
                    new Vector3(x,low+.035f,z-.06f), new Vector3(x,high-.035f,z-.06f),
                    new Vector3(x,high,z-.025f), new Vector3(x,high,z+.025f),
                    new Vector3(x,high-.035f,z+.06f), new Vector3(x,low+.035f,z+.06f),
                    new Vector3(x,low,z+.025f), new Vector3(x,low,z-.025f)
                });
                if (i == steps) continue;
                for (int j = 0; j < 8; j++)
                {
                    int a = i * 8 + j, b = i * 8 + (j + 1) % 8;
                    triangles.AddRange(new[] { a, b, a + 8, b, b + 8, a + 8 });
                }
            }
            for (int j = 1; j < 7; j++)
            {
                triangles.AddRange(new[] { 0, j + 1, j });
                int end = steps * 8;
                triangles.AddRange(new[] { end, end + j, end + j + 1 });
            }
            var mesh = new Mesh { name = name + " Wheel Arch Mesh" };
            mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds(); modelMeshes.Add(mesh);
            var obj = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
            obj.transform.SetParent(parent, false);
            obj.GetComponent<MeshFilter>().sharedMesh = mesh;
            obj.GetComponent<MeshRenderer>().sharedMaterial = paint;
            obj.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        protected override void DecorateBoardUi(RectTransform parent)
        {
            var panel = new GameObject("Racetrack Header Backing", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            panel.transform.SetParent(parent, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1080, 440);
            rect.anchoredPosition = new Vector2(0, 740);
            var image = panel.GetComponent<UnityEngine.UI.Image>();
            image.color = new Color(.08f, .12f, .15f, .84f);
            image.raycastTarget = false;
        }

        private Material Finish(Color color, float metallic, float smoothness)
        {
            var material = Material(color);
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Glossiness", smoothness);
            return material;
        }
    }
}
