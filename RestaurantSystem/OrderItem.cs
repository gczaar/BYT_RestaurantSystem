using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RestaurantSystem
{
    [Serializable]
    public class OrderItem
    {
        private static List<OrderItem> _extent = new List<OrderItem>();

        private int _quantity;
        private decimal _unitPrice;

        public OrderItem() { }

        public OrderItem(int quantity, decimal unitPrice)
        {
            Quantity = quantity;
            UnitPrice = unitPrice;
            
            AddOrderItem(this);
        }

        public int Quantity
        {
            get => _quantity;
            set
            { // exception in case of the empty order
                if (value <= 0)
                    throw new ArgumentException("Quantity must be greater than zero.");
                _quantity = value;
            }
        }

        public decimal UnitPrice
        {
            get => _unitPrice;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Price cannot be negative.");
                _unitPrice = value;
            }
        }

        public decimal LineTotal
        {
            get => _quantity * _unitPrice;
        }


        private static void AddOrderItem(OrderItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _extent.Add(item);
        }

        public static IReadOnlyList<OrderItem> GetExtent()
        {
            return _extent.AsReadOnly();
        }

        public static void SaveExtent(string filePath = "orderitems.xml")
        {
            using (var writer = new StreamWriter(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<OrderItem>));
                serializer.Serialize(writer, _extent);
            }
        }

        public static void LoadExtent(string filePath = "orderitems.xml")
        {
            if (!File.Exists(filePath)) return;

            using (var reader = new StreamReader(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<OrderItem>));
                try
                {
                    _extent = (List<OrderItem>)serializer.Deserialize(reader);
                }
                catch (Exception)
                {
                    _extent.Clear();
                }
            }
        }
    }
}