namespace Library.Domain;

public class Magazine : LibraryItem, ILendable
{
    public int CirculationCopies { get; private set; }
    public string Publisher { get; private set; }
    public Magazine(string title, string author, int circulationCopies, string publisher) : base(title, author)
    {
        CirculationCopies = circulationCopies;
        Publisher = publisher;
    }

    public override string Describe()
    {
        return $"{Title} magazine, published by {Publisher}";
    }

    // Providing implementation via new instead of override - has implications for later
    // This is technically Method Hidding - depends on the reference type
    // Calling this method in an object instantiated like this:
    // LibraryItem sportsIllustrated = new Magazine("..."); - calls LibraryItem's ShelfLabel method
    // This is most likely not what you want.
    // new vs override - very diferent behaviors
    
    public override string ShelfLabel()
    {
        return $"Magazine: {Id} - {Title} by {Publisher}";
    }

   
    public bool Checkout()
    {
        // Attempt to check out a book 
        if (CirculationCopies == 0)
            return false;
        
        // otherwise, check out the book
        CirculationCopies--;
        return true;
    }

    // providing for return behavior
    public void Return() => CirculationCopies++;


}