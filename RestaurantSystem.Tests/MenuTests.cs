using System;
using System.Collections.Generic;
using NUnit.Framework;
using RestaurantSystem;

namespace RestaurantSystem.Tests
{
    [TestFixture]
    public class MenuTests
    {
        [Test]
        public void Constructor_WhenInitialItemsEmpty_ShouldThrow()
        {
            var empty = new List<MenuItem>();

            Assert.Throws<InvalidOperationException>(() =>
                new Menu("M1", "Main", true, empty));
        }

        [Test]
        public void AddItem_ShouldCreateReverseConnection()
        {
            var item1 = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);
            var menu = new Menu("M1", "Main", true, new[] { item1 });

            var item2 = new MenuItem("I2", "Fries", "Desc", 6m, "Food", true);

            menu.AddItem(item2);

            Assert.That(menu.GetItems(), Does.Contain(item2));
            Assert.That(item2.HasMenu, Is.True);
            Assert.That(item2.Menu, Is.SameAs(menu));
        }

        [Test]
        public void RemoveItem_ShouldRemoveReverseConnection()
        {
            var item1 = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);
            var item2 = new MenuItem("I2", "Fries", "Desc", 6m, "Food", true);
            var menu = new Menu("M1", "Main", true, new[] { item1, item2 });

            menu.RemoveItem(item2);

            Assert.That(menu.GetItems(), Does.Not.Contain(item2));
            Assert.That(item2.HasMenu, Is.False);
        }

        [Test]
        public void RemoveItem_WhenTryingToRemoveLastItem_ShouldThrow()
        {
            var item1 = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);
            var menu = new Menu("M1", "Main", true, new[] { item1 });

            Assert.Throws<InvalidOperationException>(() => menu.RemoveItem(item1));
        }

        [Test]
        public void AddItem_DuplicateReference_ShouldThrow()
        {
            var item1 = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);
            var menu = new Menu("M1", "Main", true, new[] { item1 });

            Assert.Throws<InvalidOperationException>(() => menu.AddItem(item1));
        }

        [Test]
        public void AddItem_ItemAlreadyAssignedToDifferentMenu_ShouldThrow()
        {
            var item = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);

            var menu1 = new Menu("M1", "Menu1", true, new[] { item });
            var menu2 = new Menu("M2", "Menu2", true, new[] { new MenuItem("I2", "Fries", "Desc", 6m, "Food", true) });

            Assert.Throws<InvalidOperationException>(() => menu2.AddItem(item));
        }

        [Test]
        public void FindItemByName_ShouldReturnMatchingItem_IgnoringCase()
        {
            var burger = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);
            var fries = new MenuItem("I2", "Fries", "Desc", 6m, "Food", true);
            var menu = new Menu("M1", "Main", true, new[] { burger, fries });

            var found = menu.FindItemByName("bUrGeR");

            Assert.That(found, Is.SameAs(burger));
        }
    }
}
