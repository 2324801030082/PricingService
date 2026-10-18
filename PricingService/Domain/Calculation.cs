using System;
using System.Collections.Generic;

namespace PricingService.Domain;

public class Calculation
{
    public string ProductCode { get; }
    public DateTimeOffset PolicyFrom { get; }
    public DateTimeOffset PolicyTo { get; }
    public List<string> SelectedCovers { get; }
    public Dictionary<string, object> Answers { get; }

    public Calculation(
        string productCode,
        DateTimeOffset policyFrom,
        DateTimeOffset policyTo,
        List<string> selectedCovers,
        Dictionary<string, object> answers)
    {
        ProductCode = productCode;
        PolicyFrom = policyFrom;
        PolicyTo = policyTo;
        SelectedCovers = selectedCovers;
        Answers = answers;
    }
}
