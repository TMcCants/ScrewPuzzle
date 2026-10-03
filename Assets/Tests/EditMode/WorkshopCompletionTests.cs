using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ScrewPuzzle.Tests
{
    public sealed class WorkshopCompletionTests
    {
        [Test]
        public void NextToy_CyclesThroughAllBoards()
        {
            Assert.That(ThreeDBoardNavigation.NextScene(ThreeDBoardProgress.Radio), Is.EqualTo(ThreeDBoardNavigation.ToyCar));
            Assert.That(ThreeDBoardNavigation.NextScene(ThreeDBoardProgress.ToyCar), Is.EqualTo(ThreeDBoardNavigation.ToyRobot));
            Assert.That(ThreeDBoardNavigation.NextScene(ThreeDBoardProgress.ToyRobot), Is.EqualTo(ThreeDBoardNavigation.Radio));
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
            string[] faces = scene == ThreeDBoardNavigation.Radio ? new[] { "Front", "Back", "Left", "Right", "Inner" } :
                scene == ThreeDBoardNavigation.ToyCar ? new[] { "Near", "Far", "Rear", "Nose", "Inner" } : new[] { "Chest", "Back", "Left", "Right", "Inner" };
            foreach (int color in new[] { 0, 2, 1 })
                for (int face = 0; face < 3; face++)
                    yield return Take(faces[face] + " " + color, puzzle);
            for (int face = 3; face < 5; face++)
                for (int screw = 0; screw < 3; screw++)
                    yield return Take(faces[face] + " " + screw, puzzle);
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
