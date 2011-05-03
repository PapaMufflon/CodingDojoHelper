using System;
using System.Linq;
using CodingDojoHelper;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;

namespace CodingDojoHelperTests
{
    [TestFixture]
    class CodingDojoTests
    {
        private CodingDojo _target;
        private IStopwatch _stopwatch;
        private IKombatSoundPlayer _soundPlayer;
        private ISession _session;

        [SetUp]
        public void Setup()
        {
            _stopwatch = MockRepository.GenerateStub<IStopwatch>();
            _soundPlayer = MockRepository.GenerateStub<IKombatSoundPlayer>();
            _session = MockRepository.GenerateStub<ISession>();
            _target = new CodingDojo(_stopwatch, _soundPlayer, _session);
        }

        [Test]
        public void ChangeDeveloper_NotStarted_ThrowException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _target.ChangeDeveloper());
        }

        [Test]
        public void ChangeDeveloper_Started_NoException()
        {
            _target.Start();
            _target.ChangeDeveloper();

            _stopwatch.AssertWasCalled(x => x.Start());
        }

        [Test]
        public void ChangeDeveloper_StartStop_ThrowException()
        {
            _target.Start();
            _target.Stop();

            Assert.Throws<InvalidOperationException>(() =>
                _target.ChangeDeveloper());
            _stopwatch.AssertWasCalled(x => x.Start());
        }

        [Test]
        public void ChangeDeveloper_Once_CorrectCycleTime()
        {
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(1));

            _target.Start();
            _target.ChangeDeveloper();

            Assert.That(_target.AverageCycleTime, Is.EqualTo(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void ChangeDeveloper_Twice_CorrectCycleTime()
        {
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(2)).Repeat.Once();
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(6)).Repeat.Once();

            _target.Start();
            _target.ChangeDeveloper();
            _target.ChangeDeveloper();

            Assert.That(_target.AverageCycleTime, Is.EqualTo(TimeSpan.FromSeconds(3)));
        }

        [Test]
        public void ChangeDeveloper_ThreeTimes_CorrectCycleTimes()
        {
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(2)).Repeat.Once();
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(6)).Repeat.Once();
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(12)).Repeat.Once();

            _target.Start();
            _target.ChangeDeveloper();
            _target.ChangeDeveloper();
            _target.ChangeDeveloper();

            Assert.That(_target.AverageCycleTime, Is.EqualTo(TimeSpan.FromSeconds(4)));
            Assert.That(_target.CycleTimes.SequenceEqual(new List<TimeSpan> { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(6) }), Is.True);
        }

        [Test]
        public void Start_PlayStartingSound()
        {
            _target.Start();

            _soundPlayer.AssertWasCalled(x => x.BeginPlayStartSound());
        }

        [Test]
        public void ChangeDeveloper_PlayCycleSound()
        {
            _target.Start();
            _target.ChangeDeveloper();

            _soundPlayer.AssertWasCalled(x => x.BeginPlayCycleSound(TimeSpan.Zero), c => c.IgnoreArguments());
        }

        [Test]
        public void Stop_PlayStopSound()
        {
            _target.Start();
            _target.Stop();

            _soundPlayer.AssertWasCalled(x => x.BeginPlayStopSound());
        }

        [Test]
        public void ChangeDeveloper_CycleTimeNotReached_PlayFinishHimSound()
        {
            _target.Start();

            GetAlarm(Session.CycleTime).Callback.Invoke();

            _soundPlayer.AssertWasCalled(x => x.BeginPlayFinishHimSound());
        }

        [Test]
        public void ChangeDeveloper_CycleTimeNotReachedTwice_PlayFinishHimSoundTwice()
        {
            _target.Start();

            GetAlarm(Session.CycleTime).Callback.Invoke();
            _target.ChangeDeveloper();
            GetAlarm(Session.CycleTime).Callback.Invoke();

            _soundPlayer.AssertWasCalled(x => x.BeginPlayFinishHimSound(), c => c.Repeat.Twice());
        }

        [Test]
        public void Stop_TakeLastCycleTimeAlso()
        {
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(2)).Repeat.Once();
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(6)).Repeat.Once();
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(12)).Repeat.Once();

            _target.Start();
            _target.ChangeDeveloper();
            _target.ChangeDeveloper();
            _target.Stop();

            Assert.That(_target.AverageCycleTime, Is.EqualTo(TimeSpan.FromSeconds(4)));
            Assert.That(_target.CycleTimes.SequenceEqual(new List<TimeSpan> { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(6) }), Is.True);
        }

        [Test]
        public void Start_Restart_ForgetTimesOfFirstKata()
        {
            _stopwatch.Stub(x => x.Elapsed).Return(TimeSpan.FromSeconds(2));

            _target.Start();
            _target.ChangeDeveloper();
            _target.Stop();

            _target.Start();

            Assert.That(_target.CycleTimes.Count, Is.EqualTo(0));
        }

        [Test]
        public void AlarmElapsed_RaiseCycleTimeElapsedEvent()
        {
            var raised = false;

            _target.Start();
            _target.CycleTimeElapsed += (s, e) => raised = true;

            GetAlarm(Session.CycleTime).Callback.Invoke();

            Assert.That(raised, Is.True);
        }

        [Test]
        public void AlarmElapsed_SecondTime_RaiseFinishHimTimeElapsedEvent()
        {
            var raised = false;
            _session.Stub(x => x.Get<bool>(Session.FinishHimTimeActive)).Return(true);
            _session.Stub(x => x.Get<TimeSpan>(Session.FinishHimTime)).Return(TimeSpan.FromSeconds(1));

            _target.Start();
            _target.FinishHimTimeElapsed += (s, e) => raised = true;

            GetAlarm(Session.CycleTime).Callback.Invoke();
            GetAlarm(Session.FinishHimTime).Callback.Invoke();

            Assert.That(raised, Is.True);
        }

        [Test]
        public void AlarmElapsed_DojoTimeFinished_RaiseDojoTimeElapsed()
        {
            var raised = false;
            _session.Stub(x => x.Get<bool>(Session.FinishHimTimeActive)).Return(true);
            _session.Stub(x => x.Get<TimeSpan>(Session.FinishHimTime)).Return(TimeSpan.FromSeconds(1));
            _session.Stub(x => x.Get<TimeSpan>(Session.DojoTime)).Return(TimeSpan.FromSeconds(2));

            _target.Start();
            _target.DojoTimeElapsed += (s, e) => raised = true;

            GetAlarm(Session.DojoTime).Callback.Invoke();

            Assert.That(raised, Is.True);
        }

        [Test]
        public void Start_FinishHimTimeSet_SetAlarmWithCorrectTimes()
        {
            _session.Stub(x => x.Get<bool>(Session.FinishHimTimeActive)).Return(true);
            _session.Stub(x => x.Get<TimeSpan>(Session.CycleTime)).Return(TimeSpan.FromSeconds(1));
            _session.Stub(x => x.Get<TimeSpan>(Session.FinishHimTime)).Return(TimeSpan.FromSeconds(2));

            _target.Start();

            Assert.That(GetAlarm(Session.CycleTime).Alarm, Is.EqualTo(TimeSpan.FromSeconds(1)));
            Assert.That(GetAlarm(Session.FinishHimTime).Alarm, Is.EqualTo(TimeSpan.FromSeconds(2)));
        }

        [Test]
        public void Start_FinishHimTimeNotActive_DoNotSetTwoAlarms()
        {
            _session.Stub(x => x.Get<bool>(Session.FinishHimTimeActive)).Return(false);

            _target.Start();

            Assert.Throws<ArgumentException>(() => GetAlarm(Session.FinishHimTime));
        }

        [Test]
        public void ChangeDeveloper_TwiceAlarmElapsedSecondTime_RaiseFinishHimTimeElapsedEventTwice()
        {
            var raised = 0;
            _session.Stub(x => x.Get<bool>(Session.FinishHimTimeActive)).Return(true);
            _session.Stub(x => x.Get<TimeSpan>(Session.FinishHimTime)).Return(TimeSpan.FromSeconds(1));

            _target.Start();
            _target.FinishHimTimeElapsed += (s, e) => raised++;

            GetAlarm(Session.CycleTime).Callback.Invoke();
            GetAlarm(Session.FinishHimTime).Callback.Invoke();

            _target.ChangeDeveloper();

            GetAlarm(Session.CycleTime).Callback.Invoke();
            GetAlarm(Session.FinishHimTime).Callback.Invoke();

            Assert.That(raised, Is.EqualTo(2));
        }

        /// <summary>
        /// Needed to circumvent the mocked stopwatch
        /// </summary>
        private StopwatchItem GetAlarm(string key)
        {
            var dummyStopwatch = new DojoStopwatch { Alarms = _stopwatch.Alarms };
            return dummyStopwatch.GetAlarm(key);
        }
    }
}