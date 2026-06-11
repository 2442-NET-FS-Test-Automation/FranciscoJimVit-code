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

        DataTypesAndOperators();
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

}