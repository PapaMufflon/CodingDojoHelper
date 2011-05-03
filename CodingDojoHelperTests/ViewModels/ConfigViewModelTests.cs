using System;
using System.Collections.Generic;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using CodingDojoHelper.ViewModels;
using Microsoft.Practices.Composite.Events;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodingDojoHelperTests.ViewModels
{
    [TestFixture]
    public class ConfigViewModelTests
    {
        private ISession _session;
        private ConfigViewModel _target;

        [SetUp]
        public void Setup()
        {
            _session = MockRepository.GenerateStub<ISession>();
            _session.Stub(x => x.Get<List<Uri>>(Session.CombatantImages)).Return(new List<Uri> {new Uri("ftp://foo"), new Uri("http://bar"), new Uri("http://foobar")});
            _target = new ConfigViewModel(_session, new EventAggregator());
        }

        [Test]
        public void OkCommand_CycleTimeSet_SessionObjectChanged()
        {
            _target.CycleTime = TimeSpan.FromSeconds(4);

            _session.AssertWasCalled(x => x.Set(Session.CycleTime, TimeSpan.FromSeconds(4)));
        }

        [Test]
        public void Combatants_OneCombatant_OneImage()
        {
            _target.CombatantsCount = 1;

            Assert.That(_target.Combatants.Count, Is.EqualTo(1));
        }

        [Test]
        public void Combatants_TwoCombatants_TwoImages()
        {
            _target.CombatantsCount = 2;

            Assert.That(_target.Combatants.Count, Is.EqualTo(2));
        }

        [Test]
        public void CombatantsCount_SmallerThan1_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.CombatantsCount = 0);
        }

        [Test]
        public void CombatantsCount_GreaterThanMax_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.CombatantsCount = ConfigViewModel.MaxCombatants + 1);
        }

        [Test]
        public void ActiveValue_Changed_DoNotPropagateNewValue()
        {
            _target.ActiveValue = ConfigValue.CycleTime;

            _session.AssertWasNotCalled(x => x.Set(Session.CycleTime, null), c => c.IgnoreArguments());
        }
    }
}
