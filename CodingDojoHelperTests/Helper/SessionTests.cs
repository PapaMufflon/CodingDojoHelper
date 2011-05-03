using System;
using CodingDojoHelper.Helper;
using NUnit.Framework;

namespace CodingDojoHelperTests.Helper
{
    [TestFixture]
    class SessionTests
    {
        private Session _target;

        [SetUp]
        public void Setup()
        {
            _target = new Session();
        }

        [Test]
        public void Get_NothingSet_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _target.Get<string>("foo"));
        }

        [Test]
        public void Get_StringSet_GetString()
        {
            _target.Set("foo", "bar");

            Assert.That(_target.Get<string>("foo"), Is.EqualTo("bar"));
        }

        [Test]
        public void Get_WrongType_InvalidOperationException()
        {
            _target.Set("foo", "bar");

            Assert.Throws<InvalidOperationException>(() => _target.Get<Session>("foo"));
        }

        [Test]
        public void Get_KeyNull_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _target.Get<object>(null));
        }

        [Test]
        public void Get_SetTwice_GetSecondObject()
        {
            _target.Set("foo", "bar");
            _target.Set("foo", "foobar");

            Assert.That(_target.Get<string>("foo"), Is.EqualTo("foobar"));
        }

        [Test]
        public void Get_CastableType()
        {
            _target.Set("foo", "bar");

            var actual = _target.Get<object>("foo");

            Assert.That(actual, Is.EqualTo("bar"));
        }

        [Test]
        public void Set_RaisesEventWithCorrectKey()
        {
            var raised = false;
            _target.ValueChanged += (s, e) => raised = e.Key == "foo";

            _target.Set("foo", "bar");

            Assert.That(raised, Is.True);
        }

        [Test]
        public void Set_RaisesEventWithCorrectOldNewValues()
        {
            var raised = false;

            _target.ValueChanged += (s, e) =>
            {
                raised = e.Key == "foo";
                Assert.That(e.OldValue, Is.Null);
                Assert.That(e.NewValue, Is.EqualTo("bar"));
            };

            _target.Set("foo", "bar");

            Assert.That(raised, Is.True);
        }
    }
}
