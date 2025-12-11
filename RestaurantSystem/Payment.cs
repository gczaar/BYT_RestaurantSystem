using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RestaurantSystem
{
    [Serializable]
    public class Payment
    {
        private static List<Payment> _extent = new List<Payment>();

        // Initializing a payment with default values
        private string _paymentId = string.Empty;
        private decimal _amount;
        private string _status = "Pending";
        private string _method = string.Empty;

        public Payment(string paymentId, decimal amount, string method, PaymentGateway gateway)
        {
            PaymentId = paymentId;
            Amount = amount;
            Method = method;
            
            // Set initial state
            PaymentTime = DateTime.Now; // current date 
            Status = "Pending"; 
            RefundTime = DateTime.MinValue; // Default 'empty' date


            AssociatedGateway = gateway ?? throw new ArgumentNullException(nameof(gateway));

            AddPayment(this); // adding the payment to the extent class
        }

        public string PaymentId
        {
            get => _paymentId;
            set
            { // exception in case of empty payment ID
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Payment ID cannot be empty.");
                _paymentId = value;
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Amount must be greater than zero.");
                _amount = value;
            }
        }

        public DateTime PaymentTime { get; set; }
        
        public string Status
        {
            get => _status;
            set => _status = value; 
        }

        public string Method
        {
            get => _method;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Payment method required.");
                _method = value;
            }
        }


        public DateTime RefundTime { get; set; }
        
        [XmlIgnore]
        public PaymentGateway AssociatedGateway { get; set; }



        public void Authorize()
        {
            if (Status != "Pending")
            {
                AssociatedGateway.NotifyFailure($"Cannot authorize payment {PaymentId}. Current status: {Status}");
                return;
            }
            
            Status = "Authorized";
            Console.WriteLine($"Payment {PaymentId} Authorized.");
        }

        public void Capture()
        {
            if (Status != "Authorized")
            {
                AssociatedGateway.NotifyFailure($"Cannot capture payment {PaymentId}. It must be Authorized first.");
                return;
            }


            AssociatedGateway.ProcessPayment(this);
            Status = "Captured";
            Console.WriteLine($"Payment {PaymentId} Captured.");
        }

        public void Refund()
        {
            if (Status != "Captured")
            {
                AssociatedGateway.NotifyFailure($"Cannot refund payment {PaymentId}. It has not been captured.");
                return;
            }

            Status = "Refunded";
            RefundTime = DateTime.Now;
            Console.WriteLine($"Payment {PaymentId} Refunded at {RefundTime}.");
        }


        private static void AddPayment(Payment payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));
            _extent.Add(payment);
        }

        public static IReadOnlyList<Payment> GetExtent()
        {
            return _extent.AsReadOnly();
        }

        public static void SaveExtent(string filePath = "payments.xml")
        {
            using (var writer = new StreamWriter(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<Payment>));
                serializer.Serialize(writer, _extent);
            }
        }
        //Reading the file 
        public static void LoadExtent(string filePath = "payments.xml")
        {
            if (!File.Exists(filePath)) return;

            using (var reader = new StreamReader(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<Payment>));
                try
                {
                    _extent = (List<Payment>)serializer.Deserialize(reader)!;
                }
                catch (Exception)
                {
                    _extent.Clear();
                }
            }
        }
    }
}