using System.Linq;
using FluentValidation;
using MediatR;
using PricingService.Api.Commands;
using PricingService.Api.Commands.Dto;
using PricingService.Domain;

namespace PricingService.Commands;

public class CalculatePriceHandler : IRequestHandler<CalculatePriceCommand, CalculatePriceResult>
{
    private readonly IDataStore _dataStore;

    public CalculatePriceHandler(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CalculatePriceResult> Handle(CalculatePriceCommand command, CancellationToken cancellationToken)
    {
        // Validate
        var validator = new CalculatePriceCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Build answers dictionary
        var answers = command.Answers.ToDictionary(
            a => a.QuestionCode,
            a => a.GetAnswer()
        );

        // Build calculation object
        var calculation = new Calculation(
            command.ProductCode,
            command.PolicyFrom,
            command.PolicyTo,
            command.SelectedCovers,
            answers
        );

        // Load tariff and calculate
        var tariff = await _dataStore.Tariffs[command.ProductCode];
        var price = tariff.CalculatePrice(calculation);

        return new CalculatePriceResult
        {
            TotalPrice = price.TotalPremium,
            CoverPrices = price.Covers.ToDictionary(k => k.Key, v => v.Value.Price)
        };
    }
}
