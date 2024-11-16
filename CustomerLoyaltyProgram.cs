using System;
using System.Collections.Generic;

public class Customer
{
    public int Cust_ID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int LoyaltyPoints { get; private set; } = 0;
    public double Balance { get; set; }

    // Method to create a new customer profile
    public void CreateProfile(string name, string email)
    {
        Name = name;
        Email = email;
        Console.WriteLine($"Profile created for {Name}.");
    }

    // Method to make a purchase
    public void MakePurchase(double amount)
    {
        Balance -= amount;
        Console.WriteLine($"{Name} made a purchase of ${amount}. Remaining balance: ${Balance}");
    }

    // Method to accumulate loyalty points
    public void AccumulateLoyaltyPoints(int points)
    {
        LoyaltyPoints += points;
        Console.WriteLine($"{points} loyalty points added to {Name}'s account. Total points: {LoyaltyPoints}");
    }

    // Method to redeem points for a reward
    public void RedeemPoints(int points)
    {
        if (LoyaltyPoints >= points)
        {
            LoyaltyPoints -= points;
            Console.WriteLine($"{points} points redeemed by {Name}. Remaining points: {LoyaltyPoints}");
        }
        else
        {
            Console.WriteLine("Not enough loyalty points.");
        }
    }

    public void RequestBill()
    {
        Console.WriteLine($"{Name} has requested the bill.");
    }

    public void MakePayment(double amount)
    {
        Balance -= amount;
        Console.WriteLine($"{Name} paid ${amount}. Remaining balance: ${Balance}");
    }
}

public class Waitress
{
    public int User_ID { get; set; }
    public string Name { get; set; }
    public string Role { get; set; } = "Waitress";

    // Waitress login method
    public void Login()
    {
        Console.WriteLine($"{Name} has logged in.");
    }

    // Method to view profile
    public void ViewProfile()
    {
        Console.WriteLine($"{Name}'s Profile: ID={User_ID}, Role={Role}");
    }

    // Method for waitress to take an order
    public void TakeOrder(Customer customer, double amount)
    {
        customer.MakePurchase(amount);
        Console.WriteLine($"Waitress {Name} has taken an order from {customer.Name} totaling ${amount}");
    }

    // Method to process customer payments
    public void ProcessPayments(Customer customer, double amount)
    {
        customer.MakePayment(amount);
        Console.WriteLine($"Payment of ${amount} processed by {Name} for {customer.Name}");
    }
}

public class POSSystem
{
    private List<Customer> customers = new List<Customer>();

    // Method to join the loyalty program
    public void JoinLoyaltyProgram(string name, string email)
    {
        Customer newCustomer = new Customer();
        newCustomer.CreateProfile(name, email);
        customers.Add(newCustomer);
    }

    // Method to login a customer
    public Customer LoginCustomer(string email)
    {
        foreach (var customer in customers)
        {
            if (customer.Email == email)
            {
                Console.WriteLine($"Welcome back, {customer.Name}!");
                return customer;
            }
        }
        Console.WriteLine("Customer not found. Please check the email or create a new account.");
        return null;
    }

    // Method to track customer purchases
    public void TrackPurchases(Customer customer, double amountSpent)
    {
        customer.MakePurchase(amountSpent);
        int pointsEarned = CalculateLoyaltyPoints(amountSpent);
        customer.AccumulateLoyaltyPoints(pointsEarned);
    }

    private int CalculateLoyaltyPoints(double amountSpent)
    {
        return (int)(amountSpent * 0.1); // Example: 1 point per $10 spent
    }

    // Method to apply rewards to the customer's account
    public void ApplyRewards(Customer customer, int pointsToRedeem)
    {
        customer.RedeemPoints(pointsToRedeem);
    }

    // Method to generate a receipt for a purchase
    public void GenerateReceipt(Customer customer, double amount)
    {
        Console.WriteLine($"Receipt generated for {customer.Name}: Purchase of ${amount}");
    }

    // Method to log a transaction
    public void LogTransaction(Customer customer, double amount)
    {
        Console.WriteLine($"Transaction logged: {customer.Name} purchased ${amount}");
    }

    public void UpdateLoyaltyBalance(Customer customer)
    {
        Console.WriteLine($"{customer.Name}'s loyalty points balance is now {customer.LoyaltyPoints}.");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        POSSystem pos = new POSSystem();
        Customer loggedInCustomer = null;

        while (true)
        {
            Console.WriteLine("\n--- Loyalty Program Menu ---");
            Console.WriteLine("1. Join Loyalty Program");
            Console.WriteLine("2. Login to Loyalty Account");
            Console.WriteLine("3. Check Loyalty Points Balance");
            Console.WriteLine("4. Make a Purchase");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1": // Join Loyalty Program
                    Console.Write("Enter your name: ");
                    string name = Console.ReadLine();
                    Console.Write("Enter your email: ");
                    string email = Console.ReadLine();
                    pos.JoinLoyaltyProgram(name, email);
                    break;

                case "2": // Login
                    Console.Write("Enter your email: ");
                    string loginEmail = Console.ReadLine();
                    loggedInCustomer = pos.LoginCustomer(loginEmail);
                    break;

                case "3": // Check Points Balance
                    if (loggedInCustomer != null)
                    {
                        pos.UpdateLoyaltyBalance(loggedInCustomer);
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;

                case "4": // Make a Purchase
                    if (loggedInCustomer != null)
                    {
                        Console.Write("Enter purchase amount: ");
                        double amount = double.Parse(Console.ReadLine());
                        pos.TrackPurchases(loggedInCustomer, amount);
                        pos.GenerateReceipt(loggedInCustomer, amount);
                        pos.LogTransaction(loggedInCustomer, amount);
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;

                case "5": // Exit
                    Console.WriteLine("Thank you for using the Loyalty Program. Goodbye!");
                    return;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}
