// For demo sake, lets write a generic
// I want to create a shelf, and a shelf can hold anything
// I don't want to be limited to LibraryItems, I can put like compurter hardware or supplies on the shelf
namespace Library.Domain;

// T is the standard placeholder for... "some type" that we will determine Later
// You will see it all over the place in documentation and code examples

public class Shelf<T>
{
    private readonly T[]_slots;
    private int used; // As things are added to my array, the shelf object tracks how slots are used internally here.

    public Shelf(int capacity)
    {
        _slots = new T[capacity];
    }

    // exposing some array properties as needed
    public int Capacity => _slots.Length;

    public int Used => used; // exposing that as a public property

    // Method tpo add items to our shelf
    public bool TryAdd(T item)
    {
        if (used == _slots.Length)
        {
            return false;
        }
        // If the shelf isn't full then ...
        // Acces the _slots array's index of the current used + 1
        // increment used
        // set that index equal to the incoming item
        _slots[used++] = item;
        return true;
    }

    public T Get(int index)
    {
        return _slots[index];
    }
    
}