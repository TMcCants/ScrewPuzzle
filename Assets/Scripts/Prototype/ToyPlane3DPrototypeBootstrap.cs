using System.Collections.Generic;
using UnityEngine;

namespace ScrewPuzzle
{
    public sealed class ToyPlane3DPrototypeBootstrap : Radio3DPrototypeBootstrap
    {
        public PlaneScrewLayout Layout { get; private set; }
        protected override bool AllowPitch => true;
        protected override float ScrewVisualScale => .56f;
        protected override int LayoutSeed => Layout.Seed;
        protected override ScrewColorId[] TraySequence => Layout.Trays;
        protected override string BoardTitle => "THE TOY AIRPLANE WORKSHOP";
        protected override string BoardId => ThreeDBoardProgress.ToyPlane;
        protected override float ModelVerticalOffset => 190f;
        protected override string WinTitle => "AIRPLANE CLEARED!";
        private static readonly string[] Names = { "Near Wing", "Far Wing", "Tail", "Nose", "Spring", "Gearbox", "Cockpit", "Landing Gear", "Chassis" };

        protected override void BuildModel(out Transform radio, out List<Radio3DScrew> screws, out Radio3DPlate[] plates)
        {
            Camera.main.gameObject.AddComponent<WorkshopAirportBackdrop>().Configure("Art/Workshop/Airport-Background", "Airport Background");
            int savedSeed = WorkshopRunSave.Read(BoardId)?.layoutSeed ?? 0;
            Layout = new PlaneScrewLayout(savedSeed > 0 ? savedSeed : Random.Range(1, int.MaxValue));
            radio = new GameObject("Rotating Toy Airplane").transform;
            var blue = Finish(new Color(.12f, .43f, .76f), .12f, .48f);
            var cream = Finish(new Color(.98f, .91f, .73f), .08f, .38f);
            var gold = Finish(new Color(.98f, .60f, .12f), .35f, .48f);
            var dark = Finish(new Color(.055f, .075f, .10f), .05f, .22f);
            var glass = Finish(new Color(.12f, .34f, .46f), .45f, .75f);
            var steel = Finish(new Color(.58f, .65f, .68f), .65f, .50f);
            plates = new Radio3DPlate[PlaneScrewLayout.AssemblyCount];
            Vector3[] normals = { Vector3.back, Vector3.forward, Vector3.left, Vector3.right, Vector3.back, Vector3.back, Vector3.up, Vector3.down, Vector3.back };
            for (int f = 0; f < plates.Length; f++)
            {
                plates[f] = new GameObject(Names[f] + " Assembly").AddComponent<Radio3DPlate>();
                plates[f].transform.SetParent(radio, false);
            }
            for (int side = 0; side < 2; side++)
            {
                float sign = side == 0 ? -1 : 1;
                var parent = plates[side].transform;
                var wing = RoundedBody("Tapered Wing", parent, new Vector3(.15f, 0, sign * 1.25f), new Vector3(2.05f, .32f, 1.8f), .14f, blue);
                // Sweep the outer edge back, narrowing the tip while retaining a thick toy-safe rim.
                var mesh = wing.GetComponent<MeshFilter>().sharedMesh;
                var vertices = mesh.vertices;
                for (int v = 0; v < vertices.Length; v++)
                {
                    float t = Mathf.InverseLerp(-.9f, .9f, vertices[v].z * sign);
                    vertices[v].x = vertices[v].x * Mathf.Lerp(1, .72f, t) - .12f * t;
                }
                mesh.vertices = vertices; mesh.RecalculateNormals(); mesh.RecalculateBounds();
                RoundedBody("Wing Stripe", parent, new Vector3(.05f, .17f, sign * 1.60f), new Vector3(1.45f, .025f, .20f), .01f, cream);
                RoundedBody("Aileron Seam", parent, new Vector3(-.55f, .163f, sign * 1.30f), new Vector3(.022f, .012f, 1.35f), .005f, dark);
                Ellipsoid("Rounded Fuselage Side", parent, new Vector3(0, .04f, sign * .54f), new Vector3(3.45f, .94f, .32f), blue);
                var badge = Shape("Wing Roundel", PrimitiveType.Cylinder, parent, new Vector3(.28f, .177f, sign * 1.02f), new Vector3(.43f, .014f, .43f), cream);
                Shape("Roundel Center", PrimitiveType.Cylinder, parent, badge.localPosition + Vector3.up * .02f, new Vector3(.24f, .008f, .24f), gold);
            }
            var tail = plates[2].transform;
            RoundedBody("Tail End", tail, new Vector3(-1.8f, -.02f, 0), new Vector3(.12f, .65f, .88f), .05f, blue);
            RoundedBody("Tailplane", tail, new Vector3(-1.5f, .28f, 0), new Vector3(.75f, .14f, 1.9f), .065f, gold);
            Ellipsoid("Rounded Tail Fin", tail, new Vector3(-1.5f, .62f, 0), new Vector3(.72f, .95f, .15f), gold);
            RoundedBody("Rudder Seam", tail, new Vector3(-1.65f, .72f, -.08f), new Vector3(.018f, .40f, .01f), .004f, cream);
            var nose = plates[3].transform;
            RoundedBody("Nose Cowl", nose, new Vector3(1.74f, -.02f, 0), new Vector3(.28f, .78f, .98f), .12f, gold);
            Ellipsoid("Spinner", nose, new Vector3(1.98f, .42f, 0), new Vector3(.45f, .36f, .36f), gold);
            var blades = RoundedBody("Propeller Blades", nose, new Vector3(1.96f, .42f, 0), new Vector3(.10f, 1.65f, .23f), .049f, cream);
            blades.localRotation = Quaternion.Euler(30, 0, 0);
            // Layered, exposed carriers give every remaining assembly its own visible fasteners.
            float[] depth = { -.45f, -.23f, -.01f, .21f, .43f };
            for (int p = 4; p < plates.Length; p++)
                RoundedBody(Names[p] + " Mounting Plate", plates[p].transform, new Vector3(0, -.05f, depth[p - 4]), new Vector3(2.85f, .66f, .07f), .03f, p % 2 == 0 ? cream : gold);
            for (int coil = 0; coil < 12; coil++)
            {
                var winding = Shape("Spring Winding", PrimitiveType.Cylinder, plates[4].transform, new Vector3(-.72f + coil * .13f, -.20f, -.51f), new Vector3(.13f, .025f, .13f), steel);
                winding.localRotation = Quaternion.Euler(0, 0, 90);
            }
            for (int gear = 0; gear < 3; gear++)
            {
                Vector3 center = new Vector3(-.55f + gear * .55f, -.20f, -.30f);
                var cog = Shape("Propeller Gear", PrimitiveType.Cylinder, plates[5].transform, center, new Vector3(.30f, .018f, .30f), steel);
                cog.localRotation = Quaternion.Euler(90, 0, 0);
                for (int tooth = 0; tooth < 10; tooth++)
                {
                    float a = tooth * Mathf.PI / 5;
                    var t = RoundedBody("Gear Tooth", plates[5].transform, center + new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0) * .15f, new Vector3(.07f, .06f, .035f), .012f, steel);
                    t.localRotation = Quaternion.Euler(0, 0, tooth * 36);
                }
            }
            var cockpit = plates[6].transform;
            Ellipsoid("Upper Fuselage", cockpit, new Vector3(0, .40f, 0), new Vector3(3.35f, .62f, 1.06f), blue);
            Ellipsoid("Cockpit Rim", cockpit, new Vector3(-.25f, .68f, 0), new Vector3(1.04f, .36f, .76f), cream);
            Ellipsoid("Cockpit Canopy", cockpit, new Vector3(-.25f, .77f, 0), new Vector3(.90f, .49f, .64f), glass);
            Ellipsoid("Canopy Center Frame", cockpit, new Vector3(-.25f, .77f, 0), new Vector3(.06f, .50f, .65f), cream);
            // Three separate sockets around the canopy, clear of the key and tail fin.
            Vector3[] cockpitSockets = { new Vector3(-1.12f, .70f, -.30f), new Vector3(.55f, .76f, 0), new Vector3(1.15f, .69f, 0) };
            foreach (var socket in cockpitSockets)
                RoundedBody("Cockpit Screw Seat", cockpit, socket - Vector3.up * .095f, new Vector3(.34f, .15f, .34f), .04f, blue);
            var landing = plates[7].transform;
            // A removable belly bracket carries the landing gear, below the final chassis.
            RoundedBody("Landing Gear Belly Bracket", landing, new Vector3(0, -.55f, 0), new Vector3(2.8f, .09f, .45f), .04f, steel);
            Vector3[] landingSockets = { new Vector3(-1.1f, -.64f, 0), new Vector3(-.45f, -.64f, 0), new Vector3(1.15f, -.64f, 0) };
            foreach (float side in new[] { -1f, 1f })
            {
                var wheel = Shape("Landing Wheel", PrimitiveType.Cylinder, landing, new Vector3(.65f, -.80f, side * .66f), new Vector3(.51f, .10f, .51f), dark);
                wheel.localRotation = Quaternion.Euler(90, 0, 0);
                var hub = Shape("Wheel Hub", PrimitiveType.Cylinder, landing, new Vector3(.65f, -.80f, side * .77f), new Vector3(.27f, .018f, .27f), steel);
                hub.localRotation = wheel.localRotation;
                RoundedBody("Landing Strut", landing, new Vector3(.65f, -.57f, side * .51f), new Vector3(.10f, .43f, .10f), .045f, steel);
            }
            RoundedBody("Wheel Axle", landing, new Vector3(.65f, -.80f, 0), new Vector3(.10f, .10f, 1.40f), .045f, steel);
            var chassis = plates[8].transform;
            RoundedBody("Lower Fuselage", chassis, new Vector3(0, -.40f, 0), new Vector3(3.25f, .20f, .80f), .09f, cream);
            RoundedBody("Wind Up Key Stem", chassis, new Vector3(-.95f, .78f, 0), new Vector3(.10f, .70f, .10f), .045f, gold);
            foreach (float side in new[] { -1f, 1f })
                Ellipsoid("Wind Up Key Loop", chassis, new Vector3(-.95f, 1.13f, side * .17f), new Vector3(.10f, .27f, .30f), gold);
            var colors = new[] { Finish(new Color(.9f, .12f, .08f), .2f, .45f), Finish(new Color(.05f, .4f, .95f), .2f, .45f), Finish(new Color(1f, .7f, .04f), .2f, .45f) };
            screws = new List<Radio3DScrew>();
            for (int f = 0; f < plates.Length; f++)
            {
                var fasteners = new Radio3DScrew[3];
                for (int i = 0; i < 3; i++)
                {
                    int index = f * 3 + i, socket = Layout.Sockets[index];
                    Vector3 pos = f < 2 ? new Vector3(.03f + (socket - 1) * .55f, 0, f == 0 ? -2.23f : 2.23f) :
                        f < 4 ? new Vector3(f == 2 ? -1.92f : 1.95f, -.06f, (socket - 1) * .32f) :
                        f == 6 ? cockpitSockets[socket] :
                        f == 7 ? landingSockets[socket] :
                        new Vector3((socket - 1) * 1.0f, -.02f, depth[f - 4] - .11f);
                    var color = Layout.Colors[index];
                    fasteners[i] = Screw(Names[f] + " " + i, radio, pos, normals[f], colors[(int)color], dark);
                    int cover = PlaneScrewLayout.Covers[f];
                    fasteners[i].Initialize(color, cover < 0 ? null : plates[cover]);
                    screws.Add(fasteners[i]);
                }
                plates[f].Configure(fasteners, normals[f]);
            }
            radio.localRotation = Quaternion.Euler(0, 25, 0);
        }

        private Transform Ellipsoid(string name, Transform parent, Vector3 position, Vector3 size, Material material)
        {
            var shape = Shape(name, PrimitiveType.Sphere, parent, position, size, material);
            // SphereCollider uses the largest scale axis, not the rendered ellipsoid.
            // A mesh collider follows the actual shell and leaves the screw paths clear.
            var sphere = shape.GetComponent<Collider>();
            sphere.enabled = false;
            Destroy(sphere);
            shape.gameObject.AddComponent<MeshCollider>().sharedMesh = shape.GetComponent<MeshFilter>().sharedMesh;
            return shape;
        }

        protected override void DecorateBoardUi(RectTransform parent)
        {
            foreach (var region in new[] { new Vector2(740, 440) })
            {
                var panel = new GameObject("Airport UI Backing", typeof(RectTransform), typeof(UnityEngine.UI.Image));
                panel.transform.SetParent(parent, false);
                var rect = panel.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(1080, region.y);
                rect.anchoredPosition = new Vector2(0, region.x);
                var image = panel.GetComponent<UnityEngine.UI.Image>();
                image.color = new Color(.08f, .12f, .15f, .84f);
                image.raycastTarget = false;
            }
        }

        private Material Finish(Color color, float metallic, float smoothness)
        {
            var material = Material(color);
            material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", smoothness);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", smoothness);
            return material;
        }
    }
}
