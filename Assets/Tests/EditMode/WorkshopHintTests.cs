using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace ScrewPuzzle.Tests
{
    public sealed class WorkshopHintTests : WorkshopRunTestIsolation
    {
        [UnityTest]
        public IEnumerator Hints_SolveEveryBoard_WithoutSpendingMovesOrRevealingLockedScrews()
        {
            EditorSceneManager.OpenScene(ThreeDBoardNavigation.Radio);
            yield return new EnterPlayMode(); yield return null;
            foreach (string id in new[] { ThreeDBoardProgress.Radio, ThreeDBoardProgress.ToyCar, ThreeDBoardProgress.ToyRobot, ThreeDBoardProgress.ToyTrain, ThreeDBoardProgress.ToyPlane })
            {
                if (id == ThreeDBoardProgress.ToyCar) ThreeDBoardNavigation.OpenToyCar();
                if (id == ThreeDBoardProgress.ToyRobot) ThreeDBoardNavigation.OpenToyRobot();
                if (id == ThreeDBoardProgress.ToyPlane) ThreeDBoardNavigation.OpenToyPlane();
                if (id == ThreeDBoardProgress.ToyTrain) ThreeDBoardNavigation.OpenToyTrain();
                yield return null; yield return null;
                var input = Object.FindFirstObjectByType<Radio3DInteraction>();
                var puzzle = Object.FindFirstObjectByType<Radio3DPuzzle>();
                var hint = Object.FindFirstObjectByType<WorkshopHint>();
                // Begin facing away from the initial front so hints exercise other faces.
                input.Radio.rotation = Quaternion.Euler(0, 150, 0);
                for (int i = 0; i < input.TotalCount; i++)
                {
                    string save = PlayerPrefs.GetString(WorkshopRunSave.Key(id));
                    hint.ShowHint();
                    var screw = hint.Target;
                    Assert.That(screw, Is.Not.Null, id + " move " + i);
                    var head = screw.transform.Find("Head Visual");
                    Assert.That(head, Is.Not.Null, id + " uses the shared recessed head");
                    Assert.That(head.localScale.x, Is.EqualTo(.2352f).Within(.001f));
                    Assert.That(head.GetComponent<Collider>(), Is.Null);
                    Assert.That(screw.transform.Find("Head").lossyScale.x, Is.EqualTo(.42f).Within(.001f));
                    Assert.That(screw.transform.Find("Head").GetComponent<Collider>().enabled, Is.True);
                    Assert.That(screw.IsAccessible, Is.True);
                    Assert.That(screw.IsRemoved, Is.False);
                    Assert.That(puzzle.RemovedCount, Is.EqualTo(i));
                    Assert.That(PlayerPrefs.GetString(WorkshopRunSave.Key(id)), Is.EqualTo(save));
                    Vector2 point = input.View.WorldToScreenPoint(screw.transform.position);
                    Assert.That(input.PickScrew(point), Is.EqualTo(screw));
                    input.BeginPointer(point); input.EndPointer(point);
                    Assert.That(screw.IsRemoved, Is.True, "The hint ring must not block a tap.");
                    hint.ShowHint();
                    Assert.That(puzzle.RemovedCount, Is.EqualTo(i + 1), "Busy hints must not collect anything.");
                    float deadline = Time.realtimeSinceStartup + 8;
                    while (puzzle.IsBusy && Time.realtimeSinceStartup < deadline) yield return null;
                    Assert.That(puzzle.IsBusy, Is.False);
                    Assert.That(puzzle.Trays[2].IsOpen, Is.False);
                }
                Assert.That(puzzle.State, Is.EqualTo(Radio3DPuzzle.PuzzleState.Won));
                yield return null;
                hint.ShowHint(); Assert.That(hint.Target, Is.Null);
                input.ResetExperiment(); hint.ShowHint(); Assert.That(hint.Target, Is.Not.Null);
                input.ResetExperiment(); Assert.That(hint.Target, Is.Null);
            }
        }
        [UnityTearDown]
        public IEnumerator LeavePlayMode() { if (Application.isPlaying) yield return new ExitPlayMode(); }
    }
}
