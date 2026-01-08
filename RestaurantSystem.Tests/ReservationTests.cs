using NUnit.Framework;
using System;
using RestaurantSystem;

namespace RestaurantSystem_Tests
{
    public class ReservationTests
    {
        private DateTime FutureTime()
        {
            return DateTime.Now.AddHours(2);
        }
    // clearing extent before starting the tests
        [SetUp]
        public void Setup()
        {
            Staff.ClearExtent();
            Table.ClearExtent();
            Reservation.ClearExtent();
        }

        [Test]
        public void Constructor_ValidData_CreatesReservation()
        {
            var time = FutureTime();

            var r = new Reservation(
                customerName: "Leo Messi",
                peopleCount: 4,
                phoneNumber: 123456789,
                reservationTime: time,
                specialRequests: "Birthday"
            );

            Assert.That(r.CustomerName, Is.EqualTo("Leo Messi"));
            Assert.That(r.PeopleCount, Is.EqualTo(4));
            Assert.That(r.PhoneNumber, Is.EqualTo(123456789));
            Assert.That(r.ReservationTime, Is.EqualTo(time));
            Assert.That(r.SpecialRequests, Is.EqualTo("Birthday"));
        }

        [Test]
        public void Constructor_PastReservationTime_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var r = new Reservation(
                    customerName: "Test",
                    peopleCount: 2,
                    phoneNumber: 55555,
                    reservationTime: DateTime.Now.AddHours(-1),  // invalid
                    specialRequests: null
                );
            });
        }

        [Test]
        public void SpecialRequests_EmptyString_SetsNull()
        {
            var r = new Reservation(
                customerName: "Anna",
                peopleCount: 3,
                phoneNumber: 333444555,
                reservationTime: FutureTime(),
                specialRequests: ""     // empty
            );

            Assert.That(r.SpecialRequests, Is.Null);
        }

        // qualified association tests

        [Test]
        public void assignTable_ValidTable_AddsToCollectionAndRetrievableByQualifier()
        {
            var reservation = new Reservation(
                customerName: "John Doe",
                peopleCount: 4,
                phoneNumber: 123456789,
                reservationTime: DateTime.Now.AddDays(1)
            );
            var table = new Table(tableId: 5, capacity: 6);

            reservation.assignTable(table);

            Assert.That(reservation.AssignedTables.Count, Is.EqualTo(1));
            Assert.That(reservation.getTableById(5), Is.SameAs(table));
            Assert.That(reservation.AssignedTables.ContainsKey(5), Is.True);
        }

        [Test]
        public void assignTable_DuplicateTableId_ThrowsInvalidOperationException()
        {
            var reservation = new Reservation(
                customerName: "Jane Smith",
                peopleCount: 2,
                phoneNumber: 987654321,
                reservationTime: DateTime.Now.AddDays(2)
            );
            var table1 = new Table(tableId: 10, capacity: 4);
            reservation.assignTable(table1);

            Table.ClearExtent();
            var table2 = new Table(tableId: 10, capacity: 8);

            var ex = Assert.Throws<InvalidOperationException>(() => reservation.assignTable(table2));
            Assert.That(ex!.Message, Does.Contain("already assigned"));
        }

        [Test]
        public void unassignTable_NonExistentTableId_ThrowsInvalidOperationException()
        {
            var reservation = new Reservation(
                customerName: "Bob Wilson",
                peopleCount: 3,
                phoneNumber: 111222333,
                reservationTime: DateTime.Now.AddDays(3)
            );
            var table = new Table(tableId: 7, capacity: 4);
            reservation.assignTable(table);

            var ex = Assert.Throws<InvalidOperationException>(() => reservation.unassignTable(999));
            Assert.That(ex!.Message, Does.Contain("not assigned"));
        }
    }
}
