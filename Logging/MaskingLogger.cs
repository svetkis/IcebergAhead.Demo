using Serilog.Context;

namespace IcebergAhead.Demo.Logging;

//middleware для фильтрации того что логируется из запросов
public class MaskingInLogsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MaskingInLogsMiddleware> _logger;

    public MaskingInLogsMiddleware(RequestDelegate next, ILogger<MaskingInLogsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var email = context.Request.Headers["Email"].FirstOrDefault();
        var phone = context.Request.Headers["Phone"].FirstOrDefault();

        if (!string.IsNullOrEmpty(email))
        {
            var maskedEmail = MaskEmail(email);
            LogContext.PushProperty("Email", maskedEmail);
        }

        if (!string.IsNullOrEmpty(phone))
        {
            var maskedPhone = MaskPhone(phone);
            LogContext.PushProperty("Phone", maskedPhone);
        }

        await _next(context);
    }

    private string MaskEmail(string email)
    {
        try
        {
            var parts = email.Split('@');
            if (parts.Length != 2) return "***";

            var local = parts[0];
            var domain = parts[1];

            var maskedLocal = local.Length <= 2 ? "***" : local.Substring(0, 2) + "***";

            return $"{maskedLocal}@{domain}";
        }
        catch
        {
            return "***@***";
        }
    }

    private string MaskPhone(string phone)
    {
        // Пример формата: +7 926 123 45 67
        if (phone.Length < 4) return "***";

        return "+XX******" + phone[^2..]; // последние 2 символа
    }
}
