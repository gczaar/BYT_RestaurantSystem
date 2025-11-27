using NUnit.Framework;
using RestaurantSystem;
using System;
using System.IO;

namespace RestaurantSystem.Tests
{
    public class StaffTests
    {
        [SetUp]
        public void Setup()
        {
            if (File.Exists("staff_test.xml"))
                File.Delete("staff_test.xml");
            
            Staff.ClearExtent(); 
        }

        [Test]
        public void Constructor_Validation()
        {
            var worker = new Staff("John Doe", "Chef");
            var list = Staff.GetExtent();

            Assert.That(list.Count, Is.EqualTo(1), "list should contain 1 worker");
            Assert.That(list[0].FullName, Is.EqualTo("John Doe"), "The name should be correct");
        }

        [Test]
        public void FullName_ThrowExceptionWhenNull()
        {
            Assert.Throws<ArgumentException>(() => new Staff("", "Chef"));
            Assert.Throws<ArgumentException>(() => new Staff(null!, "Chef")); 
        }

        [Test]
        public void Role_ThrowExceptionWhenNull()
        {
            Assert.Throws<ArgumentException>(() => new Staff("John", ""));
            Assert.Throws<ArgumentException>(() => new Staff("John", null!));
        }

        [Test]
        public void Email_Validation()
        {
            var s = new Staff("John", "Chef");

            s.Email = "John@restaurant.pl";
            Assert.That(s.Email, Is.EqualTo("John@restaurant.pl"));

            Assert.Throws<ArgumentException>(() => s.Email = "email");

            s.Email = null;
            Assert.That(s.Email, Is.Null);
        }

        [Test]
        public void MinimumWage_ChangeStatic()
        {
            Staff.MinimumWage = 45.50m;
            Assert.That(Staff.MinimumWage, Is.EqualTo(45.50m));
            Assert.Throws<ArgumentException>(() => Staff.MinimumWage = -10);
        }

        [Test]
        public void SaveAndLoad_ShouldPersist()
        {

            new Staff("JohnTest", "ChefTest");

            Staff.SaveExtent("staff_test.xml");


            Staff.ClearExtent();
            Assert.That(Staff.GetExtent().Count, Is.EqualTo(0), "memory should be empty before loading");


            Staff.LoadExtent("staff_test.xml");

            var loadedList = Staff.GetExtent();
            Assert.That(loadedList.Count, Is.EqualTo(1));
            Assert.That(loadedList[0].FullName, Is.EqualTo("JohnTest"));
            Assert.That(loadedList[0].Role, Is.EqualTo("ChefTest"));
        }

        [Test]
        public void LoadExtent_CorruptedFile()
        {
            File.WriteAllText("broken.xml", "this aint no xml");
            
            Staff.LoadExtent("broken.xml"); 

            Assert.That(Staff.GetExtent().Count, Is.EqualTo(0));
            File.Delete("broken.xml");
        }
    }
}