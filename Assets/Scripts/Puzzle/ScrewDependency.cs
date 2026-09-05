using System.Collections.Generic;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Owns one screw's blocking rules.
    /// A screw is legal when every screw in blockers has left the radio.
    /// An empty blockers list means the screw is immediately selectable.
    /// </summary>
    public sealed class ScrewDependency : MonoBehaviour
    {
        [SerializeField] private List<Screw> blockers = new List<Screw>();

        public bool AreAllBlockersRemoved()
        {
            foreach (Screw blocker in blockers)
            {
                if (blocker != null && !blocker.IsRemovedFromObject)
                {
                    return false;
                }
            }

            return true;
        }

        public void Configure(IEnumerable<Screw> screwsThatBlockThisOne)
        {
            blockers.Clear();

            if (screwsThatBlockThisOne != null)
            {
                blockers.AddRange(screwsThatBlockThisOne);
            }
        }
    }
}
