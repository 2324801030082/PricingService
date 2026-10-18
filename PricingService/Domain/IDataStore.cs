namespace PricingService.Domain;

public interface IDataStore
{
    ITariffIndex Tariffs { get; }
}

public interface ITariffIndex
{
    Task<Tariff> this[string productCode] { get; }
}
