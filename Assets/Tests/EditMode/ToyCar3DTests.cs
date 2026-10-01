using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public sealed class ToyCar3DTests
    {
        [UnityTest]
        public IEnumerator Car_AllFacesAndInnerLayer_ClearWithFreeTrays_AndRestart()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Experiment_ToyCar3D.unity");
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            var input = Object.FindFirstObjectByType<Radio3DInteraction>();
            var puzzle = input.GetComponent<Radio3DPuzzle>();
            Assert.That(Object.FindFirstObjectByType<ToyCar3DPrototypeBootstrap>(), Is.Not.Null);
            Assert.That(input.TotalCount, Is.EqualTo(15));
            Quaternion initial = input.Radio.localRotation;
            var inner = GameObject.Find("Inner 0").GetComponent<Radio3DScrew>();
            var nose = GameObject.Find("Nose 0").GetComponent<Radio3DScrew>();
            Vector3 noseScale = nose.transform.localScale;
            var targets = Object.FindObjectsByType<Radio3DScrew>(FindObjectsSortMode.None);
            Assert.That(puzzle.TryAdd(inner), Is.False);
            for (int angle = 0; angle < 360; angle += 45)
            {
                input.Radio.rotation = Quaternion.Euler(0, angle, 0);
                Physics.SyncTransforms();
                Assert.That(input.PickScrew(input.View.WorldToScreenPoint(inner.transform.position)), Is.Not.EqualTo(inner));
            }
            foreach (string name in new[] { "Near 0", "Far 0", "Rear 0", "Near 2", "Far 2", "Rear 2",
                "Near 1", "Far 1", "Rear 1", "Nose 0", "Nose 1", "Nose 2", "Inner 0", "Inner 1", "Inner 2" })
            {
                Vector3 normal = name.StartsWith("Near") || name.StartsWith("Inner") ? Vector3.back :
                    name.StartsWith("Far") ? Vector3.forward : name.StartsWith("Rear") ? Vector3.left : Vector3.right;
                input.Radio.rotation = Quaternion.FromToRotation(normal, Vector3.back);
                Physics.SyncTransforms();
                var screw = GameObject.Find(name).GetComponent<Radio3DScrew>();
                Vector2 point = input.View.WorldToScreenPoint(screw.transform.position);
                Assert.That(input.PickScrew(point), Is.EqualTo(screw), name + " must be reachable around the car geometry.");
                input.BeginPointer(point);
                input.EndPointer(point);
                Assert.That(screw.IsRemoved, Is.True);
                float deadline = Time.realtimeSinceStartup + 5f;
                while (puzzle.IsBusy && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(puzzle.IsBusy, Is.False);
                Assert.That(puzzle.Trays[2].IsOpen, Is.False);
                if (name == "Near 1") Assert.That(inner.IsAccessible, Is.True);
                if (name == "Nose 2") Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Playing));
            }
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
            Assert.That(puzzle.ClearedCount, Is.EqualTo(15));
            Assert.That(puzzle.ReleasedPlateCount, Is.EqualTo(5));
            input.ResetExperiment();
            Assert.That(input.Radio.localRotation, Is.EqualTo(initial));
            Assert.That(puzzle.RemovedCount, Is.Zero);
            Assert.That(puzzle.ReleasedPlateCount, Is.Zero);
            Assert.That(inner.IsAccessible, Is.False);
            Assert.That(nose.transform.localScale, Is.EqualTo(noseScale));
            foreach (var screw in targets)
            {
                Assert.That(screw.gameObject.activeSelf, Is.True);
                Assert.That(screw.IsRemoved, Is.False);
            }
        }

        [UnityTearDown]
        public IEnumerator LeavePlayMode()
        {
            if (Application.isPlaying) yield return new ExitPlayMode();
        }
    }
}
