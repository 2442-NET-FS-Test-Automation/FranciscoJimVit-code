// Single Responsibility Principle: This class has only one reason to change - to modify the exception handling.
// Open/Closed Principle: This class is open for extension but closed for modification.
namespace Library.Domain;

// An exception is any class that inherits from the base Exception class.
public class LibraryException : Exception
{
    // The base class just contains a message.
    public LibraryException(string message) : base(message) { }

    
}
