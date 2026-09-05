using System.Collections.Generic;

namespace ScrewPuzzle
{
    /// <summary>
    /// Pure tray rules. Keeping this separate makes the match logic easy to test.
    /// </summary>
    public static class TrayRules
    {
        public static List<int> FindFirstMatch(
            IReadOnlyList<ScrewColorId> colors,
            int matchSize)
        {
            List<int> matchIndexes = new List<int>();

            if (colors == null || matchSize <= 0)
            {
                return matchIndexes;
            }

            for (int candidateIndex = 0; candidateIndex < colors.Count; candidateIndex++)
            {
                matchIndexes.Clear();
                ScrewColorId candidateColor = colors[candidateIndex];

                for (int trayIndex = 0; trayIndex < colors.Count; trayIndex++)
                {
                    if (colors[trayIndex] == candidateColor)
                    {
                        matchIndexes.Add(trayIndex);

                        if (matchIndexes.Count == matchSize)
                        {
                            return new List<int>(matchIndexes);
                        }
                    }
                }
            }

            matchIndexes.Clear();
            return matchIndexes;
        }
    }
}
