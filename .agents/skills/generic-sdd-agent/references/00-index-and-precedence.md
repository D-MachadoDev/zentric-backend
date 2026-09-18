# 00. Índice, jerarquía de verdad y gobernanza documental

> Núcleo agnóstico de stack y organización. Todo lo específico de un lenguaje,
> framework, empresa o repositorio vive en el **overlay** (`references/10-*-overlay.md`
> o `OVERLAY.md` en la raíz del repo), nunca aquí. Si un ejemplo menciona una
> tecnología, es ilustrativo. La plantilla para crear el overlay de cualquier
> repositorio nuevo está en `references/10b-overlay-template.md`; los perfiles
> por tipo de organización (personal, startup/pyme, enterprise, regulado) están
> en `references/11-context-profiles.md`.

## 1. Índice de referencias

| Archivo | Contenido |
|---|---|
| `01-evidence-model.md` | Etiquetas de evidencia + principios no negociables completos |
| `02-modes-and-rigor.md` | Modos A–H y niveles de rigor 1–3 |
| `03-artifacts.md` | Constitución, requisitos, diseño, tareas, verificación, ADR |
| `04-brownfield-bootstrap.md` | Bootstrap B0–B5 y niveles de extracción |
| `05-gates.md` | Gates 0–4 y condiciones de parada |
| `06-workflow-phases.md` | Fases 0–9 del ciclo operativo |
| `07-special-rules.md` | Datos, contratos, seguridad, ops, arquitectura, refactor, automatización |
| `08-communication.md` | Formatos y frases de comunicación |
| `09-templates.md` | Plantillas copiables |
| `10-zentric-overlay.md` | Overlay de ejemplo: zentric-backend (.NET, este repo) — **no copiar a otros proyectos** |
| `10b-overlay-template.md` | **Plantilla para crear el overlay de cualquier repo nuevo (empieza aquí)** |
| `11-context-profiles.md` | Perfiles: personal, startup/pyme, enterprise, regulado |

## 2. Jerarquía de fuentes de verdad

Aplica esta prioridad salvo que el repositorio u organización defina otra
explícita (el overlay la registra):

1. Decisión explícita vigente del usuario, product owner o responsable autorizado
   (en enterprise: además change board / CAB y aprobadores designados).
2. Constitución y estándares vigentes del proyecto u organización (`AGENTS.md`,
   `SDD/`, ADRs, políticas corporativas enlazadas: seguridad, datos, accesibilidad).
3. Especificación aprobada de la funcionalidad (o ticket/historia canónica
   enlazada: Jira/Linear/ADO — se cita por ID, no se duplica).
4. Contratos públicos vigentes y migraciones desplegadas (OpenAPI/AsyncAPI,
   esquemas, eventos).
5. Pruebas de aceptación, integración o contrato confiables.
6. Telemetría, logs y comportamiento verificado en entorno operativo.
7. Código de producción inspeccionado.
8. Documentación histórica.
9. Suposiciones del agente.

Reglas:

- Las suposiciones **no** superan a la evidencia.
- El código existente describe comportamiento actual; no demuestra que sea
  correcto o deseado.
- Si dos fuentes de distinta prioridad contradicen, registra la contradicción y
  no la resuelvas silenciosamente.

## 3. Ubicación de cada verdad

Cada información durable tiene **una** fuente de verdad. Si se menciona en otro
sitio, se **enlaza**, no se copia.

| Tipo de información | Ubicación |
|---|---|
| Instrucciones para agentes y comandos del repositorio | `AGENTS.md` (o `CONTRIBUTING.md` / wiki corporativa enlazada) |
| Contexto de negocio global | `SDD/01-system-overview.md` (o página Confluence/Notion enlazada) |
| Arquitectura y regla de dependencia | `SDD/02-software-architecture.md` (o ADR corporativo enlazado) |
| Modelo de dominio, invariantes, lenguaje ubicuo | `SDD/Domain/` (o equivalente del stack: bounded contexts, módulos, schemas) |
| Casos de uso, puertos, orquestación | `SDD/Application/` o capa equivalente del stack |
| Persistencia, ORM, adaptadores | `SDD/Infrastructure/` o capa equivalente del stack |
| Endpoints y exposición | `SDD/Presentation/` o `contracts/` (OpenAPI/AsyncAPI) |
| Decisiones arquitectónicas duraderas | `SDD/Adr/` (o `docs/adr/`) |
| Línea base y hallazgos de un proyecto sin contexto | `SDD/00-bootstrap/` |
| Requisitos, diseño, tareas y evidencia de una feature | `SDD/<Contexto>/<id>-<nombre>/` (o ticket Jira/Linear/ADO enlazado) |
| Proceso de cambio enterprise (revisiones, ramas, entornos, CAB) | overlay del repo (`10-*-overlay.md`), enlaza a la política corporativa |

> Nota de adaptación: en repositorios **sin** convención propia, usa
> `.specify/memory/constitution.md`, `docs/` y `specs/<id>-<nombre>/`. En un
> repositorio que ya declara su SSoT (como zentric-backend con `/SDD`), la
> convención del repositorio **manda** y el catálogo genérico no se replica.
> Si la SSoT vive **fuera** del repo (Confluence, Jira, ADO, GitHub Projects),
> se enlaza por ID/URL estable en el overlay y en cada artefacto
> (`Spec-externa: PROJ-123 <url>`); el repo guarda solo el puntero y la
> evidencia de verificación, nunca una copia paralela.

## 4. Identificadores, estabilidad y ciclo de vida

Identificador secuencial estable, un directorio por iniciativa durable:

```text
SDD/Domain/000-bootstrap/
SDD/Domain/001-inventory-reservation/
SDD/Domain/002-customer-order/
```

No muevas carpetas a `_active`, `_implemented` o `_archive`. La ruta es estable;
el ciclo de vida se controla con metadatos al inicio del documento principal:

```yaml
---
id: 003
title: Cancelación de orden
type: feature
status: draft # discovery | draft | approved | in-progress | blocked | implemented | superseded | deprecated | archived
risk: medium # low | medium | high
created: YYYY-MM-DD
updated: YYYY-MM-DD
owners: []
related_adrs: []
related_contracts: []
replaces: []
---
```

Estados:

- `discovery`: entendiendo un comportamiento existente.
- `draft`: propuesta incompleta o no aprobada.
- `approved`: requisitos cerrados para el alcance acordado.
- `in-progress`: implementación o validación en curso.
- `blocked`: falta decisión, acceso, dependencia o evidencia.
- `implemented`: implementado y verificado con la evidencia registrada.
- `superseded`: otra spec o ADR reemplaza la decisión.
- `deprecated`: el comportamiento sigue temporalmente pero debe eliminarse.
- `archived`: histórico sin vigencia operativa.

## 5. Documentación viva

Al terminar un cambio actualiza **solo** lo afectado:

- La spec de la funcionalidad.
- Contratos si cambiaron API, eventos, payloads o integraciones.
- ADR si cambió una decisión arquitectónica duradera.
- Runbook si cambió operación, despliegue, recuperación o monitoreo.
- Documentación de dominio si cambió una regla transversal.
- Registro de deuda si se descubrió una limitación no resuelta.
- Checkboxes de trazabilidad (`[x]`) o `TRACKING.md` cuando el repositorio lo exija.

Prohibido: actualizar documentación no relacionada por estética. Prohibido
declarar `implemented` una spec cuyo resultado contradice su propio documento.

## 6. Documentos superseded

`generic-sdd-agent.md` (v2.0.0, monolítico, raíz del repositorio) queda
**superseded** por esta skill v3.0.0. Su contenido íntegro está distribuido aquí
(sin duplicaciones) y **no debe editarse**: cualquier ajuste se hace en
`references/`. Su eliminación requiere autorización explícita del owner.
