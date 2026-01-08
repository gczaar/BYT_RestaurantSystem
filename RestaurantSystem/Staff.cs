using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RestaurantSystem
{
    [Serializable]
    public class Staff
    {
        private static List<Staff> _extent = new List<Staff>();

        private static decimal _minimumWage = 30.0m;
        private string _fullName;
        private string _role;
        private string _email;
        private List<string> _spokenLanguages = new List<string>();

        // Reflex association (Staff <- > Staff)
        // manager: 0..1, subordinates: 0..*
        // XmlIgnore prevents circular reference issues in XmlSerializer
        [XmlIgnore]
        private Staff? _manager;

        [XmlIgnore]
        private readonly HashSet<Staff> _subordinates = new HashSet<Staff>();

        public Staff() { }

        public Staff(string fullName, string role)
        {
            FullName = fullName;
            Role = role;

            AddStaff(this);
        }

        // Basic attributess

        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name and Surname Cannot be Empty.");
                _fullName = value.Trim();
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Role cannot be empty.");
                _role = value.Trim();
            }
        }

        // Optional attribute
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _email = null;
                    return;
                }

                if (!value.Contains("@"))
                    throw new ArgumentException("Enter an appropriate email");

                _email = value.Trim();
            }
        }

        // Multi-value attribute
        public List<string> SpokenLanguages
        {
            get => _spokenLanguages;
            set => _spokenLanguages = value ?? new List<string>();
        }

        // Static attribute
        public static decimal MinimumWage
        {
            get => _minimumWage;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Wage cannot be negative.");
                _minimumWage = value;
            }
        }
        
        // Reflex association 

        // 0..1
        public Staff? Manager => _manager;

        // 0..*
        public IReadOnlyCollection<Staff> Subordinates => _subordinates;


        public void SetManager(Staff? newManager)
        {
            if (newManager == this)
                throw new ArgumentException("A staff member cannot be their own manager.");

            if (_manager == newManager)
                return; // no change

            // Remove from old manager's subordinates
            if (_manager != null)
            {
                _manager.RemoveSubordinateInternal(this);
            }

            // Set new manager
            _manager = newManager;

            // Add to new manager's subordinates
            if (_manager != null)
            {
                _manager.AddSubordinateInternal(this);
            }
        }

        public void RemoveManager()
        {
            SetManager(null);
        }

        // menager adds subordinate (reverse handled internally)
        public void AddSubordinate(Staff subordinate)
        {
            if (subordinate == null) throw new ArgumentNullException(nameof(subordinate));
            if (subordinate == this) throw new ArgumentException("Cannot add self as subordinate.");

            // if subordinate already has this manager, dont change
            if (subordinate._manager == this) return;

            // this will also update _subordinates via SetManager()
            subordinate.SetManager(this);
        }

        public void RemoveSubordinate(Staff subordinate)
        {
            if (subordinate == null) throw new ArgumentNullException(nameof(subordinate));

            if (!_subordinates.Contains(subordinate))
                throw new InvalidOperationException("This staff member is not a subordinate.");

            // This will also remove from _subordinates via SetManager()
            subordinate.SetManager(null);
        }

        // Internal helpers to avoid recursion loops
        private void AddSubordinateInternal(Staff subordinate)
        {
            _subordinates.Add(subordinate); // HashSet prevents duplicates
        }

        private void RemoveSubordinateInternal(Staff subordinate)
        {
            _subordinates.Remove(subordinate);
        }

        // Extent
        private static void AddStaff(Staff staff)
        {
            if (staff != null) _extent.Add(staff);
        }

        public static IReadOnlyList<Staff> GetExtent() => _extent.AsReadOnly();

        public static void SaveExtent(string filePath = "staff.xml")
        {
            using (var writer = new StreamWriter(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<Staff>));
                serializer.Serialize(writer, _extent);
            }
        }

        public static void LoadExtent(string filePath = "staff.xml")
        {
            if (!File.Exists(filePath)) return;

            using (var reader = new StreamReader(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<Staff>));
                try
                {
                    _extent = (List<Staff>)serializer.Deserialize(reader)!;
                }
                catch
                {
                    _extent.Clear();
                }
            }
        }

        public static void ClearExtent()
        {
            _extent.Clear();
        }
    }
}
