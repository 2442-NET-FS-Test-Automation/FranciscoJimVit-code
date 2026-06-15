namespace ShopKata.Domain;

public class Fruit : ShopProducts, ILendable
{
    public Fruit(string name, float price) : base(name, price)
    {
    }

    public override string Describe()
    {
        return $"This is a {Name} and it costs ${Price}.";
    }

    public bool Checkout()
    {
        if (AvailableQuantity > 0)
        {
            AvailableQuantity--;
            return true;
        }
        return false;
    }

    public void Return() => AvailableQuantity++;
}