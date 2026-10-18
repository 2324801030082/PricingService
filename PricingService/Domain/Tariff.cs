namespace PricingService.Domain;

public class Tariff
{
    public string Code { get; }
    public BasePremiumRules BasePremiumRules { get; } = new();
    public DiscountMarkupRules DiscountMarkupRules { get; } = new();

    public Tariff(string code)
    {
        Code = code;
    }

    public Price CalculatePrice(Calculation calculation)
    {
        var price = new Price();
        var multiplier = DiscountMarkupRules.CalculateMultiplier(calculation.Answers);

        foreach (var coverCode in calculation.SelectedCovers)
        {
            var basePrice = BasePremiumRules.CalculateFor(coverCode, calculation.Answers);
            price.AddCoverPrice(coverCode, basePrice * multiplier);
        }

        return price;
    }
}
