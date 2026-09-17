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
- Existe una spec funcional de negocio sólida (`SDD/Domain/ZENTRIC.md`) y una spec
  de dominio detallada, pero con **solapes de numeración y contradicciones**.
- Solo existe el proyecto `Zentric.Domain`, con 6 artefactos implementados, 1
  stub vacío y **0 pruebas**.
- `AGENTS.md` referencia dos documentos de `/SDD` que no existen.

## Documentos de este bootstrap

| Documento | Contenido |
|---|---|
| `repository-map.md` | Inventario de archivos, proyectos y estructura |
| `current-state.md` | Comportamiento y arquitectura observados |
| `spec-conformance-matrix.md` | Spec ↔ código, con evidencia citada |
| `verification-baseline.md` | Comandos ejecutados y resultados |
| `risks-and-gaps.md` | Riesgos, hallazgos y deuda priorizados |
| `questions-for-owner.md` | Decisiones que el código no puede responder |
| `migration-to-sdd-plan.md` | Roadmap de adopción y siguiente vertical |
| `SDD/Adr/` | Decisiones arquitectónicas duraderas (ADR-0001 reserva, ADR-0002 clave de stock, ADR-0003 variante obligatoria) |
| `.agents/skills/generic-sdd-agent/` | Metodología operativa (skill v3.0.0: router `SKILL.md` + `references/` + `scripts/sync-skill.ps1`) · ver `AGENTS.md` §0.0 |

> `generic-sdd-agent.md` (raíz, v2.0.0) está **superseded**: no editarlo ni citarlo
> como vigente (ver nota al inicio de ese archivo).

## Regla de lectura

Todo lo `[CONFIRMADO]` proviene de inspección directa o de un comando ejecutado.
Todo lo `[INFERIDO]` requiere confirmación antes de convertirse en regla.
Ningún hallazgo de este bootstrap modifica código: es solo lectura.

## Decisiones registradas

| Fecha | Decisión | Documento |
|---|---|---|
| 2026-09-17 | Q-01 / C-01: reserva con **bodega única** y **fraccionamiento solo de contingencia** (umbral: ninguna bodega individual cubre la cantidad) | `SDD/Adr/0001-reserva-fragmentacion-contingencia.md` |
| 2026-09-17 | Q-02 / C-02: la clave del inventario es **`VariantId` (SKU)**; `ProductVariant` como entidad hija de `Product` | `SDD/Adr/0002-clave-inventario-variantid.md` |
| 2026-09-17 | Q-10: la **variante es obligatoria solo en productos `Physical`** (C3); los `Digital` pueden no tener | `SDD/Adr/0003-variante-obligatoria-productos-fisicos.md` |
| 2026-09-17 | Adopción de la skill `generic-sdd-agent` v3.0.0 como metodología operativa | `AGENTS.md` §0.0 · `.agents/skills/generic-sdd-agent/` |

## Trabajo ejecutado

| Tarea | Estado | Evidencia |
|---|---|---|
| T-001 proyecto `Zentric.Tests` (xUnit) | `[x]` | `verification-baseline.md` §7 |
| T-002 pruebas de caracterización (105 → **164** tras T-002c + T-010c) | `[x]` | `Passed! - Failed: 0, Passed: 164` |
| T-003a corrección H-08 (violación de INV-01) | `[x]` | rojo `Failed: 3` → verde `Failed: 0` |
| T-010 `ProductVariant` + `VariantAttribute` (ADR-0002) | `[x]` | `verification-baseline.md` §8 |
| T-004a `Inventory.ProductId` → `Inventory.VariantId` | `[x]` | `verification-baseline.md` §8 |
| T-010c variante obligatoria en físicos (ADR-0003, Q-10 = C3) | `[x]` | `verification-baseline.md` §9 |