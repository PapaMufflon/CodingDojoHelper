using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodingDojoHelper;
using CodingDojoHelper.Events;
using CodingDojoHelper.Helper;
using CodingDojoHelper.Helper.Interfaces;
using CodingDojoHelper.ViewModels;
using Microsoft.Practices.Prism.Events;
using NUnit.Framework;
using Rhino.Mocks;

namespace CodingDojoHelperTests.ViewModels
{
    [TestFixture]
    public class DojoViewModelTests
    {
        private IInterceptKeys _interceptKeys;
        private ICodingDojo _codingDojo;
        private DojoViewModel _target;
        private IEventAggregator _eventAggregator;
        private StartKataEvent _startKataEvent;
        private KataFinishedEvent _kataFinishedEvent;
        private ISession _session;

        [SetUp]
        public void Setup()
        {
            _interceptKeys = MockRepository.GenerateStub<IInterceptKeys>();
            _codingDojo = MockRepository.GenerateStub<ICodingDojo>();
            
            _eventAggregator = MockRepository.GenerateStub<IEventAggregator>();
            _startKataEvent = new StartKataEvent();
            _eventAggregator.Stub(x => x.GetEvent<StartKataEvent>()).Return(_startKataEvent);
            _kataFinishedEvent = new KataFinishedEvent();
            _eventAggregator.Stub(x => x.GetEvent<KataFinishedEvent>()).Return(_kataFinishedEvent);

            _session = MockRepository.GenerateStub<ISession>();

            _target = new DojoViewModel(_interceptKeys, _codingDojo, _eventAggregator, _session);
        }

        [Test]
        public void Ctor_InterceptKeysIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DojoViewModel(null, _codingDojo, _eventAggregator, _session));
        }

        [Test]
        public void Ctor_CodingDojoIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DojoViewModel(_interceptKeys, null, _eventAggregator, _session));
        }

        [Test]
        public void Ctor_EventAggregatorIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DojoViewModel(_interceptKeys, _codingDojo, null, _session));
        }

        [Test]
        public void Ctor_SessionIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
               new DojoViewModel(_interceptKeys, _codingDojo, _eventAggregator, null));
        }

        [Test]
        public void StartKataEvent_StartsCodingDojo()
        {
            _startKataEvent.Publish(null);

            _codingDojo.AssertWasCalled(x => x.Start());
        }

        [Test]
        public void InterceptKey_Scroll_LastKeyIsScroll()
        {
            _session.Stub(x => x.Get<Keys>(Session.ChangeDeveloperKey)).Return(Keys.Scroll);

            _interceptKeys.Raise(x => x.KeyIntercepted += null, _interceptKeys, new KeyInterceptedEventArgs(Keys.Scroll));
            
            Assert.That(_target.LastKey, Is.EqualTo(Keys.Scroll));
        }

        [Test]
        public void StartStopCommand_Started_CommandIsEnabled()
        {
            Assert.That(_target.StopCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void KeyIntercepted_ScrollKey_ChangeDeveloper()
        {
            _session.Stub(x => x.Get<Keys>(Session.ChangeDeveloperKey)).Return(Keys.Scroll);

            _startKataEvent.Publish(null);
            _interceptKeys.Raise(x => x.KeyIntercepted += null, _interceptKeys, new KeyInterceptedEventArgs(Keys.Scroll));

            _codingDojo.AssertWasCalled(x => x.ChangeDeveloper());
        }

        [Test]
        public void StartStopCommand_ScrollPushedStop_StartedIsFalse()
        {
            var kataFinished = false;
            _kataFinishedEvent.Subscribe(o => kataFinished = true);

            _session.Stub(x => x.Get<Keys>(Session.ChangeDeveloperKey)).Return(Keys.Scroll);

            _startKataEvent.Publish(null);
            _interceptKeys.Raise(x => x.KeyIntercepted += null, _interceptKeys, new KeyInterceptedEventArgs(Keys.Scroll));
            _target.StopCommand.Execute(null);

            Assert.That(kataFinished, Is.True);
        }

        [Test]
        public void StartStopCommand_ScrollKeyThenStop_DojoStopped()
        {
            _session.Stub(x => x.Get<Keys>(Session.ChangeDeveloperKey)).Return(Keys.Scroll);

            _interceptKeys.Raise(x => x.KeyIntercepted += null, _interceptKeys, new KeyInterceptedEventArgs(Keys.Scroll));
            _target.StopCommand.Execute(null);

            _codingDojo.AssertWasCalled(x => x.Stop());
        }

        [Test]
        public void StartStopCommand_ScrollThenPause_DojoStopped()
        {
            _session.Stub(x => x.Get<Keys>(Session.ChangeDeveloperKey)).Return(Keys.Scroll);
            _session.Stub(x => x.Get<Keys>(Session.EndKataKey)).Return(Keys.Pause);

            _startKataEvent.Publish(null);

            _interceptKeys.Raise(x => x.KeyIntercepted += null, _interceptKeys, new KeyInterceptedEventArgs(Keys.Scroll));
            _interceptKeys.Raise(x => x.KeyIntercepted += null, _interceptKeys, new KeyInterceptedEventArgs(Keys.Pause));

            _codingDojo.AssertWasCalled(x => x.Stop());
        }

        [Test]
        public void FinishHimTimeElapsedEvent_LockKeys()
        {
            _session.Stub(x => x.Get<bool>(Session.FinishHimTimeActive)).Return(true);
            _session.Stub(x => x.Get<Keys>(Session.ChangeDeveloperKey)).Return(Keys.A);
            _session.Stub(x => x.Get<Keys>(Session.EndKataKey)).Return(Keys.B);

            _startKataEvent.Publish(null);
            _codingDojo.Raise(x => x.FinishHimTimeElapsed += null, this, EventArgs.Empty);

            Assert.That(_interceptKeys.AllowedKeys, Is.EqualTo(new List<Keys>{Keys.A, Keys.B}));
        }

        [Test]
        public void DojoTimeElapsedEvent_PublishKataFinishedEvent()
        {
            var called = false;
            _kataFinishedEvent.Subscribe(c => called = true);

            _startKataEvent.Publish(null);
            _codingDojo.Raise(x => x.DojoTimeElapsed += null, _codingDojo, EventArgs.Empty);

            Assert.That(called, Is.True);
        }

        [Test]
        public void InterceptKey_ScrollAfterFinishHimTimeElapsedEvent_AllowedKeysNull()
        {
            _session.Stub(x => x.Get<bool>(Session.FinishHimTimeActive)).Return(true);
            _session.Stub(x => x.Get<Keys>(Session.ChangeDeveloperKey)).Return(Keys.Scroll);
            _startKataEvent.Publish(null);

            _codingDojo.Raise(x => x.FinishHimTimeElapsed += null, this, EventArgs.Empty);
            _interceptKeys.Raise(x => x.KeyIntercepted += null, this, new KeyInterceptedEventArgs(Keys.Scroll));

            Assert.That(_interceptKeys.AllowedKeys, Is.EqualTo(null));
        }

        [Test]
        public void FinishHimTimeElapsedEvent_EndKataKeyIsPermitted()
        {
            _session.Stub(x => x.Get<bool>(Session.FinishHimTimeActive)).Return(true);
            _session.Stub(x => x.Get<Keys>(Session.ChangeDeveloperKey)).Return(Keys.A);
            _session.Stub(x => x.Get<Keys>(Session.EndKataKey)).Return(Keys.B);
            _startKataEvent.Publish(null);

            _codingDojo.Raise(x => x.FinishHimTimeElapsed += null, this, EventArgs.Empty);
            _interceptKeys.Raise(x => x.KeyIntercepted += null, this, new KeyInterceptedEventArgs(Keys.B));

            Assert.That(_interceptKeys.AllowedKeys, Is.EqualTo(null));
        }

        [Test]
        public void FinishHimTimeElapsedEvent_FinishHimTimeNotActive_DoNotLockKeys()
        {
            _session.Stub(x => x.Get<bool>(Session.FinishHimTimeActive)).Return(false);
            _session.Stub(x => x.Get<Keys>(Session.ChangeDeveloperKey)).Return(Keys.A);
            _session.Stub(x => x.Get<Keys>(Session.EndKataKey)).Return(Keys.B);

            _startKataEvent.Publish(null);
            _codingDojo.Raise(x => x.FinishHimTimeElapsed += null, this, EventArgs.Empty);

            Assert.That(_interceptKeys.AllowedKeys, Is.Null);
        }
    }
}
