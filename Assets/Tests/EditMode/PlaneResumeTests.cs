using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public sealed class PlaneResumeTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator Plane_ResumesSameRandomLayout_DuringAnimation_AndFinishes()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.ToyPlane);
            yield return new EnterPlayMode(); yield return null;
            var bootstrap = Object.FindFirstObjectByType<ToyPlane3DPrototypeBootstrap>();
            int seed = bootstrap.Layout.Seed;
            var layout = bootstrap.Layout;
            var input = Object.FindFirstObjectByType<Radio3DInteraction>();
            var screws = input.Radio.GetComponentsInChildren<Radio3DScrew>();
            var positions = screws.Select(s => s.transform.localPosition).ToArray();
            var names = screws.Select(s => s.name).ToArray();
            var puzzle = input.GetComponent<Radio3DPuzzle>();
            for (int i = 0; i < 20; i++)
            {
                Assert.That(puzzle.TryAdd(screws[layout.Solution[i]]), Is.True);
                yield return ToyPlane3DTests.Wait(puzzle);
            }
            Assert.That(puzzle.TryAdd(screws[layout.Solution[20]]), Is.True);
            ThreeDBoardNavigation.OpenToyPlane(); yield return null; yield return null;
            bootstrap = Object.FindFirstObjectByType<ToyPlane3DPrototypeBootstrap>();
            puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(bootstrap.Layout.Seed, Is.EqualTo(seed));
            CollectionAssert.AreEqual(layout.Colors, bootstrap.Layout.Colors);
            CollectionAssert.AreEqual(layout.Sockets, bootstrap.Layout.Sockets);
            Assert.That(puzzle.RemovedCount, Is.EqualTo(21));
            for (int i = 21; i < 27; i++)
            {
                int index = layout.Solution[i];
                var screw = GameObject.Find(names[index]).GetComponent<Radio3DScrew>();
                Assert.That(screw.transform.localPosition, Is.EqualTo(positions[index]));
                Assert.That(puzzle.TryAdd(screw), Is.True);
                yield return ToyPlane3DTests.Wait(puzzle);
            }
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
            Assert.That(WorkshopRunSave.HasRun(ThreeDBoardProgress.ToyPlane), Is.False);
            ThreeDBoardNavigation.OpenToyPlane(); yield return null; yield return null;
            Assert.That(Object.FindFirstObjectByType<Radio3DPuzzle>().RemovedCount, Is.Zero);
        }
        [UnityTearDown] public IEnumerator LeavePlayMode() { if (Application.isPlaying) yield return new ExitPlayMode(); }
    }
}
