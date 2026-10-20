using System.Collections.Generic;
using UnityEngine;

namespace ScrewPuzzle
{
    public sealed class ToyRobot3DPrototypeBootstrap : Radio3DPrototypeBootstrap
    {
        protected override string BoardTitle => "THE TOY ROBOT WORKSHOP";
        protected override string BoardId => ThreeDBoardProgress.ToyRobot;
        protected override ScrewColorId[] TraySequence => new[] { ScrewColorId.Red, ScrewColorId.Blue, ScrewColorId.Yellow, ScrewColorId.Red, ScrewColorId.Blue, ScrewColorId.Yellow };
        protected override float ModelVerticalOffset => 170f;
        protected override string WinTitle => "TOY ROBOT CLEARED!";

        protected override void BuildModel(out Transform radio, out List<Radio3DScrew> screws, out Radio3DPlate[] plates)
        {
            Camera.main.gameObject.AddComponent<WorkshopAirportBackdrop>().Configure("Art/Workshop/Robot-Workshop-Background", "Robot Workshop Background");
            radio = new GameObject("Rotating Toy Robot").transform;
            Material coral = Finish(new Color(.53f, .39f, .75f), .08f, .45f);
            Material cream = Finish(new Color(.98f, .92f, .77f), .02f, .35f);
            Material dark = Finish(new Color(.065f, .11f, .17f), .08f, .35f);
            Material brass = Finish(new Color(.86f, .66f, .30f), .20f, .40f);
            Material teal = Finish(new Color(.20f, .68f, .57f), .06f, .40f);
            Material blush = Material(new Color(.94f, .48f, .46f));
            var head = new GameObject("Head Details").transform;
            head.SetParent(radio, false);
            var limbs = new Transform[2];
            foreach (float side in new[] { -1f, 1f })
                RoundedBody("Robot Chassis Rail", radio, new Vector3(side * 1.12f, 0, 0), new Vector3(0.16f, 1.65f, 0.65f), 0.07f, dark);
            RoundedBody("Neck", head, new Vector3(0, 1.00f, 0), new Vector3(.5f, .26f, .5f), .12f, brass);
            RoundedBody("Robot Head", head, new Vector3(0, 1.46f, 0), new Vector3(2.05f, 1.10f, 1.25f), .42f, coral);
            RoundedPanel("Face", head, new Vector3(0, 1.47f, -.64f), new Vector2(1.70f, .82f), .26f, cream);
            foreach (float side in new[] { -1f, 1f })
            {
                Shape("Eye", PrimitiveType.Sphere, head, new Vector3(side * .39f, 1.58f, -.735f), new Vector3(.29f, .36f, .10f), dark);
                Shape("Eye Glint", PrimitiveType.Sphere, head, new Vector3(side * .39f - .04f, 1.65f, -.79f), new Vector3(.075f,.075f,.025f), cream);
                Shape("Rosy Cheek", PrimitiveType.Sphere, head, new Vector3(side * .61f, 1.32f, -.715f), new Vector3(.19f,.09f,.025f), blush);
                Shape("Ear", PrimitiveType.Sphere, head, new Vector3(side * 1.04f, 1.47f, 0), new Vector3(.28f,.45f,.48f), brass);
                var limb = new GameObject(side < 0 ? "Left Limb Details" : "Right Limb Details").transform;
                limb.SetParent(radio, false); limbs[side < 0 ? 0 : 1] = limb;
                Shape("Shoulder", PrimitiveType.Sphere, limb, new Vector3(side * 1.48f, .43f, .16f), new Vector3(.46f,.48f,.55f), brass);
                var arm = RoundedBody("Arm", limb, new Vector3(side * 1.68f, .12f, .16f), new Vector3(.40f,.55f,.48f), .19f, coral);
                arm.localRotation = Quaternion.Euler(0,0,side * 12);
                Shape("Hand", PrimitiveType.Sphere, limb, new Vector3(side * 1.74f, -.22f, .04f), new Vector3(.52f,.44f,.56f), cream);
                RoundedBody("Leg", limb, new Vector3(side * .62f, -1.00f, 0), new Vector3(.38f,.34f,.45f), .16f, brass);
                RoundedBody("Boot", limb, new Vector3(side * .62f, -1.25f, -.12f), new Vector3(.80f,.42f,.90f), .20f, coral);
                RoundedBody("Boot Sole", limb, new Vector3(side * .62f, -1.43f, -.12f), new Vector3(.76f,.09f,.84f), .04f, cream);
            }
            for (int i = -4; i <= 4; i++)
                Shape("Smile", PrimitiveType.Sphere, head, new Vector3(i * .047f, 1.23f + i * i * .005f, -.72f),
                    new Vector3(.065f,.055f,.025f), dark);
            Shape("Antenna", PrimitiveType.Cylinder, head, new Vector3(0, 2.07f, 0), new Vector3(.065f,.10f,.065f), brass);
            Shape("Antenna Tip", PrimitiveType.Sphere, head, new Vector3(0, 2.20f, 0), Vector3.one * .25f, teal);
            plates = new Radio3DPlate[6];
            string[] names = { "Chest", "Back", "Left", "Right", "Inner", "Core" };
            Vector3[] normals = { Vector3.back, Vector3.forward, Vector3.left, Vector3.right, Vector3.back, Vector3.back };
            for (int face = 0; face < plates.Length; face++)
            {
                plates[face] = new GameObject(names[face] + " Assembly").AddComponent<Radio3DPlate>();
                plates[face].transform.SetParent(radio, false);
                Vector3 pos = face == 0 ? new Vector3(0, 0, -0.75f) : face == 1 ? new Vector3(0, 0, 0.75f) :
                    face == 2 ? new Vector3(-1.3f, 0, 0) : face == 3 ? new Vector3(1.3f, 0, 0) : new Vector3(0, 0, face == 4 ? -0.62f : -0.48f);
                var panel = RoundedPanel(names[face] + " Panel", plates[face].transform, pos,
                    new Vector2(face == 2 || face == 3 ? 1.4f : 2.5f, 1.8f),
                    face < 4 ? .35f : .18f, face == 5 ? teal : face == 4 ? brass : coral);
                if (face == 2 || face == 3) panel.localRotation = Quaternion.Euler(0,90,0);
            }
            head.SetParent(plates[1].transform, false);
            limbs[0].SetParent(plates[2].transform, false);
            limbs[1].SetParent(plates[3].transform, false);
            RoundedBody("Left Top Mount", plates[2].transform, new Vector3(-1.30f, .92f, -.48f),
                new Vector3(.48f, .10f, .5f), .045f, brass);
            RoundedBody("Right Underside Mount", plates[3].transform, new Vector3(1.30f, -.92f, -.48f),
                new Vector3(.48f, .10f, .5f), .045f, brass);
            RoundedPanel("Chest Display", plates[0].transform, new Vector3(0, -.35f, -.81f), new Vector2(1.5f,.55f), .20f, cream);
            for (int i = -2; i <= 2; i++)
                RoundedBody("Display Bar", plates[0].transform, new Vector3(i * 0.23f, -0.35f, -0.855f), new Vector3(0.12f, 0.18f + (2 - Mathf.Abs(i)) * 0.1f, 0.025f), 0.012f, teal);
            RoundedBody("Core Badge", plates[5].transform, new Vector3(0, -0.4f, -0.535f), new Vector3(1.1f, 0.25f, 0.02f), 0.009f, cream);
            RoundedBody("Battery Cradle", radio, new Vector3(0, -0.45f, 0.05f), new Vector3(1.8f, 0.55f, 0.5f), 0.1f, dark);
            foreach (float side in new[] { -1f, 1f })
                Shape("Power Cell", PrimitiveType.Cylinder, radio, new Vector3(side * 0.38f, -0.4f, -0.05f), new Vector3(0.3f, 0.25f, 0.3f), cream);
            RoundedBody("Inner Motor", plates[4].transform, new Vector3(0, -0.4f, -0.675f), new Vector3(1.7f, 0.65f, 0.025f), 0.012f, dark);
            for (int i = -1; i <= 1; i++)
            {
                Transform gear = Shape("Motor Gear", PrimitiveType.Cylinder, plates[4].transform, new Vector3(i * 0.52f, -0.4f, -0.70f), new Vector3(0.4f, 0.015f, 0.4f), brass);
                gear.localRotation = Quaternion.Euler(90, 0, 0);
                Shape("Gear Hub", PrimitiveType.Sphere, plates[4].transform, new Vector3(i * 0.52f, -0.4f, -0.72f), new Vector3(0.12f, 0.12f, 0.035f), cream);
                for (int tooth = 0; tooth < 10; tooth++)
                {
                    float angle = tooth * Mathf.PI / 5;
                    Transform cog = Shape("Gear Tooth", PrimitiveType.Cube, plates[4].transform,
                        new Vector3(i * 0.52f + Mathf.Cos(angle) * 0.21f, -0.4f + Mathf.Sin(angle) * 0.21f, -0.70f), new Vector3(0.085f, 0.065f, 0.03f), brass);
                    cog.localRotation = Quaternion.Euler(0, 0, tooth * 36);
                }
            }
            RoundedBody("Core Processor", plates[5].transform, new Vector3(0, -0.12f, -0.535f), new Vector3(0.5f, 0.4f, 0.025f), 0.012f, dark);
            foreach (float side in new[] { -1f, 1f })
                for (int pin = 0; pin < 3; pin++)
                    Shape("Core Circuit Trace", PrimitiveType.Cube, plates[5].transform,
                        new Vector3(side * 0.43f, -0.24f + pin * 0.12f, -0.53f), new Vector3(0.35f, 0.025f, 0.018f), cream);
            Material[] colors = { Material(new Color(0.9f, 0.12f, 0.08f)), Material(new Color(0.05f, 0.4f, 0.95f)), Material(new Color(1f, 0.7f, 0.04f)) };
            int[,] pattern = { { 0, 1, 1 }, { 2, 0, 2 }, { 2, 0, 2 }, { 0, 1, 2 }, { 0, 1, 1 }, { 0, 1, 2 } };
            screws = new List<Radio3DScrew>();
            for (int face = 0; face < plates.Length; face++)
            {
                var fasteners = new Radio3DScrew[3];
                for (int i = 0; i < 3; i++)
                {
                    int color = pattern[face, i];
                    Vector3 pos = face < 2 ? new Vector3((i - 1) * 0.82f, i == 1 ? 0.62f : 0.24f, face == 0 ? -0.86f : 0.86f) :
                        face < 4 ? new Vector3(face == 2 ? -1.40f : 1.40f, (i - 1) * 0.58f, -0.48f) : new Vector3((i - 1) * 0.82f, i == 1 ? 0.50f : 0.1f, face == 4 ? -0.69f : -0.55f);
                    Vector3 normal = normals[face];
                    if (face == 2 && i == 2) { pos = new Vector3(-1.30f, 1.03f, -.48f); normal = Vector3.up; }
                    if (face == 3 && i == 0) { pos = new Vector3(1.30f, -1.03f, -.48f); normal = Vector3.down; }
                    fasteners[i] = Screw(names[face] + " " + i, radio, pos, normal, colors[color], dark);
                    fasteners[i].Initialize((ScrewColorId)color, face == 5 ? plates[4] : face == 4 ? plates[0] : null);
                    screws.Add(fasteners[i]);
                }
                plates[face].Configure(fasteners, normals[face]);
            }
            // Leave room for Reset View below the boots, preserving the world-size tap proxies.
            radio.localScale = Vector3.one * .9f;
            foreach (var screw in screws)
            {
                screw.transform.Find("Head").localScale /= .9f;
                screw.transform.Find("Slot").localScale /= .9f;
            }
            radio.localRotation = Quaternion.Euler(0, -18, 0);
        }

        private Transform RoundedPanel(string name, Transform parent, Vector3 position, Vector2 size, float radius, Material material)
        {
            // Round the silhouette independently of the thin panel depth.
            const int perCorner = 10, count = 4 * perCorner;
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            for (int ring = 0; ring < 4; ring++)
            {
                float inset = ring == 0 || ring == 3 ? .015f : 0;
                float depth = ring == 0 ? -.04f : ring == 1 ? -.025f : ring == 2 ? .025f : .04f;
                for (int corner = 0; corner < 4; corner++)
                {
                    Vector2 center = new Vector2((corner == 0 || corner == 3 ? 1 : -1) * (size.x * .5f - radius),
                        (corner < 2 ? 1 : -1) * (size.y * .5f - radius));
                    for (int step = 0; step < perCorner; step++)
                    {
                        float angle = (corner * 90f + step * 90f / (perCorner - 1)) * Mathf.Deg2Rad;
                        vertices.Add(new Vector3(center.x + Mathf.Cos(angle) * (radius-inset),
                            center.y + Mathf.Sin(angle) * (radius-inset), depth));
                    }
                }
            }
            for (int ring = 0; ring < 3; ring++)
                for (int j = 0; j < count; j++)
                {
                    int a = ring * count + j, b = ring * count + (j+1)%count;
                    triangles.AddRange(new[] { a,b,a+count,b,b+count,a+count });
                }
            vertices.Add(new Vector3(0,0,-.04f)); vertices.Add(new Vector3(0,0,.04f));
            for (int j = 0; j < count; j++)
            {
                triangles.AddRange(new[] { count*4,(j+1)%count,j });
                triangles.AddRange(new[] { count*4+1,count*3+j,count*3+(j+1)%count });
            }
            var mesh = new Mesh { name = name + " Rounded Panel Mesh" };
            mesh.SetVertices(vertices); mesh.SetTriangles(triangles,0); mesh.RecalculateNormals(); mesh.RecalculateBounds();
            modelMeshes.Add(mesh);
            var obj = new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer),typeof(MeshCollider));
            obj.transform.SetParent(parent,false); obj.transform.localPosition=position;
            obj.GetComponent<MeshFilter>().sharedMesh=mesh;
            obj.GetComponent<MeshRenderer>().sharedMaterial=material;
            obj.GetComponent<MeshCollider>().sharedMesh=mesh;
            return obj.transform;
        }

        protected override void DecorateBoardUi(RectTransform parent)
        {
            var panel = new GameObject("Robot Workshop Header Backing", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            panel.transform.SetParent(parent,false);
            var rect=panel.GetComponent<RectTransform>();
            rect.sizeDelta=new Vector2(1080,440); rect.anchoredPosition=new Vector2(0,740);
            var image=panel.GetComponent<UnityEngine.UI.Image>();
            image.color=new Color(.08f,.12f,.15f,.84f); image.raycastTarget=false;
        }

        private Material Finish(Color color,float metallic,float smoothness)
        {
            var material=Material(color); material.SetFloat("_Metallic",metallic); material.SetFloat("_Glossiness",smoothness);
            return material;
        }
    }
}
