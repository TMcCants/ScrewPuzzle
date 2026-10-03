using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ScrewPuzzle.Tests
{
    public sealed class WorkshopTutorialTests
    {
        [UnityTest]
        public IEnumerator Tutorial_FollowsRealActions_ThenStaysFinishedAcrossBoards()
        {
            PlayerPrefs.DeleteKey(WorkshopTutorial.PreferenceKey);
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Radio);
            yield return new EnterPlayMode(); yield return null;
            var input = Object.FindFirstObjectByType<Radio3DInteraction>();
            var lesson = Object.FindFirstObjectByType<WorkshopTutorial>();
            var puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
            Assert.That(lesson.IsActive, Is.True);
            Assert.That(lesson.Step, Is.Zero);
            Rotate(input);
            Assert.That(lesson.Step, Is.EqualTo(1));
            input.ResetExperiment();
            Assert.That(lesson.Step, Is.Zero);
            Rotate(input);
            foreach (string name in new[] { "Front 0", "Back 0", "Left 0" }) yield return Take(puzzle, name);
            yield return null;
            Assert.That(lesson.Step, Is.EqualTo(2));
            Assert.That(WorkshopTutorial.HasFinished, Is.False);
            foreach (string name in new[] { "Front 2", "Back 2", "Left 2", "Front 1", "Back 1", "Left 1", "Right 0", "Right 1", "Right 2" }) yield return Take(puzzle, name);
            Assert.That(lesson.IsActive, Is.True, "Revealing a layer is not the same as removing an inner screw.");
            yield return Take(puzzle, "Inner 0"); yield return null;
            Assert.That(lesson.IsActive, Is.False);
            Assert.That(WorkshopTutorial.HasFinished, Is.True);
            ThreeDBoardNavigation.OpenToyCar(); yield return null; yield return null;
            Assert.That(Object.FindFirstObjectByType<WorkshopTutorial>().IsActive, Is.False);
        }

        [UnityTest]
        public IEnumerator Skip_Persists_WithoutChangingPuzzleOrBoardCompletion()
        {
            PlayerPrefs.DeleteKey(WorkshopTutorial.PreferenceKey);
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.ToyRobot);
            yield return new EnterPlayMode(); yield return null;
            bool completed = ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.ToyRobot);
            GameObject.Find("Skip Tutorial").GetComponent<Button>().onClick.Invoke();
            Assert.That(WorkshopTutorial.HasFinished, Is.True);
            Assert.That(Object.FindFirstObjectByType<Radio3DPuzzle>().RemovedCount, Is.Zero);
            Assert.That(ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.ToyRobot), Is.EqualTo(completed));
            ThreeDBoardNavigation.OpenRadio(); yield return null; yield return null;
            Assert.That(Object.FindFirstObjectByType<WorkshopTutorial>().IsActive, Is.False);
        }
        private static void Rotate(Radio3DInteraction input)
        {
            Vector2 start = input.View.WorldToScreenPoint(input.Radio.position);
            input.BeginPointer(start);
            input.EndPointer(start + Vector2.right * Screen.width * 0.12f);
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
