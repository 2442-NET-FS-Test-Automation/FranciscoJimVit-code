// This class will be our actual Library Catalog store of info

using Serilog;

namespace Library.Domain;

public class InMemoryLibraryRepository : ILibraryRepository
{
    
    // Because we don't have an outside store of info (like a SQL database)
    // we are kind of forced to rely on a list. We will store info outside of program Later.
    
    private readonly List<LibraryItem> _items = new();

    public void Add(LibraryItem item)
    {
        _items.Add(item);
        // We just added a new item - thats a significant event, let's log it.
        // Notice not using string interpolation - this uses Serilog's template
        //string format
        Log.Information("Item Added: {ItemName} - id {id}", item.Title, item.Id);
    }

    public List<LibraryItem> GetAll()
    {
        // Don't want to accidentally pass a pointer to my real list
        // return a new copy of the list
        return _items.ToList();
    }

    public LibraryItem GetById(int id)
    {
        // In order to find an item in our collection with the given Id
        // we need to search for it. we could use something like LINQ,
        // but that's is own lession today.
        foreach (LibraryItem item in _items)
        {
            if (item.Id == id)
            {
                return item;
            }
        }
        
        //if we make it here - We exited the foreach loop without finding an item for that id
        Log.Warning("Item not found: {id}", id);
        throw new ItemNotFoundException(id); // throw our custom exception

    }

    public void Update(LibraryItem item)
    {
        throw new NotImplementedException();
    }
    
    public bool Remove(int id)
    {

        foreach (LibraryItem item in _items)
        {
            if (item.Id == id)
            {
                _items.Remove(item);
                Log.Information("Item Removed: {ItemName} - id {id}", item.Title, item.Id); // log the removal
                return true;
            }
        }
        Log.Information("Item not found for removal: {id}", id);
        return false;
    }

}