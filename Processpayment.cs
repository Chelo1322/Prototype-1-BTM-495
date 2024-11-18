using System;

namespace RestaurantPaymentSystem
{
    // Customer Class
    class Customer
    {
        public void RequestBill()
        {
            Console.WriteLine("Customer requests the bill.");
        }
    }

    // Waitress Class
    class Waitress
    {
        public void SelectTable()
        {
            Console.WriteLine("Waitress selects the table.");
        }

        public void ProcessPayment(POS pos)
        {
            decimal totalAmount = pos.DisplayTotal();
            Console.WriteLine($"The total amount is: ${totalAmount}");

            Console.WriteLine("Choose payment method (1: Debit Card, 2: Credit Card, 3: Cash): ");
            string paymentMethod = Console.ReadLine();

            bool paymentSuccess = false;

            switch (paymentMethod)
            {
                case "1":
                    paymentSuccess = pos.ProcessDebitCardPayment(totalAmount);
                    break;
                case "2":
                    paymentSuccess = pos.ProcessCreditCardPayment(totalAmount);
                    break;
                case "3":
                    paymentSuccess = pos.ProcessCashPayment(totalAmount);
                    break;
                default:
                    Console.WriteLine("Invalid payment method selected.");
                    break;
            }

            if (paymentSuccess)
            {
                pos.PrintReceipt();
                pos.LogTransaction();
                pos.UpdateInventory();
                Console.WriteLine("Payment processed successfully.");
            }
            else
            {
                Console.WriteLine("Payment failed. Please try again.");
            }
        }
    }

    // POS Class
    class POS
    {
        public decimal DisplayTotal()
        {
            // Here you can calculate or fetch the actual total
            return 50.00m; // Example total amount
        }

        public bool ProcessDebitCardPayment(decimal amount)
        {
            Console.WriteLine("Processing debit card payment...");
            return ValidateCard() ? true : DisplayError();
        }

        public bool ProcessCreditCardPayment(decimal amount)
        {
            Console.WriteLine("Processing credit card payment...");
            return ValidateCard() ? true : DisplayError();
        }

        public bool ProcessCashPayment(decimal amount)
        {
            Console.WriteLine("Processing cash payment...");
            return true; // Assume cash payment always succeeds
        }

        private bool ValidateCard()
        {
            // Simulate card validation - here, we'll assume random success/failure
            Random rnd = new Random();
            return rnd.Next(0, 2) == 1; // 50% chance of success
        }

        private bool DisplayError()
        {
            Console.WriteLine("Payment declined. Please use another card.");
            return false;
        }

        public void PrintReceipt()
        {
            Console.WriteLine("Printing receipt...");
            // Code to print receipt here
        }

        public void LogTransaction()
        {
            Console.WriteLine("Logging transaction...");
            // Code to log transaction here
        }

        public void UpdateInventory()
        {
            Console.WriteLine("Updating inventory...");
            // Code to update inventory here
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            Customer customer = new Customer();
            Waitress waitress = new Waitress();
            POS pos = new POS();

            // Customer requests the bill
            customer.RequestBill();

            // Waitress selects the table
            waitress.SelectTable();

            // Waitress processes the payment using the POS system
            waitress.ProcessPayment(pos);
        }
    }
}
