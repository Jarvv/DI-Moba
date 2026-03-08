using System;
using Core.Events;
using NUnit.Framework;

namespace Tests.EditMode
{
    public class EventBusTests
    {
        private EventBus _bus;

        [SetUp]
        public void SetUp()
        {
            _bus = new EventBus();
        }

        private struct TestEvent
        {
            public int Value;
        }

        private struct OtherEvent
        {
            public string Message;
        }

        [Test]
        public void Subscribe_And_Publish_Delivers_Event()
        {
            TestEvent received = default;
            _bus.Subscribe<TestEvent>(e => received = e);

            _bus.Publish(new TestEvent { Value = 42 });

            Assert.AreEqual(42, received.Value);
        }

        [Test]
        public void Multiple_Subscribers_All_Receive_Event()
        {
            int callCount = 0;
            _bus.Subscribe<TestEvent>(_ => callCount++);
            _bus.Subscribe<TestEvent>(_ => callCount++);

            _bus.Publish(new TestEvent { Value = 1 });

            Assert.AreEqual(2, callCount);
        }

        [Test]
        public void Unsubscribe_Via_Dispose_Stops_Delivery()
        {
            int callCount = 0;
            var sub = _bus.Subscribe<TestEvent>(_ => callCount++);

            _bus.Publish(new TestEvent());
            Assert.AreEqual(1, callCount);

            sub.Dispose();
            _bus.Publish(new TestEvent());
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void Dispose_Twice_Does_Not_Throw()
        {
            var sub = _bus.Subscribe<TestEvent>(_ => { });

            sub.Dispose();
            Assert.DoesNotThrow(() => sub.Dispose());
        }

        [Test]
        public void Publish_With_No_Subscribers_Does_Not_Throw()
        {
            Assert.DoesNotThrow(() => _bus.Publish(new TestEvent { Value = 1 }));
        }

        [Test]
        public void Different_Event_Types_Are_Independent()
        {
            bool testCalled = false;
            bool otherCalled = false;

            _bus.Subscribe<TestEvent>(_ => testCalled = true);
            _bus.Subscribe<OtherEvent>(_ => otherCalled = true);

            _bus.Publish(new TestEvent());

            Assert.IsTrue(testCalled);
            Assert.IsFalse(otherCalled);
        }

        [Test]
        public void Unsubscribe_During_Publish_Does_Not_Throw()
        {
            IDisposable sub = null;
            sub = _bus.Subscribe<TestEvent>(_ => sub.Dispose());
            _bus.Subscribe<TestEvent>(_ => { });

            Assert.DoesNotThrow(() => _bus.Publish(new TestEvent()));
        }
    }
}
