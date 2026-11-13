using UnityEngine;

namespace TheZjazd.ExamSupervisor
{
    /// <summary>Geometry of the ruler swing: an arc centred on the facing direction.</summary>
    public static class SwingArc
    {
        public const float ArcDegrees = 120f;

        /// <summary>Angle in degrees of a direction (right = 0, up = 90, left = 180, down = -90).</summary>
        public static float FacingAngle(Vector2 facing) => Mathf.Atan2(facing.y, facing.x) * Mathf.Rad2Deg;

        /// <summary>Ruler angle at progress t (0 = start of the swing, 1 = end), sweeping counter-clockwise.</summary>
        public static float Angle(Vector2 facing, float t)
        {
            t = Mathf.Clamp01(t);
            return FacingAngle(facing) + Mathf.Lerp(-ArcDegrees / 2f, ArcDegrees / 2f, t);
        }
    }
}
