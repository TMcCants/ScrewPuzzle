using UnityEngine;

namespace ScrewPuzzle
{
    /// <summary>
    /// Describes the rules and screw layout for one level.
    /// It contains data only; it does not build objects or run gameplay.
    /// </summary>
    public sealed class LevelDefinition
    {
        public string LevelName { get; }
        public string RestoredStatusMessage { get; }
        public string RestoredResultMessage { get; }
        public int TrayCapacity { get; }
        public int MatchSize { get; }
        public ScrewDefinition[] Screws { get; }

        public LevelDefinition(
            string levelName,
            string restoredStatusMessage,
            string restoredResultMessage,
            int trayCapacity,
            int matchSize,
            ScrewDefinition[] screws)
        {
            LevelName = levelName;
            RestoredStatusMessage = restoredStatusMessage;
            RestoredResultMessage = restoredResultMessage;
            TrayCapacity = trayCapacity;
            MatchSize = matchSize;
            Screws = screws;
        }
    }

    /// <summary>
    /// Describes one screw and which other screws must be removed first.
    /// Blockers are stored as indexes into LevelDefinition.Screws.
    /// </summary>
    public sealed class ScrewDefinition
    {
        public ScrewColorId ColorId { get; }
        public Vector3 Position { get; }
        public int[] BlockerIndexes { get; }

        public ScrewDefinition(
            ScrewColorId colorId,
            Vector3 position,
            params int[] blockerIndexes)
        {
            ColorId = colorId;
            Position = position;
            BlockerIndexes = blockerIndexes ?? new int[0];
        }
    }
}
