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
    }
}

