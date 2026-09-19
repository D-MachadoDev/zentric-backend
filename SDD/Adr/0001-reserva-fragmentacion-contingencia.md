# ADR-0001 — Reserva de inventario: bodega única con fraccionamiento de contingencia

```yaml
id: 0001
title: Reserva de inventario con bodega única y fraccionamiento de contingencia
status: accepted
date: 2026-09-17
decided_by: owner del proyecto
supersedes: []
related_contradiction: [C-01](../00-bootstrap/risks-and-gaps.md) (SDD/00-bootstrap/risks-and-gaps.md)
```

## Contexto

La especificación de dominio contenía dos reglas incompatibles para la reserva de
stock distribuido:

- [`SDD/Domain/04-invariants-and-rules.md`](../Domain/04-invariants-and-rules.md) (invariante 3): "No Fraccionamiento de
  Variante… toda la cantidad debe surtirse desde **una (1) sola bodega**… Los
  envíos de un mismo SKU no se dividen para evitar costos exorbitantes."
- [`SDD/Domain/06-business-rules.md`](../Domain/06-business-rules.md) (INV-02) y
  [SDD/Domain/services/inventory-reservation-service.md :4](../Domain/services/inventory-reservation-service.md#4-flujo-logico-y-reglas-invariantes): el servicio "tiene
  permitido **fraccionar** la reserva en ambas bodegas".

Esto bloqueaba cualquier implementación del `InventoryReservationService`, del
agregado de inventario y del `OrderSplitterService`, porque definen de forma
incompatible cuántas guías de despacho se generan y cómo se compensa un fallo.

## Decisión

Se adopta el modelo **híbrido (A3)**:

1. **Regla base:** si existe **una** bodega individual cuyo `AvailableQuantity`
   cubra la cantidad total solicitada, la reserva se realiza íntegramente en esa
   bodega. El envío **no** se divide.
2. **Umbral exacto de la excepción:** solo si **ninguna** bodega individual cubre
   la cantidad solicitada, se permite fraccionar la reserva entre varias bodegas,
   ordenadas de mayor a menor stock disponible y respetando la prioridad de bodega
   `Marketplace` sobre `Vendor` (invariante 2).
3. **Fallo y compensación:** si la suma de todas las bodegas es menor que la
   cantidad solicitada, la operación falla y se liberan las reservas parciales
   calculadas en esa misma transacción (rollback virtual).
4. **Efecto logístico:** cuando hubo fraccionamiento, se generan múltiples
   `Shipment` (una guía por bodega de origen) dentro del `FulfillmentOrder` del
   vendedor.

Ejemplos normativos (bodega A = 6, bodega B = 4):

| Cantidad solicitada | Resultado |
|---|---|
| 5 | Una sola bodega (A) — no se divide |
| 10 | Fraccionamiento 6 + 4 — dos `Shipment` |
| 12 | Falla: la suma (10) no alcanza; no se reserva nada |

## Alternativas consideradas

| Alternativa | Motivo de rechazo |
|---|---|
| **A1. Prohibir siempre el fraccionamiento** | Obligaría a rechazar o recortar compras válidas cuando el stock existe pero está distribuido, degradando la experiencia de compra y la venta |
| **A2. Permitir siempre el fraccionamiento** | Multiplica envíos y costos logísticos en casos evitables; contradice el objetivo declarado de "evitar costos exorbitantes" |
| **A3. Híbrido (elegido)** | Cubre ambos objetivos: minimiza envíos cuando es posible y no pierde ventas cuando el stock está fragmentado |

## Consecuencias

- **Positivas:** regla única y verificable; se evitan envíos innecesarios; no se
  pierden ventas por stock distribuido.
- **Negativas / costes:** el `InventoryReservationService` debe devolver el
  desglose de reservas por bodega (`ReservationDetails`), y `OrderSplitterService`
  debe crear N `Shipment` cuando hubo fraccionamiento. Aumenta la complejidad de
  la compensación (liberar N reservas parciales ante fallo).
- **Impacto en pruebas:** se requieren casos para "una bodega cubre", "ninguna
  cubre pero la suma alcanza" y "la suma no alcanza".
- **Impacto en datos:** ninguno inmediato; no hay persistencia implementada aún.

## Documentos actualizados en la misma decisión

- [`SDD/Domain/04-invariants-and-rules.md`](../Domain/04-invariants-and-rules.md) — invariante 3 reescrita.
- [`SDD/Domain/06-business-rules.md`](../Domain/06-business-rules.md) — INV-02 reescrita.
- [`SDD/Domain/services/inventory-reservation-service.md`](../Domain/services/inventory-reservation-service.md) — reglas de prioridad,
  bodega única y fraccionamiento de contingencia.
- [`SDD/00-bootstrap/risks-and-gaps.md`](../00-bootstrap/risks-and-gaps.md) — [C-01](../00-bootstrap/risks-and-gaps.md) cerrada.
- [`SDD/00-bootstrap/questions-for-owner.md`](../00-bootstrap/questions-for-owner.md) — [Q-01](../00-bootstrap/questions-for-owner.md#q-01-c-01-se-permite-fraccionar-la-reserva-entre-bodegas-resuelta-2026-09-17) resuelta.
- [`SDD/00-bootstrap/migration-to-sdd-plan.md`](../00-bootstrap/migration-to-sdd-plan.md) — [T-004](../00-bootstrap/migration-to-sdd-plan.md)/[T-013](../00-bootstrap/migration-to-sdd-plan.md)/[T-014](../00-bootstrap/migration-to-sdd-plan.md) desbloqueadas
  respecto a [Q-01](../00-bootstrap/questions-for-owner.md#q-01-c-01-se-permite-fraccionar-la-reserva-entre-bodegas-resuelta-2026-09-17).

## Estado

`accepted` — vigente. Sustituye a la invariante 3 original y a INV-02 original.
