using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public sealed class RadioDisassemblyTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator SpeakerAndCircuit_ReleaseInOrder_ResumeAndRestore()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Radio);
            yield return new EnterPlayMode(); yield return null;
            var puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            var driver = GameObject.Find("Speaker Driver").transform;
            var speaker = GameObject.Find("Inner Front Plate Assembly").GetComponent<Radio3DPlate>();
            var circuit = GameObject.Find("Circuit 0").GetComponent<Radio3DScrew>();
            Assert.That(driver.IsChildOf(speaker.transform), Is.True);
            Assert.That(circuit.IsAccessible, Is.False);
            foreach (string name in new[] { "Front 0", "Back 0", "Left 0", "Front 2", "Back 2", "Left 2", "Front 1", "Back 1", "Left 1", "Right 0", "Right 1", "Right 2", "Inner 0", "Inner 1" })
                yield return Take(puzzle, name);
            Assert.That(driver.gameObject.activeInHierarchy, Is.True);
            Assert.That(circuit.IsAccessible, Is.False);
            Assert.That(puzzle.TryAdd(GameObject.Find("Inner 2").GetComponent<Radio3DScrew>()), Is.True);
            float deadline = Time.realtimeSinceStartup + 8;
            while (!speaker.IsAnimating && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(speaker.IsAnimating, Is.True);
            Assert.That(circuit.IsAccessible, Is.False);
            ThreeDBoardNavigation.OpenRadio(); yield return null; yield return null;
            puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.RemovedCount, Is.EqualTo(15));
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Playing));
            Assert.That(GameObject.Find("Circuit 0").GetComponent<Radio3DScrew>().IsAccessible, Is.True);
            for (int i = 0; i < 3; i++) yield return Take(puzzle, "Circuit " + i);
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
            Assert.That(puzzle.ReleasedPlateCount, Is.EqualTo(6));
            Object.FindFirstObjectByType<Radio3DInteraction>().ResetExperiment();
            Assert.That(GameObject.Find("Speaker Driver").gameObject.activeInHierarchy, Is.True);
            Assert.That(GameObject.Find("Circuit Board").gameObject.activeInHierarchy, Is.True);
            Assert.That(GameObject.Find("Circuit 0").GetComponent<Radio3DScrew>().IsAccessible, Is.False);
            Assert.That(puzzle.RemovedCount, Is.Zero);
        }
        private static IEnumerator Take(Radio3DPuzzle puzzle, string name)
        {
            Assert.That(puzzle.TryAdd(GameObject.Find(name).GetComponent<Radio3DScrew>()), Is.True, name);
            float deadline = Time.realtimeSinceStartup + 8;
            while (puzzle.IsBusy && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(puzzle.IsBusy, Is.False);
        }
        [UnityTearDown]
        public IEnumerator LeavePlayMode() { if (Application.isPlaying) yield return new ExitPlayMode(); }
    }
}
