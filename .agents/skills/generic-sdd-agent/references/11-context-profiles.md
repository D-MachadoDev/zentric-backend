# 11. Perfiles de contexto — personal, startup/pyme, enterprise, regulado

> El núcleo de la skill es idéntico en todos los contextos. Lo que cambia es el
> **rigor exigible, los aprobadores y el proceso de cambio**. Al entrar a un repo
> nuevo, identifica su perfil con la tabla §1, aplícalo sobre el overlay del repo
> y registra el perfil elegido en el overlay §6.7 (o en la política corporativa
> enlazada). Nada de lo aquí descrito sustituye una norma corporativa vigente:
> si hay conflicto, la norma corporativa **manda** y se enlaza.

## 1. Cómo identificar el perfil

| Señal en el repo u organización | Perfil |
|---|---|
| Un solo desarrollador, sin CI, sin entornos, decisiones en chat | **Personal** |
| Equipo pequeño, 1–2 entornos, CI básico, revisiones informales | **Startup / pyme** |
| Múltiples equipos, CODEOWNERS, ramas protegidas, entornos DEV/QA/STG/PROD, change board o CAB, auditoría | **Enterprise / multinacional** |
| Datos sensibles regulados (salud, pagos, banca, gobierno), requisitos legales de trazabilidad, certificaciones | **Regulado** (se suma al enterprise) |

Un proyecto personal puede crecer a startup; una startup puede operar con
controles enterprise en su módulo de pagos. El perfil se elige **por iniciativa
y riesgo**, no solo por tamaño de empresa.

## 2. Perfil Personal (proyecto propio, POC, side project)

- **SSoT mínima:** `AGENTS.md` breve + specs solo del área activa. Nada de
  bootstrap completo si el repo es nuevo (Modo A directo).
- **Gates:** Gate 0 en una línea (modo + riesgo); Gate 2 solo si hay datos que
  no quieres perder o credenciales en juego.
- **Evidencia:** comandos ejecutados + 1 prueba por regla nueva. Sin matriz de
  trazabilidad formal salvo que el proyecto crezca.
- **Lo que NO cambia:** etiquetas de evidencia, no inventar, cambio mínimo,
  verificación antes de declarar terminado.
- **Trampa típica:** acumular deuda invisible ("ya lo documento después").
  Mitigación: cada cambio deja su `verification.md` de 5 líneas.

## 3. Perfil Startup / Pyme

- **SSoT:** `SDD/` o `specs/` por feature + ADRs solo para decisiones
  transversales (stack, pagos, auth, integraciones).
- **Gates:** Gate 0 + Gate 1 siempre; Gate 2 para riesgo medio/alto con
  aprobación del tech lead o del owner (puede ser en el mismo hilo de chat).
- **Proceso:** PR con 1 revisor, CI en verde (`build + test + lint`), sin merge
  con validaciones pendientes.
- **Deuda:** se registra en `risks-and-gaps.md` o en el backlog enlazado, con
  responsable y fecha de revisión — nunca solo en la memoria del chat.

## 4. Perfil Enterprise / Multinacional

- **SSoT distribuida:** la verdad puede vivir en Confluence/Jira/ADO/GitHub.
  El repo guarda el **puntero estable** (`Spec-externa: PROJ-123 <url>`) y la
  evidencia de verificación. Prohibido duplicar el contenido corporativo en el
  repo (dos verdades = cero verdades).
- **Proceso de cambio (registrar en el overlay, enlazar la política):**
  - Ramas protegidas y estrategia de ramas (trunk / gitflow / release trains).
  - Revisores requeridos (CODEOWNERS), quién puede aprobar riesgo alto.
  - Entornos y promoción (DEV → QA → STG → PROD), ventanas de cambio.
  - Change board / CAB: qué cambios lo requieren y con qué antelación.
  - Trazabilidad ticket ↔ commit ↔ PR ↔ despliegue (IDs en cada artefacto).
- **Gates reforzados:**
  - Gate 2 siempre con aprobación **explícita y registrada** (quién + cuándo +
    ID del ticket) para riesgo medio/alto.
  - Gate 4 incluye plan de rollout, monitoreo post-despliegue y rollback
    probado o documentado.
  - Ningún cambio de contrato, datos, permisos o seguridad entra sin revisión
    de un humano con autoridad sobre el área.
- **Seguridad y cumplimiento:** threat modeling proporcional (§7.3 de
  `07-special-rules.md`), revisión de secretos/PII en cada PR, dependencias
  auditadas, accesibilidad cuando aplique.
- **Agente de IA en enterprise:** opera como contribuidor, nunca como
  aprobador. No fusiona sus propios cambios, no elude revisiones requeridas,
  no toca producción sin ventana aprobada.

## 5. Perfil Regulado (salud, pagos, banca, gobierno — HIPAA/GDPR/PCI/SOX/...)

Se suma al enterprise, y añade:

- **Trazabilidad total:** requisito → diseño → tarea → prueba → evidencia →
  despliegue, con IDs estables y fechas. La matriz de `verification.md` es
  obligatoria, no opcional.
- **Decisiones registradas:** toda excepción a una regla, todo acceso a datos
  sensibles y toda corrección retroactiva queda en ADR o `decisions.md` con
  aprobador y fecha.
- **Datos:** clasificación (público/interno/confidencial/restringido),
  retención, minimización, anonimización en logs y fixtures, prohibición de
  datos reales en entornos no productivos.
- **Validación:** suites de regresión y contrato obligatorias; ningún
  `implemented` sin evidencia ejecutada y reproducible.
- **Auditoría:** el agente debe poder reconstruir, meses después, qué se
  cambió, por qué, quién lo aprobó y cómo se verificó — solo con los
  artefactos del repo y los tickets enlazados.
