using NUnit.Framework;

namespace TheZjazd.ExamSupervisor.Tests
{
    public class EctsPoolTests
    {
        [Test]
        public void StartsAtInitialValue()
        {
            var pool = new EctsPool(5f);
            Assert.AreEqual(5f, pool.Current, 1e-5f);
        }

        [Test]
        public void ApplyPenaltyReducesPool()
        {
            var pool = new EctsPool(5f);
            pool.Reduce(1f);
            Assert.AreEqual(4f, pool.Current, 1e-5f);
        }

        [Test]
        public void PoolNeverGoesBelowZero()
        {
            var pool = new EctsPool(1f);
            pool.Reduce(3f);
            Assert.AreEqual(0f, pool.Current, 1e-5f);
        }

        [Test]
        public void NegativePenaltyIsIgnored()
        {
            var pool = new EctsPool(2f);
            pool.Reduce(-1f);
            Assert.AreEqual(2f, pool.Current, 1e-5f);
        }
    }
}
