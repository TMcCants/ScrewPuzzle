using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>Device-local completion records, separate from production puzzle progress.</summary>
    public static class ThreeDBoardProgress
    {
        public const string ToyPlane = "toy-plane";
        public const string ToyTrain = "toy-train";
        public const string ToyRobot = "toy-robot";
        public const string Radio = "radio";
        public const string ToyCar = "toy-car";
        public static string Key(string board) => "ScrewPuzzle.Workshop3D.Completed.v1." + board;
        public static bool IsCompleted(string board) => PlayerPrefs.GetInt(Key(board), 0) == 1;
        public static int CompletedCount
        {
            get
            {
                int count = 0;
                foreach (string board in WorkshopPlayerProfile.Boards) if (IsCompleted(board)) count++;
                return count;
            }
        }
        public static string CollectionStatus => CompletedCount + " OF " + WorkshopPlayerProfile.Boards.Length + " COMPLETED";
        public static string Status(string board) => WorkshopRunSave.HasRun(board)
            ? (IsCompleted(board) ? "REPLAY IN PROGRESS" : "IN PROGRESS")
            : IsCompleted(board) ? "COMPLETED" : "NEW";

        public static void MarkCompleted(string board)
        {
            if (IsCompleted(board)) return;
            PlayerPrefs.SetInt(Key(board), 1);
            PlayerPrefs.Save();
        }
    }
}
