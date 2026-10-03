using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>Device-local completion records, separate from production puzzle progress.</summary>
    public static class ThreeDBoardProgress
    {
        public const string ToyRobot = "toy-robot";
        public const string Radio = "radio";
        public const string ToyCar = "toy-car";
        public static string Key(string board) => "ScrewPuzzle.Workshop3D.Completed.v1." + board;
        public static bool IsCompleted(string board) => PlayerPrefs.GetInt(Key(board), 0) == 1;

        public static void MarkCompleted(string board)
        {
            if (IsCompleted(board)) return;
            PlayerPrefs.SetInt(Key(board), 1);
            PlayerPrefs.Save();
        }
    }
}
