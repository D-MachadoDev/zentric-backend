# 09. Plantillas

## 1. Requisitos

```md
---
id: <id>
title: <nombre>
type: feature | bug | refactor | audit | discovery
status: draft
risk: low | medium | high
created: YYYY-MM-DD
updated: YYYY-MM-DD
related_adrs: []
related_contracts: []
---

# Requisitos — <nombre>

## Problema y motivación
[Qué necesidad resuelve y por qué importa]

## Actores
[Usuarios, roles o sistemas]

## Alcance
[Qué incluye]

## Fuera de alcance
[Qué no incluye]

## Requisitos funcionales
- FR-001: ...

## Requisitos no funcionales
- NFR-001: ...

## Invariantes
- INV-001: ...

## Casos de error y límite
- ERR-001: ...

## Criterios de aceptación
- CA-001: Dado ..., cuando ..., entonces ...

## Dependencias, riesgos y supuestos
- ...

## Preguntas abiertas
- ...
```

## 2. Diseño

```md
# Diseño — <nombre>

## Requisitos cubiertos
- FR-001, FR-002, INV-001, CA-001

## Contexto confirmado
- ...

## Flujo técnico
1. ...

## Módulos afectados
| Módulo | Cambio | Justificación |
|---|---|---|
| ... | ... | ... |

## Contratos y datos
- API/evento/cola: ...
- Persistencia/migración: ...
- Compatibilidad: ...

## Seguridad y autorización
- ...

## Errores, idempotencia y concurrencia
- ...

## Estrategia de pruebas
- Unitarias: ...
- Integración: ...
- Contrato: ...
- E2E/manual: ...

## Observabilidad y operación
- ...

## Rollback y despliegue
- ...

## Trade-offs y decisiones pendientes
- ...
```

## 3. Bug

```md
---
type: bug
status: draft
risk: <nivel>
---

# Bug — <nombre>

## Comportamiento actual
[Qué ocurre realmente]

## Comportamiento esperado
[Qué debería ocurrir]

## Pasos de reproducción
1. ...

## Evidencia
- Prueba, log, captura, contrato o ruta afectada: ...

## Impacto
[Usuarios, datos, negocio, seguridad u operación]

## Hipótesis inicial
[Hipótesis; no conclusión]

## Prueba de regresión
[Qué demostrará que la corrección permanece]

## Criterio de cierre
[Cómo sabremos que quedó resuelto]
```

## 4. Auditoría

```md
# Auditoría — <área>

## Alcance
[Qué se revisó y qué no]

## Hallazgos

### H-001 — <título>
Severidad: bloqueante / alta / media / baja

Evidencia:
- ...

Riesgo:
- ...

Recomendación:
- ...

Verificación:
- ...
```

## 5. Verificación

```md
# Verificación — <nombre>

## Matriz de trazabilidad
| Requisito | Diseño/Tarea | Evidencia | Estado |
|---|---|---|---|
| FR-001 | T-001 | prueba X | PASS |

## Comandos ejecutados
| Comando | Resultado | Fecha | Observaciones |
|---|---|---|---|
| ... | PASS / FAIL | YYYY-MM-DD | ... |

## Validaciones no ejecutadas
- [Motivo, procedimiento pendiente y responsable si aplica]

## Fallos preexistentes
- ...

## Riesgos residuales
- ...
```

## 6. Contradicción de especificación

```md
# [CONTRADICCIÓN] SPEC-<n> — <tema>

## Fuente A
Archivo: <ruta>:<línea>
Texto: "<cita exacta>"

## Fuente B
Archivo: <ruta>:<línea>
Texto: "<cita exacta>"

## Impacto si se elige A
- ...

## Impacto si se elige B
- ...

## Prioridad según jerarquía de verdad
[Qué fuente pesa más y qué evidencia falta]

## Opciones
1. ...
2. ...
3. ...

## Estado
[PENDIENTE | RESUELTA por <decisión y fecha>]

## Bloquea
[Tareas, módulos o specs que no pueden avanzar]
```
