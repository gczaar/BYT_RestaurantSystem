using System;
using NUnit.Framework;
using RestaurantSystem;

namespace RestaurantSystem.Tests
{
    [TestFixture]
    public class PaymentTest
    {
        private PaymentGateway _dummyGateway;

        [SetUp]
        public void Setup()
        {
            _dummyGateway = new PaymentGateway("TestGateway"); //set up a test gateway
        }

        [Test]
        public void NoAssociatedPaymentGateway() // testing in case of error in associating the payment gateway
        {
            Assert.That(() => new Payment("123", 50.00m, "CreditCard", null!),
                Throws.ArgumentNullException);
        }

        [Test]
        public void PaymentIdCannotBeEmpty() // exception in case of the empty paymentID
        {
            Assert.That(() => new Payment("", 50.00m, "CreditCard", _dummyGateway),
                Throws.ArgumentException.With.Message.EqualTo("Payment ID cannot be empty."));
        }

        [Test]
        public void AmountCannotBeZeroOrNegative() // exception in case when the amount of the payment is 0 or less than 0
        {
            Assert.That(() => new Payment("123", 00.00m, "CreditCard", _dummyGateway),
                Throws.ArgumentException.With.Message.EqualTo("Amount must be greater than zero."));
        }

        [Test]
        public void PaymentMethodRequired() // no specified payment method
        {
            Assert.That(() => new Payment("123", 50.00m, "", _dummyGateway),
                Throws.ArgumentException.With.Message.EqualTo("Payment method required."));
        }
        
        [Test]
        public void SuccessfulPaymentCreation() // test case assuming all of the arguments are filled peoperly
        {
            Assert.DoesNotThrow(() => new Payment("123", 100.00m, "Cash", _dummyGateway));
        }
    }
}