using System;
using System.Collections.Generic;
using System.Linq;
using DynamicExpresso;

namespace PricingService.Domain;

public class BasePremiumRules
{
    private readonly List<BasePriceRule> _rules = new();

    public void AddBasePriceRule(string coverCode, string? condition, string expression)
    {
        _rules.Add(new BasePriceRule
        {
            CoverCode = coverCode,
            Condition = condition,
            Expression = expression
        });
    }

    public decimal CalculateFor(string coverCode, Dictionary<string, object> answers)
    {
        var applicableRules = _rules.Where(r => r.CoverCode == coverCode);

        foreach (var rule in applicableRules)
        {
            if (rule.Condition == null)
            {
                return EvalDecimal(rule.Expression, answers);
            }

            var condResult = EvalBool(rule.Condition, answers);
            if (condResult)
            {
                return EvalDecimal(rule.Expression, answers);
            }
        }

        return 0M;
    }

    public IEnumerable<string> GetUniqueCoverCodes() =>
        _rules.Select(r => r.CoverCode).Distinct();

    private static decimal EvalDecimal(string expression, Dictionary<string, object> answers)
    {
        var interpreter = new Interpreter();
        foreach (var kv in answers)
            interpreter.SetVariable(kv.Key, kv.Value);
        var result = interpreter.Eval(expression);
        return Convert.ToDecimal(result);
    }

    private static bool EvalBool(string condition, Dictionary<string, object> answers)
    {
        var interpreter = new Interpreter();
        foreach (var kv in answers)
            interpreter.SetVariable(kv.Key, kv.Value);
        return (bool)interpreter.Eval(condition);
    }
}

public class BasePriceRule
{
    public string CoverCode { get; set; } = "";
    public string? Condition { get; set; }
    public string Expression { get; set; } = "";
}
