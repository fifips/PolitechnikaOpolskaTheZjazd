using System;
using System.Collections.Generic;

namespace TheZjazd.ExamSupervisor
{
    public static class CheaterPicker
    {
        /// <summary>Picks unique student ids in random order (partial Fisher-Yates).</summary>
        public static List<int> Pick(int studentCount, int cheaterCount, Random rng)
        {
            if (studentCount < 0) throw new ArgumentOutOfRangeException(nameof(studentCount));
            int count = Math.Max(0, Math.Min(cheaterCount, studentCount));

            var ids = new int[studentCount];
            for (int i = 0; i < studentCount; i++) ids[i] = i;

            var result = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                int j = rng.Next(i, studentCount);
                (ids[i], ids[j]) = (ids[j], ids[i]);
                result.Add(ids[i]);
            }
            return result;
        }
    }
}
