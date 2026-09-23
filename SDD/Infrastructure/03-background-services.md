# Capa de Infraestructura — Servicios en Segundo Plano (Background Services)

> **Documento canónico SDD** para la especificación de procesos en segundo plano y workers de ejecución periódica.

---

## 1. Justificación y Propósito
El dominio define en [SDD/Domain/06-business-rules.md](../Domain/06-business-rules.md) (Regla `[PED-01]`) y [checkout-timeout-service.md](../Domain/services/checkout-timeout-service.md) que las órdenes en checkout o con pago pendiente no confirmadas dentro del umbral de **15 minutos** deben expirar para liberar las reservas de inventario y evitar el "stock congelado".

Para orquestar este proceso sin bloquear las peticiones HTTP del usuario, la infraestructura implementa un worker recurrente mediante `Microsoft.Extensions.Hosting.BackgroundService`.

---

## 2. Implementación: `CheckoutTimeoutService`
- **Ubicación en código:** `Zentric.Infrastructure/BackgroundServices/CheckoutTimeoutService.cs`
- **Herencia:** `Microsoft.Extensions.Hosting.BackgroundService`
- **Registro en DI:** `Program.cs` (`builder.Services.AddHostedService<CheckoutTimeoutService>()`).

### 2.1 Ciclo de Vida y Resiliencia
1. **Inyección con Ámbito (Scoped):** Dado que los repositorios de EF Core (`ZentricDbContext`, `ICustomerOrderRepository`) y `IUnitOfWork` tienen tiempo de vida `Scoped`, el `BackgroundService` (que es `Singleton`) utiliza `IServiceProvider.CreateScope()` en cada iteración para resolver dependencias sin causar fugas de memoria (*memory leaks*) ni problemas de concurrencia en EF Core.
2. **Intervalo:** Se ejecuta periódicamente con `Task.Delay(TimeSpan.FromMinutes(1), stoppingToken)`.
3. **Manejo Defensivo de Errores:** Bloque `try-catch` para capturar cualquier excepción transitoria de infraestructura o red y registrarla mediante `ILogger<CheckoutTimeoutService>` sin detener el proceso principal de la API.
4. **Cancelación Limpia (*Graceful Shutdown*):** Monitorea continuamente `stoppingToken.IsCancellationRequested` para abortar limpiamente cuando el contenedor o servidor se apague.

---

## 3. Estado de Madurez
- `[CONFIRMADO]`: Registrado en el Composition Root (`Program.cs`) y compilando sin errores.
- `[PENDIENTE]`: Conectar la consulta y cancelación de pedidos expirados contra PostgreSQL una vez que se homologue el servicio de reloj (`IClock`).
