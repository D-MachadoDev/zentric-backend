# 10b. Plantilla de overlay — úsala para CUALQUIER repositorio nuevo

> Copia este archivo como `references/10-<repo>-overlay.md` dentro de la skill,
> o mejor: como `OVERLAY.md` (o sección SDD en `AGENTS.md`) en la raíz del
> repositorio nuevo. El núcleo de la skill lo detecta automáticamente
> (ver `SKILL.md` § "Detección de contexto"). Rellena cada sección con evidencia
> del repo; lo que no exista, márcalo `[PENDIENTE]` — nunca lo inventes.

# Overlay: repositorio <nombre-repo>

Este overlay **manda** sobre el catálogo genérico de esta skill. Si hay conflicto
entre este archivo y `03-artifacts.md`, gana el overlay.

## 1. Detección de stack y comandos oficiales (verificados)

- Lenguaje(s) y versión: ...
- Framework(s): ...
- Gestor de dependencias: ...
- Base de datos / ORM / migraciones: ...
- Comandos oficiales (solo los confirmados en este repo):

```text
<gestor> <comando-restore>
<gestor> <comando-build>
<gestor> <comando-test>
<gestor> <comando-lint>        # si existe
<gestor> <comando-typecheck>    # si existe
<gestor> <comando-formato>      # si existe
```

- CI/CD: ... (enlace al pipeline; qué validaciones exige antes de merge)
- Evidencia de línea base: ... (ruta al `verification-baseline.md` o log)

## 2. Arquitectura y reglas inviolables del repo

- Estilo arquitectónico: ... (hexagonal, clean, modular monolith, MVC, microservicios, serverless, mobile MVVM, data pipeline, etc.)
- Regla de dependencias: ... (qué capa puede referenciar a cuál)
- Límites de módulos / bounded contexts: ...
- Convenciones: idioma del código, idioma de comentarios/docs, formato, ramas (`main`/`develop`/trunk), conventional commits (sí/no)
- Estrategia de errores: ... (Result, excepciones de dominio, Problem Details, códigos gRPC, etc.)
- Autenticación / autorización: ... (dónde se aplica, qué roles existen)
- Fuente: ... (`AGENTS.md` §..., `CONTRIBUTING.md`, ADR-xxx, wiki corporativa)

## 3. Mapa de la fuente única de verdad (SSoT)

¿Dónde vive cada verdad en ESTE proyecto? Marca `[PENDIENTE]` lo que no exista.

| Zona | Ruta en este repo | Estado |
|---|---|---|
| Instrucciones para agentes | `AGENTS.md` / `CONTRIBUTING.md` / wiki | ... |
| Contexto de negocio global | `SDD/01-system-overview.md` o página externa enlazada | ... |
| Arquitectura del software | `SDD/02-software-architecture.md` o ADR enlazado | ... |
| Dominio / reglas / invariantes | `SDD/Domain/` o equivalente del stack | ... |
| Casos de uso / orquestación | `SDD/Application/` o equivalente | ... |
| Persistencia / adaptadores | `SDD/Infrastructure/` o equivalente | ... |
| Endpoints / contratos | `SDD/Presentation/` o `contracts/` (OpenAPI/AsyncAPI/proto) | ... |
| Decisiones duraderas | `SDD/Adr/` o `docs/adr/` | ... |
| Línea base brownfield | `SDD/00-bootstrap/` | ... |
| Specs de features | `SDD/<Contexto>/<id>-<nombre>/` o tickets enlazados | ... |
| Verdad externa (Jira/Confluence/ADO/...) | IDs y URLs estables: ... | ... |
| Proceso enterprise (revisiones, ramas, entornos, CAB) | enlace a la política corporativa: ... | ... |

**Regla:** no crees una SSoT paralela (`docs/` + `specs/` + wiki + tickets con
contenido duplicado). Si la verdad vive fuera del repo, el repo guarda solo el
puntero (`Spec-externa: PROJ-123 <url>`) y la evidencia de verificación.

## 4. Lenguaje ubicuo canónico

La especificación define los términos. El código debe usarlos **exactamente**.

| Término en spec | Estado en código | Acción |
|---|---|---|
| ... | ... | decidir y unificar / **resuelto** por ADR-xxx / **contradicción** |

## 5. Estado de partida (línea base)

- Proyectos/módulos en la solución: ...
- Build / tests / lint: ... (comando + resultado + fecha)
- Contradicciones abiertas que bloquean: ... (ver `risks-and-gaps.md`)
- Hallazgos abiertos: ... (H-xx)
- Documentos de gobernanza creados: ...

## 6. Protocolo específico del repositorio

1. Antes de implementar en un área, consulta la spec correspondiente de la SSoT.
2. Si la spec no cubre el caso: **no inventes**. Actualiza o crea la spec primero
   y pide confirmación.
3. Si el código contradice la spec: registra `[CONTRADICCIÓN]` y aplica el
   protocolo de `07-special-rules.md` §8.
4. Todo cambio estructural o de regla de negocio actualiza la SSoT en el mismo
   cambio.
5. Marca progreso con `[x]` en los documentos aplicables o en `TRACKING.md`.
6. Cierra con el bloque de Fase 9 e incluye los comandos ejecutados.
7. Proceso de cambio corporativo (si aplica): revisores requeridos, ramas
   protegidas, entornos, ventana de cambio, CAB — enlazar la política, no copiarla.
