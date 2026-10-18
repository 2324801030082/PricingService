using Microsoft.EntityFrameworkCore;
using PricingService.Domain;

namespace PricingService.Data;

public class DbDataStore : IDataStore
{
    private readonly PricingDbContext _context;

    public DbDataStore(PricingDbContext context)
    {
        _context = context;
        Tariffs = new TariffIndex(context);
    }

    public ITariffIndex Tariffs { get; }
}

public class TariffIndex : ITariffIndex
{
    private readonly PricingDbContext _context;

    public TariffIndex(PricingDbContext context)
    {
        _context = context;
    }

    public Task<Tariff> this[string productCode]
    {
        get => GetTariff(productCode);
    }

    private async Task<Tariff> GetTariff(string productCode)
    {
        var entity = await _context.TariffEntities
            .Include(t => t.BasePriceRules)
            .Include(t => t.DiscountMarkupRules)
            .FirstOrDefaultAsync(t => t.Code == productCode)
            ?? throw new InvalidOperationException($"Tariff '{productCode}' not found.");

        var tariff = new Tariff(entity.Code);

        foreach (var r in entity.BasePriceRules)
            tariff.BasePremiumRules.AddBasePriceRule(r.CoverCode, r.Condition, r.Expression);

        foreach (var r in entity.DiscountMarkupRules)
            tariff.DiscountMarkupRules.AddPercentMarkup(r.Condition, r.Factor);

        return tariff;
    }
}
