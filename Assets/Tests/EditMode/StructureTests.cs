using Core.Events;
using Core.Teams;
using NUnit.Framework;
using Structures;
using Tests.EditMode.Stubs;
using UnityEngine;

namespace Tests.EditMode
{
    public class StructureTests
    {
        private TestStructure _structure;
        private EventBus _eventBus;
        private StubDamageSource _source;

        [SetUp]
        public void SetUp()
        {
            var go = new GameObject("Structure");
            _structure = go.AddComponent<TestStructure>();

            _eventBus = new EventBus();
            _source = new StubDamageSource(10f, Team.Blue);

            _structure.Init(_eventBus, 100f);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_structure.gameObject);
        }

        [Test]
        public void Starts_With_Full_Health()
        {
            Assert.AreEqual(100f, _structure.CurrentHealth);
        }

        [Test]
        public void IsAlive_True_Initially()
        {
            Assert.IsTrue(_structure.IsAlive);
        }

        [Test]
        public void TakeDamage_Reduces_Health()
        {
            _structure.TakeDamage(25f, _source);

            Assert.AreEqual(75f, _structure.CurrentHealth);
        }

        [Test]
        public void TakeDamage_Raises_DamageTaken_Event()
        {
            float received = 0f;
            _structure.DamageTaken += amount => received = amount;

            _structure.TakeDamage(25f, _source);

            Assert.AreEqual(25f, received);
        }

        [Test]
        public void TakeDamage_To_Zero_Kills_Structure()
        {
            _structure.TakeDamage(100f, _source);

            Assert.IsFalse(_structure.IsAlive);
        }

        [Test]
        public void TakeDamage_After_Death_Does_Nothing()
        {
            _structure.TakeDamage(100f, _source);
            _structure.TakeDamage(50f, _source);

            Assert.AreEqual(0f, _structure.CurrentHealth);
        }

        [Test]
        public void Death_Publishes_StructureDestroyedEvent()
        {
            StructureDestroyedEvent received = default;
            _eventBus.Subscribe<StructureDestroyedEvent>(e => received = e);

            _structure.TakeDamage(100f, _source);

            Assert.AreEqual(_structure, received.Structure);
            Assert.AreEqual(Team.Blue, received.KillerTeam);
        }

        [Test]
        public void Death_Publishes_GoldReward_From_Definition()
        {
            StructureDestroyedEvent received = default;
            _eventBus.Subscribe<StructureDestroyedEvent>(e => received = e);

            _structure.TakeDamage(100f, _source);

            Assert.AreEqual(5, received.GoldReward);
        }

        [Test]
        public void OnDeath_Called_When_Killed()
        {
            _structure.TakeDamage(100f, _source);

            Assert.IsTrue(_structure.OnDeathCalled);
        }
    }

    public class TestStructureDefinitionSO : StructureDefinitionSO { }

    public class TestStructure : Structure
    {
        private TestStructureDefinitionSO _definition;
        public bool OnDeathCalled { get; private set; }

        protected override StructureDefinitionSO Definition => _definition;

        public void Init(IEventBus eventBus, float health)
        {
            _definition = ScriptableObject.CreateInstance<TestStructureDefinitionSO>();
            _definition.Health = health;
            _definition.GoldReward = 5;

            Construct(eventBus);

            // Manually activate instead of Start (avoids TeamVisual dependency)
            SetActive(health);
        }

        private void SetActive(float health)
        {
            // Use reflection to set private fields that Start normally sets
            var baseType = typeof(Structure);
            baseType.GetField("_isActive", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(this, true);
            baseType.GetField("_currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(this, health);
        }

        protected override void OnDeath()
        {
            OnDeathCalled = true;
        }
    }
}
