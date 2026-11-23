namespace TheZjazd.ExamSupervisor
{
    /// <summary>ECTS still available in one exam. Never drops below zero.</summary>
    public class EctsPool
    {
        public float Current { get; private set; }

        public EctsPool(float initial)
        {
            Current = initial < 0f ? 0f : initial;
        }

        public void Reduce(float amount)
        {
            if (amount <= 0f) return;
            Current = Current - amount < 0f ? 0f : Current - amount;
        }
    }
}
