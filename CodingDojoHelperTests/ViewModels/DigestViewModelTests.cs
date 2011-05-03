using System;
using System.Collections.Generic;
using System.Linq;
using CodingDojoHelper;
using CodingDojoHelper.Events;
using CodingDojoHelper.Helper.Interfaces;
using CodingDojoHelper.ViewModels;
using Microsoft.Practices.Composite.Events;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodingDojoHelperTests.ViewModels
{
    [TestFixture]
    class DigestViewModelTests
    {
        private DigestViewModel _target;
        private KataFinishedEvent _kataFinishedEvent;

        [SetUp]
        public void Setup()
        {
            _kataFinishedEvent = new KataFinishedEvent();

            var eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            eventAggregator.Stub(x => x.GetEvent<KataFinishedEvent>()).Return(_kataFinishedEvent);

            var session = MockRepository.GenerateStub<ISession>();

            _target = new DigestViewModel(eventAggregator, session);
        }

        [Test]
        public void KataFinishedEvent_CodingDojoIsNull_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => _kataFinishedEvent.Publish(null));
        }

        [Test]
        public void KataFinishedEvent_CodingDojoWithAverageCycleTime_PropagateIt()
        {
            var codingDojo = MockRepository.GenerateStub<ICodingDojo>();
            codingDojo.Stub(x => x.AverageCycleTime).Return(TimeSpan.FromSeconds(3));
            codingDojo.Stub(x => x.CycleTimes).Return(new List<TimeSpan>());

            _kataFinishedEvent.Publish(codingDojo);

            Assert.That(_target.AverageCycleTime, Is.EqualTo(TimeSpan.FromSeconds(3)));
        }

        [Test]
        public void KataFinishedEvent_CodingDojoWithCycleTimes_PropagateIt()
        {
            var codingDojo = MockRepository.GenerateStub<ICodingDojo>();
            codingDojo.Stub(x => x.CycleTimes).Return(new List<TimeSpan> { TimeSpan.FromSeconds(3) });

            _kataFinishedEvent.Publish(codingDojo);

            Assert.That(_target.CycleTimes.First(), Is.EqualTo(new KeyValuePair<string, double>("0:00:00", 3.0/60.0)));
        }
    }
}
