using Microsoft.Extensions.Options;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YamlDotNet.Core.Tokens;

namespace IcebergAhead.Demo.Logging;
public class SensitiveLoggingOptions
{
    public string[] SensitiveFields { get; set; }
}

public class SensitiveDataMaskingEnricher(string[] sensitiveFields) : ILogEventEnricher
{
    private readonly HashSet<string> maskingFieldsSet = sensitiveFields?.Select(f => f.ToLowerInvariant()).ToHashSet() ?? new HashSet<string>();

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propsFactory)
    {
        var toMasking = logEvent.Properties
            .Where(p => p.Value is ScalarValue s && s.Value is string)
            .Where(p => maskingFieldsSet.Contains(p.Key.ToLowerInvariant()))
            .Select(p => new { p.Key, Value = p.Value.ToString() })
            .ToList();


        foreach (var prop in toMasking)
        {
            var maskedProperty = propsFactory.CreateProperty(prop.Key, Mask(prop.Value, prop.Key));
            logEvent.AddOrUpdateProperty(maskedProperty);
        }
    }

    private string Mask(string value, string key)
    {
        return key.ToLower() switch
        {
            "email" => MaskEmail(value),
            "phone" => MaskPhone(value),
            "password" => "passwordWasHere",
            _ => $"{key}WasHidden"
        };
    }

    private string MaskEmail(string email)
    {
        try
        {
            var parts = email.Split('@');
            if (parts.Length != 2)
                return "***@***";

            var local = parts[0];
            var domain = parts[1];

            var maskedLocal = local.Length <= 2
                ? new string('*', local.Length)
                : local.Substring(0, 2) + new string('*', local.Length - 2);

            return $"{maskedLocal}@{domain}";
        }
        catch
        {
            return "***@***";
        }
    }

    private string MaskPhone(string phone)
    {
        // Удаляем всё, кроме цифр
        string digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length < 4)
            return "***";

        // Маскируем цифры, кроме последних двух
        string last = digits[^2..];
        return $"+{new string('*', digits.Length - 2)}{last}";
    }
}