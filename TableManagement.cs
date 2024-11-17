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
            Console.WriteLine($"Reservation verified for {reservation.CustomerName} at {reservation.ReservationTime}.");
        }
        else
        {
            Console.WriteLine("Reservation not found.");
        }
    }

    public Table CheckTableAvailability(int requiredCapacity)
    {
        return tables.FirstOrDefault(t => t.Capacity >= requiredCapacity && t.Status == "Available");
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
        int averageWaitTimePerTable = 30; // Assume 30 minutes per table
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
            ReassignWaitlistToTable();
        }
    }

    public void ModifyReservation(int reservationId, DateTime newTime, int newGuests, string newName)
    {
        var reservation = reservations.FirstOrDefault(r => r.ReservationId == reservationId);
        if (reservation != null)
        {
            reservation.ReservationTime = newTime;
            reservation.NumberOfGuests = newGuests;
            reservation.CustomerName = newName;
            Console.WriteLine($"Reservation {reservationId} updated.");
        }
        else
        {
            Console.WriteLine("Reservation not found.");
        }
    }

    public void RemoveReservation(int reservationId)
    {
        var reservation = reservations.FirstOrDefault(r => r.ReservationId == reservationId);
        if (reservation != null)
        {
            reservations.Remove(reservation);
            Console.WriteLine($"Reservation {reservationId} has been canceled.");
        }
        else
        {
            Console.WriteLine("Reservation not found.");
        }
    }

    public void ViewAllReservations()
    {
        Console.WriteLine("\n---- Reservations ----");
        foreach (var reservation in reservations)
        {
            Console.WriteLine($"ID: {reservation.ReservationId}, Name: {reservation.CustomerName}, Guests: {reservation.NumberOfGuests}, Time: {reservation.ReservationTime}");
        }
    }

    public void ViewWaitlist()
    {
        Console.WriteLine("\n---- Waitlist ----");
        foreach (var entry in waitlist)
        {
            Console.WriteLine($"Name: {entry.CustomerName}, Added: {entry.AddedTime}, Estimated Wait: {entry.EstimatedWaitTimeMinutes} minutes");
        }
    }

    public void ReassignWaitlistToTable()
    {
        var nextWaitlistEntry = waitlist.FirstOrDefault();
        if (nextWaitlistEntry != null)
        {
            var availableTable = CheckTableAvailability(1); // Assume any available table
            if (availableTable != null)
            {
                AssignTable(availableTable, nextWaitlistEntry.CustomerName);
                waitlist.Remove(nextWaitlistEntry);
            }
        }
    }

    public void ClearAllTables()
    {
        foreach (var table in tables)
        {
            table.UpdateStatus("Available");
        }
        Console.WriteLine("All tables have been cleared and set to available.");
    }
}

