/* ============================
   CAR PARK
============================ */
INSERT INTO CarPark (Name, TimeZone)
VALUES ('Manchester Retail Park', 'Europe/London');

/* ============================
   VEHICLES
============================ */
INSERT INTO Vehicle (VRN)
VALUES ('AB12CDE'), ('XY99ZZZ'), ('MN45OPQ');

/* ============================
   VRN GROUPS
============================ */
INSERT INTO VRNGroup (CarParkId, Name)
VALUES (1, 'Staff'), (1, 'SeasonTicket2025');

INSERT INTO VRNGroupMember (VRNGroupId, VRN, ValidFrom)
VALUES 
(1, 'XY99ZZZ', '2025-01-01'),
(2, 'AB12CDE', '2025-01-01');

/* ============================
   TIME WINDOWS
============================ */
-- Free parking 18:00–08:00 daily
INSERT INTO TimeWindow (CarParkId, StartTime, EndTime, DaysOfWeekMask)
VALUES (1, '18:00', '08:00', 127);

/* ============================
   RULES
============================ */

-- Universal free parking at night
INSERT INTO ParkingRule (CarParkId, RuleType, Priority, AppliesToAllVRNs, TimeWindowId, CreatedBy)
VALUES (1, 'FREE_PARKING', 10, 1, 1, 'system');

INSERT INTO Rule_FreeParking (ParkingRuleId, MaxFreeMinutes)
VALUES (1, NULL);  -- unlimited

-- Staff get 3 hours free
INSERT INTO ParkingRule (CarParkId, RuleType, Priority, AppliesToAllVRNs, VRNGroupId, CreatedBy)
VALUES (1, 'FREE_PARKING', 20, 0, 1, 'system');

INSERT INTO Rule_FreeParking (ParkingRuleId, MaxFreeMinutes)
VALUES (2, 180);

-- No return within 2 hours
INSERT INTO ParkingRule (CarParkId, RuleType, Priority, AppliesToAllVRNs, CreatedBy)
VALUES (1, 'NO_RETURN', 30, 1, 'system');

INSERT INTO Rule_NoReturn (ParkingRuleId, NoReturnMinutes)
VALUES (3, 120);

-- Grace period 10 minutes
INSERT INTO ParkingRule (CarParkId, RuleType, Priority, AppliesToAllVRNs, CreatedBy)
VALUES (1, 'GRACE_PERIOD', 40, 1, 'system');

INSERT INTO Rule_GracePeriod (ParkingRuleId, GraceMinutes)
VALUES (4, 10);

-- Paid parking: 2 hours
INSERT INTO ParkingRule (CarParkId, RuleType, Priority, AppliesToAllVRNs, CreatedBy)
VALUES (1, 'PAID', 50, 1, 'system');

INSERT INTO Rule_PaidParking (ParkingRuleId, PaidMinutes)
VALUES (5, 120);

-- Validation: 4 hours
INSERT INTO ParkingRule (CarParkId, RuleType, Priority, AppliesToAllVRNs, CreatedBy)
VALUES (1, 'VALIDATION', 60, 1, 'system');

INSERT INTO Rule_Validation (ParkingRuleId, ValidatedMinutes)
VALUES (6, 240);


--==========
INSERT INTO TimeWindow (CarParkId, StartTime, EndTime, DaysOfWeekMask)
VALUES (1, '08:00', '09:00', 127);

DECLARE @tw INT = SCOPE_IDENTITY();

UPDATE ParkingRule
SET TimeWindowId = @tw
WHERE ParkingRuleId = 9;