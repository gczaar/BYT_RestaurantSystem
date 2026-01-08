using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantSystem
{
    public class Menu
    {
        public string MenuId { get; }
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        // minimum 1 elemnt (1..*)
        private readonly HashSet<MenuItem> _items = new HashSet<MenuItem>();

        public Menu(string menuId, string name, bool isActive, IEnumerable<MenuItem> initialItems)
        {
            MenuId = menuId ?? throw new ArgumentNullException(nameof(menuId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsActive = isActive;

            if (initialItems == null) throw new ArgumentNullException(nameof(initialItems));
            var list = initialItems.ToList();
            if (list.Count == 0)
                throw new InvalidOperationException("Menu must contain at least 1 MenuItem (multiplicity 1..*).");

            foreach (var item in list)
            {
                AddItem(item);
            }
        }

        public IReadOnlyCollection<MenuItem> GetItems() => _items.ToList().AsReadOnly();

        public void AddItem(MenuItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (_items.Contains(item))
                throw new InvalidOperationException("Duplicate MenuItem reference is not allowed.");

            if (item.HasMenu && !ReferenceEquals(item.Menu, this))
                throw new InvalidOperationException("MenuItem is already assigned to a different Menu.");

            _items.Add(item);

            // reverse connection
            item.SetMenuInternal(this);
        }

        public void RemoveItem(MenuItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (!_items.Contains(item))
                throw new InvalidOperationException("Cannot remove MenuItem that is not associated with this Menu.");

            // ensuring minimum multiplicity
            if (_items.Count == 1)
                throw new InvalidOperationException("Cannot remove the last MenuItem (minimum multiplicity 1..*).");

            _items.Remove(item);

            // reverse connection
            item.ClearMenuInternal(this);
        }

        public MenuItem? FindItemByName(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return _items.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        internal bool ContainsInternal(MenuItem item) => _items.Contains(item);

        internal void AddItemInternal(MenuItem item)
        {
            if (!_items.Add(item))
                throw new InvalidOperationException("Duplicate MenuItem reference is not allowed.");
        }

        internal void RemoveItemInternal(MenuItem item)
        {
            _items.Remove(item);
        }
    }
}
