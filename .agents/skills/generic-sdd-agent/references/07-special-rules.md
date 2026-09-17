# 07. Reglas especiales

## 1. Datos y migraciones

Antes de modificar persistencia:

- Identifica datos existentes, nulabilidad, índices, restricciones y relaciones.
- Evalúa compatibilidad entre versiones de aplicación y base de datos.
- Evita migraciones destructivas de un solo paso si hay producción.
- Incluye backfill, migración gradual, dual-read/dual-write, feature flag o
  rollback cuando el riesgo lo justifique.
- Prueba migración y rollback en entorno seguro si es posible.
- Nunca borres datos, tablas o columnas por conveniencia sin autorización explícita.
- Documenta retención, privacidad y clasificación de datos cuando aplique.

## 2. APIs, eventos e integraciones

Antes de cambiar un contrato:

- Identifica consumidores conocidos y posibles consumidores externos.
- Preserva compatibilidad o versiona el contrato cuando sea necesario.
- Define payloads, códigos de error, validaciones y semántica de campos.
- Considera timeouts, reintentos, idempotencia, duplicados, orden de eventos y
  manejo de fallos parciales.
- Añade pruebas de contrato o integración.
- No cambies silenciosamente nombres, tipos o semántica de campos públicos.

## 3. Seguridad y privacidad

Para toda entrada externa:

- Valida estructura, tipo, rango y semántica.
- Autoriza la acción en el límite de aplicación adecuado.
- No confíes en datos controlados por el cliente.
- Evita exponer información sensible en errores, logs o respuestas.
- Usa consultas seguras y manejo correcto de secretos.
- Evalúa abuso, escalamiento de privilegios, fuga de datos, repetición de
  acciones, CSRF, rate limiting, inyección, carga de archivos o amenazas
  relevantes según el tipo de sistema.
- Para cambios de alto riesgo, documenta amenazas, controles y pruebas.

## 4. Observabilidad y operación

Para operaciones relevantes, procesos asíncronos, automatizaciones, integraciones
o fallos difíciles de diagnosticar:

- Registra eventos estructurados útiles.
- Incluye identificadores de correlación en flujos distribuidos.
- Evita registrar credenciales o datos personales innecesarios.
- Define señales útiles: éxito, fallo, duración, reintento, cola, estado y volumen.
- Explica qué señal permitiría detectar un fallo en producción.
- Actualiza runbooks si cambia la forma de operar, desplegar, recuperar o diagnosticar.

## 5. Arquitectura modular y dominio

Cuando el proyecto use modular monolith, DDD, Clean Architecture, Hexagonal
Architecture o una variante:

- Protege límites de módulo o bounded context.
- Evita que UI, infraestructura o framework contaminen el dominio.
- Mantén las reglas de negocio donde puedan probarse sin dependencias externas.
- Prefiere puertos y adaptadores cuando la arquitectura ya los use o el cambio lo
  justifique.
- No introduzcas patrones por moda.
- Usa entidades, value objects, agregados, eventos de dominio o CQRS solo cuando
  reduzcan complejidad o protejan invariantes reales.

## 6. Refactorización

Puedes refactorizar sin aprobación adicional solo si:

- El comportamiento externo no cambia.
- Existe evidencia de cobertura suficiente o escenarios de caracterización.
- El alcance está directamente relacionado con la tarea.
- El cambio reduce complejidad o riesgo.
- No altera contratos, esquemas, permisos, rendimiento crítico ni arquitectura.

Debes pedir aprobación si el refactor: afecta múltiples módulos o límites
funcionales · cambia contratos o interfaces públicas · modifica persistencia ·
introduce una dependencia · cambia estructura arquitectónica · implica migración
masiva · puede alterar comportamiento observable.

## 7. Concurrencia, automatización y sistemas autónomos

Para colas, workers, cron jobs, bots, simuladores, notificaciones, reintentos u
operaciones automatizadas:

- Define propiedad de la operación, estados, reintentos, límites y cancelación.
- Protege idempotencia y evita ejecuciones duplicadas.
- Maneja fallos parciales y compensación cuando aplique.
- Define timeouts, backoff y circuit breaking con dependencias externas.
- Registra auditoría de acciones relevantes.
- Separa claramente simulación, dry-run y ejecución real.
- Para operaciones con impacto financiero, datos o acciones externas, exige
  revisión humana, límites configurables y evidencia de autorización.

## 8. Contradicciones de especificación (protocolo)

Cuando dos especificaciones del mismo repositorio definen la misma regla de forma
incompatible:

1. **No elijas.** Registra ambas con cita exacta (archivo + línea).
2. Etiqueta `[CONTRADICCIÓN]` y estima el impacto concreto (qué código queda
   bloqueado y qué se rompería si eligieras mal).
3. Identifica qué fuente tiene mayor prioridad en la jerarquía de verdad
   (`00-index-and-precedence.md`) y qué evidencia falta para decidir.
4. Propón 2–3 opciones con consecuencias.
5. **Detén la implementación del área afectada** hasta obtener decisión humana.
6. Si el owner decide, registra la decisión, actualiza las especificaciones y
   solo entonces implementa.

Este protocolo aplica también cuando el **código** contradice la especificación
y cuando dos documentos del mismo número (p. ej. `02-*.md`) se solapan.