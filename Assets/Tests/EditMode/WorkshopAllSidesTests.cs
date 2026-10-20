using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ScrewPuzzle.Tests
{
    public sealed class WorkshopAllSidesTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator EveryToy_OrbitsAndUsesCoveredTrays_NewVerticalScrewsAreReachable()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Radio);
            yield return new EnterPlayMode(); yield return null;
            for (int board = 0; board < 5; board++)
            {
                if (board == 1) ThreeDBoardNavigation.OpenToyCar();
                if (board == 2) ThreeDBoardNavigation.OpenToyRobot();
                if (board == 3) ThreeDBoardNavigation.OpenToyTrain();
                if (board == 4) ThreeDBoardNavigation.OpenToyPlane();
                yield return null; yield return null;
                var input = Object.FindFirstObjectByType<Radio3DInteraction>();
                var puzzle = input.GetComponent<Radio3DPuzzle>();
                Assert.That(input.AllowPitch, Is.True);
                Assert.That(input.GetComponent<WorkshopTrayPresentation>(), Is.Not.Null);
                Assert.That(input.ExplainLockedScrews, Is.True);
                if (board == 1)
                {
                    var inner = GameObject.Find("Inner 0").GetComponent<Radio3DScrew>();
                    Physics.SyncTransforms();
                    Assert.That(input.PickVisibleScrew(input.View.WorldToScreenPoint(inner.transform.position)),
                        Is.Not.EqualTo(inner), "Covered heads must not produce floating lock badges through the body.");
                }
                Quaternion original = input.Radio.rotation;
                Vector2 center = new Vector2(Screen.width * .5f, Screen.height * .54f);
                foreach (var direction in new[] { Vector2.up, Vector2.right })
                {
                    for (int turn = 0; turn < 4; turn++)
                    {
                        input.BeginPointer(center);
                        input.EndPointer(center + Vector2.Scale(direction, new Vector2(Screen.width, Screen.height)) * .3f);
                    }
                    Assert.That(Quaternion.Angle(original, input.Radio.rotation), Is.LessThan(.1f));
                    Assert.That(puzzle.RemovedCount, Is.Zero);
                }
                int top = 0, bottom = 0;
                foreach (var screw in input.Radio.GetComponentsInChildren<Radio3DScrew>())
                {
                    Vector3 normal = input.Radio.InverseTransformDirection(screw.transform.up);
                    if (normal.y > .9f) top++;
                    else if (normal.y < -.9f) bottom++;
                    else continue;
                    if (board >= 3) continue; // Their layered vertical screws have separate solve tests.
                    input.Radio.rotation = Quaternion.FromToRotation(normal, Vector3.back);
                    Physics.SyncTransforms();
                    Assert.That(input.PickScrew(input.View.WorldToScreenPoint(screw.transform.position)),
                        Is.EqualTo(screw), screw.name + " must be reachable from above/below");
                }
                Assert.That(top, Is.GreaterThan(0));
                Assert.That(bottom, Is.GreaterThan(0));
                GameObject.Find("Reset View").GetComponent<Button>().onClick.Invoke();
                Assert.That(Quaternion.Angle(original, input.Radio.rotation), Is.LessThan(.1f));
                var lid = GameObject.Find("Tray Lid 2");
                Assert.That(lid, Is.Not.Null);
                GameObject.Find("Unlock Tray 2").GetComponent<Button>().onClick.Invoke();
                GameObject.Find("Cancel Unlock").GetComponent<Button>().onClick.Invoke();
                Assert.That(puzzle.Trays[2].IsOpen, Is.False);
                GameObject.Find("Unlock Tray 2").GetComponent<Button>().onClick.Invoke();
                GameObject.Find("Preview Open").GetComponent<Button>().onClick.Invoke();
                float deadline = Time.realtimeSinceStartup + 3;
                while (lid.activeSelf && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.That(lid.activeSelf, Is.False);
                Assert.That(puzzle.Trays[2].IsOpen, Is.True);
                Assert.That(puzzle.Trays[3].IsOpen, Is.False);
                input.ResetExperiment(); yield return null;
                Assert.That(lid.activeSelf, Is.True);
                Assert.That(puzzle.Trays[2].IsOpen, Is.False);
            }
        }

        [UnityTearDown]
        public IEnumerator LeavePlayMode() { if (Application.isPlaying) yield return new ExitPlayMode(); }
    }
}
