using System.Collections.Generic;
using NUnit.Framework;

namespace ScrewPuzzle.Tests
{
    public sealed class PlaneLayoutTests
    {
        private sealed class State
        {
            public int mask, next = 2;
            public int[] colors, counts = new int[2];
            public State Copy() => new State { mask = mask, next = next, colors = (int[])colors.Clone(), counts = (int[])counts.Clone() };
        }
        private static bool Take(PlaneScrewLayout layout, State state, int i)
        {
            int cover = PlaneScrewLayout.Covers[i / 3];
            if ((state.mask & (1 << i)) != 0 || (cover >= 0 && ((state.mask >> (cover * 3)) & 7) != 7)) return false;
            int tray = System.Array.FindIndex(state.colors, c => c == (int)layout.Colors[i]);
            if (tray < 0) return false;
            state.mask |= 1 << i;
            if (++state.counts[tray] == 3)
            {
                state.counts[tray] = 0;
                state.colors[tray] = state.next < layout.Trays.Length ? (int)layout.Trays[state.next++] : -1;
            }
            return true;
        }
        private static State Start(PlaneScrewLayout layout) => new State { colors = new[] { (int)layout.Trays[0], (int)layout.Trays[1] } };

        [Test] public void ThousandSeeds_AreDeterministicVariedAndSolvableWithTwoTrays()
        {
            var variants = new HashSet<string>();
            for (int seed = 1; seed <= 1000; seed++)
            {
                var layout = new PlaneScrewLayout(seed);
                var again = new PlaneScrewLayout(seed);
                CollectionAssert.AreEqual(layout.Colors, again.Colors);
                CollectionAssert.AreEqual(layout.Sockets, again.Sockets);
                variants.Add(string.Join(",", layout.Colors) + "/" + string.Join(",", layout.Sockets));
                var state = Start(layout);
                foreach (int i in layout.Solution) Assert.That(Take(layout, state, i), Is.True, "Seed " + seed + " screw " + i);
                Assert.That(state.mask, Is.EqualTo((1 << 27) - 1));
                CollectionAssert.AreEqual(new[] { 0, 0 }, state.counts);
                for (int p = 0; p < 9; p++) CollectionAssert.AreEquivalent(new[] { 0, 1, 2 }, new[] { layout.Sockets[p * 3], layout.Sockets[p * 3 + 1], layout.Sockets[p * 3 + 2] });
            }
            Assert.That(variants.Count, Is.GreaterThan(900));
        }

        [Test] public void Template_HasNoReachableDeadEnds_WithOnlyTwoTrays()
        {
            // Socket permutations and color renaming are graph isomorphisms, so one
            // exhaustive template audit covers every generated member of this family.
            var layout = new PlaneScrewLayout(1);
            var queue = new Queue<State>(); queue.Enqueue(Start(layout));
            var visited = new HashSet<string>();
            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                string key = state.mask + ":" + state.next + ":" + string.Join(",", state.colors) + ":" + string.Join(",", state.counts);
                if (!visited.Add(key) || state.mask == (1 << 27) - 1) continue;
                int moves = 0;
                for (int i = 0; i < 27; i++)
                {
                    var next = state.Copy();
                    if (Take(layout, next, i)) { queue.Enqueue(next); moves++; }
                }
                Assert.That(moves, Is.GreaterThan(0), "Dead end: " + key);
            }
            Assert.That(visited.Count, Is.GreaterThan(1000));
        }
    }
}
