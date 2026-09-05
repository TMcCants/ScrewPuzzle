using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Owns tray capacity, slot placement, match detection, and match clearing.
    /// </summary>
    public sealed class TrayManager : MonoBehaviour
    {
        [SerializeField] private Transform[] traySlots;
        [SerializeField, Min(2)] private int matchSize = 3;
        [SerializeField, Min(0f)] private float matchPause = 0.2f;
        [SerializeField] private GameManager gameManager;

        private readonly List<Screw> heldScrews = new List<Screw>();

        public int HeldCount { get { return heldScrews.Count; } }
        public int Capacity { get { return traySlots == null ? 0 : traySlots.Length; } }
        public bool IsBusy { get; private set; }

        public bool TryAddScrew(Screw screw)
        {
            if (screw == null || IsBusy || HeldCount >= Capacity)
            {
                return false;
            }

            heldScrews.Add(screw);
            screw.MoveToTray(traySlots[heldScrews.Count - 1].position);
            StartCoroutine(EvaluateAfterMovement());
            return true;
        }

        public void Configure(Transform[] newTraySlots, int newMatchSize, GameManager newGameManager)
        {
            traySlots = newTraySlots;
            matchSize = newMatchSize;
            gameManager = newGameManager;
        }

        private IEnumerator EvaluateAfterMovement()
        {
            IsBusy = true;
            yield return new WaitForSeconds(0.28f);

            List<int> matchIndexes = FindMatchIndexes();

            if (matchIndexes.Count == matchSize)
            {
                yield return ClearMatch(matchIndexes);
            }
            else
            {
                IsBusy = false;

                if (HeldCount >= Capacity)
                {
                    gameManager.NotifyTrayBecameUnusable();
                }
            }
        }

        private List<int> FindMatchIndexes()
        {
            List<ScrewColorId> colors = new List<ScrewColorId>();

            foreach (Screw screw in heldScrews)
            {
                colors.Add(screw.ColorId);
            }

            return TrayRules.FindFirstMatch(colors, matchSize);
        }

        private IEnumerator ClearMatch(List<int> matchIndexes)
        {
            yield return new WaitForSeconds(matchPause);

            List<Screw> matchedScrews = new List<Screw>();

            foreach (int index in matchIndexes)
            {
                matchedScrews.Add(heldScrews[index]);
            }

            foreach (Screw matchedScrew in matchedScrews)
            {
                heldScrews.Remove(matchedScrew);
                matchedScrew.ClearFromTray();
            }

            MoveRemainingScrewsIntoOpenSlots();
            gameManager.NotifyScrewsCleared(matchedScrews.Count);

            yield return new WaitForSeconds(0.25f);
            IsBusy = false;
            gameManager.CheckForWin();
        }

        private void MoveRemainingScrewsIntoOpenSlots()
        {
            for (int index = 0; index < heldScrews.Count; index++)
            {
                heldScrews[index].MoveToAnotherTraySlot(traySlots[index].position);
            }
        }
    }
}
