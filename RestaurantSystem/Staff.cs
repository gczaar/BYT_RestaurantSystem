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

        public Staff() { }

        public Staff(string fullName, string role)
        {
            FullName = fullName;
            Role = role;

            AddStaff(this);
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) // Validation - Can't be empty
                    throw new ArgumentException("Name and Surname Cannot be Empty.");
                _fullName = value;
            }

        }

        public string Role
        {
            get => _role;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) // Validation - Can't be empty
                    throw new ArgumentException("Role cannot be empty.");
                _role = value;
            }
        }
        
        public string Email // Optional attribute
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) // If empty - set to null
                {
                    _email = null;
                }
                else
                {
                if (!value.Contains("@"))
                    throw new ArgumentException("Enter an appropriate email"); // If doesn't contain an '@' sign it cannot be an email
                _email = value;
                }
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

        private static void AddStaff(Staff staff)
        {
            if (staff != null) _extent.Add(staff);
        }

        public static IReadOnlyList<Staff> GetExtent() => _extent.AsReadOnly();

        // Saving the list to an XML file
        public static void SaveExtent(string filePath = "staff.xml")
        {
            using (var writer = new StreamWriter(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<Staff>));
                serializer.Serialize(writer, _extent);
            }
        }

        // Reading the file
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