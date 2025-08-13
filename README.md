# Test Plan — Partial Close Service

## 1. Introduction
This test plan outlines the testing approach, objectives, scope, and acceptance for the Partial Close feature.  
The feature allows a user to close part of an open position (e.g., close 40 units out of 100).  
The plan focuses on verifying validation rules, price drift constraints, transaction update, and transaction creation.

---

## 2. Scope
- **Position state validation** — must be active and valid.
- **Units-to-close validation** — legal range and boundaries.
- **Price drift check** — within ±% of reference price per unit.
- **Persistence** — position units/price updated correctly in the existing transaction.
- **Transaction record creation** — with required fields.

---

## 3. Risks

1. **Unclear business rules**  
   *Issue:* Minimum units, calculation of price drift tolerance, allowed range between sell price and market price, or allowing full close via partial close are not well-defined.  
   *Mitigation:* Confirm all rules with stakeholders and document them before testing.

2. **Data not updated correctly**  
   *Issue:* Position units or prices might not be saved correctly in the DB after a partial close.  
   *Mitigation:* Add verification steps in tests to confirm DB changes.

3. **Transaction records missing or incomplete**  
   *Issue:* The system might not create a transaction record, or it could be missing details like units sold or timestamp.  
   *Mitigation:* Check all transaction fields in tests.

4. **Performance issues**  
   *Issue:* Partial close might take longer than the 10-second SLA under load or with slow dependencies.  
   *Mitigation:* Use performance testing and mock delays to ensure SLA compliance.

---

## 4. Test Cases (Given / When / Then)

### [TC001] Position active and valid
- **Given** an active position with 100 units and a valid reference price  
- **When** I request to close 40 units  
- **Then** Status is Success, no error, remaining units are 60, position stays active

---

### [TC002] Position inactive — Negative
- **Given** a position that is inactive  
- **When** I request to close number of units in the range  
- **Then** Status is Failed with error `InvalidOperationException 'Position not active'`

---

### [TC003] Position not found — Negative
- **Given** a position with unknown PositionId  
- **When** I request to close number of units in the range  
- **Then** Status is Failed with error `InvalidOperationException 'Position not found'`

---

### [TC004] Units within legal range including boundaries
- **Given** an active position with 100 units and a min step of 1 unit  
- **When** I request to close 1, 50, and exactly 100 units  
- **Then** Each attempt returns Success; remaining units updated correctly

---

### [TC005] Units out of legal range — Negative
- **Given** an active position with 100 units  
- **When** I request to close 0, negative, or more than 100 units  
- **Then** Status is Failed with error `ArgumentException` (<=0) or `ArgumentOutOfRangeException` (>held units)

---

### [TC006] Execution time SLA (complete within 10 seconds)
- **Given** healthy dependencies and no artificial delays  
- **When** I request to close 50 units  
- **Then** Operation completes within 10 seconds and returns Success

---

### [TC007] Execution time exceeded (dependency slow) — Negative
- **Given** a simulated slow dependency (e.g., repository or publisher delay)  
- **When** I request to close 50 units  
- **Then** Operation returns Failed with a timeout/latency exception

---

### [TC008] Price drift within ±% window
- **Given** a reference price per unit and live price within ±0.1%  
- **When** I request to close 10 units  
- **Then** Status is Success, transaction uses the live execution price

---

### [TC009] Price drift outside ±% window — Negative
- **Given** a reference price per unit and live price drifting by >0.1%  
- **When** I request to close 50 units  
- **Then** Status is Failed with error `Price drift not in range`

---

### [TC010] Persistence: position units & price updated — Negative
- **Given** an active position and a stable execution price  
- **When** a partial close of 50 units succeeds  
- **Then** Transaction update is called with remaining units=50 and stored price is not equal to the execution price

---

### [TC011] Transaction record created with full details
- **Given** a successful partial close  
- **When** the operation completes  
- **Then** a new transaction with closed details is created with: `TransactionId`, `UnitsSold`, `SellPrice`, `Timestamp`, `TotalValue = units × price`  
  And the current transaction is updated

---

## 5. Acceptance Tests

### [AT001] Successful partial close within SLA
- **Given** an active position with 100 units and live price within ±0.5% of reference  
- **When** I close 40 units  
- **Then** Service returns Success within 10 seconds, remaining units are 60, transaction record is created

---

### [AT002] Price drift breach blocks execution
- **Given** a reference price and a live price that deviates by >0.5%  
- **When** I attempt to close 40 units  
- **Then** Service returns Failed with a price-drift exception, no persistence updates occur

---

### [AT003] Persistence and transaction correctness
- **Given** a successful partial close  
- **When** the operation completes  
- **Then** Position repository reflects the new units and execution price, and the transaction record includes: id, unitsSold, sellPrice, timestamp, total value

