using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public sealed class Radio3DInteractionTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator Feedback_RespectsSoundPreference_AndStopsOnRestart()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Experiment_Radio3D.unity");
            yield return new EnterPlayMode();
            yield return null;
            bool hadPreference = PlayerPrefs.HasKey("SoundEnabled");
            int preference = PlayerPrefs.GetInt("SoundEnabled", 1);
            try
            {
                var feedback = Object.FindFirstObjectByType<Radio3DFeedback>();
                var source = feedback.GetComponent<AudioSource>();
                PlayerPrefs.SetInt("SoundEnabled", 0);
                feedback.PlayUnscrew();
                Assert.That(source.mute, Is.True);
                PlayerPrefs.SetInt("SoundEnabled", 1);
                feedback.PlayRelease();
                Assert.That(source.mute, Is.False);
                Object.FindFirstObjectByType<Radio3DInteraction>().ResetExperiment();
                Assert.That(source.isPlaying, Is.False);
                Assert.That(Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length, Is.EqualTo(1));
            }
            finally
            {
                if (hadPreference) PlayerPrefs.SetInt("SoundEnabled", preference);
                else PlayerPrefs.DeleteKey("SoundEnabled");
            }
        }

        [UnityTest]
        public IEnumerator RotationAndSelection_RespectVisibilityAndGestureIntent()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Experiment_Radio3D.unity");
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            Radio3DInteraction input = Object.FindFirstObjectByType<Radio3DInteraction>();
            Assert.That(input.TotalCount, Is.EqualTo(15));
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
            Vector3 screwStart = front.transform.position;
            Quaternion screwRotation = front.transform.rotation;
            input.BeginPointer(point);
            input.EndPointer(point);
            Assert.That(input.RemovedCount, Is.EqualTo(1));
            Assert.That(front.IsRemoved, Is.True);
            float midFlight = Time.time + 0.1f;
            while (Time.time < midFlight) yield return null;
            Vector3 lift = front.transform.position - screwStart;
            Vector3 shaft = screwRotation * Vector3.up;
            Assert.That(Vector3.Dot(lift, shaft), Is.GreaterThan(0f), "Unscrewing lifts along the shaft before tray flight.");
            Assert.That(Vector3.Cross(lift, shaft).magnitude, Is.LessThan(0.001f));
            Assert.That(Quaternion.Angle(screwRotation, front.transform.rotation), Is.GreaterThan(1f));
            input.ResetExperiment();
            Assert.That(front.gameObject.activeSelf, Is.True);
            Assert.That(front.IsRemoved, Is.False);
            Assert.That(front.transform.parent, Is.EqualTo(input.Radio));
            float afterOldAnimation = Time.time + 0.7f;
            while (Time.time < afterOldAnimation) yield return null;
            Assert.That(input.RemovedCount, Is.Zero);
            Assert.That(input.GetComponent<Radio3DPuzzle>().HeldCount, Is.Zero);

            // Restart after the unscrew phase, while the screw is traveling to the tray.
            Select(input, "Front 0");
            float duringFlight = Time.time + 0.46f;
            while (Time.time < duringFlight) yield return null;
            input.ResetExperiment();
            float settleReset = Time.time + 0.8f;
            while (Time.time < settleReset) yield return null;
            Assert.That(front.IsRemoved, Is.False);
            Assert.That(front.transform.parent, Is.EqualTo(input.Radio));
            Assert.That(input.RemovedCount, Is.Zero);

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
                "Front 1", "Back 1", "Left 1", "Right 0", "Right 1", "Right 2", "Inner 0", "Inner 1", "Inner 2" })
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
                if (name == "Right 2") Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Playing), "The inner layer is still unfinished.");
            }
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
            Assert.That(puzzle.HeldCount, Is.Zero);
            Assert.That(puzzle.ClearedCount, Is.EqualTo(15));
            Assert.That(puzzle.ReleasedPlateCount, Is.EqualTo(5));
            input.ResetExperiment();
            Assert.That(puzzle.UnlockTestTray(2), Is.True);
            Assert.That(puzzle.Trays[2].Color, Is.EqualTo(ScrewColorId.Yellow));
            Assert.That(puzzle.UnlockTestTray(3), Is.True);
            Assert.That(puzzle.Trays[3].Color, Is.EqualTo(ScrewColorId.Red));
            Assert.That(puzzle.UnlockTestTray(2), Is.False);
            foreach (string name in new[] { "Front 2", "Back 2", "Left 2", "Front 0", "Back 0", "Left 0",
                "Right 0", "Right 1", "Right 2", "Front 1", "Inner 0", "Inner 1", "Inner 2", "Back 1", "Left 1" })
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
            Assert.That(puzzle.ReleasedPlateCount, Is.Zero);
        }

        [UnityTest]
        public IEnumerator Plates_ReleaseOnlyAfterTheirLastScrew_AndRestoreDuringMotion()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Experiment_Radio3D.unity");
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            var input = Object.FindFirstObjectByType<Radio3DInteraction>();
            var puzzle = input.GetComponent<Radio3DPuzzle>();
            var front = GameObject.Find("Front Plate Assembly").GetComponent<Radio3DPlate>();
            var rear = GameObject.Find("Rear Plate Assembly").GetComponent<Radio3DPlate>();
            var inner = GameObject.Find("Inner 0").GetComponent<Radio3DScrew>();
            Assert.That(inner.IsAccessible, Is.False);
            Assert.That(puzzle.TryAdd(inner), Is.False);
            for (int angle = 0; angle < 360; angle += 45)
            {
                input.Radio.rotation = Quaternion.Euler(0, angle, 0);
                Physics.SyncTransforms();
                Assert.That(input.PickScrew(input.View.WorldToScreenPoint(inner.transform.position)), Is.Not.EqualTo(inner));
            }
            Transform speaker = GameObject.Find("Speaker").transform;
            Vector3 origin = front.transform.localPosition;
            Quaternion orientation = front.transform.localRotation;
            Vector3 scale = front.transform.localScale;
            Assert.That(speaker.IsChildOf(front.transform), Is.True);
            puzzle.UnlockTestTray(2);
            Select(input, "Front 0");
            yield return Settle(puzzle);
            Select(input, "Front 1");
            yield return Settle(puzzle);
            Assert.That(front.IsReleased, Is.False, "Two screws must not release a three-screw plate.");
            Select(input, "Front 2");
            float deadline = Time.realtimeSinceStartup + 5f;
            while (!front.IsAnimating && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(front.IsAnimating, Is.True);
            Assert.That(inner.IsAccessible, Is.False, "The inner screws stay locked throughout the outer plate animation.");
            Assert.That(puzzle.CanInteract, Is.False);
            Assert.That(rear.IsReleased, Is.False, "Removing front screws must not release another face.");
            foreach (Collider collider in front.GetComponentsInChildren<Collider>()) Assert.That(collider.enabled, Is.False);
            input.ResetExperiment();
            float until = Time.time + 1f;
            while (Time.time < until) yield return null;
            Assert.That(front.transform.localPosition, Is.EqualTo(origin));
            Assert.That(front.transform.localRotation, Is.EqualTo(orientation));
            Assert.That(front.transform.localScale, Is.EqualTo(scale));
            Assert.That(front.IsReleased, Is.False);
            Assert.That(front.IsAnimating, Is.False);
            Assert.That(speaker.gameObject.activeInHierarchy, Is.True);
            foreach (Collider collider in front.GetComponentsInChildren<Collider>()) Assert.That(collider.enabled, Is.True);
            Assert.That(puzzle.RemovedCount, Is.Zero);
            Assert.That(inner.IsAccessible, Is.False);
            puzzle.UnlockTestTray(2);
            foreach (string name in new[] { "Front 0", "Front 1", "Front 2" })
            {
                Select(input, name);
                yield return Settle(puzzle);
            }
            Assert.That(front.IsReleased, Is.True);
            Assert.That(inner.IsAccessible, Is.True);
            Assert.That(front.gameObject.activeSelf, Is.False);
            Assert.That(speaker.gameObject.activeInHierarchy, Is.False);
            Assert.That(rear.gameObject.activeSelf, Is.True);
            Assert.That(puzzle.ReleasedPlateCount, Is.EqualTo(1));
            var rearScrew = GameObject.Find("Back 0").GetComponent<Radio3DScrew>();
            Physics.SyncTransforms();
            Assert.That(input.PickScrew(input.View.WorldToScreenPoint(rearScrew.transform.position)),
                Is.Not.EqualTo(rearScrew), "The inner chassis still blocks selection through the radio.");
            var innerPlate = GameObject.Find("Inner Front Plate Assembly").GetComponent<Radio3DPlate>();
            puzzle.UnlockTestTray(3);
            Select(input, "Inner 0");
            yield return Settle(puzzle);
            Select(input, "Inner 1");
            yield return Settle(puzzle);
            Assert.That(innerPlate.IsReleased, Is.False);
            Select(input, "Inner 2");
            deadline = Time.realtimeSinceStartup + 5f;
            while (!innerPlate.IsAnimating && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(innerPlate.IsAnimating, Is.True);
            input.ResetExperiment();
            until = Time.time + 1f;
            while (Time.time < until) yield return null;
            Assert.That(innerPlate.gameObject.activeSelf, Is.True);
            Assert.That(innerPlate.IsReleased, Is.False);
            Assert.That(front.gameObject.activeSelf, Is.True);
            Assert.That(inner.IsAccessible, Is.False);
            Assert.That(inner.IsRemoved, Is.False);
            Assert.That(puzzle.ReleasedPlateCount, Is.Zero);
        }
        private static void Select(Radio3DInteraction input, string name)
        {
            Vector3 normal = name.StartsWith("Front") || name.StartsWith("Inner") ? Vector3.back :
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

