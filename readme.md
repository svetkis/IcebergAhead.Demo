IcebergAhead.Demo 🚢🧊 Лучшие практики устойчивости и наблюдаемости для .NET приложений

Этот проект демонстрирует, как строить отказоустойчивые, масштабируемые и легко поддерживаемые .NET веб-сервисы, используя такие инструменты, как OpenTelemetry, Serilog, Polly, Health Checks и Feature Management.

📦 Возможности

📈 OpenTelemetry: трассировка, логирование с маскированием чувствительных данных, экспорт в Jaeger/Console
🔁 Polly: Retry, Fallback, Circuit Breaker
📚 Dependency Injection: Transient / Scoped / Singleton сервисы
✅ Health Checks: GracefulShutdown, External APIs и причудливый Random
🧪 Feature Management с кастомным фильтром браузера (Edge / Chrome)
📄 Swagger UI с описанием API
⚙️ Dynamic Configuration c IOptionsMonitor
🔐 Защита логов: скрытие email, телефона, пароля и др.
🔎 HttpClientFactory с resiliency-политиками разного уровня
📂 Структура проекта

IcebergAhead.Demo/ │ ├── Controllers/ - REST API контроллеры ├── Services/ - DI сервисы + DynamicService c live-настройкой ├── FeatureFilters/ - Feature Management фильтры ├── HealthChecks/ - Расширенные Health Checks ├── Logging/ - Middleware и Enricher для маскировки данных из логов ├── Settings/ - POCO-классы для конфигурации ├── OpenTelemetryDIExtension.cs - Конфигурация Telemetry ├── PollyRegistryDIExtension.cs - Polly registry и resiliency-политики ├── Program.cs - Точка входа, настройки DI, API, Health и т.д.

🚀 Быстрый старт

Клонируй репозиторий:
git clone https://github.com/yourorg/IcebergAhead.Demo.git cd IcebergAhead.Demo

Настрой appsettings.json:
{ "IcebergAheadSettings": { "ShipName": "Titanic", "ResilienceLevel": 5 }, "DynamicSettings": { "FeatureEnabled": true, "MaxItems": 100 }, "HttpClients": { "ExternalAPI": { "BaseAddress": "https://httpbin.org", "TimeoutSeconds": 5, "UserAgent": "DemoApp" } }, "Serilog": { "MinimumLevel": "Information", ... } }

Запусти проект:
dotnet run

Открой Swagger:
http://localhost:5000/swagger

🧪 API Примеры
/api/diagnostics/ip — текущий IP
/api/diagnostics/trace-log — пробный лог с TraceId и SpanId
/api/dilivecircles — Показ GUID разных жизненных циклов
/api/fault-tolerance/fallback — пример работы fallback через Polly
/api/feature-flags/new-feature — фича-флаг (Feature Management)
/healthz — комбинированный Health Check
/api/dynamicconfig — Live-конфигурация из IOptionsMonitor

📊 Наблюдаемость и логирование
Используется Serilog + OpenTelemetry Logging
Маскирование секретов в логах (email, phone, password) с помощью SensitiveDataMaskingEnricher
Поддержка TraceId + SpanId в логах
Поддержка Jaeger Exporter (раскомментируй в OpenTelemetryDIExtension.cs)

📉 Health Checks
Встроенные и кастомные проверки:
GracefulShutdown
ExternalAPI (через типизированный HttpClient)
Random OK (для демонстрации)
Интерфейс HealthChecks UI поддерживается (раскомментируй в Program.cs)

🧠 Feature Management
Использует Microsoft.FeatureManagement
Поддержка кастомных фильтров (по User-Agent)
Примеры в FeatureFlagsController

💻 HttpClient + Polly
Регистрируются политики:
RetryWithExponentialBackoff
CircuitBreakerPolicy
FallbackPolicy
HttpClient с именем:
HttpBin
WithFallback
ExternalAPI (strongly typed)

📐 Жизненные циклы DI
Singleton: создается один раз
Scoped: один на запрос
Transient: новый экземпляр каждый раз
DILiveCirclesController демонстрирует GUID-сравнение между Injected Services и IServiceProvider

📎 Требования
.NET SDK 9
Jaeger (опционально, если нужно трассировка)
Docker (если захочешь поднять инфраструктуру)


🧪 Тестирование фич вручную:
curl http://localhost:5000/api/dynamicconfig curl http://localhost:5000/api/feature-flags/new-feature

🧑💻 Контакты

Проект подготовлен для демонстрации best practices:

Автор: Светлана Мелешкина
Email: meleshkina_si@nlmk.com

📄 Лицензия MIT

🏁 Заключение

Это демонстрационное приложение служит шаблоном для построения устойчивых, наблюдаемых, безопасных .NET приложений с богатой интеграцией с возможностями платформы.

Pull Requests приветствуются!