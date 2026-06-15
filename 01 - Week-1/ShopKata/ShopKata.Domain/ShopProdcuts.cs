namespace ShopKata.Domain;

public abstract class ShopProducts
{
    public string Name { get; private set; }
    public float Price { get; private set; }
    public int AvailableQuantity { get; set; }

    private static int _nextId = 1;
    public int Id { get; }

    protected ShopProducts(string name, float price)
    {
        Id = _nextId++;
        Name = name;
        Price = price;

    }

    //abstract method - only a signature - no body
    public abstract string Describe();
    // abstract classes CAN contain concrete implementation - and we can mix our abstract methods to save time later
    // potentially. Our child WILL implement Describe() - use that for the ToString()
    public override string ToString() => Describe();

    public virtual string ShelfLabel()
    {
        return $"[{Id}] {Name} by ${Price}";
    }

}
