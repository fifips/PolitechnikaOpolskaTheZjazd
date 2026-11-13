using System;
using System.Linq;
using NUnit.Framework;

namespace TheZjazd.ExamSupervisor.Tests
{
    public class CheaterPickerTests
    {
        [Test]
        public void ReturnsRequestedCountOfUniqueIdsInRange()
        {
            var ids = CheaterPicker.Pick(16, 5, new Random(1));
            Assert.AreEqual(5, ids.Count);
            Assert.AreEqual(5, ids.Distinct().Count());
            Assert.IsTrue(ids.All(i => i >= 0 && i < 16));
        }

        [Test]
        public void SameSeedGivesSameResult()
        {
            var a = CheaterPicker.Pick(16, 5, new Random(42));
            var b = CheaterPicker.Pick(16, 5, new Random(42));
            CollectionAssert.AreEqual(a, b);
        }

        [Test]
        public void CountIsClampedToStudentCount()
        {
            var ids = CheaterPicker.Pick(3, 10, new Random(1));
            Assert.AreEqual(3, ids.Count);
            Assert.AreEqual(3, ids.Distinct().Count());
        }

        [Test]
        public void ZeroCheatersGivesEmptyList()
        {
            Assert.AreEqual(0, CheaterPicker.Pick(16, 0, new Random(1)).Count);
        }

        [Test]
        public void DifferentSeedsEventuallyDiffer()
        {
            var first = CheaterPicker.Pick(16, 5, new Random(1));
            bool differs = Enumerable.Range(2, 20)
                .Any(s => !CheaterPicker.Pick(16, 5, new Random(s)).SequenceEqual(first));
            Assert.IsTrue(differs);
        }
    }
}
