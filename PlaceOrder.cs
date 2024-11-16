using System;
using System.Collections.Generic;

namespace POSApp
{
    class Program
    {
        static void Main(string[] args)
        {
            POSSystem pos = new POSSystem();
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n---- POS System ----");
                Console.WriteLine("1. Show Inventory");
                Console.WriteLine("2. Place an Order");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        pos.ShowInventory();
                        break;
                    case 2:
                        pos.PlaceOrder();
                        break;
                    case 0:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }

    public class POSSystem
    {
        private List<Product> Inventory;
        private List<Order> Orders;
        private int NextOrderID = 1;

        public POSSystem()
        {
            Inventory = new List<Product>();
            Orders = new List<Order>();
            SeedInventory();
        }

        private void SeedInventory()
        {
            Inventory.Add(new Product(1, "Sashimi", 15.99m, 10));
            Inventory.Add(new Product(2, "Ramen", 9.99m, 20));
            Inventory.Add(new Product(3, "Tempura", 12.99m, 15));
        }

        public void ShowInventory()
        {
            Console.WriteLine("---- Available Products ----");
            foreach (var product in Inventory)
            {
                product.DisplayProduct();
            }
        }

        public void PlaceOrder()
        {
            Order newOrder = new Order(NextOrderID++);
            bool orderInProgress = true;

            while (orderInProgress)
            {
                Console.WriteLine("Enter product ID and quantity (or '0' to finish): ");
                int productID = int.Parse(Console.ReadLine());

                if (productID == 0)
                {
                    orderInProgress = false;
                    continue;
                }

                Console.WriteLine("Enter quantity: ");
                int quantity = int.Parse(Console.ReadLine());

                Product product = Inventory.Find(p => p.ProductID == productID);
                if (product != null)
                {
                    bool isAdded = newOrder.AddProduct(product, quantity);
                    if (!isAdded)
                    {
                        Console.WriteLine($"Unable to add {product.Name} to the order.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid product ID.");
                }
            }

            Console.WriteLine("Confirm the order? (y/n)");
            string confirmation = Console.ReadLine();
            if (confirmation.ToLower() == "y")
            {
                newOrder.ConfirmOrder();
                Orders.Add(newOrder);

                newOrder.SendToKitchen();
                newOrder.GenerateReceipt();
            }
            else
            {
                Console.WriteLine("Order was not confirmed.");
            }
        }
    }

    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public Product(int productID, string name, decimal price, int quantity)
        {
            ProductID = productID;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public bool IsAvailable(int orderQuantity)
        {
            return Quantity >= orderQuantity;
        }

        public void UpdateStock(int orderQuantity)
        {
            Quantity -= orderQuantity;
        }

        public void DisplayProduct()
        {
            Console.WriteLine($"Product ID: {ProductID}, Name: {Name}, Price: {Price:C}, Quantity: {Quantity}");
        }
    }

    public class Order
    {
        public int OrderID { get; set; }
        public List<Product> OrderedProducts { get; set; }
        public decimal OrderTotal { get; private set; }
        public bool IsConfirmed { get; private set; }

        public Order(int orderID)
        {
            OrderID = orderID;
            OrderedProducts = new List<Product>();
            OrderTotal = 0;
            IsConfirmed = false;
        }

        public bool AddProduct(Product product, int quantity)
        {
            if (product.IsAvailable(quantity))
            {
                product.UpdateStock(quantity);
                OrderedProducts.Add(product);
                OrderTotal += product.Price * quantity;
                return true;
            }
            else
            {
                Console.WriteLine($"Not enough stock for {product.Name}. Order cannot be placed.");
                return false;
            }
        }

        public void ConfirmOrder()
        {
            IsConfirmed = true;
            Console.WriteLine("Order confirmed by the waitress.");
        }

        public void SendToKitchen()
        {
            if (IsConfirmed)
            {
                Console.WriteLine("Order sent to the kitchen.");
            }
            else
            {
                Console.WriteLine("Order is not confirmed yet.");
            }
        }

        public void GenerateReceipt()
        {
            Console.WriteLine("---- Receipt ----");
            foreach (var product in OrderedProducts)
            {
                Console.WriteLine($"{product.Name} - {product.Price:C}");
            }
            Console.WriteLine($"Total: {OrderTotal:C}");
        }
    }
}

