using System;
using System.Data.SqlClient;

public class User
{
    public int UserID { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    private string Password { get; set; }

    public User(int userId, string username, string email, string role, string passwordHash)
    {
        UserID = userId;
        Username = username;
        Email = email;
        Role = role;
        Password = password;
    }

    public bool VerifyPassword(string inputPassword)
    {
        return Password == inputPassword;
    }
}

public class LoginService
{
    private readonly string _connectionString;

    public LoginService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public User AuthenticateUser(string username, string password)
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new User(
                    (int)reader["UserID"],
                    reader["Username"].ToString(),
                    reader["Email"].ToString(),
                    reader["Role"].ToString(),
                    reader["PasswordHash"].ToString()
                );
            }
        }
        Console.WriteLine("Invalid credentials.");
        return null;
    }
    
    public void ViewSalesReport()
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

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Server=localhost;Database=UserManagement;User Id=your_username;Password=your_password;";
        LoginService loginService = new LoginService(connectionString);

        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        User user = loginService.AuthenticateUser(username, password);

        if (user != null)
        {
            Console.WriteLine($"Welcome, {user.Username}!");
            LoginService.ViewSalesReport();
        }
        else
        {
            Console.WriteLine("Login failed. Try again.");
        }
    }
}



