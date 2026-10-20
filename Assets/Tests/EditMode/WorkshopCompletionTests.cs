using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ScrewPuzzle.Tests
{
    public sealed class WorkshopCompletionTests : WorkshopRunTestIsolation
    {
        [Test]
        public void NextToy_CyclesThroughAllBoards()
        {
            Assert.That(ThreeDBoardNavigation.NextScene(ThreeDBoardProgress.Radio), Is.EqualTo(ThreeDBoardNavigation.ToyCar));
            Assert.That(ThreeDBoardNavigation.NextScene(ThreeDBoardProgress.ToyCar), Is.EqualTo(ThreeDBoardNavigation.ToyRobot));
            Assert.That(ThreeDBoardNavigation.NextScene(ThreeDBoardProgress.ToyRobot), Is.EqualTo(ThreeDBoardNavigation.ToyTrain));
        }

        [UnityTest]
        public IEnumerator Win_Replay_NextToy_AndShop_WorkAcrossAllBoards()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Radio);
            yield return new EnterPlayMode();
            yield return null;
            foreach (string scene in new[] { ThreeDBoardNavigation.Radio, ThreeDBoardNavigation.ToyCar, ThreeDBoardNavigation.ToyRobot })
            {
                Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(scene));
                var puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
                var celebration = Object.FindFirstObjectByType<WorkshopCompletion>();
                Assert.That(celebration.IsVisible, Is.False);
                yield return Win(scene, puzzle);
                Assert.That(celebration.IsVisible, Is.True);
                if (scene == ThreeDBoardNavigation.Radio)
                {
                    GameObject.Find("Replay Toy").GetComponent<Button>().onClick.Invoke();
                    yield return null; yield return null;
                    puzzle=Object.FindFirstObjectByType<Radio3DPuzzle>();
                    celebration=Object.FindFirstObjectByType<WorkshopCompletion>();
                    Assert.That(celebration.IsVisible, Is.False);
                    Assert.That(puzzle.RemovedCount, Is.Zero);
                    Assert.That(ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.Radio), Is.True);
                    yield return Win(scene, puzzle);
                    Assert.That(celebration.IsVisible, Is.True);
                }
                GameObject.Find(scene == ThreeDBoardNavigation.ToyRobot ? "Back to Shop" : "Next Toy").GetComponent<Button>().onClick.Invoke();
                yield return null;
                yield return null;
                Assert.That(Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
            }
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(ThreeDBoardNavigation.Selector));
            Assert.That(Object.FindFirstObjectByType<WorkshopCompletion>(), Is.Null);
        }

        private static IEnumerator Win(string scene, Radio3DPuzzle puzzle)
        {
            if (scene != ThreeDBoardNavigation.Radio)
            {
                string[] route = scene == ThreeDBoardNavigation.ToyRobot
                    ? new[] { "Chest 0", "Chest 1", "Chest 2", "Right 1", "Back 1", "Left 1", "Back 0", "Back 2", "Left 0", "Inner 0", "Inner 1", "Inner 2", "Core 0", "Core 1", "Right 0", "Left 2", "Right 2", "Core 2" }
                    : new[] { "Near 0", "Near 1", "Near 2", "Far 0", "Far 2", "Rear 1", "Nose 0", "Nose 1", "Inner 0", "Inner 1", "Rear 2", "Nose 2", "Far 1", "Rear 0", "Inner 2" };
                foreach (string screw in route) yield return Take(screw, puzzle);
                Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
                yield break;
            }
            string[] faces = scene == ThreeDBoardNavigation.Radio ? new[] { "Front", "Back", "Left", "Right", "Inner" } :
                scene == ThreeDBoardNavigation.ToyCar ? new[] { "Near", "Far", "Rear", "Nose", "Inner" } : new[] { "Chest", "Back", "Left", "Right", "Inner" };
            foreach (int color in new[] { 0, 2, 1 })
                for (int face = 0; face < 3; face++)
                    yield return Take(faces[face] + " " + (scene == ThreeDBoardNavigation.ToyCar ? (color - face + 3) % 3 : color), puzzle);
            for (int face = 3; face < 5; face++)
                for (int screw = 0; screw < 3; screw++)
                    yield return Take(faces[face] + " " + screw, puzzle);
            if (scene == ThreeDBoardNavigation.ToyRobot)
                for (int screw = 0; screw < 3; screw++) yield return Take("Core " + screw, puzzle);
            if (scene == ThreeDBoardNavigation.Radio)
                for (int screw = 0; screw < 3; screw++) yield return Take("Circuit " + screw, puzzle);
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
        }
        private static IEnumerator Take(string name, Radio3DPuzzle puzzle)
        {
            Assert.That(puzzle.TryAdd(GameObject.Find(name).GetComponent<Radio3DScrew>()), Is.True, name);
            float deadline = Time.realtimeSinceStartup + 8f;
            while (puzzle.IsBusy && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(puzzle.IsBusy, Is.False);
        }
        [UnityTearDown]
        public IEnumerator LeavePlayMode() { if (Application.isPlaying) yield return new ExitPlayMode(); }
    }
}
