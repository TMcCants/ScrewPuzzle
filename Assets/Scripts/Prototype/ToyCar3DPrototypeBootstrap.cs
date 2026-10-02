using System.Collections.Generic;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>A second shape using the same tray, depth, input and feedback rules as the radio.</summary>
    public sealed class ToyCar3DPrototypeBootstrap : Radio3DPrototypeBootstrap
    {
        protected override string BoardTitle => "THE TOY CAR WORKSHOP";
        protected override string BoardId => ThreeDBoardProgress.ToyCar;
        protected override float ModelVerticalOffset => 250f;
        protected override string WinTitle => "TOY CAR CLEARED!";

        protected override void BuildModel(out Transform radio, out List<Radio3DScrew> screws, out Radio3DPlate[] plates)
        {
            radio = new GameObject("Rotating Toy Car").transform;
            Material paint = Material(new Color(0.12f, 0.63f, 0.62f));
            Material dark = Material(new Color(0.07f, 0.09f, 0.12f));
            Material silver = Material(new Color(0.55f, 0.64f, 0.68f));
            Material glass = Material(new Color(0.08f, 0.22f, 0.32f));
            Material cream = Material(new Color(0.96f, 0.88f, 0.69f));
            Material redLamp = Material(new Color(0.8f, 0.07f, 0.035f));
            Material lamp = Material(new Color(1f, 0.86f, 0.48f));
            Shape("Car Chassis", PrimitiveType.Cube, radio, new Vector3(0, -0.12f, 0), new Vector3(3.9f, 0.85f, 1.3f), dark);
            RoundedBody("Cabin", radio, new Vector3(-0.2f, 0.92f, 0), new Vector3(1.9f, 0.85f, 1.5f), 0.16f, paint);
            RoundedBody("Cream Roof", radio, new Vector3(-0.2f, 1.36f, 0), new Vector3(1.98f, 0.18f, 1.58f), 0.085f, cream);
            foreach (float side in new[] { -1f, 1f })
            {
                Shape("Side Window", PrimitiveType.Cube, radio, new Vector3(-0.2f, 0.94f, side * 0.76f), new Vector3(1.62f, 0.54f, 0.025f), glass);
                Shape("Window Pillar", PrimitiveType.Cube, radio, new Vector3(-0.2f, 0.94f, side * 0.78f), new Vector3(0.09f, 0.58f, 0.03f), paint);
                foreach (float axle in new[] { -1.35f, 1.35f })
                {
                    Transform wheel = Shape("Wheel", PrimitiveType.Cylinder, radio, new Vector3(axle, -0.65f, side * 0.91f), new Vector3(0.85f, 0.15f, 0.85f), dark);
                    wheel.localRotation = Quaternion.Euler(90, 0, 0);
                    Transform hub = Shape("Wheel Hub", PrimitiveType.Cylinder, radio, new Vector3(axle, -0.65f, side * 1.07f), new Vector3(0.44f, 0.025f, 0.44f), silver);
                    hub.localRotation = Quaternion.Euler(90, 0, 0);
                    Transform rim = Shape("Cream Wheel Rim", PrimitiveType.Cylinder, radio, new Vector3(axle, -0.65f, side * 1.075f), new Vector3(0.65f, 0.025f, 0.65f), cream);
                    rim.localRotation = Quaternion.Euler(90, 0, 0);
                    hub.localPosition = new Vector3(axle, -0.65f, side * 1.11f);
                    for (int spoke = 0; spoke < 6; spoke++)
                    {
                        float angle = spoke * Mathf.PI / 3f;
                        Shape("Wheel Bolt", PrimitiveType.Sphere, radio,
                            new Vector3(axle + Mathf.Cos(angle) * 0.14f, -0.65f + Mathf.Sin(angle) * 0.14f, side * 1.145f), Vector3.one * 0.065f, dark);
                    }
                }
            }
            Shape("Windshield", PrimitiveType.Cube, radio, new Vector3(0.76f, 0.94f, 0), new Vector3(0.025f, 0.54f, 1.28f), glass);
            Shape("Rear Window", PrimitiveType.Cube, radio, new Vector3(-1.16f, 0.94f, 0), new Vector3(0.025f, 0.54f, 1.28f), glass);
            RoundedBody("Hood", radio, new Vector3(1.38f, 0.55f, 0), new Vector3(1.25f, 0.20f, 1.7f), 0.09f, paint);
            RoundedBody("Trunk", radio, new Vector3(-1.59f, 0.55f, 0), new Vector3(0.8f, 0.20f, 1.7f), 0.09f, paint);
            plates = new Radio3DPlate[5];
            string[] names = { "Near Side Assembly", "Far Side Assembly", "Rear Bumper Assembly", "Nose Assembly", "Inner Side Assembly" };
            for (int i = 0; i < plates.Length; i++)
            {
                plates[i] = new GameObject(names[i]).AddComponent<Radio3DPlate>();
                plates[i].transform.SetParent(radio, false);
            }
            RoundedBody("Near Body Panel", plates[0].transform, new Vector3(0, 0.08f, -0.86f), new Vector3(3.9f, 0.92f, 0.12f), 0.055f, paint);
            RoundedBody("Far Body Panel", plates[1].transform, new Vector3(0, 0.08f, 0.86f), new Vector3(3.9f, 0.92f, 0.12f), 0.055f, paint);
            for (int face = 0; face < 2; face++)
            {
                float side = face == 0 ? -1f : 1f;
                RoundedBody("Cream Sill", plates[face].transform, new Vector3(0, -0.27f, side * 0.935f), new Vector3(3.5f, 0.09f, 0.045f), 0.02f, cream);
                RoundedBody("Door Handle", plates[face].transform, new Vector3(-0.55f, 0.40f, side * 0.94f), new Vector3(0.32f, 0.07f, 0.07f), 0.03f, cream);
            }
            for (int i = 2; i < 4; i++)
            {
                float end = i == 2 ? -1f : 1f;
                RoundedBody("End Panel", plates[i].transform, new Vector3(end * 2.04f, 0.08f, 0), new Vector3(0.12f, 0.92f, 1.7f), 0.055f, paint);
                RoundedBody("Bumper", plates[i].transform, new Vector3(end * 2.12f, -0.32f, 0), new Vector3(0.22f, 0.19f, 1.86f), 0.085f, cream);
                foreach (float side in new[] { -1f, 1f })
                    RoundedBody("Lamp", plates[i].transform, new Vector3(end * 2.10f, 0.32f, side * 0.6f), new Vector3(0.09f, 0.23f, 0.3f), 0.04f, i == 2 ? redLamp : lamp);
                RoundedBody("End Grille", plates[i].transform, new Vector3(end * 2.115f, 0.33f, 0), new Vector3(0.04f, 0.18f, 0.60f), 0.018f, dark);
                for (int bar = -2; bar <= 2; bar++)
                    Shape("Grille Bar", PrimitiveType.Cube, plates[i].transform, new Vector3(end * 2.14f, 0.33f, bar * 0.1f), new Vector3(0.015f, 0.14f, 0.025f), silver);
            }
            Shape("Inner Side Plate", PrimitiveType.Cube, plates[4].transform, new Vector3(0, 0.08f, -0.73f), new Vector3(2.7f, 0.88f, 0.04f), silver);
            Shape("Motor", PrimitiveType.Cube, radio, new Vector3(0, 0.08f, -0.67f), new Vector3(1.5f, 0.6f, 0.03f), Material(new Color(0.68f, 0.43f, 0.16f)));
            Material[] colors = { Material(new Color(0.9f, 0.12f, 0.08f)), Material(new Color(0.05f, 0.4f, 0.95f)), Material(new Color(1f, 0.7f, 0.04f)) };
            Vector3[] normals = { Vector3.back, Vector3.forward, Vector3.left, Vector3.right, Vector3.back };
            string[] prefixes = { "Near", "Far", "Rear", "Nose", "Inner" };
            screws = new List<Radio3DScrew>();
            for (int face = 0; face < 5; face++)
            {
                var fasteners = new Radio3DScrew[3];
                for (int i = 0; i < 3; i++)
                {
                    int color = face == 3 ? 0 : face == 4 ? 1 : i;
                    Vector3 position = face < 2 ? new Vector3((i - 1) * 1.15f, 0.14f, face == 0 ? -0.98f : 0.98f) :
                        face < 4 ? new Vector3(face == 2 ? -2.16f : 2.16f, 0.02f, (i - 1) * 0.53f) :
                        new Vector3((i - 1) * 0.85f, 0.14f, -0.79f);
                    fasteners[i] = Screw(prefixes[face] + " " + i, radio, position, normals[face], colors[color], dark);
                    fasteners[i].Initialize((ScrewColorId)color, face == 4 ? plates[0] : null);
                    screws.Add(fasteners[i]);
                }
                plates[face].Configure(fasteners, normals[face]);
            }
            radio.localRotation = Quaternion.Euler(0, 20, 0);
        }
    }
}
