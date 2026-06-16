namespace ShopKata.Domain;

public abstract class ShopProducts
{
    public string Name { get; private set; }
    public float Price { get; private set; }
    public int AvailableQuantity { get; private set; }

    private static int _nextId = 1;
    public int Id { get; }

    protected ShopProducts(string name, float price, int quantity)
    {
        Id = _nextId++;
        Name = name;
        Price = price;
        AvailableQuantity = quantity;

    }

    public bool ReduceStock(int quantity)
    {
        if (quantity <= AvailableQuantity && quantity > 0)
        {
            AvailableQuantity -= quantity;
            return true; // Succesful sale
        }

        return false; // Unsuccessful sale
    }

    public void Restock(int quantity)
    {
        if (quantity > 0)
        {
            AvailableQuantity += quantity;
        }
    }

    // Overloaded Method: Same name, Diferent parameters
    public void Restock()
    {
        Restock(1); // Default: Restock 1
    }

    //abstract method - only a signature - no body
    public abstract string Describe(); // Define obligatory contract: each child shall decide how describe itself
    // abstract classes CAN contain concrete implementation - and we can mix our abstract methods to reuse code.
    public override string ToString() => Describe(); // Our child WILL implement Describe() - rewrite behavior for ToString()

    public virtual string ShelfLabel()
    {
        return $"[{Id}] {Name} costs ${Price} (Available: {AvailableQuantity})";
    }

}
