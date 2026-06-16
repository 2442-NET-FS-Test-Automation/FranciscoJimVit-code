namespace Library.Domain;

public class ItemNotAvailableException : LibraryException
{
    public ItemNotAvailableException(string title) 
        : base($"Item '{title}' is not available to borrow.") { }
    
}
