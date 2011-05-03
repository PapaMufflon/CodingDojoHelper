using System;
using System.Collections.Generic;
using CodingDojoHelper.Helper;
using NUnit.Framework;

namespace CodingDojoHelperTests.Helper
{
    [TestFixture]
    class DojoStopwatchTests
    {
        private DojoStopwatch _target;

        [SetUp]
        public void Setup()
        {
            _target = new DojoStopwatch();
        }

        [Test]
        public void Alarms_AlarmsNotAscending_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _target.Alarms = new List<StopwatchItem>
                {
                    new StopwatchItem("a", TimeSpan.FromSeconds(2), null),
                    new StopwatchItem("b", TimeSpan.FromSeconds(1), null)
                });
        }

        [Test]
        public void Alarms_KeyDuplicate_ThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _target.Alarms = new List<StopwatchItem>
                {
                    new StopwatchItem("a", TimeSpan.FromSeconds(1), null),
                    new StopwatchItem("a", TimeSpan.FromMinutes(1), null)
                });
        }

        [Test]
        public void Alarms_OneAlarmSet_RaiseOneEvent()
        {
            var raised = 0;

            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(10), () => raised++)
            };

            _target.Start();
            System.Threading.Thread.Sleep(300);

            Assert.That(raised, Is.EqualTo(1));
        }

        [Test]
        public void Alarms_CallbackForAlarm()
        {
            var called = false;

            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(10), () => called = true)
            };

            _target.Start();
            System.Threading.Thread.Sleep(300);

            Assert.IsTrue(called);
        }

        [Test]
        public void GetAlarm_ByKey_ReturnCorrectAlarm()
        {
            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("key", TimeSpan.FromMinutes(1), null),
                new StopwatchItem("falseKey", TimeSpan.FromMinutes(2), null)
            };

            var actual = _target.GetAlarm("key");

            Assert.That(actual.Alarm, Is.EqualTo(TimeSpan.FromMinutes(1)));
        }

        [Test]
        public void GetAlarm_KeyNotFound_ArgumentException()
        {
            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("key", TimeSpan.FromMinutes(1), null),
                new StopwatchItem("falseKey", TimeSpan.FromMinutes(2), null)
            };

            Assert.Throws<ArgumentException>(() => _target.GetAlarm("foo"));
        }

        [Test]
        public void Alarms_TwoAlarmsSet_RaiseTwoEvents()
        {
            var raised = 0;

            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(100), () => raised++),
                new StopwatchItem("b", TimeSpan.FromMilliseconds(200), () => raised++)
            };

            _target.Start();
            System.Threading.Thread.Sleep(1000);

            Assert.That(raised, Is.EqualTo(2));
        }

        [Test]
        public void Alarms_ThreeAlarmsSetButThirdIsTooLong_RaiseTwoEvents()
        {
            var raised = 0;

            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(100), () => raised++),
                new StopwatchItem("b", TimeSpan.FromMilliseconds(200), () => raised++),
                new StopwatchItem("c", TimeSpan.FromHours(1), () => raised++)
            };

            _target.Start();
            System.Threading.Thread.Sleep(1000);

            Assert.That(raised, Is.EqualTo(2));
        }

        [Test]
        public void Restart_TwoAlarms_RaisesTwiceTwoEvents()
        {
            var raised = 0;

            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(100), () => raised++),
                new StopwatchItem("b", TimeSpan.FromMilliseconds(200), () => raised++)
            };

            _target.Start();
            System.Threading.Thread.Sleep(1000);
            
            _target.Restart();
            System.Threading.Thread.Sleep(1000);

            Assert.That(raised, Is.EqualTo(4));
        }

        [Test]
        public void RestartAlarm_TwoAlarmsFirstRestart_ThreeEvents()
        {
            var raised = 0;

            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(40), () => raised++),
                new StopwatchItem("b", TimeSpan.FromMilliseconds(80), () => raised++)
            };

            _target.Start();
            System.Threading.Thread.Sleep(1000);

            _target.RestartAlarm("a");
            System.Threading.Thread.Sleep(1000);

            Assert.That(raised, Is.EqualTo(3));
        }

        [Test]
        public void RestartAlarm_AlarmWillBeInFrontOfActiveAlarm_ThreeEvents()
        {
            var raised = 0;

            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(40), () => raised++),
                new StopwatchItem("b", TimeSpan.FromMilliseconds(1500), () => raised++)
            };

            _target.Start();
            System.Threading.Thread.Sleep(1000);

            _target.RestartAlarm("a");
            System.Threading.Thread.Sleep(1000);

            Assert.That(raised, Is.EqualTo(3));
        }

        [Test]
        public void RestartAlarm_AlarmKeyOutOfRange_ThrowException()
        {
            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(400), null),
                new StopwatchItem("b", TimeSpan.FromMilliseconds(1500), null)
            };

            _target.Start();

            Assert.Throws<ArgumentException>(() => _target.RestartAlarm("c"));
        }

        [Test]
        public void RestartAlarm_OneAlarmBefore_OneAlarmAfterRestart()
        {
            var counter = 0;
            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(400), () => counter++),
                new StopwatchItem("b", TimeSpan.FromMilliseconds(700), () => counter++)
            };

            _target.Start();
            _target.RestartAlarm("a");
            System.Threading.Thread.Sleep(1000);

            Assert.That(counter, Is.EqualTo(2));
        }

        [Test]
        public void RestartAlarm_TwoAlarms_CallbackCalled()
        {
            var called = false;
            _target.Alarms = new List<StopwatchItem>
            {
                new StopwatchItem("a", TimeSpan.FromMilliseconds(500), () => called = true),
                new StopwatchItem("b", TimeSpan.FromMilliseconds(7000), () => called = true)
            };

            _target.Start();
            _target.RestartAlarm("a");

            System.Threading.Thread.Sleep(1000);

            Assert.That(called, Is.True);
        }
    }
}