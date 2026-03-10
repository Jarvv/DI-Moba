using Core.Combat.UI;
using Core.Teams;
using NUnit.Framework;
using Tests.EditMode.Stubs;

namespace Tests.EditMode
{
    public class HealthBarViewModelTests
    {
        private StubDamageable _target;
        private HealthBarViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _target = new StubDamageable(100f, Team.Red);
            _viewModel = new HealthBarViewModel(_target);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel.Dispose();
        }

        [Test]
        public void FillAmount_Full_At_Max_Health()
        {
            Assert.AreEqual(1f, _viewModel.FillAmount);
        }

        [Test]
        public void FillAmount_Reflects_Damage()
        {
            _target.TakeDamage(25f, null);

            Assert.AreEqual(0.75f, _viewModel.FillAmount);
        }

        [Test]
        public void FillAmount_Zero_When_Dead()
        {
            _target.TakeDamage(100f, null);

            Assert.AreEqual(0f, _viewModel.FillAmount);
        }

        [Test]
        public void IsVisible_False_At_Full_Health()
        {
            Assert.IsFalse(_viewModel.IsVisible);
        }

        [Test]
        public void IsVisible_True_After_Damage()
        {
            _target.TakeDamage(10f, null);

            Assert.IsTrue(_viewModel.IsVisible);
        }

        [Test]
        public void IsVisible_False_When_Dead()
        {
            _target.TakeDamage(100f, null);

            Assert.IsFalse(_viewModel.IsVisible);
        }

        [Test]
        public void HealthChanged_Fires_On_Damage()
        {
            int fireCount = 0;
            _viewModel.HealthChanged += () => fireCount++;

            _target.TakeDamage(10f, null);

            Assert.AreEqual(1, fireCount);
        }

        [Test]
        public void HealthChanged_Does_Not_Fire_After_Dispose()
        {
            int fireCount = 0;
            _viewModel.HealthChanged += () => fireCount++;

            _viewModel.Dispose();
            _target.TakeDamage(10f, null);

            Assert.AreEqual(0, fireCount);
        }
    }
}
