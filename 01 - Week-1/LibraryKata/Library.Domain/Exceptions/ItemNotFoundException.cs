namespace Library.Domain;

public class ItemNotFoundException : LibraryException
{
    // We van hold the offending Id that triggered the exception.
    // We will use this for logging later.
    public int Id { get; }
    public ItemNotFoundException(int id) 
        : base($"No library item with id {id}")
    {
        Id = id;
    }
    
}
