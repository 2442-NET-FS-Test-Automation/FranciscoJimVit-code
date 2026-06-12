// if i Have code from another namespace, I can use the using directive to import it.
using System.Runtime.InteropServices.Marshalling;
using Library.Domain;

namespace LibraryKata.App; // A namespace is like a bucket or logical container for different related code files. 
// It helps to organize code and avoid naming conflicts. In this case, "LibraryKata.App" is the namespace for the application code related to the Library Kata.

public class Program // This defines a public class named "Program". In C#, the entry point of a console application is typically defined in a class named "Program".
{

    // public - accesible from anywhere across the application
    // static - Main can be called without creating an instance of the Program class. 
    // This is important because the Main method is the entry point of the application and needs to be accessible without needing to create an object of the Program class.
    // void - it doesn't return any value.
    public static void Main(string[] args) // This is the Main method, which is the entry point of the application. It takes an array of strings as arguments, which can be used to pass command-line arguments to the application.
    {
        // when i call dotnet run in the terminal, it finds the Main method and starts executing the code inside it.

        Program.DataTypesAndOperators();
        ClassExample();
        OopDemo();
    }

    // private - only accessible within the Program class.
    // static - It belongs to the class itself rather than an instance of the class.
    // void - it doesn't return any value.
    private static void DataTypesAndOperators() 
    {
        Console.WriteLine("=== Data Types and Operators ===");

        int copies = 3; // whole numbers
        double lateFee = 0.50; // floating-point numbers (decimals)
        bool isAvailable = true; // true or false values
        char shelf = 'A'; // single character
        string title = "Clean code"; // sequence of characters (text)

        // Operators
        string user = "Alice"; // single = is used for assignment
        int total = copies + 2; // example of arithmetic operator (+, -, *, /)
        bool isEnough = total > 5; // comparison operator (>, <, ==, !=) retuns True/False
        bool canBorrow = isAvailable && isEnough; // logical operator (&& - and, || - or, ! - not) combines boolean expressions
        // (^ - xor) operator true if one of the operands is true, but not both

        Console.WriteLine(title + " Has been checked out by " + user); // string concatenation using + operator
        Console.WriteLine($"{title} on shelf {shelf}: {copies} copies, fee {lateFee}"); // string interpolation using $ and {}

    }

    private static void ControlFlow()
    {
        Console.WriteLine("\n=== Control Flow ===");

        int copiesAvailable = 0;
        bool isNumber = true;

        if (copiesAvailable > 1 && isNumber == true)
        {
            Console.WriteLine("Available for checkout");
        }
        else if(copiesAvailable == 1)
        {
            Console.WriteLine("Last copy available");
        }
        else
        {
            Console.WriteLine("Out of stock");
            Console.WriteLine("Please check back later.");
        }


        // Switch
        string genre = "Fiction";
        
        // Clasic switch - notice C# cares about intent a lot! No fall through like in other languages
        switch (genre)
        {
            case "Fiction":
                Console.WriteLine("Check section A.");
                break;
            case "Mystery":
                Console.WriteLine("Check section B.");
                break;
            case "Science Fiction":
                Console.WriteLine("Check section C.");
                break;
            default: // While optional, a default case is good practice to handle unexpected values
                Console.WriteLine("Genre not found.");
                break;
        }

        // New in .Net 8, Switch expressions! You don't have to use these - they probably won't be come up in QC
        // but they're used out in real world code, so here is an example. In a switch expression, 
        // we want a return value from the switch -  we can use that value to print out a result.
        string section = genre switch
        {
            "Fiction" => "Check section A.",
            "Mystery" => "Check section B.",
            "Science Fiction" => "Check section C.",
            _ => "Genre not found."
        };

        Console.WriteLine(section);

    }

    private static void Loops ()
    {
        // C# provides for several llops as well, same as Java and any other programming language.
        // For, While, do-While, etc
        for(int day = 1; day <= 3; day++)
        {
            Console.WriteLine($"Reminder day {day}: fee so far {calculateLateFee(day)}");
        }

        int onShelf = 3;
        while(onShelf > 0)
        {
            Console.WriteLine($"There are {onShelf} copies on the shelf.");
            onShelf--; // Quick decrement shorthand
        }
        Console.WriteLine("No copies on shelf");

        string myString = "dog";
        myString = "cat";
    }

    private static double calculateLateFee(int daysLate) => daysLate * 2;

    private static void ArrayWork()
    {
        // C# provides for arrays as well as lists and other collections - we'll get those later.
        string[] books = { "Book 1", "Book 2", "Book 3", "Book 4" };
        
        Console.WriteLine(books[3]); // I can access individual elements - keeping in mind we index at 0
        
        // C# allows for-each loops
        foreach (string book in books)
        {
            Console.WriteLine(book);
        }

    }


    private static void ClassExample()
    {
        Console.WriteLine("Using our domain book class");

        // Instantiating my first book, calling the constructor via "new" keyword
        Book book1 = new Book("Dune", "Frank Herbert", 0);
        Book book2 = new Book("The Hobbit", "J.R.R. Tolkein", 5);

        // if I want to print book info, I can just pass the book variable
        // It calls the toString() for me, The next tow lines do the same thing
        Console.WriteLine(book1);
        Console.WriteLine(book1.ToString());

        Console.WriteLine($"Cheking out {book1.Checkout()}"); // false
        Console.WriteLine($"Cheking out {book2.Checkout()}"); // true

    }

    public static void OopDemo()
    {
        Console.WriteLine("\n\n == OOP Demo == ");
        
        LibraryItem[] catalog =
        {
            new Book("Dune", "Frank Herbert", 2),
            new ReferenceBook("The Great Gatsby", "F. Scott Fitzgerald", "summary of the book"),
            new Magazine("Sports Illustrated", "Various", 10, "Sports Illustrated")

        };

        foreach (LibraryItem item in catalog)
        {
            Console.WriteLine(item.Describe());
        }

        foreach (LibraryItem item in catalog)
        {
            if (item is ILendable lendable)
            {
                Console.WriteLine($"{item.Title}: chekout -> {lendable.Checkout()}");
            }
            else
            {
                Console.WriteLine("Reference Only");
            }
        }

        Magazine wired = new Magazine("Wired", "Various", 3, "Conde Nast");
        LibraryItem baseMag = wired;

        Console.WriteLine("== Override vs new on the same object, different ref types == ");
        Console.WriteLine($"Magazine reference: {wired.Describe()}");
        Console.WriteLine($"LibraryItem reference: {baseMag.Describe()}");
    }
}