# 01. Modelo de evidencia y principios no negociables

## 1. Etiquetas de evidencia

Toda afirmación relevante se clasifica con una de estas etiquetas:

- `[CONFIRMADO]`: respaldada por decisión explícita, contrato vigente, prueba
  confiable, código inspeccionado, migración desplegada, log o evidencia
  operativa verificable.
- `[INFERIDO]`: hipótesis razonable con evidencia incompleta. **No** es regla
  oficial hasta confirmarse.
- `[CONTRADICCIÓN]`: dos fuentes relevantes presentan comportamientos o
  decisiones incompatibles.
- `[PENDIENTE]`: falta una decisión, acceso o información necesaria.
- `[OBSOLETO]`: implementación, contrato o documentación que ya no rige.
- `[RIESGO]`: posibilidad concreta de fallo, pérdida, exposición,
  incompatibilidad o degradación.

Reglas:

- Nunca conviertas una inferencia en requisito, invariante o decisión de negocio
  sin confirmación explícita.
- Si documentación, pruebas, contratos y código se contradicen, no elijas
  arbitrariamente: registra el conflicto, estima el impacto y solicita decisión
  cuando afecte producto, datos, seguridad, contratos o compatibilidad.
- No prometas "cero alucinaciones". Evita afirmaciones sin evidencia, declara
  incertidumbre y bloquea decisiones cuando sea necesario.

## 2. Evidencia antes que suposición

- Nunca inventes reglas de negocio, endpoints, archivos, modelos, dependencias,
  permisos, convenciones, resultados de pruebas, consumidores ni comportamientos.
- Antes de afirmar que algo existe, inspecciónalo con las herramientas disponibles.
- Si no hay evidencia suficiente, elige **una** acción:
  1. Investigar repositorio, documentación, tickets, contratos, pruebas o logs.
  2. Preguntar de forma precisa al usuario o responsable.
  3. Declarar la incertidumbre y bloquear la decisión.

## 3. Intención antes que código

No escribas código de producción antes de comprender, en la proporción que exija
el riesgo:

- Qué problema se resuelve.
- Qué actor, usuario o sistema se ve afectado.
- Qué comportamiento observable debe cambiar.
- Qué comportamiento **no** debe cambiar.
- Qué queda fuera de alcance.
- Qué criterios demuestran que la entrega está terminada.

El "qué" y el "por qué" pertenecen a requisitos. El "cómo" pertenece al diseño
técnico. Las tareas convierten el diseño en unidades verificables.

## 4. Cambio mínimo, reversible y observable

- Prefiere el cambio más pequeño que cumpla los requisitos.
- No reescribas subsistemas completos para una funcionalidad puntual.
- No hagas refactors oportunistas no relacionados sin declararlos y justificarlos.
- Prioriza compatibilidad hacia atrás con consumidores, usuarios, datos
  productivos o contratos externos.
- Todo cambio se verifica con pruebas, build, lint, análisis estático, contrato,
  prueba manual reproducible o evidencia equivalente.
- En alto riesgo define rollout, rollback, migración gradual o feature flag.

## 5. Calidad como definición de terminado

Una tarea no está terminada porque compila ni porque el código parece correcto.
Se considera terminada solo cuando se cumple lo aplicable:

- [ ] El criterio de aceptación está cubierto por evidencia.
- [ ] Existe prueba automatizada, o justificación explícita y verificación
      alternativa reproducible.
- [ ] Las pruebas existentes no presentan regresiones introducidas por el cambio.
- [ ] Tipado, lint, formato, build y análisis estático aplicables son correctos.
- [ ] Contratos, documentación y runbooks relevantes están actualizados.
- [ ] Los errores se manejan coherentemente.
- [ ] Seguridad, autorización y validación se evaluaron.
- [ ] Observabilidad añadida si el flujo lo requiere.
- [ ] No se exponen secretos, credenciales, tokens ni datos sensibles.
- [ ] Se entrega cierre con evidencia, límites y validaciones pendientes.
