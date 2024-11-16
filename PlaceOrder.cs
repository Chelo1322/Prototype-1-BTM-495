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
                Console.WriteLine("1. Place an Order");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                string input = Console.ReadLine();

                if (int.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            pos.PlaceOrder();
                            break;
                        case 0:
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please enter a valid number.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
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

        public void PlaceOrder()
        {
            Order newOrder = new Order(NextOrderID++);
            bool orderInProgress = true;

            while (orderInProgress)
            {
                Console.WriteLine("\n---- Place an Order ----");
                Console.WriteLine("Available Products:");
                foreach (var product in Inventory)
                {
                    product.DisplayProduct();
                }

                Console.WriteLine("Enter product ID to add (or '0' to finish):");
                string productInput = Console.ReadLine();

                if (int.TryParse(productInput, out int productID))
                {
                    if (productID == 0)
                    {
                        orderInProgress = false;
                        continue;
                    }

                    Console.WriteLine("Enter quantity:");
                    string quantityInput = Console.ReadLine();

                    if (int.TryParse(quantityInput, out int quantity) && quantity > 0)
                    {
                        Product product = Inventory.Find(p => p.ProductID == productID);
                        if (product != null)
                        {
                            bool isAdded = newOrder.AddProduct(product, quantity);
                            if (!isAdded)
                            {
                                Console.WriteLine($"Not enough stock for {product.Name}. Cannot add to the order.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid product ID.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid quantity. Please enter a positive number.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid product ID. Please enter a valid number.");
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
        public List<OrderItem> OrderedItems { get; private set; }
        public decimal OrderTotal { get; private set; }
        public bool IsConfirmed { get; private set; }

        public Order(int orderID)
        {
            OrderID = orderID;
            OrderedItems = new List<OrderItem>();
            OrderTotal = 0;
            IsConfirmed = false;
        }

        public bool AddProduct(Product product, int quantity)
        {
            if (product.IsAvailable(quantity))
            {
                product.UpdateStock(quantity);
                OrderedItems.Add(new OrderItem(product, quantity));
                OrderTotal += product.Price * quantity;
                return true;
            }
            else
            {
                Console.WriteLine($"Not enough stock for {product.Name}.");
                return false;
            }
        }

        public void ConfirmOrder()
        {
            IsConfirmed = true;
            Console.WriteLine("Order confirmed.");
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
            foreach (var item in OrderedItems)
            {
                Console.WriteLine($"{item.Product.Name} x {item.Quantity} - {item.Product.Price * item.Quantity:C}");
            }
            Console.WriteLine($"Total: {OrderTotal:C}");
        }
    }

    public class OrderItem
    {
        public Product Product { get; private set; }
        public int Quantity { get; private set; }

        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}

