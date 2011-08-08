using System;
using System.Windows;
using System.Windows.Forms;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using CodingDojoHelper.ViewModels;
using Microsoft.Practices.Prism.Events;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodingDojoHelperTests.ViewModels
{
    [TestFixture]
    class AdvancedConfigViewModelTests
    {
        private ISession _session;
        private AdvancedConfigViewModel _target;
        private IInterceptKeys _interceptKeys;

        [SetUp]
        public void Setup()
        {
            _session = MockRepository.GenerateStub<ISession>();
            _interceptKeys = MockRepository.GenerateStub<IInterceptKeys>();
            _target = new AdvancedConfigViewModel(new EventAggregator(), _session, _interceptKeys);
        }

        [Test]
        public void CycleTimeChanged_MinimumFinishHimTimeAdapted()
        {
            var raised = false;

            _target.PropertyChanged += (s, e) => raised = true;
            _session.Stub(x => x.Get<TimeSpan>(Session.CycleTime)).Return(TimeSpan.FromMinutes(5));
            _session.Raise(x => x.ValueChanged += null, this,
                new ValueChangedEventArgs(Session.CycleTime, null, TimeSpan.FromMinutes(5)));

            Assert.That(raised, Is.True);
            Assert.That(_target.ValueForCycleTime, Is.EqualTo(55));
        }

        [Test]
        public void CycleTimeChanged_CycleTimeLongerThanCurrentFinishHimTime_AdjustFinishHimTimeToCycleTime()
        {
            _session.Stub(x => x.Get<TimeSpan>(Session.FinishHimTime)).Return(TimeSpan.FromMinutes(5));
            _session.Stub(x => x.Get<TimeSpan>(Session.CycleTime)).Return(TimeSpan.FromMinutes(6));
            _session.Raise(x => x.ValueChanged += null, this,
                new ValueChangedEventArgs(Session.CycleTime, null, TimeSpan.FromMinutes(6)));

            _session.AssertWasCalled(x => x.Set(Session.FinishHimTime, TimeSpan.FromMinutes(6)));
        }

        [Test]
        public void ValueForFinishHimTime_FinishHimTimeInSessionSet_CalculateValueForFinishHimTime()
        {
            var raised = false;
            _target.PropertyChanged += (s, e) => raised = true;

            _session.Stub(x => x.Get<TimeSpan>(Session.FinishHimTime)).Return(TimeSpan.FromMinutes(5));
            _session.Raise(x => x.ValueChanged += null, this,
                new ValueChangedEventArgs(Session.FinishHimTime, null, TimeSpan.FromMinutes(5)));

            Assert.That(raised, Is.True);
            Assert.That(_target.ValueForFinishHimTime, Is.EqualTo(55));
        }

        [Test]
        public void SelectChangeDeveloperKeyCommand_SetAKey_SetInSession()
        {
            _interceptKeys.Stub(x => x.WaitForNextKeyAsync(TimeSpan.Zero, null)).IgnoreArguments().
                Do((Action<TimeSpan, Action<Keys>>) ((t, a) => a.Invoke(Keys.A)));

            _target.SelectChangeDeveloperKeyCommand.Execute(null);

            _session.AssertWasCalled(x => x.Set(Session.ChangeDeveloperKey, Keys.A));
        }

        [Test]
        public void SelectChangeDeveloperKeyCommand_NoKeyWasPressed_DoNotChangeSession()
        {
            _interceptKeys.Stub(x => x.WaitForNextKeyAsync(TimeSpan.Zero, null)).IgnoreArguments().
                Do((Action<TimeSpan, Action<Keys>>)((t, a) => a.Invoke(Keys.None)));

            _target.SelectChangeDeveloperKeyCommand.Execute(null);

            _session.AssertWasNotCalled(x => x.Set(Session.ChangeDeveloperKey, Keys.None));
        }

        [Test]
        public void SelectChangeDeveloperKeyCommand_SetAKey_UiVisibleAgain()
        {
            _interceptKeys.Stub(x => x.WaitForNextKeyAsync(TimeSpan.Zero, null)).IgnoreArguments().
                Do((Action<TimeSpan, Action<Keys>>)((t, a) => a.Invoke(Keys.A)));

            _target.SelectChangeDeveloperKeyCommand.Execute(null);

            Assert.That(_target.AdornerVisibility, Is.EqualTo(Visibility.Hidden));
        }

        [Test]
        public void SelectEndKataKeyCommand_SetAKey_SetInSession()
        {
            _interceptKeys.Stub(x => x.WaitForNextKeyAsync(TimeSpan.Zero, null)).IgnoreArguments().
                Do((Action<TimeSpan, Action<Keys>>)((t, a) => a.Invoke(Keys.A)));

            _target.SelectEndKataKeyCommand.Execute(null);

            _session.AssertWasCalled(x => x.Set(Session.EndKataKey, Keys.A));
        }

        [Test]
        public void SelectEndKataKeyCommand_NoKeyWasPressed_DoNotChangeSession()
        {
            _interceptKeys.Stub(x => x.WaitForNextKeyAsync(TimeSpan.Zero, null)).IgnoreArguments().
                Do((Action<TimeSpan, Action<Keys>>)((t, a) => a.Invoke(Keys.None)));

            _target.SelectEndKataKeyCommand.Execute(null);

            _session.AssertWasNotCalled(x => x.Set(Session.EndKataKey, Keys.None));
        }

        [Test]
        public void SelectEndKataKeyCommand_SetAKey_UiVisibleAgain()
        {
            _interceptKeys.Stub(x => x.WaitForNextKeyAsync(TimeSpan.Zero, null)).IgnoreArguments().
                Do((Action<TimeSpan, Action<Keys>>)((t, a) => a.Invoke(Keys.A)));

            _target.SelectEndKataKeyCommand.Execute(null);

            Assert.That(_target.AdornerVisibility, Is.EqualTo(Visibility.Hidden));
        }
    }
}
