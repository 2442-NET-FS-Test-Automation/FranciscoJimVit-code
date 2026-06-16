namespace Library.Domain;
public class Catalog
{
    // Backing out catalog is going to be a list.
    // List<T>: ordered, grow/shrink dynamically, accesible via index.
    // Your default collection - even above Arrays.
    public readonly List<LibraryItem> _items = new();

    // This method is technically redundant - this class basically just wraps the List
    // BUT if we wanted to restrict people from Adding or Removing or even accessing via index
    // from other places in the code, we could wrap not only the list, but its instance methods
    // with our own wrapper methods and make them internal, private, protected, etc.
    public int Count => _items.Count();

    // Stack<T>: LIFO (Last In, First Out) - modeling a hold queue, customers placing holds on books
    // is re-shelved first.
    // primary methods - push(): puts an item at the top of the stack. pop() - removes the top item.
    public readonly Stack<LibraryItem> _returnCart = new();

    // Queue<T>: FIFO (First In, First Out) - modeling a hold queue, customers placing holds on books
    // primary methods - Enqueue(): adds an item to the end of the queue. Dequeue() - removes from the front of the line.
    public readonly Queue<LibraryItem> _holdQueue = new();

    // Reading list
    // LinkedList<T>: cheap inserts/removals anywhere in my list, but NO index access.
    public readonly LinkedList<LibraryItem> _readingList = new();




    public void Add(LibraryItem item) => _items.Add(item);

    


}