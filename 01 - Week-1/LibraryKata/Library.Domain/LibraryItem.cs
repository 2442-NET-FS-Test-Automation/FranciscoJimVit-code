namespace Library.Domain;

// Library item will be an abstract class - it cannot be instantiated.
// it will still have a constructor - because child classes NEED to be able.
// to call their parent's constructor - but we cant call it via new

public abstract class LibraryItem
{
    // Things about a book we can model - what is the "shape" of a book
    // becase i want to use a no-arg constructor, its best practice to make my properties nullable

    public string? Title { get; private set; }
    public string? Author { get; private set; }
    public int? CopiesAvailable { get; set; }

    // The same way we can have static methods (belong to the class)
    // we can have static properties/members
    private static int nextId = 1; // by convention, sytatic properties have an underscore

    public int Id { get; } // no setter, I dont want someone to reassign this

    // My abstract class DOES have a constructor
    // so far, we've dealt with public and private access modifiers.
    // public: anyone can access/ call this
    // private: only accessible within the same class
    // protected - this class and derived (child) classes only
    protected LibraryItem( string title, string author)
    {
        Id = nextId++;
        Title = title;
        Author = author;
    }

    //abstract method - only a signature - no body
    public abstract string Describe();

    // abstract classes CAN contain concrete implementation - and we can mix our abstract methods to save time later
    // potentially. Our child WILL implement Describe() - use that for the ToString()
    public override string ToString() => Describe();
    
    public virtual string ShelfLabel()
    {
        return $"[{Id}] {Title} by {Author}";
    }
}