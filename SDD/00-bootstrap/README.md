# Bootstrap — Adopción SDD en zentric-backend

```yaml
id: 000
title: Bootstrap brownfield zentric-backend
type: discovery
status: in-progress
risk: medium
created: 2026-09-17
updated: 2026-09-17
owners: []
related_adrs: []
```

## Propósito

Establecer una **línea base verificable** del repositorio antes de continuar la
implementación, para poder cambiar el sistema con seguridad y adoptar SDD de
forma progresiva (Modo F del agente SDD).

## Contexto detectado (resumen)

- Modo de operación: **F (Brownfield sin contexto confiable)** + **A (arranque)**.
- Existe una spec funcional de negocio sólida ([SDD/Domain/ZENTRIC.md](../Domain/ZENTRIC.md)) y una spec
  de dominio detallada, pero con **solapes de numeración y contradicciones**.
- `[OBSOLETO — 2026-09-18]` ~~Solo existe el proyecto `Zentric.Domain`, con 6 artefactos implementados, 1 stub vacío y **0 pruebas**~~ → hoy la solución tiene **5 proyectos** (Domain, Application, Infrastructure, Api, Tests) con **178 pruebas** en verde, pero las capas exteriores y los dominios Orders/Logistics/Billing/Returns se implementaron **sin registrar** y con decisiones no dictadas por el Owner.
- `[OBSOLETO — 2026-09-18]` ~~[AGENTS.md](../../AGENTS.md) referencia dos documentos de `/SDD` que no existen~~ → existen [SDD/01-system-overview.md](../01-system-overview.md), [SDD/02-software-architecture.md](../02-software-architecture.md) y las carpetas `SDD/Application/`, `SDD/Infrastructure/` y `SDD/Presentation/`.
- `[RIESGO]` **Nuevo bloqueante:** la Ley define `DOMINIO 8/9/10` dos veces con contenidos cruzados → **[Q-13](questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)** ([questions-for-owner.md](questions-for-owner.md)).

## Documentos de este bootstrap

| Documento | Contenido |
|---|---|
| [SDD.md](../SDD.md) | **NUEVO (2026-09-18)** memoria viva: mapa de entidades, SPEC-000…006, ADDENDA, verificación, riesgos y estado |
| [repository-map.md](repository-map.md) | Inventario de archivos, proyectos y estructura |
| [current-state.md](current-state.md) | Comportamiento y arquitectura observados (+ [:6](current-state.md#6-estado-observado-el-2026-09-18-tanda-no-registrada) estado del 2026-09-18) |
| [spec-conformance-matrix.md](spec-conformance-matrix.md) | Spec ↔ código, con evidencia citada (reconteo 2026-09-18) |
| [verification-baseline.md](verification-baseline.md) | Comandos ejecutados y resultados ([:10](verification-baseline.md#10-quinta-iteracion-auditoria-de-la-tanda-no-registrada-2026-09-18): quinta iteración) |
| [risks-and-gaps.md](risks-and-gaps.md) | Riesgos, hallazgos y deuda priorizados ([C-08](risks-and-gaps.md)…[C-10](risks-and-gaps.md), [R-11](risks-and-gaps.md)…[R-16](risks-and-gaps.md)) |
| [questions-for-owner.md](questions-for-owner.md) | Decisiones que el código no puede responder (**[Q-13](questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) abierta y bloqueante**) |
| [migration-to-sdd-plan.md](migration-to-sdd-plan.md) | Roadmap de adopción y siguiente vertical (Fases 3b/4/5 auditadas) |
| `SDD/Adr/` | Decisiones arquitectónicas duraderas ([ADR-0001](../Adr/0001-reserva-fragmentacion-contingencia.md) reserva, [ADR-0002](../Adr/0002-clave-inventario-variantid.md) clave de stock, [ADR-0003](../Adr/0003-variante-obligatoria-productos-fisicos.md) variante obligatoria) |
| `SDD/Application/`, `SDD/Infrastructure/`, `SDD/Presentation/` | Specs de las capas exteriores creadas durante la implementación no registrada |
| `.agents/skills/generic-sdd-agent/` | Metodología operativa (skill **v6.0.0**: `SKILL.md` monolítico + `scripts/sync-skill.ps1`) · ver [AGENTS.md :0.0](../../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) |

> **ELIMINADO el 2026-09-18.** El prompt monolítico de la raíz (`generic-sdd-agent.md`, v2.0.0, superseded) ya no existe: el Owner autorizó su eliminación y el propio archivo la pedía.
> Punto de entrada metodológico vigente y único: [AGENTS.md :0.0](../../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) y `.agents/skills/generic-sdd-agent/SKILL.md` (**v6.0.0**). El histórico permanece en git (`git checkout -- generic-sdd-agent.md`).

## Regla de lectura

Todo lo `[CONFIRMADO]` proviene de inspección directa o de un comando ejecutado.
Todo lo `[INFERIDO]` requiere confirmación antes de convertirse en regla.
Ningún hallazgo de este bootstrap modifica código: es solo lectura.

## Decisiones registradas

| Fecha | Decisión | Documento |
|---|---|---|
| 2026-09-17 | [Q-01](questions-for-owner.md#q-01-c-01-se-permite-fraccionar-la-reserva-entre-bodegas-resuelta-2026-09-17) / [C-01](risks-and-gaps.md): reserva con **bodega única** y **fraccionamiento solo de contingencia** (umbral: ninguna bodega individual cubre la cantidad) | [SDD/Adr/0001-reserva-fragmentacion-contingencia.md](../Adr/0001-reserva-fragmentacion-contingencia.md) |
| 2026-09-17 | [Q-02](questions-for-owner.md#q-02-c-02-la-clave-del-inventario-es-el-producto-o-la-variante-sku-resuelta-2026-09-17) / [C-02](risks-and-gaps.md): la clave del inventario es **`VariantId` (SKU)**; `ProductVariant` como entidad hija de `Product` | [SDD/Adr/0002-clave-inventario-variantid.md](../Adr/0002-clave-inventario-variantid.md) |
| 2026-09-17 | [Q-10](questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3): la **variante es obligatoria solo en productos `Physical`** (C3); los `Digital` pueden no tener | [SDD/Adr/0003-variante-obligatoria-productos-fisicos.md](../Adr/0003-variante-obligatoria-productos-fisicos.md) |
| 2026-09-17 | Adopción de la skill `generic-sdd-agent` v3.0.0 como metodología operativa | [AGENTS.md :0.0](../../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) · `.agents/skills/generic-sdd-agent/` |
| 2026-09-18 | Actualización de la skill a **v6.0.0** (documento único), creación de [SDD/SDD.md](../SDD.md) y sincronización de la copia instalada (opción "trazabilidad primero" del Owner) | [AGENTS.md :0.0](../../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) · [SDD/SDD.md](../SDD.md) · [verification-baseline.md :10](verification-baseline.md#10-quinta-iteracion-auditoria-de-la-tanda-no-registrada-2026-09-18) |
| 2026-09-18 | Registro de la **auditoría de la Fase 3b/4** (solo lectura, cero código) y de la contradicción **[C-08](risks-and-gaps.md)** como **[Q-13](questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) bloqueante** | [SDD/SDD.md :6](../SDD.md#6-riesgos-contradicciones-y-preguntas)-[:7](../SDD.md#7-estado-y-proximos-pasos) · [risks-and-gaps.md :1](risks-and-gaps.md#1-contradicciones-de-especificacion-bloqueantes)-[:3](risks-and-gaps.md#3-vacios-funcionales-gaps-de-implementacion) |

## Trabajo ejecutado

| Tarea | Estado | Evidencia |
|---|---|---|
| [T-001](migration-to-sdd-plan.md) proyecto `Zentric.Tests` (xUnit) | `[x]` | [verification-baseline.md :7](verification-baseline.md#7-segunda-iteracion-suite-de-pruebas-y-correccion-t-003) |
| [T-002](migration-to-sdd-plan.md) pruebas de caracterización (105 → **164** tras [T-002c](migration-to-sdd-plan.md) + [T-010c](migration-to-sdd-plan.md)) | `[x]` | `Passed! - Failed: 0, Passed: 164` |
| [T-003a](migration-to-sdd-plan.md) corrección [H-08](spec-conformance-matrix.md) (violación de [INV-01](../Domain/06-business-rules.md)) | `[x]` | rojo `Failed: 3` → verde `Failed: 0` |
| [T-010](migration-to-sdd-plan.md) `ProductVariant` + `VariantAttribute` ([ADR-0002](../Adr/0002-clave-inventario-variantid.md)) | `[x]` | [verification-baseline.md :8](verification-baseline.md#8-tercera-iteracion-adr-0002-clave-del-inventario-variantid) |
| [T-004a](migration-to-sdd-plan.md) `Inventory.ProductId` → `Inventory.VariantId` | `[x]` | [verification-baseline.md :8](verification-baseline.md#8-tercera-iteracion-adr-0002-clave-del-inventario-variantid) |
| [T-010c](migration-to-sdd-plan.md) variante obligatoria en físicos ([ADR-0003](../Adr/0003-variante-obligatoria-productos-fisicos.md), [Q-10](questions-for-owner.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) = [C3](../Adr/0003-variante-obligatoria-productos-fisicos.md)) | `[x]` | [verification-baseline.md :9](verification-baseline.md#9-cuarta-iteracion-adr-0003-variante-obligatoria-en-fisicos-q-10-c3) |
| [T-023](migration-to-sdd-plan.md)/SPEC-006 memoria viva [SDD/SDD.md](../SDD.md) | `[x]` | [:1](../SDD.md#1-contexto-y-alcance)–[:8](../SDD.md#8-investigacion-y-discovery) completas · mapa 30/33 entidades |
| [T-024](migration-to-sdd-plan.md) sincronización de la skill + corrección de `sync-skill.ps1` | `[x]` | [verification-baseline.md :10](verification-baseline.md#10-quinta-iteracion-auditoria-de-la-tanda-no-registrada-2026-09-18) (`Get-FileHash` origen = destino) |
| [T-025](migration-to-sdd-plan.md) [AGENTS.md :0.0](../../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) alineado con la skill v6.0.0 | `[x]` | este README y [AGENTS.md :0.0](../../AGENTS.md#00-skill-de-ingenieria-sdd-obligatoria) |
| [T-026](migration-to-sdd-plan.md) registro de la tanda no registrada ([T-011](migration-to-sdd-plan.md)…[T-022](migration-to-sdd-plan.md)) y de **[Q-13](questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante)** | `[x]` | [migration-to-sdd-plan.md](migration-to-sdd-plan.md) Fases 3b/4/5 · [questions-for-owner.md](questions-for-owner.md) [Q-13](questions-for-owner.md#q-13-c-08-la-ley-define-dominio-8-9-y-10-dos-veces-con-significados-cruzados-abierta-bloqueante) |
| Auditoría de la Fase 3b/4 (solo lectura, 2026-09-18) | `[x]` | [current-state.md :6](current-state.md#6-estado-observado-el-2026-09-18-tanda-no-registrada) · [spec-conformance-matrix.md :5](spec-conformance-matrix.md#5-ordering)–[:8](spec-conformance-matrix.md#8-totales) · [risks-and-gaps.md :2](risks-and-gaps.md#2-riesgos-tecnicos) |
| SPEC-007/[T-027](migration-to-sdd-plan.md)…[T-031](migration-to-sdd-plan.md) validación FluentValidation + RFC 7807 + higiene (cierra [H-09](spec-conformance-matrix.md), [H-11](spec-conformance-matrix.md), [H-12](spec-conformance-matrix.md)) | `[x]` | [verification-baseline.md :11](verification-baseline.md#11-sexta-iteracion-spec-007-validacion-de-entrada-rfc-7807-e-higiene-2026-09-18): build 0/0 · **206/206 PASS** |
| [T-032](migration-to-sdd-plan.md) pruebas HTTP con TestServer | `[ ]` | pendiente: la API no se levantó en este entorno |
| [Q-14](questions-for-owner.md#q-14-licenciamiento-de-mediatr-14-y-fluentvalidation-abierta-no-bloquea-el-codigo-actual) licenciamiento de MediatR 14 | `[ ]` | pendiente de dictamen del Owner ([questions-for-owner.md](questions-for-owner.md)) |
