using NUnit.Framework;
using System;
using RestaurantSystem;

namespace RestaurantSystem_Tests
{
    [TestFixture]
    public class StaffReflexTests
    {
        [SetUp]
        public void Setup()
        {
            // keep tests isolated
            Staff.ClearExtent();
        }

        [Test]
        public void SetManager_UpdatesBothSides()
        {
            var manager = new Staff("Alice Manager", "Manager");
            var emp = new Staff("Bob Employee", "Cashier");

            emp.SetManager(manager);

            Assert.That(emp.Manager, Is.EqualTo(manager));
            Assert.That(manager.Subordinates, Does.Contain(emp));
        }

        [Test]
        public void ChangeManager_MovesSubordinateToNewManager()
        {
            var m1 = new Staff("Manager One", "Manager");
            var m2 = new Staff("Manager Two", "Manager");
            var emp = new Staff("Employee", "Cashier");

            emp.SetManager(m1);
            emp.SetManager(m2);

            Assert.That(emp.Manager, Is.EqualTo(m2));
            Assert.That(m1.Subordinates, Does.Not.Contain(emp));
            Assert.That(m2.Subordinates, Does.Contain(emp));
        }

        [Test]
        public void RemoveManager_RemovesFromOldManagersSubordinates()
        {
            var manager = new Staff("Alice Manager", "Manager");
            var emp = new Staff("Bob Employee", "Cashier");

            emp.SetManager(manager);
            emp.RemoveManager();

            Assert.That(emp.Manager, Is.Null);
            Assert.That(manager.Subordinates, Does.Not.Contain(emp));
        }

        [Test]
        public void AddSubordinate_SetsManagerAutomatically()
        {
            var manager = new Staff("Alice Manager", "Manager");
            var emp = new Staff("Bob Employee", "Cashier");

            manager.AddSubordinate(emp);

            Assert.That(emp.Manager, Is.EqualTo(manager));
            Assert.That(manager.Subordinates, Does.Contain(emp));
        }

        [Test]
        public void SetManager_ToSelf_ThrowsException()
        {
            var emp = new Staff("Bob Employee", "Cashier");

            Assert.Throws<ArgumentException>(() => emp.SetManager(emp));
        }

        [Test]
        public void RemoveSubordinate_NotInList_ThrowsException()
        {
            var manager = new Staff("Alice Manager", "Manager");
            var emp = new Staff("Bob Employee", "Cashier");

            Assert.Throws<InvalidOperationException>(() => manager.RemoveSubordinate(emp));
        }
    }
}
