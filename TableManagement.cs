using System;
using System.Collections.Generic;
using System.Linq;

public class Table
{
    public int TableNumber { get; set; }
    public int Capacity { get; set; }
    public bool IsOccupied { get; set; }
    public string Status { get; set; } // "Available", "Occupied", etc.

    public Table(int tableNumber, int capacity)
    {
        TableNumber = tableNumber;
        Capacity = capacity;
        IsOccupied = false;
        Status = "Available";
    }

    public void UpdateStatus(string status)
    {
        Status = status;
        IsOccupied = status == "Occupied";
    }
}

public class Reservation
{
    public int ReservationId { get; set; }
    public DateTime ReservationTime { get; set; }
    public int NumberOfGuests { get; set; }
    public string CustomerName { get; set; }

    public Reservation(int id, DateTime time, int guests, string name)
    {
        ReservationId = id;
        ReservationTime = time;
        NumberOfGuests = guests;
        CustomerName = name;
    }
}

public class WaitlistEntry
{
    public string CustomerName { get; set; }
    public DateTime AddedTime { get; set; }
    public int EstimatedWaitTimeMinutes { get; set; }

    public WaitlistEntry(string name, int waitTime)
    {
        CustomerName = name;
        AddedTime = DateTime.Now;
        EstimatedWaitTimeMinutes = waitTime;
    }
}

public class Waitstaff
{
    private List<Table> tables;
    private List<Reservation> reservations;
    private List<WaitlistEntry> waitlist;

    public Waitstaff(List<Table> tables, List<Reservation> reservations)
    {
        this.tables = tables;
        this.reservations = reservations;
        this.waitlist = new List<WaitlistEntry>();
    }

    public void VerifyReservation(int reservationId)
    {
        var reservation = reservations.FirstOrDefault(r => r.ReservationId == reservationId);
        if (reservation != null)
        {
            Console.WriteLine($"Reservation verified for {reservation.CustomerName} at {reservation.ReservationTime}");
        }
        else
        {
            Console.WriteLine("Reservation not found.");
        }
    }

    public Table CheckTableAvailability(int requiredCapacity)
    {
        var availableTable = tables.FirstOrDefault(t => t.Capacity >= requiredCapacity && t.Status == "Available");
        return availableTable;
    }

    public void AssignTable(Table table, string customerName)
    {
        if (table != null && !table.IsOccupied)
        {
            table.UpdateStatus("Occupied");
            Console.WriteLine($"Table {table.TableNumber} assigned to {customerName}.");
        }
        else
        {
            Console.WriteLine($"No available tables. Adding {customerName} to the waitlist.");
            AddToWaitlist(customerName);
        }
    }

    public void AddToWaitlist(string customerName)
    {
        int estimatedWaitTime = CalculateEstimatedWaitTime();
        waitlist.Add(new WaitlistEntry(customerName, estimatedWaitTime));
        Console.WriteLine($"{customerName} added to the waitlist. Estimated wait time: {estimatedWaitTime} minutes.");
    }

    public int CalculateEstimatedWaitTime()
    {
        // Assume each table takes an average of 30 minutes per seating
        int averageWaitTimePerTable = 30;
        int occupiedTables = tables.Count(t => t.IsOccupied);
        return occupiedTables * averageWaitTimePerTable;
    }

    public void HandleTableConflict(string customerName)
    {
        var otherAvailableTable = tables.FirstOrDefault(t => t.Status == "Available");
        if (otherAvailableTable != null)
        {
            Console.WriteLine($"Conflict resolved. Assigning {customerName} to Table {otherAvailableTable.TableNumber}.");
            AssignTable(otherAvailableTable, customerName);
        }
        else
        {
            Console.WriteLine($"Unable to resolve conflict. Informing {customerName} of table availability issue.");
            AddToWaitlist(customerName);
        }
    }

    public void SeatCustomer(Table table, string customerName)
    {
        if (table != null && !table.IsOccupied)
        {
            table.UpdateStatus("Occupied");
            Console.WriteLine($"Customer {customerName} seated at table {table.TableNumber}");
        }
        else
        {
            Console.WriteLine("Table is already occupied or unavailable.");
            HandleTableConflict(customerName);
        }
    }

    public void FreeTable(Table table)
    {
        if (table != null)
        {
            table.UpdateStatus("Available");
            Console.WriteLine($"Table {table.TableNumber} is now available.");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // Sample tables and reservations setup
        var tables = new List<Table>
        {
            new Table(1, 4),
            new Table(2, 2),
            new Table(3, 6)
        };
        var reservations = new List<Reservation>
        {
            new Reservation(101, DateTime.Now.AddMinutes(30), 2, "John Doe")
        };

        var waitstaff = new Waitstaff(tables, reservations);

        // Trigger reservation verification
        waitstaff.VerifyReservation(101);

        // Check table availability
        var availableTable = waitstaff.CheckTableAvailability(2);

        // Assign the table and seat the customer
        if (availableTable != null)
        {
            waitstaff.AssignTable(availableTable, "John Doe");
            waitstaff.SeatCustomer(availableTable, "John Doe");
        }
        else
        {
            waitstaff.AddToWaitlist("John Doe");
        }

        // Free up the table after customer finishes dining
        waitstaff.FreeTable(availableTable);
    }
}