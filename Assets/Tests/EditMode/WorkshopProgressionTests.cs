using NUnit.Framework;
using UnityEngine;

namespace ScrewPuzzle.Tests
{
    public sealed class WorkshopProgressionTests
    {
        [Test]
        public void StatusAndCount_PreserveCompletionDuringReplay()
        {
            string board=ThreeDBoardProgress.Radio;
            int old=PlayerPrefs.GetInt(ThreeDBoardProgress.Key(board),0);
            string run=PlayerPrefs.GetString(WorkshopRunSave.Key(board),"");
            string last=PlayerPrefs.GetString(WorkshopPlayerProfile.LastBoardKey,"");
            try
            {
                PlayerPrefs.DeleteKey(ThreeDBoardProgress.Key(board));
                WorkshopRunSave.Clear(board);
                int baseline=ThreeDBoardProgress.CompletedCount;
                Assert.That(ThreeDBoardProgress.Status(board),Is.EqualTo("NEW"));
                WorkshopRunSave.Write(board,"test",new[]{0});
                Assert.That(ThreeDBoardProgress.Status(board),Is.EqualTo("IN PROGRESS"));
                WorkshopRunSave.Clear(board);
                ThreeDBoardProgress.MarkCompleted(board);
                ThreeDBoardProgress.MarkCompleted(board);
                Assert.That(ThreeDBoardProgress.CompletedCount,Is.EqualTo(baseline+1));
                Assert.That(ThreeDBoardProgress.Status(board),Is.EqualTo("COMPLETED"));
                WorkshopRunSave.Write(board,"test",new[]{0});
                Assert.That(ThreeDBoardProgress.Status(board),Is.EqualTo("REPLAY IN PROGRESS"));
                Assert.That(ThreeDBoardProgress.CompletedCount,Is.EqualTo(baseline+1));
            }
            finally
            {
                PlayerPrefs.SetInt(ThreeDBoardProgress.Key(board),old);
                if(run=="") PlayerPrefs.DeleteKey(WorkshopRunSave.Key(board)); else PlayerPrefs.SetString(WorkshopRunSave.Key(board),run);
                PlayerPrefs.SetString(WorkshopPlayerProfile.LastBoardKey,last);
                PlayerPrefs.Save();
            }
        }

        [Test]
        public void EveryReplayPermutation_PreservesAllColorMatchesAndMismatches()
        {
            for(int variant=0;variant<6;variant++)
            {
                var colors=new System.Collections.Generic.HashSet<ScrewColorId>();
                for(int screw=0;screw<3;screw++)
                {
                    colors.Add(WorkshopReplayLayout.Map((ScrewColorId)screw,variant));
                    for(int tray=0;tray<3;tray++)
                        Assert.That(WorkshopReplayLayout.Map((ScrewColorId)screw,variant)==WorkshopReplayLayout.Map((ScrewColorId)tray,variant),Is.EqualTo(screw==tray));
                }
                Assert.That(colors.Count,Is.EqualTo(3));
            }
        }
    }
}
