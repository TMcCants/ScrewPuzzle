using System;
using UnityEngine;

namespace ScrewPuzzle
{
    public static class WorkshopRunSave
    {
        [Serializable]
        public sealed class Run
        {
            public int version = 1;
            public string signature;
            public int[] actions;
        }
        public static string Key(string board) => "ScrewPuzzle.Workshop3D.Run.v1." + board;
        public static bool HasRun(string board)
        {
            var run = Read(board);
            return run != null && run.actions != null && run.actions.Length > 0;
        }
        public static Run Read(string board)
        {
            if (!PlayerPrefs.HasKey(Key(board))) return null;
            try
            {
                var run = JsonUtility.FromJson<Run>(PlayerPrefs.GetString(Key(board)));
                if (run != null && run.version == 1 && !string.IsNullOrEmpty(run.signature) && run.actions != null) return run;
            }
            catch (ArgumentException) { }
            Clear(board);
            return null;
        }
        public static void Write(string board, string signature, int[] actions)
        {
            PlayerPrefs.SetString(Key(board), JsonUtility.ToJson(new Run { signature = signature, actions = actions }));
            PlayerPrefs.Save();
        }
        public static void Clear(string board) { PlayerPrefs.DeleteKey(Key(board)); PlayerPrefs.Save(); }
    }
}
