namespace ShopKata.Domain;

public interface ILendable
{
    bool Checkout();
    void Return();
}