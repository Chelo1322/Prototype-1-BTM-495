using System;

namespace RestaurantPaymentSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessPayment();
        }

        static void ProcessPayment()
        {
            Console.WriteLine("Customer requests the bill.");
            Console.WriteLine("Waiter/Waitress selects the table.");

            decimal totalAmount = DisplayTotal();
            Console.WriteLine($"The total amount is: ${totalAmount}");

            Console.WriteLine("Choose payment method (1: Debit Card, 2: Credit Card, 3: Cash): ");
            string paymentMethod = Console.ReadLine();

            bool paymentSuccess = false;

            switch (paymentMethod)
            {
                case "1":
                    paymentSuccess = ProcessDebitCardPayment(totalAmount);
                    break;
                case "2":
                    paymentSuccess = ProcessCreditCardPayment(totalAmount);
                    break;
                case "3":
                    paymentSuccess = ProcessCashPayment(totalAmount);
                    break;
                default:
                    Console.WriteLine("Invalid payment method selected.");
                    break;
            }

            if (paymentSuccess)
            {
                PrintReceipt();
                LogTransaction();
                UpdateInventory();
                Console.WriteLine("Payment processed successfully.");
            }
            else
            {
                Console.WriteLine("Payment failed. Please try again.");
            }
        }

        static decimal DisplayTotal()
        {
            // Here you can calculate or fetch the actual total
            return 50.00m; // Example total amount
        }

        static bool ProcessDebitCardPayment(decimal amount)
        {
            Console.WriteLine("Processing debit card payment...");
            return ValidateCard() ? true : DisplayError();
        }

        static bool ProcessCreditCardPayment(decimal amount)
        {
            Console.WriteLine("Processing credit card payment...");
            return ValidateCard() ? true : DisplayError();
        }

        static bool ProcessCashPayment(decimal amount)
        {
            Console.WriteLine("Processing cash payment...");
            return true; // Assume cash payment always succeeds
        }

        static bool ValidateCard()
        {
            // Simulate card validation - here, we'll assume random success/failure
            Random rnd = new Random();
            return rnd.Next(0, 2) == 1; // 50% chance of success
        }

        static bool DisplayError()
        {
            Console.WriteLine("Payment declined. Please use another card.");
            return false;
        }

        static void PrintReceipt()
        {
            Console.WriteLine("Printing receipt...");
            // Code to print receipt here
        }

        static void LogTransaction()
        {
            Console.WriteLine("Logging transaction...");
            // Code to log transaction here
        }

        static void UpdateInventory()
        {
            Console.WriteLine("Updating inventory...");
            // Code to update inventory here
        }
    }
}