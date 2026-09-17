# 03. Artefactos SDD

Crea solo lo proporcional al proyecto, riesgo y etapa. Un artefacto que nadie
actualiza es deuda, no gobernanza.

## 1. Catálogo documental (referencia)

```text
SDD/                              # o .specify/ + docs/ + specs/ si el repo no define otra cosa
├── 00-bootstrap/                 # línea base de un proyecto sin contexto confiable
├── 01-system-overview.md
├── 02-software-architecture.md
├── Domain/  Application/  Infrastructure/  Presentation/
├── Adr/
└── <Contexto>/<id>-<nombre-corto>/
    ├── README.md                 # metadatos y estado
    ├── requirements.md
    ├── design.md
    ├── tasks.md
    ├── verification.md
    ├── decisions.md              # decisiones locales de la feature
    └── contracts/                # solo si crea o cambia contratos
```

No todos los archivos son obligatorios: es un catálogo de capacidades.

## 2. Constitución del proyecto

Contiene decisiones y estándares duraderos:

- Stack y versiones permitidas.
- Estilo arquitectónico y límites de módulos/bounded contexts.
- Reglas de dependencias.
- Convenciones de nombres, estructura y estilo.
- Estrategia de errores.
- Autenticación, autorización y seguridad.
- Validaciones de entrada, aplicación y dominio.
- Política de pruebas y cobertura crítica.
- Política de migraciones, datos y compatibilidad.
- Observabilidad y manejo de incidentes.
- Definition of Done y comandos oficiales de validación.
- Política de documentación y ADR.

Si no existe constitución: no la inventes como verdad. Propón una versión
inicial basada en evidencia, marca cada regla como `[CONFIRMADO]`, `[PROPUESTO]`
o `[PENDIENTE]` y solicita confirmación antes de imponer reglas que alteren el
modo de trabajo del proyecto.

## 3. Requisitos (`requirements.md`)

Para cambios medianos, grandes, críticos o ambiguos. Expresa **qué** y **por qué**;
sin detalles innecesarios de clases, frameworks, tablas o algoritmos.

Debe incluir: metadatos de estado y riesgo · problema y motivación · actores o
sistemas afectados · alcance incluido · fuera de alcance · historias o flujos ·
requisitos funcionales (`FR-xxx`) · no funcionales (`NFR-xxx`) · reglas de
negocio e invariantes (`INV-xxx`) · casos límite y estados de error (`ERR-xxx`) ·
criterios de aceptación observables (`CA-xxx`) · dependencias, supuestos, riesgos
y preguntas abiertas · trazabilidad a tickets, ADRs, APIs y decisiones previas.

Cuando sea útil, usa EARS:

- Ubicuo: "El sistema deberá…"
- Basado en evento: "Cuando ocurra X, el sistema deberá…"
- Basado en estado: "Mientras el sistema esté en estado X, deberá…"
- Opcional: "Cuando el usuario habilite X, el sistema deberá…"
- Comportamiento no deseado: "Si ocurre X inválido, el sistema no deberá…"

Prohibido: "debe ser rápido", "debe ser seguro", "debe funcionar bien", "debe
ser intuitivo". Sustituye por condiciones medibles o escenarios observables.

## 4. Diseño técnico (`design.md`)

Para riesgo medio o alto. Expresa el **cómo** y traza cada decisión importante
hacia requisitos concretos.

Incluye según aplique: contexto técnico confirmado · módulos, capas y
archivos/símbolos afectados · flujo de datos y de control · entidades, value
objects, agregados, modelos, comandos o consultas afectados · contratos de API,
eventos, colas o integraciones · persistencia, índices, migraciones, backfill y
compatibilidad · validación en entrada, aplicación, dominio y salida ·
autenticación, autorización y amenazas · errores, idempotencia, concurrencia,
transacciones y reintentos · estrategia de compatibilidad · estrategia de
pruebas · observabilidad · despliegue, rollback o feature flag · trade-offs y
decisiones pendientes.

## 5. Tareas (`tasks.md`)

Cada tarea: pequeña, ordenada por dependencia, verificable y trazable.

```text
[T-<id>] Título
Requisitos/criterios cubiertos: FR-xxx, INV-xxx, CA-xxx
Área afectada: módulo, contrato, esquema, UI o infraestructura
Cambio esperado: descripción concreta
Verificación: prueba, comando o escenario
Dependencias: T-xxx o ninguna
Riesgo: bajo / medio / alto
Estado: pending / in-progress / done / blocked
```

Orden recomendado: 1) contratos, fixtures, infraestructura o migraciones
necesarias; 2) pruebas que expresen el comportamiento esperado; 3) dominio y
reglas de negocio; 4) casos de uso, persistencia y adaptadores; 5) API, eventos,
UI o interfaces; 6) integración, errores y observabilidad; 7) documentación
afectada; 8) validación final de regresión y cierre.

## 6. Verificación (`verification.md`)

```text
| Requisito | Diseño/Tarea | Evidencia | Estado |
|---|---|---|---|
| FR-001 | T-002 | prueba o escenario | PASS / FAIL / PENDING |
```

Además registra: comandos exactos ejecutados · resultados y fecha ·
validaciones no ejecutadas y motivo · fallos preexistentes diferenciados de
regresiones · riesgos residuales · revisión manual requerida si aplica.

## 7. Decisiones locales y ADR

`decisions.md` para decisiones limitadas a una feature.

ADR en `SDD/Adr/` solo si la decisión: cambia arquitectura o límites de módulos ·
introduce o reemplaza tecnología relevante · establece política duradera de
seguridad, datos, integración o despliegue · afecta múltiples features o equipos ·
tiene consecuencias significativas a largo plazo.

Un ADR incluye contexto, decisión, alternativas consideradas, consecuencias,
estado y fecha.
