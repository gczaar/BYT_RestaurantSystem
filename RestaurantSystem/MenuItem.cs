using System;

namespace RestaurantSystem
{
    public class MenuItem
    {
        public string ItemId { get; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal BasePrice { get; private set; }
        public string Category { get; private set; }
        public bool IsAvailable { get; private set; }

        private Menu? _menu;

        public bool HasMenu => _menu != null;
        public Menu Menu => _menu ?? throw new InvalidOperationException("MenuItem is not assigned to any Menu.");

        public MenuItem(string itemId, string name, string description, decimal basePrice, string category, bool isAvailable)
        {
            ItemId = itemId ?? throw new ArgumentNullException(nameof(itemId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            BasePrice = basePrice;
            Category = category ?? throw new ArgumentNullException(nameof(category));
            IsAvailable = isAvailable;
        }

        public void ChangePrice(decimal newPrice)
        {
            if (newPrice < 0) throw new ArgumentOutOfRangeException(nameof(newPrice));
            BasePrice = newPrice;
        }

        public void MarkUnavailable() => IsAvailable = false;
        public void MarkAvailable() => IsAvailable = true;

        // Reverse connection – TYLKO do użycia przez Menu
        internal void SetMenuInternal(Menu menu)
        {
            if (menu == null) throw new ArgumentNullException(nameof(menu));

            if (_menu != null && !ReferenceEquals(_menu, menu))
                throw new InvalidOperationException("MenuItem cannot be assigned to multiple Menus.");

            if (_menu == null)
                _menu = menu;

            if (!menu.ContainsInternal(this))
                menu.AddItemInternal(this);
        }

        internal void ClearMenuInternal(Menu menu)
        {
            if (menu == null) throw new ArgumentNullException(nameof(menu));
            if (_menu == null) return;

            if (!ReferenceEquals(_menu, menu))
                throw new InvalidOperationException("Inconsistent reverse connection (Menu mismatch).");

            _menu = null;

            if (menu.ContainsInternal(this))
                menu.RemoveItemInternal(this);
        }
    }
}
