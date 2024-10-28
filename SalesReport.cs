namespace BTM496;

public class SalesReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Criteria { get; set; }

    public SalesReport(DateTime startDate, DateTime endDate, string criteria)
    {
        StartDate = startDate;
        EndDate = endDate;
        Criteria = criteria;
    }

    public void GenerateReport()
    {
        Console.WriteLine("Generating sales report...");
        Console.WriteLine($"From: {StartDate.ToShortDateString()} To: {EndDate.ToShortDateString()}");
        Console.WriteLine($"Criteria: {Criteria}");

        // Logic to gather data and generate the report
        // For demonstration, we are just printing placeholder data.
        Console.WriteLine("Report generated successfully!");
    }
}