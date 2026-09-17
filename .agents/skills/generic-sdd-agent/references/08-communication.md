# 08. Comunicación

## 1. Antes de implementar

Sé conciso, concreto y basado en evidencia.

No digas:

- "Voy a hacerlo."
- "Parece que…" sin indicar evidencia o incertidumbre.
- "Probablemente funciona."
- "Ya está listo" sin validaciones.

Prefiere:

- "Confirmé que el flujo actual está en X y la validación ocurre en Y."
- "Falta decidir Z; esta respuesta cambia la regla de negocio."
- "El plan protege estas invariantes."
- "La prueba falla por el comportamiento actual esperado; implementaré el cambio
  mínimo definido en la tarea T-xxx."
- "No pude ejecutar la prueba E2E porque falta la variable de entorno X; queda
  como validación pendiente."

## 2. Durante la implementación

Reporta checkpoints solo cuando aporten valor:

```text
Checkpoint:
- Tarea completada:
- Evidencia:
- Resultado:
- Riesgo nuevo, contradicción o bloqueo:
- Siguiente tarea:
```

No expongas razonamiento interno detallado ni vuelques archivos completos salvo
que el usuario lo solicite.

## 3. Después de implementar

No pegues bloques grandes de código salvo solicitud explícita. Explica:

- Qué cambió.
- Qué regla o criterio protege.
- Cómo se verifica.
- Qué contratos, datos o documentos se actualizaron.
- Qué decisiones, riesgos o validaciones quedan pendientes.

## 4. Idioma y nombres

- Comunícate en el idioma del usuario.
- Código en inglés, comentarios y documentación en español (convención de este
  repositorio).
- Usa los términos del lenguaje ubicuo sin sinónimos. Si el repo define un
  diccionario (overlay §4), ese diccionario manda sobre tu preferencia.

## 5. Al bloquear

Cuando te detengas por una decisión pendiente, entrega:

1. El bloqueo en una frase.
2. La evidencia exacta (archivo + línea o comando + salida).
3. El impacto si se elige mal.
4. 2–3 opciones con consecuencias.
5. Qué desbloquea cada opción.
6. Confirmación de qué NO se ha tocado todavía.
