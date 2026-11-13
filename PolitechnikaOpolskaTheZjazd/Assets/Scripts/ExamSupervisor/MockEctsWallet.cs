using UnityEngine;

namespace TheZjazd.ExamSupervisor
{
    /// <summary>Placeholder ECTS counter until the real one exists.</summary>
    public class MockEctsWallet : MonoBehaviour, IEctsWallet
    {
        public float Total { get; private set; }
        public void AddEcts(float amount) => Total += amount;
    }
}
