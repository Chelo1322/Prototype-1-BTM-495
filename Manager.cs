namespace BTM496;

public class Manager
{
    private string username;
    private string password;

    public Manager(string username, string password)
    {
        this.username = username;
        this.password = password;
    }

    public bool Login()
    {
        // Placeholder login logic
        Console.WriteLine("Logging into the POS system...");
        if (username == "manager" && password == "password") // Dummy credentials
        {
            Console.WriteLine("Login successful!");
            return true;
        }
        else
        {
            Console.WriteLine("Invalid credentials.");
            return false;
        }
    }

    public void ViewSalesReport()
    {
        if (Login())
        {
            // Step 2: Select Dates
            Console.WriteLine("Enter start date (yyyy-mm-dd):");
            DateTime startDate = DateTime.Parse(Console.ReadLine());

            Console.WriteLine("Enter end date (yyyy-mm-dd):");
            DateTime endDate = DateTime.Parse(Console.ReadLine());

            // Step 4: Select Criteria
            Console.WriteLine("Enter criteria for the report (e.g., top selling plates):");
            string criteria = Console.ReadLine();

            // Step 5: Generate Report
            SalesReport report = new SalesReport(startDate, endDate, criteria);
            report.GenerateReport();
        }
    }
}