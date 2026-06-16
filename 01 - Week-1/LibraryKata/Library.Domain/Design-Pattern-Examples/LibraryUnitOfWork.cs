using Serilog;

namespace Library.Domain;

public class LibraryUnitOfWork : IUnitOfWork
{
    // This property is mandtory because its in my interface
    public ILibraryRepository Items { get; }

    // I want something to hold my list of staged changes
    // we will represent those as strings, this is a shallow demo example
    private readonly List<string> _staged = new();

    // we need a constructor
    // we are technically using dependency injection here. 
    // We never instantiate the ILibraryRepository object directly, we ask for an existing one.
    public LibraryUnitOfWork(ILibraryRepository items)
    {
        Items = items;
    }

    public int Commit()
    {
        // shallow commit implementation
        // We will just log how many things were sataged + commited
        int count = _staged.Count;

        // log the count via serilog
        Log.Information($"Committed {count} change(s)", count);

        // Once you're done doing whatever work you needed to do, clear the stagging area
        // same logic as git
        _staged.Clear();

        return count;
    }

    public void Stage(string change)
    {
        _staged.Add(change); // staging a change
    }
}