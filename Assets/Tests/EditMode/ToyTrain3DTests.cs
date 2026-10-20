using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public sealed class ToyTrain3DTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator Train_CompletelyDismantles_WithPhoneSizedTargets_AndRestarts()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.ToyTrain);
            yield return new EnterPlayMode(); yield return null;
            var bootstrap = Object.FindFirstObjectByType<ToyTrain3DPrototypeBootstrap>();
            var input = Object.FindFirstObjectByType<Radio3DInteraction>();
            var puzzle = input.GetComponent<Radio3DPuzzle>();
            var screws = input.Radio.GetComponentsInChildren<Radio3DScrew>();
            var renderers = input.Radio.GetComponentsInChildren<Renderer>();
            Assert.That(screws.Length, Is.EqualTo(27));
            Assert.That(puzzle.TotalPlateCount, Is.EqualTo(9));
            foreach (var renderer in renderers)
                Assert.That(renderer.GetComponentInParent<Radio3DPlate>() != null || renderer.GetComponentInParent<Radio3DScrew>() != null,
                    Is.True, renderer.name + " must belong to a removable assembly");
            int move = 0;
            foreach (int index in bootstrap.Layout.Solution)
            {
                var screw = screws[index];
                Vector3 normal = screw.transform.localRotation * Vector3.up;
                if (index >= 18 && index < 24)
                {
                    Assert.That(Vector3.Dot(normal, index < 21 ? Vector3.up : Vector3.down), Is.GreaterThan(.99f));
                    // Start from an awkward view: hints must tilt toward these new faces.
                    input.Radio.rotation = Quaternion.Euler(25, 140, 30);
                    var hinted = input.FindHint();
                    Assert.That(hinted, Is.Not.Null, "Hint must find the top/underside fasteners");
                    Assert.That(hinted.name, Does.StartWith(index < 21 ? "Cab " : "Wheels "));
                    Assert.That(input.PickScrew(input.View.WorldToScreenPoint(hinted.transform.position)), Is.EqualTo(hinted));
                }
                input.Radio.rotation = Quaternion.FromToRotation(normal, Vector3.back);
                Physics.SyncTransforms();
                Vector2 point = input.View.WorldToScreenPoint(screw.transform.position);
                if (move++ == 0)
                {
                    Assert.That(screw.transform.Find("Head Visual").localScale.x, Is.EqualTo(.2352f).Within(.001f));
                    point = input.View.WorldToScreenPoint(screw.transform.TransformPoint(new Vector3(.18f, 0, 0)));
                }
                Assert.That(input.PickScrew(point), Is.EqualTo(screw), screw.name);
                input.BeginPointer(point); input.EndPointer(point);
                Assert.That(screw.IsRemoved, Is.True, screw.name);
                yield return Wait(puzzle);
                Assert.That(puzzle.Trays[2].IsOpen || puzzle.Trays[3].IsOpen, Is.False);
            }
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
            Assert.That(puzzle.ReleasedPlateCount, Is.EqualTo(9));
            Assert.That(renderers.Any(r => r.enabled && r.gameObject.activeInHierarchy), Is.False, "No fixed toy geometry may remain.");
            int seed = bootstrap.Layout.Seed;
            input.ResetExperiment();
            Assert.That(bootstrap.Layout.Seed, Is.EqualTo(seed));
            Assert.That(puzzle.RemovedCount, Is.Zero);
            Assert.That(screws.All(s => !s.IsRemoved && s.gameObject.activeInHierarchy), Is.True);
            Assert.That(input.Radio.GetComponentsInChildren<Radio3DPlate>().Length, Is.EqualTo(9));
        }

        [UnityTest]
        public IEnumerator Restart_DuringRelease_RestoresAllAssemblies()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.ToyTrain);
            yield return new EnterPlayMode(); yield return null;
            var bootstrap = Object.FindFirstObjectByType<ToyTrain3DPrototypeBootstrap>();
            var input = Object.FindFirstObjectByType<Radio3DInteraction>();
            var puzzle = input.GetComponent<Radio3DPuzzle>();
            var screws = input.Radio.GetComponentsInChildren<Radio3DScrew>();
            for (int i = 0; i < 2; i++) { Assert.That(puzzle.TryAdd(screws[bootstrap.Layout.Solution[i]]), Is.True); yield return Wait(puzzle); }
            Assert.That(puzzle.TryAdd(screws[bootstrap.Layout.Solution[2]]), Is.True);
            var wing = GameObject.Find("Engine Assembly").GetComponent<Radio3DPlate>();
            float deadline = Time.realtimeSinceStartup + 8;
            while (!wing.IsAnimating && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(wing.IsAnimating, Is.True);
            input.ResetExperiment(); yield return null;
            Assert.That(wing.IsReleased, Is.False);
            Assert.That(screws[12].IsAccessible, Is.False);
            Assert.That(puzzle.RemovedCount, Is.Zero);
            Assert.That(puzzle.Trays[0].Color, Is.EqualTo(bootstrap.Layout.Trays[0]));
            Assert.That(puzzle.Trays[1].Color, Is.EqualTo(bootstrap.Layout.Trays[1]));
        }

        public static IEnumerator Wait(Radio3DPuzzle puzzle)
        {
            float deadline = Time.realtimeSinceStartup + 8;
            while (puzzle.IsBusy && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(puzzle.IsBusy, Is.False);
        }
        [UnityTearDown] public IEnumerator LeavePlayMode() { if (Application.isPlaying) yield return new ExitPlayMode(); }
    }
}
