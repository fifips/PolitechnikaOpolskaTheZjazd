using NUnit.Framework;
using UnityEngine;

namespace TheZjazd.ExamSupervisor.Tests
{
    public class SwingArcTests
    {
        [TestCase(1f, 0f, 0f)]
        [TestCase(0f, 1f, 90f)]
        [TestCase(-1f, 0f, 180f)]
        [TestCase(0f, -1f, -90f)]
        public void FacingAngle_MatchesCardinalDirections(float x, float y, float expected)
        {
            Assert.AreEqual(expected, SwingArc.FacingAngle(new Vector2(x, y)), 0.001f);
        }

        [Test]
        public void Angle_StartsAndEndsHalfAnArcAroundFacing()
        {
            var facing = Vector2.up; // 90 degrees
            Assert.AreEqual(90f - SwingArc.ArcDegrees / 2f, SwingArc.Angle(facing, 0f), 0.001f);
            Assert.AreEqual(90f + SwingArc.ArcDegrees / 2f, SwingArc.Angle(facing, 1f), 0.001f);
        }

        [Test]
        public void Angle_MiddleOfSwingPointsAtFacing()
        {
            Assert.AreEqual(-90f, SwingArc.Angle(Vector2.down, 0.5f), 0.001f);
        }

        [Test]
        public void Angle_ClampsProgressOutsideZeroToOne()
        {
            Assert.AreEqual(SwingArc.Angle(Vector2.right, 0f), SwingArc.Angle(Vector2.right, -5f), 0.001f);
            Assert.AreEqual(SwingArc.Angle(Vector2.right, 1f), SwingArc.Angle(Vector2.right, 5f), 0.001f);
        }
    }
}
