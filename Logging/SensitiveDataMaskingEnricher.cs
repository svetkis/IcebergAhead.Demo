using Serilog.Core;
using Serilog.Events;
using System.Text.RegularExpressions;

namespace IcebergAhead.Demo.Logging;

public class SensitiveDataMaskingEnricher : ILogEventEnricher
{
    private static readonly string[] SensitiveKeys = { "email", "phone", "password", "creditcard" };

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        // перебираем все свойства события
        foreach (var key in SensitiveKeys)
        {
            var match = logEvent.Properties
                .FirstOrDefault(p => string.Equals(p.Key, key, StringComparison.OrdinalIgnoreCase));

            if (!match.Equals(default(KeyValuePair<string, LogEventPropertyValue>)))
            {
                var original = match.Value as ScalarValue;

                if (original?.Value is string str)
                {
                    var masked = Mask(str, key);
                    var maskedProp = propertyFactory.CreateProperty(match.Key, masked);
                    logEvent.AddOrUpdateProperty(maskedProp);
                }
            }
        }
    }

    private string Mask(string value, string key)
    {
        return key.ToLower() switch
        {
            "email" => MaskEmail(value),
            "phone" => MaskPhone(value),
            "password" => "passwordWasHere",
            _ => value
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