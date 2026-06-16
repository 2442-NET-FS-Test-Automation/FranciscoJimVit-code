namespace ShopKata.Domain;

public class Fruit : ShopProducts
{
    public DateTime ExpiryDate { get; private set; }

    public Fruit(string name, float price, int quantity, DateTime expiryDate) 
        : base(name, price, quantity) // Sends the common data to the father.
    {
        ExpiryDate = expiryDate;
    }

    public override string Describe()
    {
        return $"This is a {Name} and it costs ${Price}. It expires on {ExpiryDate:yyyy-MM-dd}.";
    }

}