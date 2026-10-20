using System;

namespace ScrewPuzzle
{
    /// <summary>
    /// Randomized members of a solvable layout family. Permuting color labels and
    /// physical sockets preserves the template's dependency and tray relationships.
    /// Shared by the airplane and train. Keep this version deterministic: saved games store the seed.
    /// </summary>
    public sealed class PlaneScrewLayout
    {
        public const int AssemblyCount = 9;
        public readonly int Seed;
        public readonly ScrewColorId[] Colors = new ScrewColorId[27];
        public readonly int[] Sockets = new int[27];
        public readonly ScrewColorId[] Trays = new ScrewColorId[9];
        public readonly int[] Solution = { 0, 1, 2, 10, 4, 7, 3, 5, 6, 12, 13, 14, 15, 16, 9, 8, 11, 17,
            18, 19, 20, 21, 22, 23, 24, 25, 26 };
        public static readonly int[] Covers = { -1, -1, -1, -1, 0, 4, 5, 6, 7 };

        public PlaneScrewLayout(int seed)
        {
            Seed = seed;
            var random = new Random(seed);
            int[] palette = { 0, 1, 2 };
            Shuffle(palette, random);
            int[] template = { 0,1,1, 2,0,2, 2,0,2, 0,1,2, 0,1,1, 0,1,2, 0,0,0, 1,1,1, 2,2,2 };
            for (int p = 0; p < AssemblyCount; p++)
            {
                int[] sockets = { 0, 1, 2 };
                Shuffle(sockets, random);
                for (int i = 0; i < 3; i++)
                {
                    Colors[p * 3 + i] = (ScrewColorId)palette[template[p * 3 + i]];
                    Sockets[p * 3 + i] = sockets[i];
                }
                Trays[p] = (ScrewColorId)palette[p % 3];
            }
        }

        private static void Shuffle(int[] values, Random random)
        {
            for (int i = values.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int value = values[i]; values[i] = values[j]; values[j] = value;
            }
        }
    }
}
