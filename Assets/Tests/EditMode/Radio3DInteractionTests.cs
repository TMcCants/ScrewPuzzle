using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public sealed class Radio3DInteractionTests
    {
        [UnityTest]
        public IEnumerator RotationAndSelection_RespectVisibilityAndGestureIntent()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Experiment_Radio3D.unity");
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            Radio3DInteraction input = Object.FindFirstObjectByType<Radio3DInteraction>();
            Assert.That(input.TotalCount, Is.EqualTo(12));
            Radio3DScrew front = GameObject.Find("Front 0").GetComponent<Radio3DScrew>();
            Radio3DScrew rear = GameObject.Find("Back 0").GetComponent<Radio3DScrew>();
            Physics.SyncTransforms();
            Vector2 point = input.View.WorldToScreenPoint(front.transform.position);
            Assert.That(input.PickScrew(point), Is.EqualTo(front));
            Assert.That(input.PickScrew(input.View.WorldToScreenPoint(rear.transform.position)), Is.Not.EqualTo(rear));

            input.BeginPointer(point);
            input.MovePointer(point + Vector2.right * (input.DragThreshold + 30f));
            input.MovePointer(point);
            input.EndPointer(point);
            Assert.That(input.RemovedCount, Is.Zero, "A drag returning to its start must not become a tap.");
            input.ResetExperiment();
            Physics.SyncTransforms();
            input.BeginPointer(point);
            input.CancelPointer();
            input.EndPointer(point);
            Assert.That(input.RemovedCount, Is.Zero);
            input.BeginPointer(point);
            input.EndPointer(point);
            Assert.That(input.RemovedCount, Is.EqualTo(1));
            Assert.That(front.IsRemoved, Is.True);
            float midFlight = Time.time + 0.1f;
            while (Time.time < midFlight) yield return null;
            input.ResetExperiment();
            Assert.That(front.gameObject.activeSelf, Is.True);
            Assert.That(front.IsRemoved, Is.False);
            Assert.That(front.transform.parent, Is.EqualTo(input.Radio));
            float afterOldAnimation = Time.time + 0.7f;
            while (Time.time < afterOldAnimation) yield return null;
            Assert.That(input.RemovedCount, Is.Zero);
            Assert.That(input.GetComponent<Radio3DPuzzle>().HeldCount, Is.Zero);

            Radio3DPuzzle puzzle = input.GetComponent<Radio3DPuzzle>();
            Assert.That(puzzle.Trays[0].Color, Is.EqualTo(ScrewColorId.Red));
            Assert.That(puzzle.Trays[1].Color, Is.EqualTo(ScrewColorId.Blue));
            Assert.That(puzzle.Trays[2].IsOpen, Is.False);
            var yellow = GameObject.Find("Front 2").GetComponent<Radio3DScrew>();
            Assert.That(puzzle.TryAdd(yellow), Is.False);
            Assert.That(yellow.IsRemoved, Is.False);
            Assert.That(puzzle.RemovedCount, Is.Zero);
            // Both free trays alone must be sufficient to win.
            foreach (string name in new[] { "Front 0", "Back 0", "Left 0", "Front 2", "Back 2", "Left 2",
                "Front 1", "Back 1", "Left 1", "Right 0", "Right 1", "Right 2" })
            {
                Select(input, name);
                yield return Settle(puzzle);
                if (name == "Left 0")
                {
                    Assert.That(puzzle.Trays[0].Color, Is.EqualTo(ScrewColorId.Yellow));
                    Assert.That(puzzle.Trays[0].Count, Is.Zero);
                    Assert.That(puzzle.ClearedCount, Is.EqualTo(3));
                }
                Assert.That(puzzle.Trays[2].IsOpen, Is.False);
            }
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
            Assert.That(puzzle.HeldCount, Is.Zero);
            Assert.That(puzzle.ClearedCount, Is.EqualTo(12));
            input.ResetExperiment();
            Assert.That(puzzle.UnlockTestTray(2), Is.True);
            Assert.That(puzzle.Trays[2].Color, Is.EqualTo(ScrewColorId.Yellow));
            Assert.That(puzzle.UnlockTestTray(3), Is.True);
            Assert.That(puzzle.Trays[3].Color, Is.EqualTo(ScrewColorId.Red));
            Assert.That(puzzle.UnlockTestTray(2), Is.False);
            foreach (string name in new[] { "Front 2", "Back 2", "Left 2", "Front 0", "Back 0", "Left 0",
                "Right 0", "Right 1", "Right 2", "Front 1", "Back 1", "Left 1" })
            {
                Select(input, name);
                yield return Settle(puzzle);
            }
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
            input.ResetExperiment();
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Playing));
            Assert.That(puzzle.HeldCount, Is.Zero);
            Assert.That(puzzle.ClearedCount, Is.Zero);
            Assert.That(puzzle.Trays[2].IsOpen, Is.False);
            Assert.That(puzzle.Trays[3].IsOpen, Is.False);
        }
        private static void Select(Radio3DInteraction input, string name)
        {
            Vector3 normal = name.StartsWith("Front") ? Vector3.back :
                name.StartsWith("Back") ? Vector3.forward :
                name.StartsWith("Left") ? Vector3.left : Vector3.right;
            input.Radio.rotation = Quaternion.FromToRotation(normal, Vector3.back);
            Physics.SyncTransforms();
            Radio3DScrew screw = GameObject.Find(name).GetComponent<Radio3DScrew>();
            Vector2 point = input.View.WorldToScreenPoint(screw.transform.position);
            Assert.That(input.PickScrew(point), Is.EqualTo(screw), name);
            input.BeginPointer(point);
            input.EndPointer(point);
            Assert.That(screw.IsRemoved, Is.True, name);
        }

        private static IEnumerator Settle(Radio3DPuzzle puzzle)
        {
            float deadline = Time.realtimeSinceStartup + 5f;
            while (puzzle.IsBusy && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(puzzle.IsBusy, Is.False, "Tray resolution timed out.");
        }

        [UnityTearDown]
        public IEnumerator LeavePlayMode()
        {
            if (Application.isPlaying) yield return new ExitPlayMode();
        }
    }
}

