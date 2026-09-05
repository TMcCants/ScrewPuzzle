using System.Collections.Generic;
using NUnit.Framework;

namespace ScrewPuzzle.Tests
{
    public sealed class TrayRulesTests
    {
        [Test]
        public void FindFirstMatch_ReturnsThreeIndexes_WhenThreeColorsMatch()
        {
            List<ScrewColorId> colors = new List<ScrewColorId>
            {
                ScrewColorId.Red,
                ScrewColorId.Blue,
                ScrewColorId.Red,
                ScrewColorId.Yellow,
                ScrewColorId.Red
            };

            List<int> result = TrayRules.FindFirstMatch(colors, 3);

            CollectionAssert.AreEqual(new[] { 0, 2, 4 }, result);
        }

        [Test]
        public void FindFirstMatch_ReturnsEmpty_WhenNoColorReachesMatchSize()
        {
            List<ScrewColorId> colors = new List<ScrewColorId>
            {
                ScrewColorId.Red,
                ScrewColorId.Blue,
                ScrewColorId.Yellow,
                ScrewColorId.Red,
                ScrewColorId.Blue
            };

            List<int> result = TrayRules.FindFirstMatch(colors, 3);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void FindFirstMatch_UsesConfiguredMatchSize()
        {
            List<ScrewColorId> colors = new List<ScrewColorId>
            {
                ScrewColorId.Yellow,
                ScrewColorId.Yellow
            };

            List<int> result = TrayRules.FindFirstMatch(colors, 2);

            CollectionAssert.AreEqual(new[] { 0, 1 }, result);
        }
    }
}
