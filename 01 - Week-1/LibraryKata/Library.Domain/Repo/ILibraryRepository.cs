namespace Library.Domain;

public interface ILibraryRepository
{
    // This interface is an abstraction over an actual repository class (concrete implementation).
    // Lets think of things we want to be able to do against our library's strore of information.

    // At minimum we probably want to provide for a basis CRUD:
    
    // - Create (Add)
    void Add(LibraryItem item); // takes in the item to be added, can 
    // - Read (Find)
    LibraryItem GetById(int id);
    List<LibraryItem> GetAll();
    // - Update
    void Update(LibraryItem item);
    // - Delete 
    bool Remove(int id);


}