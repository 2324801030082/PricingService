using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace PricingService.Domain;

public class DiscountMarkupRules
{
    private readonly List<DiscountMarkupRule> _rules = new();

    public void AddPercentMarkup(string condition, decimal factor)
    {
        _rules.Add(new DiscountMarkupRule { Condition = condition, Factor = factor });
    }

    public decimal CalculateMultiplier(Dictionary<string, object> answers)
    {
        decimal multiplier = 1M;

        foreach (var rule in _rules)
        {
            var interpreter = new Interpreter();
            foreach (var kv in answers)
                interpreter.SetVariable(kv.Key, kv.Value);

            var condResult = (bool)interpreter.Eval(rule.Condition);
            if (condResult)
            {
                multiplier *= rule.Factor;
            }
        }

        return multiplier;
    }
}

public class DiscountMarkupRule
{
    public string Condition { get; set; } = "";
    public decimal Factor { get; set; }
}
