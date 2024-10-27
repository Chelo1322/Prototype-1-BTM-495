

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public string ProductSize { get; set; }
    public double ProductWeight { get; set; }
    public decimal ProductPrice { get; set; }
    public string ProductSupplier { get; set; }
    public int StockQuantity { get; set; }
    public bool IsOutOfStock => StockQuantity <= 0;
}

public class Inventory
{
    public List<Product> ProductList { get; set; } = new List<Product>();
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public int TotalStock => ProductList.Sum(p => p.StockQuantity);
    public decimal InventoryValue => ProductList.Sum(p => p.ProductPrice * p.StockQuantity);
    public List<Product> OutOfStockItems => ProductList.Where(p => p.IsOutOfStock).ToList();

    public void AddProduct(Product product)
    {
        ProductList.Add(product);
        LastUpdated = DateTime.Now;
    }

    public void RemoveProduct(int productId)
    {
        var product = ProductList.FirstOrDefault(p => p.ProductID == productId);
        if (product != null)
        {
            ProductList.Remove(product);
            LastUpdated = DateTime.Now;
        }
    }

    public void UpdateProductStock(int productId, int quantity)
    {
        var product = ProductList.FirstOrDefault(p => p.ProductID == productId);
        if (product != null)
        {
            product.StockQuantity += quantity;
            LastUpdated = DateTime.Now;
        }
    }
}

public class InventoryManager
{
    private Inventory inventory = new Inventory();

    public bool Login(string userId, string password)
    {
        // Simulated login logic
        if (userId == "cook123" && password == "password")
        {
            Console.WriteLine("Login successful!");
            return true;
        }
        else
        {
            Console.WriteLine("Login failed. Invalid credentials.");
            return false;
        }
    }

    public void ShowInventoryOptions()
    {
        Console.WriteLine("Inventory Management:");
        Console.WriteLine("1. View Inventory");
        Console.WriteLine("2. Modify Inventory");
        Console.Write("Select an option (1 or 2): ");
    }

    public void ViewInventory()
    {
        Console.WriteLine("Viewing Inventory Details:");
        Console.WriteLine($"Total Stock: {inventory.TotalStock}");
        Console.WriteLine($"Inventory Value: {inventory.InventoryValue:C}");
        Console.WriteLine($"Last Updated: {inventory.LastUpdated}");

        Console.WriteLine("\nOut of Stock Items:");
        foreach (var product in inventory.OutOfStockItems)
        {
            Console.WriteLine($"- {product.ProductName} (ID: {product.ProductID})");
        }

        Console.WriteLine("\nProduct List:");
        foreach (var product in inventory.ProductList)
        {
            Console.WriteLine($"ID: {product.ProductID}, Name: {product.ProductName}, Stock: {product.StockQuantity}");
        }
    }

    public void ModifyInventory()
    {
        Console.Write("Enter Product ID to modify: ");
        int productId = int.Parse(Console.ReadLine());

        Console.Write("Enter Product Name: ");
        string productName = Console.ReadLine();

        Console.Write("Enter Stock Quantity to Add/Remove (use negative value to remove): ");
        int quantity = int.Parse(Console.ReadLine());

        if (inventory.ProductList.Any(p => p.ProductID == productId))
        {
            inventory.UpdateProductStock(productId, quantity);
            Console.WriteLine("Inventory updated successfully.");
        }
        else
        {
            Console.WriteLine("Product not found. Adding as a new product.");

            var newProduct = new Product
            {
                ProductID = productId,
                ProductName = productName,
                StockQuantity = quantity,
                ProductSize = "Standard",
                ProductWeight = 1.0,
                ProductPrice = 10.0m,
                ProductSupplier = "Default Supplier"
            };
            inventory.AddProduct(newProduct);
            Console.WriteLine("New product added successfully.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var manager = new InventoryManager();

        Console.WriteLine("Welcome to the POS Inventory Management System");

        Console.Write("Enter your User ID: ");
        string userId = Console.ReadLine();

        Console.Write("Enter your Password: ");
        string password = Console.ReadLine();

        if (manager.Login(userId, password))
        {
            manager.ShowInventoryOptions();

            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    manager.ViewInventory();
                    break;
                case 2:
                    manager.ModifyInventory();
                    break;
                default:
                    Console.WriteLine("Invalid option selected.");
                    break;
            }
        }
    }
}
