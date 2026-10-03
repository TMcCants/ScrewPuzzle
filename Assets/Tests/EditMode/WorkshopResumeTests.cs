using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ScrewPuzzle.Tests
{
    public sealed class WorkshopResumeTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator MidMove_RestoresTrayAndUnlock_ThenRestartClearsOnlyRun()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.ToyCar);
            yield return new EnterPlayMode(); yield return null;
            var puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.UnlockTestTray(2), Is.True);
            Assert.That(puzzle.TryAdd(GameObject.Find("Near 0").GetComponent<Radio3DScrew>()), Is.True);
            Assert.That(puzzle.IsBusy, Is.True);
            Assert.That(WorkshopRunSave.HasRun(ThreeDBoardProgress.ToyCar), Is.True);
            ThreeDBoardNavigation.OpenSelector(); yield return null; yield return null;
            Assert.That(GameObject.Find("Start Toy Car").GetComponentInChildren<Text>().text, Does.StartWith("RESUME"));
            GameObject.Find("Play Toy Car").GetComponent<Button>().onClick.Invoke(); yield return null; yield return null;
            puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.IsBusy, Is.False);
            Assert.That(puzzle.RemovedCount, Is.EqualTo(1));
            Assert.That(puzzle.Trays[0].Count, Is.EqualTo(1));
            Assert.That(puzzle.Trays[2].IsOpen, Is.True);
            yield return new ExitPlayMode();
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.ToyCar);
            yield return new EnterPlayMode(); yield return null;
            puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.RemovedCount, Is.EqualTo(1), "Resume must survive a Unity play-session restart.");
            Assert.That(puzzle.Trays[0].Count, Is.EqualTo(1));
            Assert.That(puzzle.Trays[2].IsOpen, Is.True);
            bool completed = ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.ToyCar);
            Object.FindFirstObjectByType<Radio3DInteraction>().ResetExperiment();
            Assert.That(WorkshopRunSave.HasRun(ThreeDBoardProgress.ToyCar), Is.False);
            Assert.That(puzzle.Trays[2].IsOpen, Is.False);
            Assert.That(puzzle.RemovedCount, Is.Zero);
            Assert.That(ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.ToyCar), Is.EqualTo(completed));
        }

        [UnityTest]
        public IEnumerator Robot_ResumesDeeperLayer_AndCanFinishWithFreeTrays()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.ToyRobot);
            yield return new EnterPlayMode(); yield return null;
            var puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            foreach (string name in new[] { "Chest 0", "Back 0", "Left 0", "Chest 2", "Back 2", "Left 2", "Chest 1", "Back 1", "Left 1", "Right 0", "Right 1", "Right 2", "Inner 0", "Inner 1" })
                yield return Take(puzzle, name);
            Assert.That(puzzle.TryAdd(GameObject.Find("Inner 2").GetComponent<Radio3DScrew>()), Is.True);
            ThreeDBoardNavigation.OpenRadio(); yield return null; yield return null;
            Assert.That(Object.FindFirstObjectByType<Radio3DPuzzle>().RemovedCount, Is.Zero);
            ThreeDBoardNavigation.OpenToyRobot(); yield return null; yield return null;
            puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.RemovedCount, Is.EqualTo(15));
            Assert.That(puzzle.ClearedCount, Is.EqualTo(15));
            Assert.That(puzzle.ReleasedPlateCount, Is.EqualTo(5));
            Assert.That(GameObject.Find("Core 0").GetComponent<Radio3DScrew>().IsAccessible, Is.True);
            Assert.That(puzzle.Trays[2].IsOpen, Is.False);
            for (int i = 0; i < 2; i++) yield return Take(puzzle, "Core " + i);
            Assert.That(puzzle.TryAdd(GameObject.Find("Core 2").GetComponent<Radio3DScrew>()), Is.True);
            ThreeDBoardNavigation.OpenToyRobot(); yield return null; yield return null;
            puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
            Assert.That(WorkshopRunSave.HasRun(ThreeDBoardProgress.ToyRobot), Is.False);
            Assert.That(ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.ToyRobot), Is.True);
        }

        [UnityTest]
        public IEnumerator InvalidSave_StartsFresh_WithoutChangingCompletion()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Radio);
            yield return new EnterPlayMode(); yield return null;
            var puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.TryAdd(GameObject.Find("Front 0").GetComponent<Radio3DScrew>()), Is.True);
            var run = WorkshopRunSave.Read(ThreeDBoardProgress.Radio);
            // Valid layout but duplicate action: restore must roll back partial replay.
            WorkshopRunSave.Write(ThreeDBoardProgress.Radio, run.signature, new[] { 0, 0 });
            ThreeDBoardNavigation.OpenRadio(); yield return null; yield return null;
            puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.RemovedCount, Is.Zero);
            Assert.That(WorkshopRunSave.HasRun(ThreeDBoardProgress.Radio), Is.False);
            PlayerPrefs.SetString(WorkshopRunSave.Key(ThreeDBoardProgress.Radio), "not-json");
            Assert.That(WorkshopRunSave.Read(ThreeDBoardProgress.Radio), Is.Null);
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
