using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RestaurantSystem
{
    [Serializable]
    public class Table
    {
        private static List<Table> _extent = new List<Table>();

        private int _tableId;
        private int _capacity;
        private bool _isOccupied;

        public Table() { }

        public Table(int tableId, int capacity)
        {
            TableId = tableId;
            Capacity = capacity;
            _isOccupied = false;

            AddTable(this);
        }

        public int TableId
        {
            get => _tableId;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Table ID must be a positive number.");
                _tableId = value;
            }
        }

        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value < 1)
                    throw new ArgumentException("Capacity must be at least 1.");
                if (value > 20)
                    throw new ArgumentException("Capacity cannot exceed 20.");
                _capacity = value;
            }
        }

        public bool IsOccupied => _isOccupied;

        public void markOccupied()
        {
            if (_isOccupied)
                throw new InvalidOperationException("Table is already occupied.");
            _isOccupied = true;
        }

        public void markFree()
        {
            if (!_isOccupied)
                throw new InvalidOperationException("Table is already free.");
            _isOccupied = false;
        }

        // Extent management
        private static void AddTable(Table table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            
            // Check for duplicate table ID
            foreach (var t in _extent)
            {
                if (t.TableId == table.TableId)
                    throw new InvalidOperationException($"Table with ID {table.TableId} already exists.");
            }
            
            _extent.Add(table);
        }

        public static IReadOnlyList<Table> GetExtent() => _extent.AsReadOnly();

        public static void ClearExtent()
        {
            _extent.Clear();
        }

        public static void SaveExtent(string filePath = "tables.xml")
        {
            using (var writer = new StreamWriter(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<Table>));
                serializer.Serialize(writer, _extent);
            }
        }

        public static void LoadExtent(string filePath = "tables.xml")
        {
            if (!File.Exists(filePath)) return;

            using (var reader = new StreamReader(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<Table>));
                try
                {
                    _extent = (List<Table>)serializer.Deserialize(reader)!;
                }
                catch
                {
                    _extent.Clear();
                }
            }
        }
    }
}
