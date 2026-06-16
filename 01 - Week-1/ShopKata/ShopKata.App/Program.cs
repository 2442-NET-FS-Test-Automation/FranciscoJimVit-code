using System;
using System.Collections.Generic;
using ShopKata.Domain;

namespace ShopKata.App;


public class Program
{
    
    public static void Main()
    {   
        Program.ShopDemo();
    }

    static List<ShopProducts> productsList = new List<ShopProducts>
    {
        new Fruit("Apple", 0.5f, 10, new DateTime(2026, 12, 31)),
        new Drink("Soda", 2.5f, 20, 330),
        new Book("1984", 12.99f, 3, "George Orwell", "Dystopian Fiction")
    };
    
    public static void ShopMenu()
    {
        Console.WriteLine("\n\n=== Welcome to the Shop! ===");
        
        Console.WriteLine("\n\n=== Shop Menu ===");
        Console.WriteLine("1. Add Products");
        Console.WriteLine("2. sell Products");
        Console.WriteLine("3. restock Products");
        Console.WriteLine("4. View Products");
        Console.WriteLine("5. Exit");
        
    }

    public static void ShopDemo()
    {
        string input = "";
        while(input != "5")
        {
            
            ShopMenu();
            Console.WriteLine("\nPlease select an option:");
            input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.WriteLine("You selected: Add Products");
                    AddProducts();
                    break;
                case "2":
                    Console.WriteLine("You selected: sell Products");
                    SellProducts();
                    break;
                case "3":
                    Console.WriteLine("You selected: restock Products");
                    RestockProducts();
                    break;
                case "4":
                    Console.WriteLine("You selected: View Products");
                    ViewProducts();
                    break;
                case "5":
                    Console.WriteLine("Exiting the shop. Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    public static void AddProducts()
    {
        Console.WriteLine("Adding products to the shop...");

        // append the new products to the existing list
        productsList.Add(new Fruit("Orange", 0.4f, 12, new DateTime(2026, 11, 15)));
        productsList.Add(new Drink("Juice", 2.5f, 20, 1000));
        productsList.Add(new Book("Aquiles", 15.99f, 5, "Homer", "Epic Poetry"));
        
    }

    public static void SellProducts()
    {
        Console.WriteLine("Selling products from the shop...");

        Console.WriteLine("Enter the name of the product to sell:");
        string productName = Console.ReadLine();
        Console.WriteLine("Enter the number of items to sell:");
        int quantityToSell = int.Parse(Console.ReadLine());
       
        foreach (ShopProducts product in productsList)
        {
            if (product.Name == productName)
            {
                bool success = product.ReduceStock(quantityToSell);
                if (success)
                {
                    Console.WriteLine($"Successfully sold {quantityToSell} units of {productName}.");
                }
                else
                {
                    Console.WriteLine($"Not enough stock available for {productName}.");
                }
                break;
            }
        }
    }

    public static void RestockProducts()
    {
        Console.WriteLine("Restocking products in the shop...");
        Console.WriteLine("Enter the name of the product to restock:");
        string productName = Console.ReadLine();
        Console.WriteLine("Enter the number of items to restock:");
        int quantityToRestock = int.Parse(Console.ReadLine());
       
        foreach (ShopProducts product in productsList)
        {
            if (product.Name == productName)
            {
                if (quantityToRestock > 1)
                {
                    product.Restock(quantityToRestock);
                    Console.WriteLine($"Successfully restocked {quantityToRestock} units of {productName}.");
                }
                else
                {
                    product.Restock();
                }
                break;
            }
        }
    }

    public static void ViewProducts()
    {
        Console.WriteLine("Viewing products in the shop...");

        if (productsList.Count == 0)
        {
            Console.WriteLine("The shop is currently empty!");
            return;
        }

        foreach (ShopProducts product in productsList)
        {
            Console.WriteLine(product.ShelfLabel());
        }
    }



}



