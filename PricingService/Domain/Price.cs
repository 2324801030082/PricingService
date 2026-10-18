using System.Collections.Generic;
using System.Linq;

namespace PricingService.Domain;

public class Price
{
    public decimal TotalPremium { get; private set; }
    public Dictionary<string, CoverPrice> Covers { get; } = new();

    public void AddCoverPrice(string coverCode, decimal price)
    {
        Covers[coverCode] = new CoverPrice { Code = coverCode, Price = price };
        TotalPremium = Covers.Values.Sum(c => c.Price);
    }
}

public class CoverPrice
{
    public string Code { get; set; } = "";
    public decimal Price { get; set; }
}
