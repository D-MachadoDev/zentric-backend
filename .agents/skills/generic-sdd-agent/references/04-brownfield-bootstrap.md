# 04. Bootstrap Brownfield (B0–B5)

## 1. Propósito

Sobre un proyecto existente sin SDD confiable, no asumas que debe reescribirse ni
que el código representa correctamente el negocio. Primero construye una línea
base basada en evidencia.

El bootstrap no pretender documentar todo el sistema de una vez: produce el
conocimiento suficiente para cambiar el área objetivo con seguridad y adopta SDD
de forma progresiva.

## 2. Seguridad de inspección

Durante el descubrimiento inicial:

- Trabaja en modo lectura hasta terminar el inventario y el análisis inicial.
- No modifiques código, datos, configuración ni documentación vigente sin necesidad.
- No ejecutes comandos destructivos, migraciones, borrados, despliegues ni scripts
  desconocidos sin revisar su efecto y obtener aprobación cuando corresponda.
- No expongas secretos, tokens, credenciales, datos personales ni claves privadas.
- Usa archivos de ejemplo (`.env.example`); nunca copies valores sensibles a la
  documentación ni a la conversación.

## 3. Estructura de bootstrap

```text
SDD/00-bootstrap/            # o specs/000-bootstrap/ si el repo no define SSoT
├── README.md
├── repository-map.md
├── current-state.md
├── domain-discovery.md
├── architecture-baseline.md
├── contracts-inventory.md
├── data-baseline.md
├── quality-baseline.md
├── verification-baseline.md
├── risks-and-gaps.md
├── questions-for-owner.md
└── migration-to-sdd-plan.md
```

En repositorios pequeños pueden combinarse en `README.md`, `repository-map.md`,
`current-state.md`, `verification-baseline.md` y `risks-and-gaps.md`.

## 4. Fases

### B0 — Clasificación e inventario seguro

Inspecciona, según aplique: raíz, estructura, módulos y puntos de entrada ·
README y documentación de producto, arquitectura y dominio · dependencias,
versiones y configuración de build · variables de entorno de ejemplo sin revelar
secretos · persistencia, esquemas, migraciones, índices y restricciones · rutas,
controladores, casos de uso, servicios y adaptadores · APIs, eventos, colas,
webhooks e integraciones · pruebas, cobertura, fixtures y entornos de test ·
CI/CD, lint, formato, typecheck y build · manejo de errores, logs, métricas,
trazas y alertas · Docker, infraestructura y despliegue.

Genera o actualiza `repository-map.md`.

### B1 — Extracción de comportamiento

Para cada flujo relevante identifica: actor o sistema iniciador · entrada y
validaciones observadas · flujo de control y reglas observadas · persistencia y
efectos secundarios · salida, errores y contrato · pruebas existentes ·
dependencias e integraciones.

Genera o actualiza `current-state.md`, `domain-discovery.md`,
`contracts-inventory.md`.

Regla crítica:

- Una regla extraída solo desde código se marca `[INFERIDO]` salvo que esté
  respaldada por una fuente de mayor confianza.
- Si código y documentación difieren, registra `[CONTRADICCIÓN]`; no "arregles"
  automáticamente ni el código ni la documentación.

### B2 — Línea base de calidad

Identifica y ejecuta, si es seguro y está permitido: formateador · linter ·
typecheck · pruebas unitarias, integración y E2E existentes · build de producción ·
escáneres de dependencias o seguridad disponibles.

Registra en `verification-baseline.md`: comando exacto · resultado · fallos
preexistentes conocidos o sospechados · limitaciones del entorno · suites
ausentes · cobertura útil o áreas sin cobertura.

Registra en `quality-baseline.md` la evaluación de calidad, deuda y riesgos.

### B3 — Arquitectura real

Documenta la arquitectura **observada**, no la ideal: módulos y responsabilidades ·
dependencias y acoplamientos · capas, límites y violaciones identificadas ·
flujos de datos e integración · áreas críticas y frágiles · diferencia explícita
entre arquitectura actual y objetivo si existe.

Genera o actualiza `architecture-baseline.md`.

### B4 — Riesgos y preguntas

Registra en `risks-and-gaps.md`: riesgos de seguridad · riesgos de datos y
migración · contratos desconocidos o consumidores no identificados · falta de
pruebas · deuda técnica · dependencias obsoletas · comportamientos contradictorios
o no documentados.

Formato de cada riesgo:

```text
ID:
Categoría:
Evidencia:
Impacto:
Probabilidad: baja / media / alta / desconocida
Acción recomendada:
¿Bloquea el siguiente cambio?: sí / no
```

Registra en `questions-for-owner.md` únicamente decisiones que el código no
puede responder de forma confiable.

### B5 — Adopción gradual de SDD

Crea o actualiza `migration-to-sdd-plan.md`. Prioriza en este orden:

1. El módulo que se modificará próximamente.
2. Flujos que afecten dinero, permisos, datos personales o auditoría.
3. Contratos públicos e integraciones externas.
4. Áreas con bugs recurrentes.
5. Procesos sin pruebas o sin observabilidad.
6. Deuda que bloquea cambios futuros.

No propongas documentar o refactorizar todo el proyecto de una vez salvo que sea
el objetivo explícito y exista un plan aprobado.

## 5. Tres niveles de extracción

Para evitar documentación masiva e incorrecta:

1. **Inventario global:** mapa de repositorio, módulos, contratos, riesgos y
   flujos principales.
2. **Especificación profunda del área activa:** documenta completamente la parte
   que vas a modificar.
3. **Consolidación progresiva:** cada cambio convierte hallazgos confirmados en
   documentación viva y reduce incertidumbre en el módulo tocado.
