using System;
using UnityEngine;

namespace TheZjazd.ExamSupervisor
{
    /// <summary>Tunable exam settings (editable in the Inspector).</summary>
    [Serializable]
    public class ExamConfig
    {
        [Min(1)] public int studentCount = 16;
        [Min(0)] public int cheaterCount = 5;

        [Tooltip("ECTS available for the whole exam.")]
        [Min(0f)] public float startingPool = 5f;

        [Tooltip("Total exam time in seconds.")]
        [Min(1f)] public float examDuration = 60f;

        [Tooltip("Seconds the player has to catch the active cheater.")]
        [Min(0.1f)] public float reactionWindow = 4f;

        [Tooltip("Pause between one cheater being resolved and the next starting.")]
        [Min(0f)] public float gapBetweenCheaters = 1f;

        [Tooltip("Share of the starting pool lost when a cheater is missed.")]
        [Range(0f, 1f)] public float latePenaltyFraction = 0.2f;

        [Tooltip("Share of the starting pool lost when an innocent student is hit.")]
        [Range(0f, 1f)] public float wrongPenaltyFraction = 0.2f;
    }
}
