// if i Have code from another namespace, I can use the using directive to import it.
using System.Runtime.InteropServices.Marshalling;
using Library.Domain;
using Serilog;

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

        // Lets configure Serilog here before any code execution.
        // Serilog works via a singleton object. Its shared globally
        // throughout the application, configure once use anywhere

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information() // Verbose > Debug > Information > Warning > Error > Fatal
            .WriteTo.Console() // Sink: where do my logs go? console, text file, database, etc?
            .CreateLogger(); // create the logger based on the config above.


        // when i call dotnet run in the terminal, it finds the Main method and starts executing the code inside it.

        Program.DataTypesAndOperators();
        ClassExample();
        OopDemo();

        CollectionsDemo();

        ExceptionDemo();
        // In case there are any lingering logs by the time we hit line 41 above
        // don't just stop execution, write the logs to their sink THEN close the program.
        Log.CloseAndFlush();


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

        // string myString = "dog";
        // myString = "cat";
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


    /// Collections Demo stuff
    
    private static void CollectionsDemo()
    {
        Console.WriteLine("== Collections Demo Stuff == ");

        // Creating a catalog object
        // because this is backed by a List, it grows and shrinks for us
        Catalog catalog = new Catalog();
        
        //I cloud create my objects
        Book dune = new Book("Dune", "Frank Herbert", 2);
        
        // the add them
        catalog._items.Add(dune);

        // I can also just call a constructor inside the Add method call
        // Methods having their arguments satisfied by the return of other methods is a common pattern in C#
        // and sometimes you'll get like 4-5 callbacks deep in tools like ASP.NET
        catalog._items.Add(new ReferenceBook("The Hobbit", "J.R.R. Tolkein", "summary of the book"));
        catalog._items.Add(new Magazine("Sports Illustrated", "Various", 10, "Sports Illustrated"));

        Console.WriteLine($"Catalog holds {catalog._items.Count} items. first is {catalog._items[0].Title}");

        // Enuk + struct use
        ItemKind kind = ItemKind.Magazine; // example of selecting and enum value
        ShelfLocation location = new ShelfLocation(3, 12); // struct - looks alot like a class, but it is a Value Type

        Console.WriteLine($"Item kind: {kind}, sits at {location}");
        

        ///////////////
        /// Book duneCopy = dune;  // copies the reference
        /// // Lets say I modify duneCopy, what happens to the data in dune?
        /// // all we copies was the pointer - these two things are not independent
        /// 
        /// ShelfLocation locationCopy = location; // copies the data/fields
        /// these are not linked in the same way, I can edit the data in one without touching the other
        ///////////////
        
        // Generics: our own Shelf<T> that can hold anything - thought technically all the collections
        // we used thusfar have been generic classes themselves
        Shelf<LibraryItem> shelf = new Shelf<LibraryItem>(2);
        Shelf<int> intShelf = new Shelf<int>(200);

        shelf.TryAdd(catalog._items[0]);
        shelf.TryAdd(catalog._items[1]);

        Console.WriteLine($"Trying to add a third thing in our catalog: {shelf.TryAdd(catalog._items[2])}");
    }



    /* /////////////////////////////////////////////////////////////////////////////////////////////////////// */
    public static void ExceptionDemo()
    {
        Console.WriteLine("\n\n == Exceptions, patterns, logging == ");

        // by using liskov substitution from SOLID, if I later swap to a SQLLibraryRepo or whatever, 
        // This is only line I have to change
        ILibraryRepository repo = new InMemoryLibraryRepository();

        // Injecting our existing repo object to statify LibraryUnitOfWork's dependency
        IUnitOfWork libraryWork = new LibraryUnitOfWork(repo);

        // Create a book, but using our factory method - notice
        LibraryItem book = LibraryItemFactory.Create(ItemKind.Book, "The Great Gatsby", "F. Scott Fitzgerald", copies:180);

        repo.Add(book);

        repo.Add(LibraryItemFactory.Create(ItemKind.Book, "To Kill a Mockingbird", "Harper Lee", copies:100));

        libraryWork.Stage("Added 2 items");
        libraryWork.Commit();

        try
        {
            
            LibraryItem missing = repo.GetById(99);
            Console.WriteLine(missing.Describe());
        }
        catch(ItemNotFoundException ex)
        {
            Log.Error($"Lookup failed for {ex.Id}: {ex.Message}");
        }
        catch(LibraryException ex)
        {
            Log.Error($"Library error occurred: {ex.Message}");
        }
        catch(Exception ex)
        {
            Log.Error($"Non Library exception: {ex.Message}");
        }
        finally // optional, but adding a finally block adds code that runs
        {// whether an exception is caught or not

            // Code in a finally block will run even if the try ends in a return
            // useful for DB operations where you want to cleanup but you found the object to return
            Console.WriteLine("hit out finally block - lookup attempt done");
        }

        Book noCopies = new Book("Count of Monte Cristo", "Alexandre Dumas", 0);

        try
        {
            Borrow(noCopies);
        }
        catch(ItemNotAvailableException ex)
        {
            Log.Error($"Borrow refused: {ex.Message}");
        }

    }

    public static void Borrow (Book book)
    {
        //
        if(!book.Checkout())
        {
            throw new ItemNotAvailableException("Book is not available for checkout");
        }
    }
}