# 02. Modos de operación y nivel de rigor

> Núcleo agnóstico de stack, lenguaje, empresa y estado de proyecto. Los modos
> describen **situaciones de trabajo**, no tecnologías: aplican igual a un script
> personal, a una app móvil, a un monolito enterprise o a un sistema regulado.

Antes de actuar, clasifica la solicitud. Puede combinar modos, pero declara el
principal y los secundarios.

## Modo A — Proyecto nuevo

Úsalo cuando no existe código funcional, se iniciará un sistema nuevo, o el
repo actual está vacío o es solo plantilla (típico en proyectos personales,
POCs, startups y nuevos servicios enterprise).

Objetivo:

- Convertir una intención de producto en una primera vertical funcional y verificable.
- Evitar arquitectura prematura, patrones por moda y documentación sin valor.
- Dejar sentadas desde el día 1 la SSoT, los comandos de verificación y el
  overlay del repo (ver `10b-overlay-template.md`).

Prioridad:

1. Constitución inicial proporcional.
2. Descubrimiento del dominio y actores.
3. Especificación del primer incremento de valor.
4. Diseño técnico proporcional al riesgo.
5. Implementación vertical, validada y observable.

## Modo B — Nueva funcionalidad

Existe base funcional y se pide una historia de usuario, endpoint, flujo, módulo
o capacidad adicional.

Objetivo:

- Implementar el cambio respetando contratos, invariantes, arquitectura y
  compatibilidad existentes.
- Evaluar impacto en módulos, datos, permisos, pruebas, interfaces e integraciones.

## Modo C — Bug o comportamiento incorrecto

Objetivo:

- Reproducir antes de modificar.
- Separar comportamiento actual de comportamiento esperado.
- Proteger la corrección con prueba de regresión o escenario reproducible.
- Aplicar el cambio mínimo que corrige la **causa**, no el síntoma.

Regla: no "arregles" un bug modificando código hasta que deje de fallar. Primero
recoge evidencia de reproducción; después formula hipótesis y crea la prueba que
protege la corrección.

## Modo D — Revisión o auditoría

Revisar código, arquitectura, calidad, seguridad, rendimiento, deuda técnica o
cumplimiento de especificaciones.

Objetivo:

- Emitir hallazgos priorizados por severidad, evidencia, impacto y recomendación.
- No modificar ni refactorizar automáticamente salvo que el usuario haya pedido
  corrección y el cambio sea seguro, acotado y sin cambios de contrato o negocio.

Formato obligatorio de hallazgo:

```text
[H-<id>] Título
Severidad: bloqueante / alta / media / baja
Evidencia: archivo, símbolo, flujo, prueba, contrato o comando
Riesgo: qué puede fallar y a quién afecta
Recomendación: cambio concreto y proporcional
Verificación: prueba, comando o escenario para validar la solución
```

## Modo E — Refactorización o deuda técnica

El comportamiento deseado ya existe pero el diseño debe mejorar.

Objetivo:

- Preservar comportamiento externo.
- Establecer línea base de comportamiento y calidad antes de cambiar estructura.
- Describir el dolor técnico concreto, el beneficio esperado y el riesgo.
- Separar refactors seguros de cambios funcionales.

Regla: si el refactor modifica contrato, regla de negocio, respuesta pública,
esquema de datos, permiso, rendimiento crítico o experiencia observable, deja de
ser refactor puro y vuelve a requisitos, diseño y aprobación según su riesgo.

## Modo F — Proyecto existente sin contexto confiable (Brownfield)

Existe código, pero la documentación es ausente, antigua, contradictoria o
insuficiente para cambiar con seguridad.

Objetivo:

- Construir un mapa de realidad antes de cambiar comportamiento.
- Extraer conocimiento con evidencia sin convertir automáticamente el código en
  verdad de negocio.
- Adoptar SDD gradualmente, empezando por zonas activas y de mayor riesgo.

Actívalo **obligatoriamente** si:

- El proyecto no tiene especificaciones confiables.
- Se pide modificación relevante en código legacy.
- Existen contradicciones entre documentación, pruebas y código.
- No se conocen contratos, consumidores, datos o límites de módulos.
- Se pide un refactor transversal.
- Se va a introducir un agente de IA en un repositorio existente.

---

# Nivel de rigor según riesgo

Clasifica cada cambio **antes** de diseñarlo. El rigor es proporcional; la
evidencia nunca es opcional.

## Nivel 1 — Bajo riesgo

Ejemplos: corrección de texto, ajuste visual local, cambio aislado sin datos ni
permisos ni contrato externo, bug reproducible con cobertura local.

Requiere: contexto mínimo confirmado, criterio de aceptación claro, cambio
pequeño, validación focalizada, resumen final.

## Nivel 2 — Riesgo medio

Ejemplos: funcionalidad nueva dentro de un módulo, endpoint interno, cambio de
validación, flujo UI+backend sin migración destructiva, refactor de varios
archivos dentro de un mismo límite funcional.

Requiere: requisitos breves, diseño técnico proporcional, tareas trazables,
análisis de impacto, pruebas unitarias e integración según aplique, revisión de
regresión.

## Nivel 3 — Alto riesgo

Ejemplos: pagos, permisos, datos personales, auditoría o cumplimiento;
migraciones de datos; APIs públicas o eventos consumidos por terceros;
integraciones externas; autenticación o autorización; procesos asíncronos,
colas, consistencia eventual; cambios arquitectónicos o refactor transversal;
sistemas automatizados que toman decisiones o ejecutan acciones.

Requiere: requisitos completos y aprobados, diseño técnico aprobado, evaluación
de seguridad o modelo de amenazas proporcional, estrategia explícita de
compatibilidad, despliegue y rollback, pruebas de contrato e integración,
observabilidad, revisión manual de cambios y plan de validación posterior al
despliegue cuando aplique.
