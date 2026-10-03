using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ScrewPuzzle.Tests
{
    public sealed class ThreeDBoardNavigationTests : WorkshopRunTestIsolation
    {
        [Test]
        public void TestBuilds_IncludeSelectorAndAllBoards_WithRequestedStartScene()
        {
            foreach (string first in new[] { ThreeDBoardNavigation.Selector, ThreeDBoardNavigation.Radio, ThreeDBoardNavigation.ToyCar, ThreeDBoardNavigation.ToyRobot })
            {
                var scenes = ThreeDBoardNavigation.BuildScenes(first);
                Assert.That(scenes[0], Is.EqualTo(first));
                CollectionAssert.AreEquivalent(new[] { ThreeDBoardNavigation.Selector, ThreeDBoardNavigation.Radio, ThreeDBoardNavigation.ToyCar, ThreeDBoardNavigation.ToyRobot }, scenes);
                foreach (string path in scenes) Assert.That(AssetDatabase.LoadAssetAtPath<SceneAsset>(path), Is.Not.Null);
            }
        }

        [UnityTest]
        public IEnumerator Selector_SwitchesBoardsDuringMotion_AndResumesWithoutDuplicates()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Selector);
            yield return new EnterPlayMode();
            yield return null;
            bool sound = FeedbackAudio.IsSoundEnabled;
            GameObject.Find("Play Radio").GetComponent<Button>().onClick.Invoke();
            yield return WaitForScene(ThreeDBoardNavigation.Radio);
            Assert.That(Object.FindFirstObjectByType<ToyCar3DPrototypeBootstrap>(), Is.Null);
            var puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.TryAdd(GameObject.Find("Front 0").GetComponent<Radio3DScrew>()), Is.True);
            Assert.That(puzzle.IsBusy, Is.True);
            GameObject.Find("Boards").GetComponent<Button>().onClick.Invoke();
            yield return WaitForScene(ThreeDBoardNavigation.Selector);
            Assert.That(Object.FindObjectsByType<Radio3DScrew>(FindObjectsSortMode.None), Is.Empty);
            GameObject.Find("Play Toy Car").GetComponent<Button>().onClick.Invoke();
            yield return WaitForScene(ThreeDBoardNavigation.ToyCar);
            Assert.That(Object.FindFirstObjectByType<ToyCar3DPrototypeBootstrap>(), Is.Not.Null);
            puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.RemovedCount, Is.Zero);
            Assert.That(puzzle.UnlockTestTray(2), Is.True);
            Assert.That(puzzle.TryAdd(GameObject.Find("Near 0").GetComponent<Radio3DScrew>()), Is.True);
            GameObject.Find("Boards").GetComponent<Button>().onClick.Invoke();
            yield return WaitForScene(ThreeDBoardNavigation.Selector);
            GameObject.Find("Play Toy Car").GetComponent<Button>().onClick.Invoke();
            yield return WaitForScene(ThreeDBoardNavigation.ToyCar);
            puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(puzzle.RemovedCount, Is.EqualTo(1));
            Assert.That(puzzle.Trays[2].IsOpen, Is.True);
            Assert.That(Object.FindObjectsByType<Radio3DScrew>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length, Is.EqualTo(15));
            Assert.That(FeedbackAudio.IsSoundEnabled, Is.EqualTo(sound));
            GameObject.Find("Boards").GetComponent<Button>().onClick.Invoke();
            yield return WaitForScene(ThreeDBoardNavigation.Selector);
        }

        private static IEnumerator WaitForScene(string path)
        {
            float deadline = Time.realtimeSinceStartup + 10f;
            while (SceneManager.GetActiveScene().path != path && Time.realtimeSinceStartup < deadline) yield return null;
            yield return null;
            yield return null;
            Assert.That(SceneManager.GetActiveScene().path, Is.EqualTo(path));
            Assert.That(Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
            Assert.That(Object.FindObjectsByType<Camera>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
        }

        [UnityTearDown]
        public IEnumerator LeavePlayMode()
        {
            if (Application.isPlaying) yield return new ExitPlayMode();
        }
    }
}
