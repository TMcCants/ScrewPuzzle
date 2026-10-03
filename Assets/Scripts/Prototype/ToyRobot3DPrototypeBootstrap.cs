using System.Collections.Generic;
using UnityEngine;

namespace ScrewPuzzle
{
    public sealed class ToyRobot3DPrototypeBootstrap : Radio3DPrototypeBootstrap
    {
        protected override string BoardTitle => "THE TOY ROBOT WORKSHOP";
        protected override string BoardId => ThreeDBoardProgress.ToyRobot;
        protected override float ModelVerticalOffset => 110f;
        protected override string WinTitle => "TOY ROBOT CLEARED!";

        protected override void BuildModel(out Transform radio, out List<Radio3DScrew> screws, out Radio3DPlate[] plates)
        {
            radio = new GameObject("Rotating Toy Robot").transform;
            Material coral = Material(new Color(0.78f, 0.27f, 0.15f));
            Material cream = Material(new Color(0.96f, 0.88f, 0.69f));
            Material dark = Material(new Color(0.08f, 0.13f, 0.16f));
            Material brass = Material(new Color(0.78f, 0.54f, 0.22f));
            Material teal = Material(new Color(0.08f, 0.52f, 0.5f));
            RoundedBody("Robot Chassis", radio, Vector3.zero, new Vector3(2.35f, 1.65f, 1.05f), 0.12f, dark);
            RoundedBody("Neck", radio, new Vector3(0, 1.02f, 0), new Vector3(0.5f, 0.3f, 0.5f), 0.08f, brass);
            RoundedBody("Robot Head", radio, new Vector3(0, 1.52f, 0), new Vector3(1.7f, 0.8f, 1.15f), 0.16f, coral);
            RoundedBody("Face", radio, new Vector3(0, 1.52f, -0.59f), new Vector3(1.42f, 0.61f, 0.08f), 0.035f, cream);
            foreach (float side in new[] { -1f, 1f })
            {
                Shape("Eye", PrimitiveType.Sphere, radio, new Vector3(side * 0.39f, 1.64f, -0.66f), new Vector3(0.25f, 0.25f, 0.10f), dark);
                Shape("Eye Glint", PrimitiveType.Sphere, radio, new Vector3(side * 0.39f - 0.035f, 1.69f, -0.715f), Vector3.one * 0.06f, cream);
                RoundedBody("Shoulder", radio, new Vector3(side * 1.55f, 0.93f, 0), new Vector3(0.42f, 0.40f, 0.55f), 0.16f, brass);
                RoundedBody("Arm", radio, new Vector3(side * 1.85f, 0.55f, 0.1f), new Vector3(0.35f, 0.95f, 0.45f), 0.16f, coral);
                RoundedBody("Hand", radio, new Vector3(side * 1.85f, -0.02f, 0.1f), new Vector3(0.45f, 0.36f, 0.52f), 0.15f, cream);
                RoundedBody("Leg", radio, new Vector3(side * 0.65f, -1.19f, 0), new Vector3(0.42f, 0.65f, 0.5f), 0.1f, brass);
                RoundedBody("Boot", radio, new Vector3(side * 0.65f, -1.58f, -0.15f), new Vector3(0.85f, 0.34f, 1f), 0.15f, coral);
            }
            for (int i = -2; i <= 2; i++)
                RoundedBody("Smile Grille", radio, new Vector3(i * 0.13f, 1.35f, -0.66f), new Vector3(0.07f, 0.10f, 0.03f), 0.014f, dark);
            Shape("Antenna", PrimitiveType.Cylinder, radio, new Vector3(0, 2.03f, 0), new Vector3(0.07f, 0.15f, 0.07f), brass);
            Shape("Antenna Tip", PrimitiveType.Sphere, radio, new Vector3(0, 2.2f, 0), Vector3.one * 0.22f, teal);
            plates = new Radio3DPlate[5];
            string[] names = { "Chest", "Back", "Left", "Right", "Inner" };
            Vector3[] normals = { Vector3.back, Vector3.forward, Vector3.left, Vector3.right, Vector3.back };
            for (int face = 0; face < 5; face++)
            {
                plates[face] = new GameObject(names[face] + " Assembly").AddComponent<Radio3DPlate>();
                plates[face].transform.SetParent(radio, false);
                Vector3 pos = face == 0 ? new Vector3(0, 0, -0.75f) : face == 1 ? new Vector3(0, 0, 0.75f) :
                    face == 2 ? new Vector3(-1.3f, 0, 0) : face == 3 ? new Vector3(1.3f, 0, 0) : new Vector3(0, 0, -0.62f);
                RoundedBody(names[face] + " Panel", plates[face].transform, pos,
                    face == 2 || face == 3 ? new Vector3(0.08f, 1.8f, 1.4f) : new Vector3(2.5f, 1.8f, 0.08f), 0.035f, face == 4 ? brass : coral);
            }
            RoundedBody("Chest Display", plates[0].transform, new Vector3(0, -0.35f, -0.81f), new Vector3(1.5f, 0.55f, 0.06f), 0.025f, cream);
            for (int i = -2; i <= 2; i++)
                RoundedBody("Display Bar", plates[0].transform, new Vector3(i * 0.23f, -0.35f, -0.855f), new Vector3(0.12f, 0.18f + (2 - Mathf.Abs(i)) * 0.1f, 0.025f), 0.012f, teal);
            RoundedBody("Inner Motor", radio, new Vector3(0, -0.25f, -0.55f), new Vector3(1.6f, 0.7f, 0.05f), 0.02f, teal);
            for (int i = -1; i <= 1; i++)
            {
                Transform gear = Shape("Motor Gear", PrimitiveType.Cylinder, radio, new Vector3(i * 0.52f, -0.25f, -0.59f), new Vector3(0.38f, 0.025f, 0.38f), brass);
                gear.localRotation = Quaternion.Euler(90, 0, 0);
            }
            Material[] colors = { Material(new Color(0.9f, 0.12f, 0.08f)), Material(new Color(0.05f, 0.4f, 0.95f)), Material(new Color(1f, 0.7f, 0.04f)) };
            screws = new List<Radio3DScrew>();
            for (int face = 0; face < 5; face++)
            {
                var fasteners = new Radio3DScrew[3];
                for (int i = 0; i < 3; i++)
                {
                    int color = face == 3 ? 0 : face == 4 ? 1 : i;
                    Vector3 pos = face < 2 ? new Vector3((i - 1) * 0.82f, 0.4f, face == 0 ? -0.86f : 0.86f) :
                        face < 4 ? new Vector3(face == 2 ? -1.40f : 1.40f, (i - 1) * 0.58f, -0.48f) : new Vector3((i - 1) * 0.82f, 0.4f, -0.69f);
                    fasteners[i] = Screw(names[face] + " " + i, radio, pos, normals[face], colors[color], dark);
                    fasteners[i].Initialize((ScrewColorId)color, face == 4 ? plates[0] : null);
                    screws.Add(fasteners[i]);
                }
                plates[face].Configure(fasteners, normals[face]);
            }
            radio.localRotation = Quaternion.Euler(0, -18, 0);
        }
    }
}
