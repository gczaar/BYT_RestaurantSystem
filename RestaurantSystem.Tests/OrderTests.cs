using System;
using System.Linq;
using NUnit.Framework;
using RestaurantSystem;

namespace RestaurantSystem.Tests
{
    [TestFixture]
    public class OrderTests
    {
        [Test]
        public void AddItem_ShouldAddToOrder_AndSetAssociations()
        {
            var order = new Order("O1", DateTime.Now, "Created");
            var menuItem = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);

            var oi = order.AddItem(menuItem, 2, 9.5m);

            Assert.That(order.GetItems(), Does.Contain(oi));
            Assert.That(oi.HasOrder, Is.True);
            Assert.That(oi.Order, Is.SameAs(order));

            Assert.That(oi.HasMenuItem, Is.True);
            Assert.That(oi.MenuItem, Is.SameAs(menuItem));
        }

        [Test]
        public void AddItem_InvalidQuantity_ShouldThrow()
        {
            var order = new Order("O1", DateTime.Now, "Created");
            var menuItem = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);

            Assert.Throws<ArgumentException>(() => order.AddItem(menuItem, 0, 9.5m));
        }

        [Test]
        public void AddItem_NegativePrice_ShouldThrow()
        {
            var order = new Order("O1", DateTime.Now, "Created");
            var menuItem = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);

            Assert.Throws<ArgumentException>(() => order.AddItem(menuItem, 1, -1m));
        }

        [Test]
        public void TotalAmount_ShouldBeSumOfLineTotals()
        {
            var order = new Order("O1", DateTime.Now, "Created");
            var burger = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);
            var fries = new MenuItem("I2", "Fries", "Desc", 6m, "Food", true);

            order.AddItem(burger, 2, 9m); 
            order.AddItem(fries, 3, 5m);  

            var total = order.TotalAmount;

            Assert.That(total, Is.EqualTo(33m));
        }

        [Test]
        public void RemoveItem_ShouldRemoveFromOrder_AndClearAssociations()
        {
            var order = new Order("O1", DateTime.Now, "Created");
            var burger = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);
            var fries = new MenuItem("I2", "Fries", "Desc", 6m, "Food", true);

            var oi1 = order.AddItem(burger, 1, 10m);
            var oi2 = order.AddItem(fries, 1, 6m);

            order.RemoveItem(oi2);

            Assert.That(order.GetItems(), Does.Not.Contain(oi2));
            Assert.That(oi2.HasOrder, Is.False);
            Assert.That(oi2.HasMenuItem, Is.False);

            Assert.That(order.GetItems(), Does.Contain(oi1));
            Assert.That(oi1.HasOrder, Is.True);
        }

        [Test]
        public void RemoveItem_WhenRemovingLastItem_ShouldThrow()
        {
            var order = new Order("O1", DateTime.Now, "Created");
            var burger = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);

            var oi = order.AddItem(burger, 1, 10m);

            Assert.Throws<InvalidOperationException>(() => order.RemoveItem(oi));
        }

        [Test]
        public void RemoveItem_ItemNotInThisOrder_ShouldThrow()
        {
            var order1 = new Order("O1", DateTime.Now, "Created");
            var order2 = new Order("O2", DateTime.Now, "Created");

            var burger = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);

            var oi = order1.AddItem(burger, 1, 10m);
            order2.AddItem(new MenuItem("I2", "Fries", "Desc", 6m, "Food", true), 1, 6m);

            Assert.Throws<InvalidOperationException>(() => order2.RemoveItem(oi));
        }

        [Test]
        public void Delete_ShouldDetachAllItems()
        {
            var order = new Order("O1", DateTime.Now, "Created");
            var burger = new MenuItem("I1", "Burger", "Desc", 10m, "Food", true);
            var fries = new MenuItem("I2", "Fries", "Desc", 6m, "Food", true);

            var oi1 = order.AddItem(burger, 1, 10m);
            var oi2 = order.AddItem(fries, 2, 6m);

            order.Delete();

            Assert.That(order.GetItems().Count, Is.EqualTo(0));

            Assert.That(oi1.HasOrder, Is.False);
            Assert.That(oi1.HasMenuItem, Is.False);

            Assert.That(oi2.HasOrder, Is.False);
            Assert.That(oi2.HasMenuItem, Is.False);
        }
    }
}
