using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace ScrewPuzzle.Tests
{
    public class PlaneScrewGeometryTests
    {
        [Test]
        public void PhillipsOpening_HasALowerFloorThanTheHeadFace()
        {
            var mesh = (Mesh)typeof(Radio3DPrototypeBootstrap)
                .GetMethod("CreateRecessedHead", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
            var probe = new GameObject("Recess depth probe");
            try
            {
                probe.transform.position = Vector3.one * 100;
                var collider = probe.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh;
                Physics.SyncTransforms();
                Assert.That(collider.Raycast(new Ray(probe.transform.position + Vector3.up * 2, Vector3.down), out var floor, 3), Is.True);
                Assert.That(collider.Raycast(new Ray(probe.transform.position + new Vector3(.4f, 2, 0), Vector3.down), out var face, 3), Is.True);
                Assert.That(floor.distance, Is.EqualTo(1.85f).Within(.002f));
                Assert.That(face.distance, Is.EqualTo(1f).Within(.002f));
                Assert.That(floor.distance, Is.GreaterThan(face.distance + .8f));
            }
            finally
            {
                Object.DestroyImmediate(probe);
                Object.DestroyImmediate(mesh);
            }
        }
    }
}
