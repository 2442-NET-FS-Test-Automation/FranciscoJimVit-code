namespace Library.Domain;

// interface in C# - they are a contract for behavior - they do not define the implementation of the methods with within

public interface ILendable
{
    // only method signatures, no bodies, not even
    bool Checkout();
    void Return();
}