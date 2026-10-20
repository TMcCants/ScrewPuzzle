using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public sealed class PlaneOrbitTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator FullOrbit_RevealsTopAndUnderside_DoesNotSelect_AndHintRecovers()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.ToyPlane);
            yield return new EnterPlayMode(); yield return null; yield return null;
            var input = Object.FindFirstObjectByType<Radio3DInteraction>();
            var puzzle = input.GetComponent<Radio3DPuzzle>();
            Assert.That(input.AllowPitch, Is.True);
            Quaternion original = input.Radio.rotation;
            Vector2 center = new Vector2(Screen.width * .5f, Screen.height * .54f);
            input.BeginPointer(center);
            input.EndPointer(center + Vector2.up * (Screen.height * .3f));
            Assert.That(Quaternion.Angle(original, input.Radio.rotation), Is.EqualTo(90).Within(.1f));
            Assert.That(Vector3.Dot(input.Radio.up, input.View.transform.forward), Is.GreaterThan(.95f));
            Assert.That(puzzle.RemovedCount, Is.Zero);
            input.ResetExperiment();
            input.BeginPointer(center);
            input.EndPointer(center - Vector2.up * (Screen.height * .3f));
            Assert.That(Vector3.Dot(input.Radio.up, input.View.transform.forward), Is.LessThan(-.95f));
            Assert.That(puzzle.RemovedCount, Is.Zero);
            input.ResetExperiment();
            for (int i = 0; i < 4; i++)
            {
                input.BeginPointer(center);
                input.EndPointer(center + Vector2.right * (Screen.width * .3f));
            }
            Assert.That(Quaternion.Angle(original, input.Radio.rotation), Is.LessThan(.1f), "Full horizontal revolution");
            for (int i = 0; i < 4; i++)
            {
                input.BeginPointer(center);
                input.EndPointer(center + Vector2.up * (Screen.height * .3f));
            }
            Assert.That(Quaternion.Angle(original, input.Radio.rotation), Is.LessThan(.1f), "Full vertical revolution");
            input.Radio.rotation = Quaternion.Euler(155, 75, 35);
            var screw = input.FindHint();
            Assert.That(screw, Is.Not.Null);
            Physics.SyncTransforms();
            Vector2 point = input.View.WorldToScreenPoint(screw.transform.position);
            Assert.That(input.PickScrew(point), Is.EqualTo(screw));
            input.BeginPointer(point); input.EndPointer(point);
            Assert.That(screw.IsRemoved, Is.True);
            GameObject.Find("Reset View").GetComponent<UnityEngine.UI.Button>().onClick.Invoke();
            Assert.That(puzzle.RemovedCount, Is.EqualTo(1), "Reset View must preserve puzzle progress");
            Assert.That(Quaternion.Angle(original, input.Radio.rotation), Is.LessThan(.1f));
            input.ResetExperiment();
            Assert.That(Quaternion.Angle(original, input.Radio.rotation), Is.LessThan(.1f));
            Assert.That(puzzle.RemovedCount, Is.Zero);
            var backdrop = GameObject.Find("Airport Background");
            Assert.That(backdrop, Is.Not.Null);
            Assert.That(backdrop.GetComponent<Collider>(), Is.Null);
            Assert.That(backdrop.GetComponent<Renderer>().sharedMaterial.shader.isSupported, Is.True);
            Assert.That(backdrop.GetComponent<Renderer>().sharedMaterial.mainTexture, Is.Not.Null);
            ThreeDBoardNavigation.OpenRadio(); yield return null; yield return null;
            Assert.That(Object.FindFirstObjectByType<Radio3DInteraction>().AllowPitch, Is.True);
            Assert.That(GameObject.Find("Airport Background"), Is.Null);
        }
        [UnityTearDown] public IEnumerator LeavePlayMode() { if (Application.isPlaying) yield return new ExitPlayMode(); }
    }
}
