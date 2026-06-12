namespace Library.Domain;

public class ReferenceBook : LibraryItem, ILendable
{

    public string Section { get; }

    public ReferenceBook(string title, string author, string section) 
        : base(title, author)
    {
        Section = section;
    }

    public override string Describe()
    {
        // throw new NotImplementedException();
        
        return $"{Id}: {Title} by {Author} is in the {Section} section";
    }

    // overriding shelfLabel() - this is a "true" override
    public override string ShelfLabel()
    {
        return $"Reference: {Id} - {Section}";
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