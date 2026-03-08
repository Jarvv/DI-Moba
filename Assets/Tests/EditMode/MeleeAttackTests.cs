using Core.Combat;
using Core.Teams;
using Creeps.Behaviour;
using NUnit.Framework;
using UnityEngine;

namespace Tests.EditMode
{
    public class MeleeAttackTests
    {
        private MeleeAttack _attack;
        private Transform _owner;
        private DummyTarget _target;
        private DummySource _source;

        [SetUp]
        public void SetUp()
        {
            _attack = new MeleeAttack(range: 2f, damage: 5f, attackSpeed: 2f);
            var go = new GameObject("Owner");
            _owner = go.transform;
            _target = new DummyTarget(health: 20f, Team.Red, Vector3.zero);
            _source = new DummySource(5f, Team.Blue);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_owner.gameObject);
        }

        [Test]
        public void IsReady_True_Initially()
        {
            Assert.IsTrue(_attack.IsReady);
        }

        [Test]
        public void Execute_Deals_Damage_To_Target()
        {
            _attack.Execute(_owner, _target, _source);

            Assert.AreEqual(15f, _target.CurrentHealth);
        }

        [Test]
        public void Execute_Sets_Cooldown()
        {
            _attack.Execute(_owner, _target, _source);

            Assert.IsFalse(_attack.IsReady);
        }

        [Test]
        public void Execute_Does_Nothing_While_On_Cooldown()
        {
            _attack.Execute(_owner, _target, _source);
            _attack.Execute(_owner, _target, _source);

            // Only one hit should land
            Assert.AreEqual(15f, _target.CurrentHealth);
        }

        [Test]
        public void Tick_Reduces_Cooldown()
        {
            _attack.Execute(_owner, _target, _source);
            Assert.IsFalse(_attack.IsReady);

            // attackSpeed=2 => cooldown = 1/2 = 0.5s
            _attack.Tick(0.5f);
            Assert.IsTrue(_attack.IsReady);
        }

        [Test]
        public void Can_Attack_Again_After_Cooldown()
        {
            _attack.Execute(_owner, _target, _source);
            _attack.Tick(0.5f);
            _attack.Execute(_owner, _target, _source);

            Assert.AreEqual(10f, _target.CurrentHealth);
        }

        [Test]
        public void ResetState_Clears_Cooldown()
        {
            _attack.Execute(_owner, _target, _source);
            Assert.IsFalse(_attack.IsReady);

            _attack.ResetState();
            Assert.IsTrue(_attack.IsReady);
        }

        private class DummyTarget : IDamageable
        {
            public float CurrentHealth { get; private set; }
            public float MaxHealth { get; }
            public Team Team { get; }
            public Vector3 Position { get; }
            public bool IsAlive => CurrentHealth > 0;

            public DummyTarget(float health, Team team, Vector3 position)
            {
                CurrentHealth = health;
                MaxHealth = health;
                Team = team;
                Position = position;
            }

            public void TakeDamage(float amount, IDamageSource source)
            {
                CurrentHealth -= amount;
            }
        }

        private class DummySource : IDamageSource
        {
            public float Damage { get; }
            public Team Team { get; }

            public DummySource(float damage, Team team)
            {
                Damage = damage;
                Team = team;
            }
        }
    }
}
