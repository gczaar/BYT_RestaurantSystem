using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantSystem
{
    public class Order
    {
        public string OrderId { get; }
        public DateTime CreatedAt { get; }
        public string Status { get; private set; }

        // Composition
        private readonly HashSet<OrderItem> _items = new HashSet<OrderItem>();

        public Order(string orderId, DateTime createdAt, string status)
        {
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            CreatedAt = createdAt;
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }

        public IReadOnlyCollection<OrderItem> GetItems()
        {
            return _items.ToList().AsReadOnly();
        }

        // derived attribute
        public decimal TotalAmount => _items.Sum(i => i.LineTotal);

        public OrderItem AddItem(MenuItem menuItem, int quantity, decimal unitPrice)
        {
            if (menuItem == null) throw new ArgumentNullException(nameof(menuItem));
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            if (unitPrice < 0) throw new ArgumentException("Price cannot be negative.", nameof(unitPrice));

            var item = new OrderItem(quantity, unitPrice);

            if (item.HasOrder && !ReferenceEquals(item.Order, this))
                throw new InvalidOperationException("OrderItem cannot be associated with multiple Orders (composition).");

            if (!_items.Add(item))
                throw new InvalidOperationException("Duplicate OrderItem reference is not allowed.");

            item.SetOrderInternal(this);
            item.SetMenuItemInternal(menuItem);

            return item;
        }

        public void RemoveItem(OrderItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (!_items.Contains(item))
                throw new InvalidOperationException("Cannot remove OrderItem that is not part of this Order.");
            // ensuring minimum multiplicity
            if (_items.Count == 1)
                throw new InvalidOperationException("Cannot remove the last OrderItem (minimum multiplicity 1..*).");

            _items.Remove(item);

            item.ClearMenuItemInternal(item.MenuItem); 
            item.ClearOrderInternal(this);
        }

        public void Delete()
        {
            foreach (var item in _items.ToList())
            {
                _items.Remove(item);

                if (item.HasMenuItem)
                    item.ClearMenuItemInternal(item.MenuItem);

                if (item.HasOrder)
                    item.ClearOrderInternal(this);
            }
        }
    }
}
