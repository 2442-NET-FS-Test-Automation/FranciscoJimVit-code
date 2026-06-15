using ShopKata.Domain;

namespace ShopKata.App;

public class Program
{
    public static void Main(string[] args)
    {
        ShopMenu();
        ShopDemo();

    }

    public static void ShopMenu()
    {
        Console.WriteLine("=== Welcome to the Shop! ===");
        
        Console.WriteLine("\n\n=== Shop Menu ===");
        Console.WriteLine("\n1. Add Products");
        Console.WriteLine("\n2. sell Products");
        Console.WriteLine("\n3. restock Products");
        Console.WriteLine("\n4. View Products");
        Console.WriteLine("\n5. Exit");
        
    }

    public static void ShopDemo()
    {

        Console.WriteLine("\nPlease select an option:");
        string input = Console.ReadLine();
        switch (input)
        {
            case "1":
                Console.WriteLine("You selected: Add Products");
                break;
            case "2":
                Console.WriteLine("You selected: sell Products");
                break;
            case "3":
                Console.WriteLine("You selected: restock Products");
                break;
            case "4":
                Console.WriteLine("You selected: View Products");
                break;
            case "5":
                Console.WriteLine("Exiting the shop. Goodbye!");
                break;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
        
        // Create a shop and add products to it
        /*
        Shop shop = new Shop();
        shop.AddProduct(new Product("Apple", 0.5, 10));
        shop.AddProduct(new Product("Bread", 1.0, 5));
        shop.AddProduct(new Product("Milk", 1.5, 3));

        // Display all products in the shop
        Console.WriteLine("Products available in the shop:");
        foreach (var product in shop.Products)
        {
            Console.WriteLine($"{product.Name}: ${product.Price}");
        }
        */
    }

}