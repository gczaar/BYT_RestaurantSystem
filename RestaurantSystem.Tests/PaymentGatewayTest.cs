using NUnit.Framework;
using RestaurantSystem;
using System;

namespace RestaurantSystem.Tests
{
    [TestFixture]
    public class PaymentGatewayTest
    {
        [Test]
        public void GatewayName_WhenEmptyOrWhitespace_ThrowsArgumentException() 
        {
            Assert.That(() => new PaymentGateway(""), // tests for PaymentGateway being empty
                Throws.ArgumentException.With.Message.EqualTo("Gateway name cannot be empty."));

            Assert.That(() => new PaymentGateway("   "), //tests fot PaymentGateway being whitespace
                Throws.ArgumentException.With.Message.EqualTo("Gateway name cannot be empty."));
        }

        [Test]
        public void ProcessPayment_WhenPaymentIsNull_ThrowsArgumentNullException()
        {
            var gateway = new PaymentGateway("Stripe");

            Assert.That(() => gateway.ProcessPayment(null!), 
                Throws.ArgumentNullException);
        }
    }
}