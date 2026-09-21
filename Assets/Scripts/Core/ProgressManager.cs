using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Stores the highest level unlocked on this device.
    /// V0.2 intentionally saves only this one progression value.
    /// </summary>
    public static class ProgressManager
    {
        private const string HighestUnlockedLevelKey = "ScrewPuzzle.HighestUnlockedLevel";
        private const int FirstLevelNumber = 1;
        private const int TotalLevelCount = 3;

        public static int HighestUnlockedLevel
        {
            get
            {
                return PlayerPrefs.GetInt(HighestUnlockedLevelKey, FirstLevelNumber);
            }
        }

        public static bool IsLevelUnlocked(int levelNumber)
        {
            return levelNumber <= HighestUnlockedLevel;
        }

        public static void RecordLevelCompleted(int completedLevelNumber)
        {
            int newlyUnlockedLevel = Mathf.Min(completedLevelNumber + 1, TotalLevelCount);

            if (newlyUnlockedLevel <= HighestUnlockedLevel)
            {
                return;
            }

            PlayerPrefs.SetInt(HighestUnlockedLevelKey, newlyUnlockedLevel);
            PlayerPrefs.Save();
        }
    }
}
