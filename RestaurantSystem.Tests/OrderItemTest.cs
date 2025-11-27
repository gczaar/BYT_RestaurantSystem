using NUnit.Framework;
using RestaurantSystem;
using System;
using System.IO;

namespace RestaurantSystem.Tests
{
    public class OrderItemTest
    {
        [SetUp]
        public void Setup()
        {
            if (File.Exists("orderitems_test.xml"))
                File.Delete("orderitems_test.xml");

            OrderItem.ClearExtent();
        }

        [Test]
        public void LineTotal_CalculatedCorrectly()
        {
            decimal price = 12.50m;
            int quantity = 3;
            var item = new OrderItem(quantity, price);

            Assert.That(item.LineTotal, Is.EqualTo(37.50m), "Math is not correct");
        }

        [Test]
        public void Quantity_ZeroOrNegative_Exception()
        {
            Assert.Throws<ArgumentException>(() => new OrderItem(0, 20m));
            Assert.Throws<ArgumentException>(() => new OrderItem(-1, 20m));
        }

        [Test]
        public void UnitPrice_Negative_Exception()
        {
            Assert.Throws<ArgumentException>(() => new OrderItem(2, -20m));
        }

        [Test]
        public void Constructor_ShouldAddToExtent()
        {
            var item = new OrderItem(5, 10m);
            var list = OrderItem.GetExtent();

            Assert.That(list.Count, Is.EqualTo(1), "Object not added to extension");
            Assert.That(list[0].LineTotal, Is.EqualTo(50.0m));
        }

        [Test]
        public void SaveAndLoad_ShouldPersist()
        {
            new OrderItem(2, 30.0m);
            OrderItem.SaveExtent("orderitem_test.xml");

            OrderItem.ClearExtent();
            Assert.That(OrderItem.GetExtent().Count, Is.EqualTo(0), "memory not empty");

            OrderItem.LoadExtent("orderitem_test.xml");

            var list = OrderItem.GetExtent();
            Assert.That(list.Count, Is.EqualTo(1), "Object not loaded from the file");
            Assert.That(list[0].LineTotal, Is.EqualTo(60.0m), "Values after reading are wrong");
        }
    }
}