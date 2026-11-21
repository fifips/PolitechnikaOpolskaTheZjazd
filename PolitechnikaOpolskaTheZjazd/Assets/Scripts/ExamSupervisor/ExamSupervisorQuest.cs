using System;
using UnityEngine;

namespace TheZjazd.ExamSupervisor
{
    /// <summary>
    /// Quest entry point. Integration: call <see cref="StartExam"/>; listen to <see cref="Finished"/>.
    /// ECTS is paid out once, at the end, through <see cref="IEctsWallet"/>.
    /// </summary>
    public class ExamSupervisorQuest : MonoBehaviour
    {
        [SerializeField] ExamConfig config = new ExamConfig();
        [SerializeField] ClassroomLayout classroom;
        [SerializeField] ExamUI ui;
        [SerializeField] PlayerControls player;
        [SerializeField] bool startOnPlay = true;
        [SerializeField] RulerSwing ruler;
        [SerializeField] MockEctsWallet walletBehaviour;
        [SerializeField] Transform classroomEntry;
        [SerializeField] Transform classroomExit;
        [SerializeField] float interventionRange = 1.2f;
        // 0.7 = the student must be within about 45 degrees of where the player is facing.
        [SerializeField] float minFacingDot = 0.7f;

        ExamSession _session;
        bool _showingResult;

        public event Action<ExamResult> Finished;
        public bool IsRunning => _session != null && _session.State == ExamState.Running;
        IEctsWallet Wallet => walletBehaviour;

        void OnEnable() { if (player != null) player.Interacted += OnPlayerInteracted; }
        void OnDisable() { if (player != null) player.Interacted -= OnPlayerInteracted; }

        void Start()
        {
            if (startOnPlay) StartExam();
        }

        // E pressed by the player: hits a student during the exam, closes the summary afterwards.
        void OnPlayerInteracted()
        {
            if (_showingResult) CloseResult();
            // The swing has a cooldown: pressing E during it neither swings nor hits anyone.
            else if (IsRunning && (ruler == null || ruler.TrySwing(player.Facing))) Intervene();
        }

        public void StartExam()
        {
            if (IsRunning) return;
            classroom.Build(config.studentCount);
            _session = new ExamSession(config, new System.Random());
            _session.CheaterStarted += id => classroom.Get(id)?.SetCheating(true);
            _session.CheaterCaught += id => { var s = classroom.Get(id); s?.SetCheating(false); s?.PlayHit(); };
            _session.CheaterMissed += id => classroom.Get(id)?.SetCheating(false);
            _session.WrongHit += id => classroom.Get(id)?.PlayHit();
            _session.Finished += OnFinished;

            player.transform.position = classroomEntry.position;
            _session.Start();
        }

        void Update()
        {
            if (_showingResult) return;

            if (_session == null) { RefreshHud(); return; }

            if (IsRunning) _session.Tick(Time.deltaTime);
            RefreshHud();
        }

        // Hits the closest student the player is actually facing.
        // Measured from the player's feet collider (the sprite centre is a whole body above the floor),
        // and a student only counts if he is within ~45 degrees of the facing direction.
        void Intervene()
        {
            var feet = player.GetComponent<Collider2D>();
            Vector2 origin = feet != null ? (Vector2)feet.bounds.center : (Vector2)player.transform.position;
            Student best = null;
            float bestDist = float.MaxValue;
            foreach (var col in Physics2D.OverlapCircleAll(origin, interventionRange))
            {
                var s = col.GetComponent<Student>();
                if (s == null) continue;

                Vector2 toStudent = col.ClosestPoint(origin) - origin;
                float d = toStudent.magnitude;
                if (d > 0.001f && Vector2.Dot(toStudent / d, player.Facing) < minFacingDot) continue;

                if (d < bestDist) { best = s; bestDist = d; }
            }
            _session.Intervene(best != null ? best.Id : (int?)null);
        }

        void OnFinished(ExamResult r)
        {
            Wallet.AddEcts(r.Ects);
            player.InputEnabled = false;
            _showingResult = true;
            ui.ShowResult($"Koniec egzaminu\nZdobyte ECTS: {r.Ects:0.0}\nPrzyłapani: {r.Caught}  Spóźnienia: {r.Missed}  Pomyłki: {r.WrongHits}\n\n[E] zamknij");
            Finished?.Invoke(r);
        }

        void CloseResult()
        {
            _showingResult = false;
            ui.HideResult();
            classroom.Clear();
            player.transform.position = classroomExit.position;
            player.InputEnabled = true;
            _session = null;
        }

        void RefreshHud()
        {
            string text = $"ECTS: {Wallet.Total:0.0}";
            if (_session != null && _session.State == ExamState.Running)
                text += $"\nCzas: {_session.TimeRemaining:0}s   Pula: {_session.Pool.Current:0.0}";
            ui.SetHud(text);
        }
    }
}
