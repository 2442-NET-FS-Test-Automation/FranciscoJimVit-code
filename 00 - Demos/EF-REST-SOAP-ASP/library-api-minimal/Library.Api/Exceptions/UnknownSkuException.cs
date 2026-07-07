namespace Library.Api.Fulfillment;

public sealed class unknownSkuException : Exception
{
    public string Sku {get;}

    public unknownSkuException(string sku) : base ($"Unknown SKU: {sku}")
    {
        Sku = sku;
    }
}