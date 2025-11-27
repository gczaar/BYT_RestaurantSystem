using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RestaurantSystem
{
    [Serializable]
    public class Reservation
    {
        private static List<Reservation> _extent = new List<Reservation>();

       
        private Guid _id;
        private string _customerName;
        private int _peopleCount;
        private int _phoneNumber;             
        private DateTime _reservationTime;    // complex attribute
        private string _specialRequests;      // optional

        

        /
        public Reservation()
        {
        }

        public Reservation(
            string customerName,
            int peopleCount,
            int phoneNumber,
            DateTime reservationTime,
            string specialRequests = null)
        {
            _id = Guid.NewGuid();
            CustomerName = customerName;
            PeopleCount = peopleCount;
            PhoneNumber = phoneNumber;
            ReservationTime = reservationTime;
            SpecialRequests = specialRequests;

            AddReservation(this);
        }


        public Guid Id
        {
            get => _id;
            private set => _id = value;
        }

        
        public string CustomerName
        {
            get => _customerName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Customer name cannot be empty.");
                }

                if (value.Length > 100)
                {
                    throw new ArgumentException("Customer name is too long.");
                }

                _customerName = value.Trim();
            }
        }

        
        public int PeopleCount
        {
            get => _peopleCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("People count must be at least 1.");
                }

                if (value > 20)
                {
                    throw new ArgumentException("People count is too large for a single table.");
                }

                _peopleCount = value;
            }
        }

        
        public int PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Phone number must be a positive number.");
                }

                
                if (value.Length != 9)  // has to be 9 digits
                {
                    throw new ArgumentException("Phone number too short or too long");
                }

                _phoneNumber = value;
            }
        }

        
        public DateTime ReservationTime
        {
            get => _reservationTime;
            set
            {
                if (value <= DateTime.Now)//must be in future
                {
                    throw new ArgumentException("Reservation time must be in the future.");
                }

                if (value > DateTime.Now.AddYears(1))//max 1 year form now
                {
                    throw new ArgumentException("Reservation time cannot be more than 1 year ahead.");
                }

                _reservationTime = value;
            }
        }

        // Optional attribute
        public string SpecialRequests
        {
            get => _specialRequests;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _specialRequests = null;
                    return;
                }

                if (value.Length > 500)
                {
                    throw new ArgumentException("Special requests text is too long.");
                }

                _specialRequests = value.Trim();
            }
        }

        // Derived attribute example
        public bool IsLargeGroup
        {
            get
            {
                return PeopleCount >= 6;
            }
        }

        

        private static void AddReservation(Reservation reservation)
        {
            if (reservation == null)
            {
                throw new ArgumentNullException(nameof(reservation));
            }

            _extent.Add(reservation);
        }

        public static IReadOnlyList<Reservation> GetExtent()
        {
            return _extent.AsReadOnly();
        }

        

        public static void SaveExtent(string filePath = "reservations.xml")
        {
            using (var writer = new StreamWriter(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<Reservation>));
                serializer.Serialize(writer, _extent);
            }
        }

        public static void LoadExtent(string filePath = "reservations.xml")
        {
            if (!File.Exists(filePath))
            {
                _extent.Clear();
                return;
            }

            using (var reader = new StreamReader(filePath))
            {
                var serializer = new XmlSerializer(typeof(List<Reservation>));

                try
                {
                    var loaded = (List<Reservation>)serializer.Deserialize(reader);
                    _extent = loaded ?? new List<Reservation>();
                }
                catch (Exception)
                {
                    _extent.Clear();
                }
            }
        }
    }
}
