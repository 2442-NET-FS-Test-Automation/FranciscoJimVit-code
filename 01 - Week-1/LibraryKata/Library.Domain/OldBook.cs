namespace Library.Domain;

public class OldBook
{
    // Things about a book we can model - what is the "shape" of a book
    // becase i want to use a no-arg constructor, its best practice to make my properties nullable

    public string? Title { get; private set; }
    public string? Author { get; private set; }
    public int? CopiesAvailable { get; private set; }

    // The same way we can have static methods (belong to the class)
    // we can have static properties/members
    private static int nextId = 1; // by convention, sytatic properties have an underscore

    public int Id { get; } // no setter, I dont want someone to reassign this

    // every class has a very specific method within it
    // the constructor - you can hace as many as you need/want
    // Lets make a full argument constructor

    public OldBook(string title, string author, int copiesAvailable)
    {
        Title = title;
        Author = author;
        CopiesAvailable = copiesAvailable;
        Id = nextId++; // get the value of nextId and assign it, then increment it
    }
    public OldBook() {}
    

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

    public override string ToString()
    {
        // commented out below is a call to base.ToString()
        // we can use the base keyword to refer to the paret class of the class we are working in
        // book's parent is object, so thios is calling the default ToString() method of the object class

        //return base.ToString();
        return $"Book: {Title} by {Author}, Available: {CopiesAvailable}";

    }
}
