using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public sealed class ToyCar3DTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator Car_AllFacesAndInnerLayer_ClearWithFreeTrays_AndRestart()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Experiment_ToyCar3D.unity");
            yield return new EnterPlayMode();
            yield return null;
            yield return null;
            PlayerPrefs.DeleteKey(ThreeDBoardProgress.Key(ThreeDBoardProgress.ToyCar));
            var input = Object.FindFirstObjectByType<Radio3DInteraction>();
            var puzzle = input.GetComponent<Radio3DPuzzle>();
            var motor = GameObject.Find("Motor");
            var hood = GameObject.Find("Hood");
            var spoiler = GameObject.Find("Rear Spoiler");
            Assert.That(spoiler.transform.parent.name, Is.EqualTo("Rear Bumper Assembly"));
            var backdrop = GameObject.Find("Racetrack Background");
            Assert.That(backdrop, Is.Not.Null);
            Assert.That(backdrop.GetComponent<Collider>(), Is.Null);
            Assert.That(backdrop.GetComponent<Renderer>().sharedMaterial.mainTexture, Is.Not.Null);
            Assert.That(backdrop.GetComponent<Renderer>().sharedMaterial.shader.isSupported, Is.True);
            Assert.That(input.AllowPitch, Is.True);
            Assert.That(input.GetComponent<WorkshopTrayPresentation>(), Is.Not.Null);
            Assert.That(motor.transform.parent.name, Is.EqualTo("Inner Side Assembly"));
            Assert.That(hood.transform.parent.name, Is.EqualTo("Nose Assembly"));
            Assert.That(Object.FindFirstObjectByType<ToyCar3DPrototypeBootstrap>(), Is.Not.Null);
            Assert.That(input.TotalCount, Is.EqualTo(15));
            Quaternion initial = input.Radio.localRotation;
            var inner = GameObject.Find("Inner 0").GetComponent<Radio3DScrew>();
            var nose = GameObject.Find("Nose 0").GetComponent<Radio3DScrew>();
            Vector3 noseScale = nose.transform.localScale;
            var targets = Object.FindObjectsByType<Radio3DScrew>(FindObjectsSortMode.None);
            int topCount = 0, bottomCount = 0;
            foreach (var target in targets)
            {
                float vertical = input.Radio.InverseTransformDirection(target.transform.up).y;
                if (vertical > .9f) topCount++;
                if (vertical < -.9f) bottomCount++;
            }
            Assert.That(topCount, Is.EqualTo(2), "Two clearly seated hood screws");
            Assert.That(bottomCount, Is.EqualTo(2), "Two underside screws");
            Assert.That(puzzle.TryAdd(inner), Is.False);
            Assert.That(ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.ToyCar), Is.False);
            for (int angle = 0; angle < 360; angle += 45)
            {
                input.Radio.rotation = Quaternion.Euler(0, angle, 0);
                Physics.SyncTransforms();
                Assert.That(input.PickScrew(input.View.WorldToScreenPoint(inner.transform.position)), Is.Not.EqualTo(inner));
            }
            foreach (string name in new[] { "Near 0", "Near 1", "Near 2", "Far 0", "Far 2", "Rear 1",
                "Nose 0", "Nose 1", "Inner 0", "Inner 1", "Rear 2", "Nose 2", "Far 1", "Rear 0", "Inner 2" })
            {
                var screw = GameObject.Find(name).GetComponent<Radio3DScrew>();
                Vector3 normal = input.Radio.InverseTransformDirection(screw.transform.up);
                input.Radio.rotation = Quaternion.FromToRotation(normal, Vector3.back);
                Physics.SyncTransforms();
                Vector2 point = input.View.WorldToScreenPoint(screw.transform.position);
                Assert.That(input.PickScrew(point), Is.EqualTo(screw), name + " must be reachable around the car geometry.");
                if (Mathf.Abs(normal.y) > .9f)
                {
                    Vector3 crown = screw.transform.Find("Head Visual").TransformPoint(Vector3.up * .8f);
                    Vector3 ray = crown - input.View.transform.position;
                    foreach (var hit in Physics.RaycastAll(input.View.transform.position, ray.normalized, ray.magnitude))
                        Assert.That(hit.collider.GetComponentInParent<Radio3DScrew>(), Is.EqualTo(screw),
                            name + ": head obscured by " + hit.collider.name);
                }
                input.BeginPointer(point);
                input.EndPointer(point);
                Assert.That(screw.IsRemoved, Is.True);
                float deadline = Time.realtimeSinceStartup + 5f;
                while (puzzle.IsBusy && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(puzzle.IsBusy, Is.False);
                Assert.That(ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.ToyCar), Is.EqualTo(name == "Inner 2"));
                Assert.That(puzzle.Trays[2].IsOpen, Is.False);
                if (name == "Near 2") Assert.That(inner.IsAccessible, Is.True);
                if (name == "Nose 2") Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Playing));
            }
            Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
            Assert.That(puzzle.ClearedCount, Is.EqualTo(15));
            Assert.That(puzzle.ReleasedPlateCount, Is.EqualTo(5));
            Assert.That(motor.activeInHierarchy, Is.False);
            Assert.That(hood.activeInHierarchy, Is.False);
            Assert.That(spoiler.activeInHierarchy, Is.False);
            input.ResetExperiment();
            Assert.That(motor.activeInHierarchy, Is.True);
            Assert.That(hood.activeInHierarchy, Is.True);
            Assert.That(spoiler.activeInHierarchy, Is.True);
            Assert.That(input.Radio.localRotation, Is.EqualTo(initial));
            Assert.That(puzzle.RemovedCount, Is.Zero);
            Assert.That(puzzle.ReleasedPlateCount, Is.Zero);
            Assert.That(inner.IsAccessible, Is.False);
            Assert.That(ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.ToyCar), Is.True, "Restart must preserve completion.");
            Assert.That(nose.transform.localScale, Is.EqualTo(noseScale));
            foreach (var screw in targets)
            {
                Assert.That(screw.gameObject.activeSelf, Is.True);
                Assert.That(screw.IsRemoved, Is.False);
            }
            ThreeDBoardNavigation.OpenSelector();
            yield return null;
            yield return null;
            Assert.That(GameObject.Find("Toy Car Completed"), Is.Not.Null);
            Assert.That(GameObject.Find("Racetrack Background"), Is.Null);
            Assert.That(GameObject.Find("Start Toy Car").GetComponentInChildren<UnityEngine.UI.Text>().text, Does.StartWith("REPLAY"));
        }

        [UnityTearDown]
        public IEnumerator LeavePlayMode()
        {
            if (Application.isPlaying) yield return new ExitPlayMode();
        }
    }
}
