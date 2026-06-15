namespace ShopKata.Domain;

public class ShopProducts
{
    public string Name { get; set; }
    public float Price { get; set; }
    public int AvailableQuantity { get; set; }


    public ShopProducts(string name, float price, int availableQuantity)
    {
        Name = name;
        Price = price;
        AvailableQuantity = availableQuantity;

    }
}
