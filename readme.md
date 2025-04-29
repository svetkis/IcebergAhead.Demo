IcebergAhead.Demo
🚢🧊 Лучшие практики устойчивости и наблюдаемости для .NET приложений

Этот проект демонстрирует, как строить отказоустойчивые, масштабируемые и легко поддерживаемые .NET сервисы, используя такие инструменты, как OpenTelemetry, Serilog, Polly, Health Checks и Feature Management.

📦 Возможности

📈 OpenTelemetry: трассировка, логирование с маскированием чувствительных данных, экспорт в Jaeger
🔁 Polly: Retry, Fallback, Circuit Breaker
📚 Dependency Injection: Transient / Scoped / Singleton сервисы
✅ Health Checks: GracefulShutdown, External APIs и причудливый Random
🧪 Feature Management с кастомным фильтром браузера (Edge / Chrome)
📄 Swagger UI с описанием API
⚙️ Dynamic Configuration c IOptionsMonitor
🔐 Защита логов: скрытие email, телефона, пароля и др.
🔎 HttpClientFactory с resiliency-политиками разного уровня


📂 Структура проекта

IcebergAhead.Demo/ 
│ ├── Controllers/ - REST API контроллеры 
├── Services/ - DI сервисы + DynamicService c live-настройкой
├── FeatureFilters/ - Feature Management фильтры
├── HealthChecks/ - Расширенные Health Checks 
├── Logging/ - Middleware и Enricher для маскировки данных из логов 
├── Settings/ - POCO-классы для конфигурации 
├── OpenTelemetryDIExtension.cs - Конфигурация Telemetry 
├── PollyRegistryDIExtension.cs - Polly registry и resiliency-политики 
├── Program.cs - Точка входа, настройки DI, API, Health и т.д.

🚀 Быстрый старт

Клонируй репозиторий:
git clone https://github.com/yourorg/IcebergAhead.Demo.git cd IcebergAhead.Demo

Запусти проект:
dotnet run


🧪 API Примеры
/api/diagnostics/trace-log — пробный лог с TraceId и SpanId и проверкой маскировки данных
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
Интерфейс HealthChecks UI поддерживается

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
ExternalAPI

📐 Жизненные циклы DI
Singleton: создается один раз
Scoped: один на запрос
Transient: новый экземпляр каждый раз
DILiveCirclesController демонстрирует разницу в поведение сервисов с разным жизненным циклом

📎 Требования
.NET SDK 9

Jaeger (опционально, если нужно трассировка)

🧑💻 Контакты

Проект подготовлен для демонстрации по итогам доклада "Айсберг на горизонте. Готовим приложение к продакшен"

Автор: Светлана Мелешкина
Email: meleshkina_si@nlmk.com

📄 Лицензия MIT

Pull Requests приветствуются!
