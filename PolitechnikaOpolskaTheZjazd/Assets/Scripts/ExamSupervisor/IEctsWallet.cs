namespace TheZjazd.ExamSupervisor
{
    /// <summary>Integration point: the game's ECTS counter. Replace the mock with the real one.</summary>
    public interface IEctsWallet
    {
        float Total { get; }
        void AddEcts(float amount);
    }
}
