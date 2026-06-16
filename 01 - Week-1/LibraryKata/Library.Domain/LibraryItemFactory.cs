namespace Library.Domain;

// This factory class is static. It can ONLY contain static members. 
// What is static?, it means that the members of the class can be accessed without creating an instance of the class.
// It CAN NOT be instantiated.
// It CAN NOT be inherited.
public static class LibraryItemFactory
{
    // Our class is responsible for creating LibraryItems of any type
    // we will use that enum here to make sure users ONLY attempt to create valid types.
    public static LibraryItem Create (
        ItemKind kind,
        string title,
        string author,
        int copies = 1,
        string section = "General",
        string publisher = "N/a"
    )
    {
        
        // Thios method is going to use a switch to call the correct constructor.
        switch (kind)
        {
            case ItemKind.Book:
                return new Book(title, author, copies);
                break;

            case ItemKind.ReferenceBook:
                return new ReferenceBook(title, author, section);
                break;

            case ItemKind.Magazine:
                return new Magazine(title, author, copies, publisher);
                break;
            default:
                throw new LibraryException($"Unknown item kind: {kind}");
        }
    }
    
}