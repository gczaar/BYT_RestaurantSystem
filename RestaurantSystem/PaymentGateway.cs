using System;

namespace RestaurantSystem
{
    public class PaymentGateway
    {

        private string _gatewayName;


        public PaymentGateway(string gatewayName)
        {
            GatewayName = gatewayName;
        }

        public string GatewayName
        {
            get => _gatewayName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Gateway name cannot be empty.");
                _gatewayName = value;
            }
        }

        public void ProcessPayment(Payment payment)
        {
            if (payment == null) throw new ArgumentNullException(nameof(payment));

            Console.WriteLine($"[Gateway: {GatewayName}] Processing payment {payment.PaymentId}...");            
        }

        public void NotifyFailure(string message)
        {
            Console.WriteLine($"[Gateway: {GatewayName}] FAILURE NOTICE: {message}");
        }
    }
}