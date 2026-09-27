using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace ScrewPuzzle.Tests
{
    public sealed class OldCameraLevelTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void SafeRoutes_ClearAllScrews(bool yellowFirst)
        {
            int[] middle = yellowFirst ? new[] { 6, 7, 8, 3, 4, 5 } : new[] { 3, 4, 5, 6, 7, 8 };
            int[] route = new[] { 0, 1, 2 }.Concat(middle).Concat(new[] { 9, 10, 11 }).ToArray();
            Assert.That(PlayRoute(route), Is.Empty);
        }

        [Test]
        public void MixedRoute_FillsTrayWithoutMatch()
        {
            Assert.That(PlayRoute(new[] { 0, 3, 6, 1, 4 }).Count, Is.EqualTo(5));
        }

        [UnityEngine.TestTools.UnityTest]
        public System.Collections.IEnumerator CameraRuntime_LosesThenRestartsAndWins()
        {
            yield return new UnityEngine.TestTools.EnterPlayMode();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level04_OldCamera");
            yield return null;
            yield return null;
            foreach (string artName in new[] { "Camera Body Art", "Flash Housing Art",
                "Film Door Art", "Grip Art", "Lens Art" })
            {
                GameObject art = GameObject.Find(artName);
                Assert.That(art, Is.Not.Null, artName + " must load production artwork.");
                SpriteRenderer renderer = art.GetComponent<SpriteRenderer>();
                Assert.That(renderer.sprite, Is.Not.Null);
                Assert.That(renderer.sortingOrder, Is.LessThan(10), "Screws must remain above art.");
            }
            GameManager game = Object.FindFirstObjectByType<GameManager>();
            foreach (int index in new[] { 0, 3, 6, 1, 4 })
            {
                SelectRuntimeScrew(index);
                float readyAt = Time.time + 0.9f;
                while (Time.time < readyAt) yield return null;
            }
            Assert.That(game.State, Is.EqualTo(GameManager.LevelState.Lost));
            game.RestartLevel();
            yield return null;
            yield return null;
            game = Object.FindFirstObjectByType<GameManager>();
            const string key = "ScrewPuzzle.HighestUnlockedLevel";
            bool existed = PlayerPrefs.HasKey(key);
            int saved = PlayerPrefs.GetInt(key);
            try
            {
                foreach (int index in new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 })
                {
                    SelectRuntimeScrew(index);
                    float readyAt = Time.time + 0.9f;
                while (Time.time < readyAt) yield return null;
                }
                float restoredAt = Time.time + 2f;
                while (Time.time < restoredAt) yield return null;
                Assert.That(game.State, Is.EqualTo(GameManager.LevelState.Won));
                Assert.That(Object.FindFirstObjectByType<TrayManager>().HeldCount, Is.Zero);
                Assert.That(GameObject.Find("Flash Glass").GetComponent<SpriteRenderer>().color.r,
                    Is.LessThan(0.5f), "Flash should return to idle before the result.");
                Assert.That(Object.FindObjectsByType<UnityEngine.UI.Text>(FindObjectsSortMode.None)
                    .Any(text => text.text == "RESTORED!\nReady for another memory."), Is.True);
            }
            finally
            {
                if (existed) PlayerPrefs.SetInt(key, saved);
                else PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }
        }

        [UnityEngine.TestTools.UnityTearDown]
        public System.Collections.IEnumerator LeavePlayMode()
        {
            if (Application.isPlaying) yield return new UnityEngine.TestTools.ExitPlayMode();
        }

        private static void SelectRuntimeScrew(int index)
        {
            LevelDefinition level = OldCameraLevelBootstrap.CreateLevelDefinition();
            string name = level.Screws[index].ColorId + " Screw " + (index + 1);
            Screw screw = GameObject.Find(name).GetComponent<Screw>();
            screw.TrySelect();
            Assert.That(screw.IsRemovedFromObject, Is.True, name);
        }

        private static List<ScrewColorId> PlayRoute(int[] route)
        {
            LevelDefinition level = OldCameraLevelBootstrap.CreateLevelDefinition();
            var removed = new HashSet<int>();
            var tray = new List<ScrewColorId>();
            foreach (int index in route)
            {
                Assert.That(tray.Count, Is.LessThan(level.TrayCapacity));
                foreach (int blocker in level.Screws[index].BlockerIndexes)
                    Assert.That(removed.Contains(blocker), Is.True, "Blocked screw: " + index);
                Assert.That(removed.Add(index), Is.True);
                tray.Add(level.Screws[index].ColorId);
                List<int> match = TrayRules.FindFirstMatch(tray, level.MatchSize);
                for (int i = match.Count - 1; i >= 0; i--) tray.RemoveAt(match[i]);
            }
            return tray;
        }

        [Test]
        public void CameraScene_IsEnabledAndHasBootstrap()
        {
            const string path = "Assets/Scenes/Level04_OldCamera.unity";
            Assert.That(EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path == path), Is.True);
            var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path,
                UnityEditor.SceneManagement.OpenSceneMode.Additive);
            try
            {
                Assert.That(scene.GetRootGameObjects().Any(root =>
                    root.GetComponent<OldCameraLevelBootstrap>() != null), Is.True);
            }
            finally
            {
                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
            }
        }

        [Test]
        public void CompletingRobot_UnlocksCamera_AndCameraDoesNotUnlockMissingLevel()
        {
            const string key = "ScrewPuzzle.HighestUnlockedLevel";
            bool existed = PlayerPrefs.HasKey(key);
            int saved = PlayerPrefs.GetInt(key);
            try
            {
                PlayerPrefs.SetInt(key, 3);
                Assert.That(ProgressManager.IsLevelUnlocked(4), Is.False);
                ProgressManager.RecordLevelCompleted(3);
                Assert.That(ProgressManager.IsLevelUnlocked(4), Is.True);
                ProgressManager.RecordLevelCompleted(4);
                Assert.That(ProgressManager.HighestUnlockedLevel, Is.EqualTo(4));
                ProgressManager.RecordLevelCompleted(1);
                Assert.That(ProgressManager.HighestUnlockedLevel, Is.EqualTo(4));
            }
            finally
            {
                if (existed) PlayerPrefs.SetInt(key, saved);
                else PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }
        }
    }
}
