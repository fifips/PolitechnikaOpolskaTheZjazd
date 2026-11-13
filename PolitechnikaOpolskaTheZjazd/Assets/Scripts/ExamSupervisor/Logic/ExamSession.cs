using System;
using System.Collections.Generic;

namespace TheZjazd.ExamSupervisor
{
    public enum ExamState { Idle, Running, Finished }

    /// <summary>
    /// Exam rules without any Unity dependency: one cheater at a time, a reaction
    /// window, penalties taken from a single ECTS pool.
    /// </summary>
    public class ExamSession
    {
        readonly ExamConfig _config;
        readonly Random _rng;
        readonly Queue<int> _cheaters = new Queue<int>();
        readonly ExamResult _result = new ExamResult();

        float _timeLeft;
        float _reactionLeft;
        float _gapLeft;

        public ExamState State { get; private set; } = ExamState.Idle;
        public EctsPool Pool { get; }
        public int? ActiveCheaterId { get; private set; }
        public float TimeRemaining => _timeLeft;
        public float ReactionRemaining => _reactionLeft;

        public event Action<int> CheaterStarted;
        public event Action<int> CheaterCaught;
        public event Action<int> CheaterMissed;
        public event Action<int> WrongHit;
        public event Action<ExamResult> Finished;

        public ExamSession(ExamConfig config, Random rng)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
            Pool = new EctsPool(config.startingPool);
        }

        public void Start()
        {
            if (State != ExamState.Idle)
                throw new InvalidOperationException("Exam already started; create a new session.");

            foreach (int id in CheaterPicker.Pick(_config.studentCount, _config.cheaterCount, _rng))
                _cheaters.Enqueue(id);

            _timeLeft = _config.examDuration;
            _gapLeft = _config.gapBetweenCheaters;
            State = ExamState.Running;

            if (_cheaters.Count == 0) Finish();
        }

        public void Tick(float dt)
        {
            if (State != ExamState.Running) return;

            _timeLeft -= dt;
            if (_timeLeft <= 0f)
            {
                _timeLeft = 0f;
                if (ActiveCheaterId.HasValue) MissActive();
                Finish();
                return;
            }

            if (ActiveCheaterId.HasValue)
            {
                _reactionLeft -= dt;
                if (_reactionLeft <= 0f) MissActive();
            }
            else if (_cheaters.Count > 0)
            {
                _gapLeft -= dt;
                if (_gapLeft <= 0f) StartNextCheater();
            }
        }

        /// <param name="studentId">Student in front of the player, or null if nobody.</param>
        public void Intervene(int? studentId)
        {
            if (State != ExamState.Running || !studentId.HasValue) return;

            int id = studentId.Value;
            if (ActiveCheaterId == id)
            {
                ActiveCheaterId = null;
                _result.Caught++;
                CheaterCaught?.Invoke(id);
                AfterResolved();
            }
            else
            {
                Pool.Reduce(_config.startingPool * _config.wrongPenaltyFraction);
                _result.WrongHits++;
                WrongHit?.Invoke(id);
            }
        }

        void StartNextCheater()
        {
            int id = _cheaters.Dequeue();
            ActiveCheaterId = id;
            _reactionLeft = _config.reactionWindow;
            CheaterStarted?.Invoke(id);
        }

        void MissActive()
        {
            int id = ActiveCheaterId.Value;
            ActiveCheaterId = null;
            Pool.Reduce(_config.startingPool * _config.latePenaltyFraction);
            _result.Missed++;
            CheaterMissed?.Invoke(id);
            AfterResolved();
        }

        void AfterResolved()
        {
            if (_cheaters.Count == 0) Finish();
            else _gapLeft = _config.gapBetweenCheaters;
        }

        void Finish()
        {
            if (State == ExamState.Finished) return;
            State = ExamState.Finished;
            _result.Ects = Pool.Current;
            Finished?.Invoke(_result);
        }
    }
}
