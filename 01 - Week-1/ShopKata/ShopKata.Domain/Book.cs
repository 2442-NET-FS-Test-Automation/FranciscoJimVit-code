namespace ShopKata.Domain;

public class Book : ShopProducts
{
    // Unique properties - Father hasn't these properties
    public string Author { get; private set; }
    public string Genre { get; private set; }

    // Constructor with unique and father properties
    public Book(string name, float price, int quantity, string author, string genre) 
        : base(name, price, quantity) // Sends the common data to the father.
    {
        Author = author; 
        Genre = genre;   
    }

    public override string Describe()
    {
        return $"Book: '{Name}' by {Author} (Genre: {Genre}) Copies: {AvailableQuantity}. Cost: ${Price}.";
    }
}