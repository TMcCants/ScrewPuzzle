using UnityEngine;

namespace ScrewPuzzle
{
    // Relabel both screws and trays together: the original solution remains valid.
    // Keep these mappings stable because unfinished games persist the variant.
    public static class WorkshopReplayLayout
    {
        private static readonly int[,] Colors = { {0,1,2}, {0,2,1}, {1,0,2}, {1,2,0}, {2,0,1}, {2,1,0} };
        public static ScrewColorId Map(ScrewColorId color, int variant) => (ScrewColorId)Colors[Mathf.Clamp(variant,0,5),(int)color];
        public static int Select(string board)
        {
            var run=WorkshopRunSave.Read(board);
            if(run!=null) return Mathf.Clamp(run.layoutSeed,0,5);
            if(!ThreeDBoardProgress.IsCompleted(board)) return 0;
            string key="ScrewPuzzle.ReplayVariant.v1."+board;
            int previous=PlayerPrefs.GetInt(key,0);
            int next=(previous+Random.Range(1,6))%6;
            PlayerPrefs.SetInt(key,next);
            PlayerPrefs.Save();
            return next;
        }
    }
}
