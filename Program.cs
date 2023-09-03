using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace parkinglot_net
{

    class Program
    {
        static void Main(string[] args)
        {
            ParkingLot parkingLot = null;

            while (true)
            {
                Console.Write("Enter command: ");
                string input = Console.ReadLine();
                string[] tokens = input.Split(' ');

                if (tokens.Length == 0)
                    continue;

                string command = tokens[0].ToLower();

                switch (command)
                {
                    case "create_parking_lot":
                        int capacity = int.Parse(tokens[1]);
                        parkingLot = new ParkingLot(capacity);
                        Console.WriteLine($"Created a parking lot with {capacity} slots");
                        break;

                    case "park":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Please create a parking lot first.");
                            continue;
                        }

                        string registrationNumber = tokens[1];
                        string color = tokens[2];
                        string vehicleType = tokens[3];
                        Vehicle vehicle = new Vehicle(registrationNumber, color, vehicleType);

                        int allocatedSlot = parkingLot.ParkVehicle(vehicle);
                        Console.WriteLine($"Allocated slot number: {allocatedSlot}");
                        break;

                    case "leave":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Please create a parking lot first.");
                            continue;
                        }

                        int slotNumberToFree = int.Parse(tokens[1]);
                        parkingLot.FreeSlot(slotNumberToFree);
                        Console.WriteLine($"Slot number {slotNumberToFree} is free");
                        break;

                    case "status":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Please create a parking lot first.");
                            continue;
                        }

                        parkingLot.PrintStatus();
                        break;

                    case "type_of_vehicles":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Please create a parking lot first.");
                            continue;
                        }

                        string requestedType = tokens[1];
                        int count = parkingLot.CountVehiclesByType(requestedType);
                        Console.WriteLine(count);
                        break;

                    case "registration_numbers_for_vehicles_with_odd_plate":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Please create a parking lot first.");
                            continue;
                        }

                        var oddPlateNumbers = parkingLot.GetOddPlateNumbers();
                        Console.WriteLine(string.Join(", ", oddPlateNumbers));
                        break;

                    case "registration_numbers_for_vehicles_with_even_plate":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Please create a parking lot first.");
                            continue;
                        }

                        var evenPlateNumbers = parkingLot.GetEvenPlateNumbers();
                        Console.WriteLine(string.Join(", ", evenPlateNumbers));
                        break;

                    case "registration_numbers_for_vehicles_with_colour":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Please create a parking lot first.");
                            continue;
                        }

                        string requestedColor = tokens[1];
                        var vehiclesWithColor = parkingLot.GetVehiclesByColor(requestedColor);
                        Console.WriteLine(string.Join(", ", vehiclesWithColor.Select(v => v.RegistrationNumber)));
                        break;

                    case "slot_numbers_for_vehicles_with_colour":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Please create a parking lot first.");
                            continue;
                        }

                        string colorToFind = tokens[1];
                        var slotsWithColor = parkingLot.GetSlotsByColor(colorToFind);
                        Console.WriteLine(string.Join(", ", slotsWithColor));
                        break;

                    case "slot_number_for_registration_number":
                        if (parkingLot == null)
                        {
                            Console.WriteLine("Please create a parking lot first.");
                            continue;
                        }

                        string registrationToFind = tokens[1];
                        int slotNumber = parkingLot.GetSlotNumberByRegistration(registrationToFind);
                        if (slotNumber != -1)
                            Console.WriteLine(slotNumber);
                        else
                            Console.WriteLine("Not found");
                        break;

                    case "exit":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid command.");
                        break;
                }
            }
        }
    }

    class ParkingLot
    {
        private int capacity;
        private List<Vehicle> slots;

        public ParkingLot(int capacity)
        {
            this.capacity = capacity;
            this.slots = new List<Vehicle>(capacity);
        }

        public int ParkVehicle(Vehicle vehicle)
        {
            if (IsFull())
            {
                return -1;
            }

            int slotNumber = GetNextAvailableSlot();
            slots.Add(vehicle);
            return slotNumber;
        }

        public void FreeSlot(int slotNumber)
        {
            if (IsValidSlot(slotNumber))
            {
                slots[slotNumber - 1] = null;
            }
        }

        public void PrintStatus()
        {
            Console.WriteLine("Slot\tNo.\tType\tRegistration No\tColour");
            for (int i = 0; i < capacity; i++)
            {
                var vehicle = slots[i];
                if (vehicle != null)
                {
                    Console.WriteLine($"{i + 1}\t{vehicle.RegistrationNumber}\t{vehicle.VehicleType}\t{vehicle.Color}");
                }
            }
        }

        public int CountVehiclesByType(string type)
        {
            return slots.Count(vehicle => vehicle != null && vehicle.VehicleType.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        public List<string> GetOddPlateNumbers()
        {
            return slots
                .Where(vehicle => vehicle != null && IsOddPlate(vehicle.RegistrationNumber))
                .Select(vehicle => vehicle.RegistrationNumber)
                .ToList();
        }

        public List<string> GetEvenPlateNumbers()
        {
            return slots
                .Where(vehicle => vehicle != null && IsEvenPlate(vehicle.RegistrationNumber))
                .Select(vehicle => vehicle.RegistrationNumber)
                .ToList();
        }

        public List<Vehicle> GetVehiclesByColor(string color)
        {
            return slots
                .Where(vehicle => vehicle != null && vehicle.Color.Equals(color, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<int> GetSlotsByColor(string color)
        {
            return slots
                .Select((vehicle, index) => new { Vehicle = vehicle, Index = index + 1 })
                .Where(item => item.Vehicle != null && item.Vehicle.Color.Equals(color, StringComparison.OrdinalIgnoreCase))
                .Select(item => item.Index)
                .ToList();
        }

        public int GetSlotNumberByRegistration(string registrationNumber)
        {
            for (int i = 0; i < capacity; i++)
            {
                var vehicle = slots[i];
                if (vehicle != null && vehicle.RegistrationNumber.Equals(registrationNumber, StringComparison.OrdinalIgnoreCase))
                {
                    return i + 1;
                }
            }
            return -1;
        }

        private int GetNextAvailableSlot()
        {
            for (int i = 0; i < capacity; i++)
            {
                if (slots[i] == null)
                {
                    return i + 1;
                }
            }
            return -1;
        }

        private bool IsValidSlot(int slotNumber)
        {
            return slotNumber >= 1 && slotNumber <= capacity;
        }

        private bool IsFull()
        {
            return slots.All(vehicle => vehicle != null);
        }

        private bool IsOddPlate(string registrationNumber)
        {
            char lastDigitChar = registrationNumber[registrationNumber.Length - 1];

            if (int.TryParse(lastDigitChar.ToString(), out int lastDigit))
            {
                return lastDigit % 2 != 0;
            }

            return false;
        }

        private bool IsEvenPlate(string registrationNumber)
        {
            char lastDigitChar = registrationNumber[registrationNumber.Length - 1];

            if (int.TryParse(lastDigitChar.ToString(), out int lastDigit))
            {
                return lastDigit % 2 == 0;
            }

            return false;
        }
    }

    class Vehicle
    {
        public string RegistrationNumber { get; }
        public string Color { get; }
        public string VehicleType { get; }

        public Vehicle(string registrationNumber, string color, string vehicleType)
        {
            RegistrationNumber = registrationNumber;
            Color = color;
            VehicleType = vehicleType;
        }
    }

}
