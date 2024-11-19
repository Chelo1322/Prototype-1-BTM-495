using System;
// Base User Class
public abstract class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string ProfileInfo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Constructor
    public User(string username, string password, string email, string role)
    {
        Username = username;
        Password = password;
        Email = email;
        Role = role;
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }

    // Virtual method for profile update
    public virtual void UpdateProfile(string profileInfo)
    {
        ProfileInfo = profileInfo;
        UpdatedAt = DateTime.Now;
    }
}

// Manager Class (Specialized User)
public class Manager : User
{
    public decimal Salary { get; set; }
    public string Department { get; set; }
    public bool IsAdmin { get; set; }

    // Constructor
    public Manager(
        string username, 
        string password, 
        string email, 
        decimal salary, 
        string department) 
        : base(username, password, email, "Manager")
    {
        Salary = salary;
        Department = department;
        IsAdmin = true;
    }

    // Override profile update with additional management logic
    public override void UpdateProfile(string profileInfo)
    {
        base.UpdateProfile(profileInfo);
        Console.WriteLine("Manager profile updated with enhanced logging.");
    }

    // Method specific to Manager
    public void AdjustSalary(decimal newSalary)
    {
        if (newSalary > 0)
        {
            Salary = newSalary;
            Console.WriteLine($"Salary adjusted to {newSalary}");
        }
        else
        {
            throw new ArgumentException("Salary must be a positive value");
        }
    }
}

// Updated UserManager Class
public class UserManager
{
    private List<User> users = new List<User>();

    public UserManager()
    {
        // Sample user creation with email
        users.Add(new Manager(
            "johnmanager", 
            "password123", 
            "john.manager@restaurant.com", 
            75000.00m, 
            "Restaurant Management"
        ));
    }

    // Authentication method
    public User Authenticate(string username, string password)
    {
        return users.FirstOrDefault(u => 
            u.Username == username && 
            u.Password == password);
    }

    // Method to add new user
    public void AddUser(User user)
    {
        // Validate email format
        if (IsValidEmail(user.Email))
        {
            users.Add(user);
        }
        else
        {
            throw new ArgumentException("Invalid email format");
        }
    }

    // Email validation method
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
