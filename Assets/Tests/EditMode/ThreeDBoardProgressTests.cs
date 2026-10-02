using NUnit.Framework;
using UnityEngine;

namespace ScrewPuzzle.Tests
{
    // Preserve the developer's existing completion records while gameplay tests win boards.
    [SetUpFixture]
    public sealed class WorkshopProgressTestPreferences
    {
        private readonly string[] boards = { ThreeDBoardProgress.Radio, ThreeDBoardProgress.ToyCar };
        private readonly int[] values = new int[2];
        [OneTimeSetUp]
        public void Save()
        {
            for (int i = 0; i < boards.Length; i++)
                values[i] = PlayerPrefs.GetInt(ThreeDBoardProgress.Key(boards[i]), -1);
        }
        [OneTimeTearDown]
        public void Restore()
        {
            for (int i = 0; i < boards.Length; i++)
            {
                string key = ThreeDBoardProgress.Key(boards[i]);
                if (values[i] == -1) PlayerPrefs.DeleteKey(key);
                else PlayerPrefs.SetInt(key, values[i]);
            }
            PlayerPrefs.Save();
        }
    }

    public sealed class ThreeDBoardProgressTests
    {
        [Test]
        public void Completion_IsIndependent_Idempotent_AndStoredInPreferences()
        {
            PlayerPrefs.DeleteKey(ThreeDBoardProgress.Key(ThreeDBoardProgress.Radio));
            PlayerPrefs.DeleteKey(ThreeDBoardProgress.Key(ThreeDBoardProgress.ToyCar));
            Assert.That(ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.Radio), Is.False);
            ThreeDBoardProgress.MarkCompleted(ThreeDBoardProgress.Radio);
            ThreeDBoardProgress.MarkCompleted(ThreeDBoardProgress.Radio);
            Assert.That(PlayerPrefs.GetInt(ThreeDBoardProgress.Key(ThreeDBoardProgress.Radio)), Is.EqualTo(1));
            Assert.That(ThreeDBoardProgress.IsCompleted(ThreeDBoardProgress.ToyCar), Is.False);
        }
    }
}
