using NUnit.Framework;
using UnityEngine;

namespace ScrewPuzzle.Tests
{
    public class WorkshopRunTestIsolation
    {
        [SetUp]
        public void StartWithoutUnfinishedRuns()
        {
            foreach (string board in new[] { ThreeDBoardProgress.Radio, ThreeDBoardProgress.ToyCar, ThreeDBoardProgress.ToyRobot, ThreeDBoardProgress.ToyTrain, ThreeDBoardProgress.ToyPlane })
                PlayerPrefs.DeleteKey(WorkshopRunSave.Key(board));
            PlayerPrefs.Save();
        }
    }
}
