using System;
using System.Collections.Generic;

public class Customer
{
    public string Name { get; set; }
    public string Email { get; set; }
    public int LoyaltyPoints { get; private set; } = 0;

    public void AddPoints(int points)
    {
        LoyaltyPoints += points;
        Console.WriteLine($"{points} points added to {Name}'s account.");
    }

    public void RedeemPoints(int points)
    {
        if (LoyaltyPoints >= points)
        {
            LoyaltyPoints -= points;
            Console.WriteLine($"{points} points redeemed by {Name}.");
        }
        else
        {
            Console.WriteLine("Insufficient loyalty points.");
        }
    }
}

public class POSSystem
{
    private List<Customer> customers = new List<Customer>();

    public void JoinLoyaltyProgram(string name, string email)
    {
        Customer newCustomer = new Customer { Name = name, Email = email };
        customers.Add(newCustomer);
        Console.WriteLine($"{newCustomer.Name} has joined the loyalty program.");
    }

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

    public void TrackPurchase(Customer customer, double amountSpent)
    {
        int pointsEarned = CalculatePoints(amountSpent);
        customer.AddPoints(pointsEarned);
        Console.WriteLine($"{customer.Name} earned {pointsEarned} points for spending ${amountSpent}.");
    }

    private int CalculatePoints(double amountSpent)
    {
        return (int)(amountSpent * 0.1); // Example: 1 point per $10 spent
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
                        pos.TrackPurchase(loggedInCustomer, amount);
                        pos.UpdateLoyaltyBalance(loggedInCustomer);
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