namespace Library.Domain;

public class Book : LibraryItem, ILendable
{
    // Child class constructors look a little different
    // We take in all our arguments for the parent + child, then call base with a colon
    public Book(string title, string author, int copiesAvailable) : base(title, author)
    {
        CopiesAvailable = copiesAvailable;
    }

    public override string Describe()
    {
        return $"{Id}: {Title} by {Author} has {CopiesAvailable} copies available for checkout";
    }

    // our first instance method - no "static" kayword
    // just an acces modifier + return type + any arguments if any
    public bool Checkout()
    {
        // Attempt to check out a book 
        if (CopiesAvailable == 0)
            return false;
        
        // otherwise, check out the book
        CopiesAvailable--;
        return true;
    }

    // providing for return behavior
    public void Return() => CopiesAvailable++;
}
