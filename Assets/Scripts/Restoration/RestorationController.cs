using System;
using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Defines the two restoration actions every puzzle object must support.
    /// Each object can provide its own visual implementation.
    /// </summary>
    public abstract class RestorationController : MonoBehaviour
    {
        public abstract void HandleScrewRemoved(Screw removedScrew);
        public abstract void PlayFinalRestoration(Action onFinished);
    }
}
