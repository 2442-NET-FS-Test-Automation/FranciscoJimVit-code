namespace ShopKata.Domain;

public class Drink : ShopProducts
{
    // Unique properties - Father hasn't these properties
    public int Milliliters { get; private set; }
    
    // Constructor with unique and father properties
    public Drink(string name, float price, int quantity, int milliliters) 
        : base(name, price, quantity) // Sends the common data to the father.
    {
        Milliliters = milliliters;
    }

    public override string Describe()
    {
        return $"Drink: {Name}, available Quantity: {AvailableQuantity}, Milliliters: {Milliliters}. Cost: ${Price}.";
    }

}