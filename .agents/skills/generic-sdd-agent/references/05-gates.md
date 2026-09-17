# 05. Gates de calidad y aprobación

No avances automáticamente entre fases cuando falte información crítica.

## Gate 0 — Descubrimiento y clasificación

Antes de cambiar código:

- Clasifica modo de operación y nivel de riesgo.
- Lee documentación y archivos relevantes.
- Identifica comandos oficiales de build, test, lint, formato y typecheck.
- Identifica arquitectura, módulos, contratos y datos afectados.
- Declara incertidumbres y riesgos.
- Para brownfield, inicia bootstrap proporcional si no existe contexto confiable.

Salida mínima en chat:

```text
Modo:
Nivel de riesgo:
Contexto confirmado:
Área afectada:
Incertidumbres y contradicciones:
Siguiente paso:
```

## Gate 1 — Requisitos claros

No pases a diseño mientras haya ambigüedades que puedan cambiar: reglas de
negocio · datos creados, editados, eliminados o retenidos · permisos, roles o
auditoría · contratos externos o públicos · compatibilidad · seguridad · métricas
de éxito · criterios de aceptación.

- Pregunta de forma precisa. Ofrece opciones cuando reduzcan ambigüedad.
- No preguntes lo que pueda confirmarse inspeccionando documentación, código,
  pruebas o contratos.
- Si una ambigüedad no bloquea, documenta la suposición, explica su impacto y
  pide confirmación antes de convertirla en regla persistente.

## Gate 2 — Plan técnico y autorización

Antes de implementar un cambio de riesgo medio o alto presenta el plan cerrado
definido en `06-workflow-phases.md` (Fase 4) y **pide aprobación explícita**.

Condiciones que exigen aprobación explícita antes de modificar:

- Cambio arquitectónico o nueva dependencia relevante.
- Migración, borrado o transformación de datos.
- Cambio de contrato público, API, evento o integración.
- Cambio de permisos, autenticación o seguridad.
- Riesgo de ruptura de compatibilidad.
- Cambio de alcance o regla de negocio.
- Refactor grande o transversal.
- Despliegue, infraestructura, servicio de pago o coste externo.

Para cambios de bajo riesgo: informa un plan breve y continúa si el usuario ya
pidió explícitamente implementar o corregir. Detente si aparece una ambigüedad o
un riesgo no declarado.

## Gate 3 — Consistencia antes de código

Antes de implementar revisa coherencia entre: constitución · requisitos · diseño ·
tareas · arquitectura existente · contratos y consumidores · pruebas existentes ·
datos, migraciones y restricciones operativas.

Busca contradicciones como:

- Un requisito exige algo que el contrato vigente prohíbe.
- Una tarea no cubre un criterio de aceptación.
- El diseño viola un límite arquitectónico.
- La migración rompe datos o versiones anteriores.
- La autorización no protege una operación sensible.
- Una respuesta nueva rompe consumidores.
- La estrategia de pruebas no cubre invariantes.
- **Dos documentos de especificación definen la misma regla de forma incompatible.**

Si hay contradicciones: **no implementes**. Preséntalas con impacto, alternativas
y recomendación. Actualiza artefactos después de la decisión humana.

## Gate 4 — Verificación de entrega

No declares finalizada una funcionalidad sin evidencia proporcional al riesgo.

Ejecuta según corresponda: formateador · linter · typecheck · pruebas unitarias ·
integración · contrato · end-to-end · build de producción · análisis de seguridad
y dependencias · revisión de migraciones · prueba manual reproducible · revisión
de logs, métricas o trazas.

Si no puedes ejecutar una validación:

- Declara cuál no se ejecutó.
- Explica por qué.
- Indica el comando o procedimiento exacto pendiente.
- No presentes la entrega como completamente verificada.
