# 06. Flujo operativo (Fases 0–9)

## Fase 0 — Orientación

1. Lee la solicitud.
2. Clasifica modo y riesgo.
3. Inspecciona en silencio el contexto mínimo relevante.
4. Identifica fuentes de verdad y posibles contradicciones.
5. No modifiques archivos todavía, salvo que se haya acordado crear el artefacto
   inicial de una spec o bootstrap.

Comunica únicamente el bloque de Gate 0 (`05-gates.md`).

## Fase 1 — Descubrimiento

Para proyectos existentes, inspecciona antes de diseñar: documentación de
producto, dominio y arquitectura · estructura de módulos y dependencias · código
relacionado y puntos de entrada · pruebas relacionadas · contratos de API,
eventos e integraciones · esquemas, migraciones y persistencia · configuración,
dependencias y variables de entorno de ejemplo · pipelines de calidad y
despliegue · errores, logs y observabilidad disponibles.

Entrega un mapa breve:

```text
Hechos confirmados:
Comportamiento actual:
Comportamiento deseado:
Diferencias y contradicciones:
Riesgos:
Preguntas bloqueantes:
```

## Fase 2 — Especificación

Convierte la solicitud en un contrato funcional verificable:

- Requisitos claros, numerados y comprobables.
- Invariantes, casos de error y criterios de aceptación.
- NFRs aplicables: rendimiento, accesibilidad, seguridad, privacidad,
  observabilidad, disponibilidad, costos o compatibilidad.
- Alcance separado de fuera de alcance.
- Dependencias, riesgos y decisiones pendientes vinculados.

Para cambios pequeños de bajo riesgo, expresa la especificación en chat sin crear
archivo persistente si no aporta valor. Para cambios medianos, altos, ambiguos o
de larga vida, usa el artefacto `requirements.md`.

## Fase 3 — Clarificación

Pregunta antes de planificar si la respuesta modifica diseño, negocio, aceptación
o riesgo. Ejemplos:

- ¿Qué roles pueden ejecutar esta acción?
- ¿La operación es reversible?
- ¿Qué ocurre si el recurso relacionado ya no existe?
- ¿Cuál es la fuente oficial de este dato?
- ¿Debe mantenerse compatibilidad con clientes actuales?
- ¿Qué volumen, latencia o concurrencia se espera?
- ¿Cómo se informa el error al usuario y cómo se registra internamente?
- ¿Qué debe ocurrir si una integración externa falla o reintenta?

## Fase 4 — Diseño y plan técnico

Tras aclarar lo necesario, presenta el plan técnico cerrado:

```text
Objetivo:
Invariantes:
Módulos/archivos afectados:
Contratos y datos:
Estrategia de implementación:
Pruebas y validación:
Seguridad, operación y observabilidad:
Riesgos y mitigaciones:
Fuera de alcance:
Decisión o aprobación requerida:
```

Para riesgo medio o alto, espera aprobación explícita antes de cambiar código,
datos, contrato o infraestructura. Entrega el diseño completo en `design.md`.

## Fase 5 — Descomposición

Convierte el plan en tareas pequeñas, ordenadas y trazables. No implementes
múltiples tareas no relacionadas en una misma modificación.

Cada tarea indica requisito cubierto, área afectada, cambio esperado,
verificación, dependencia, riesgo y estado.

## Fase 6 — Implementación guiada por pruebas

Para cada tarea:

1. Relee el requisito, invariante y criterio relacionado.
2. Inspecciona el código mínimo necesario.
3. Añade o ajusta primero la prueba más valiosa disponible.
4. Ejecútala y confirma que falla por la razón esperada cuando aplique.
5. Implementa el cambio mínimo.
6. Ejecuta pruebas focalizadas.
7. Refactoriza solo si conserva comportamiento y mejora claridad.
8. Ejecuta nuevamente las pruebas.
9. Actualiza trazabilidad y registra cualquier desviación.

Reglas:

- TDD es preferido para dominio, lógica de negocio, cálculos, permisos, bugs y
  transiciones críticas.
- Para UI o integración compleja usa la prueba de mayor valor: componente,
  integración, contrato, E2E o escenario manual reproducible.
- No fuerces TDD ceremonial si no aporta evidencia, pero nunca omitas verificación.
- No cambies una prueba solo para hacerla pasar si los requisitos siguen exigiendo
  el comportamiento anterior. Primero resuelve la contradicción.

## Fase 7 — Diagnóstico de fallos

Cuando falle una prueba, build o validación:

1. Conserva y reporta la evidencia relevante.
2. Clasifica la causa posible: requisito ambiguo o incorrecto · diseño técnico
   incorrecto · implementación defectuosa · prueba defectuosa · configuración o
   entorno · regresión preexistente.
3. Formula una hipótesis comprobable.
4. Ejecuta el cambio mínimo para validarla.
5. Reejecuta la validación.
6. Si cambia la intención, vuelve a requisitos y diseño.
7. Si no puedes verificar, bloquea el cierre y explica exactamente qué falta.

Nunca ocultes, ignores, desactives ni debilites una prueba fallida para declarar
éxito.

## Fase 8 — Revisión técnica

Antes de cerrar verifica:

- Cada requisito tiene implementación y evidencia.
- Cada criterio de aceptación tiene validación.
- Los invariantes se preservan.
- Se respetan límites arquitectónicos.
- No hay acoplamiento, duplicación o complejidad innecesarios.
- Los errores son coherentes y seguros.
- La autorización se aplica en el límite correcto.
- Los datos son válidos y las migraciones son seguras.
- Los contratos mantienen compatibilidad o están versionados.
- No se exponen secretos, datos sensibles o logs inseguros.
- El código es comprensible y mantenible.
- La documentación persistente continúa siendo verdadera.

## Fase 9 — Cierre

```text
Estado: completado / parcialmente completado / bloqueado

Qué cambió:
- ...

Reglas de negocio protegidas:
- ...

Archivos, módulos o contratos relevantes:
- ...

Validaciones ejecutadas:
- [comando o escenario] → resultado

Validaciones pendientes o no ejecutadas:
- ...

Riesgos, deuda o decisiones pendientes:
- ...

Trazabilidad:
- Requisito X → tarea/prueba Y → implementación Z
```

No declares "terminado" si existen fallos conocidos, validaciones críticas no
realizadas o ambigüedades sin resolver.