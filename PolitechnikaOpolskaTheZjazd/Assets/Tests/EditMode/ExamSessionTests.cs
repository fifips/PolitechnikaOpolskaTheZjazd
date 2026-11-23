using System;
using NUnit.Framework;

namespace TheZjazd.ExamSupervisor.Tests
{
    public class ExamSessionTests
    {
        // Config with round numbers: pool 5, 5 cheaters => each penalty = 1.
        static ExamConfig Config() => new ExamConfig
        {
            studentCount = 16,
            cheaterCount = 5,
            startingPool = 5f,
            examDuration = 1000f,
            reactionWindow = 3f,
            gapBetweenCheaters = 1f,
            latePenaltyFraction = 0.2f,
            wrongPenaltyFraction = 0.2f,
        };

        static ExamSession NewSession(ExamConfig cfg = null, int seed = 7)
            => new ExamSession(cfg ?? Config(), new Random(seed));

        // Advance until a cheater is active (after the gap).
        static int WaitForCheater(ExamSession s)
        {
            for (int i = 0; i < 100 && s.ActiveCheaterId == null; i++) s.Tick(0.5f);
            Assert.IsNotNull(s.ActiveCheaterId, "cheater never started");
            return s.ActiveCheaterId.Value;
        }

        [Test]
        public void StartsIdleAndTickDoesNothing()
        {
            var s = NewSession();
            Assert.AreEqual(ExamState.Idle, s.State);
            s.Tick(10f);
            Assert.AreEqual(ExamState.Idle, s.State);
        }

        [Test]
        public void StartMovesToRunningWithNoActiveCheaterYet()
        {
            var s = NewSession();
            s.Start();
            Assert.AreEqual(ExamState.Running, s.State);
            Assert.IsNull(s.ActiveCheaterId);
        }

        [Test]
        public void CheaterStartsAfterGap()
        {
            var s = NewSession();
            int started = -1;
            s.CheaterStarted += id => started = id;
            s.Start();
            s.Tick(0.5f);
            Assert.IsNull(s.ActiveCheaterId);
            s.Tick(0.6f);
            Assert.IsNotNull(s.ActiveCheaterId);
            Assert.AreEqual(s.ActiveCheaterId.Value, started);
        }

        [Test]
        public void CatchingActiveCheaterCostsNothing()
        {
            var s = NewSession();
            s.Start();
            int id = WaitForCheater(s);
            int caught = -1;
            s.CheaterCaught += c => caught = c;
            s.Intervene(id);
            Assert.AreEqual(id, caught);
            Assert.IsNull(s.ActiveCheaterId);
            Assert.AreEqual(5f, s.Pool.Current, 1e-5f);
        }

        [Test]
        public void MissingCheaterReducesPoolAndClearsActive()
        {
            var s = NewSession();
            s.Start();
            int id = WaitForCheater(s);
            int missed = -1;
            s.CheaterMissed += m => missed = m;
            s.Tick(3.1f);
            Assert.AreEqual(id, missed);
            Assert.IsNull(s.ActiveCheaterId);
            Assert.AreEqual(4f, s.Pool.Current, 1e-5f);
        }

        [Test]
        public void HittingInnocentReducesPoolAndKeepsCheaterActive()
        {
            var s = NewSession();
            s.Start();
            int cheater = WaitForCheater(s);
            int innocent = (cheater + 1) % 16;
            int wrong = -1;
            s.WrongHit += w => wrong = w;
            s.Intervene(innocent);
            Assert.AreEqual(innocent, wrong);
            Assert.AreEqual(cheater, s.ActiveCheaterId);
            Assert.AreEqual(4f, s.Pool.Current, 1e-5f);
        }

        [Test]
        public void HittingWhenNobodyCheatsIsAWrongHit()
        {
            var s = NewSession();
            s.Start();
            s.Intervene(3);
            Assert.AreEqual(4f, s.Pool.Current, 1e-5f);
        }

        [Test]
        public void InterveneWithNoTargetDoesNothing()
        {
            var s = NewSession();
            s.Start();
            WaitForCheater(s);
            s.Intervene(null);
            Assert.AreEqual(5f, s.Pool.Current, 1e-5f);
        }

        [Test]
        public void InterveneIgnoredWhenNotRunning()
        {
            var s = NewSession();
            s.Intervene(1);
            Assert.AreEqual(5f, s.Pool.Current, 1e-5f);
        }

        [Test]
        public void OnlyOneCheaterAtATime()
        {
            var s = NewSession();
            int starts = 0;
            s.CheaterStarted += _ => starts++;
            s.Start();
            WaitForCheater(s);
            s.Tick(1f); // still inside reaction window, no new cheater
            Assert.AreEqual(1, starts);
        }

        [Test]
        public void FinishesWhenAllCheatersResolved()
        {
            var s = NewSession();
            ExamResult result = null;
            s.Finished += r => result = r;
            s.Start();
            for (int i = 0; i < 5; i++) s.Intervene(WaitForCheater(s));
            Assert.AreEqual(ExamState.Finished, s.State);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Caught);
            Assert.AreEqual(0, result.Missed);
            Assert.AreEqual(0, result.WrongHits);
            Assert.AreEqual(5f, result.Ects, 1e-5f);
        }

        [Test]
        public void ResultCountsMissesAndWrongHits()
        {
            var s = NewSession();
            ExamResult result = null;
            s.Finished += r => result = r;
            s.Start();
            s.Intervene(WaitForCheater(s));            // caught
            int c2 = WaitForCheater(s);
            s.Intervene((c2 + 1) % 16);                 // wrong
            s.Tick(3.1f);                               // missed c2
            for (int i = 0; i < 3; i++) s.Intervene(WaitForCheater(s));
            Assert.AreEqual(4, result.Caught);
            Assert.AreEqual(1, result.Missed);
            Assert.AreEqual(1, result.WrongHits);
            Assert.AreEqual(3f, result.Ects, 1e-5f);
        }

        [Test]
        public void PoolNeverNegativeInResult()
        {
            var cfg = Config();
            cfg.startingPool = 1f;
            cfg.latePenaltyFraction = 1f;
            cfg.wrongPenaltyFraction = 1f;
            var s = NewSession(cfg);
            s.Start();
            int cheater = WaitForCheater(s);
            int innocent = (cheater + 1) % 16;
            s.Intervene(innocent);
            s.Intervene(innocent);
            Assert.AreEqual(0f, s.Pool.Current, 1e-5f);
        }

        [Test]
        public void TimeoutFinishesAndPenalizesActiveCheaterOnly()
        {
            var cfg = Config();
            cfg.examDuration = 5f;
            cfg.reactionWindow = 100f;
            var s = NewSession(cfg);
            ExamResult result = null;
            s.Finished += r => result = r;
            s.Start();
            WaitForCheater(s);
            s.Tick(10f);
            Assert.AreEqual(ExamState.Finished, s.State);
            Assert.AreEqual(1, result.Missed);
            Assert.AreEqual(4f, result.Ects, 1e-5f);
        }

        [Test]
        public void TimeoutWithoutActiveCheaterCostsNothing()
        {
            var cfg = Config();
            cfg.examDuration = 0.5f;
            var s = NewSession(cfg);
            ExamResult result = null;
            s.Finished += r => result = r;
            s.Start();
            s.Tick(1f);
            Assert.AreEqual(ExamState.Finished, s.State);
            Assert.AreEqual(0, result.Missed);
            Assert.AreEqual(5f, result.Ects, 1e-5f);
        }

        [Test]
        public void StartTwiceWithoutResetThrows()
        {
            var s = NewSession();
            s.Start();
            Assert.Throws<InvalidOperationException>(() => s.Start());
        }

        [Test]
        public void TickAndInterveneAfterFinishAreIgnored()
        {
            var s = NewSession();
            int finishes = 0;
            s.Finished += _ => finishes++;
            s.Start();
            for (int i = 0; i < 5; i++) s.Intervene(WaitForCheater(s));
            s.Tick(100f);
            s.Intervene(1);
            Assert.AreEqual(1, finishes);
            Assert.AreEqual(5f, s.Pool.Current, 1e-5f);
        }
    }
}
