-- Results summary dashboard (4 Nov 2026 plan): GET /ResultsSlip/summary/{municipality},
-- /coverage/{municipality} and /summary/region/{region}.
-- Run once on the VoteScan database before deploying.

-- Council size per municipality, so the seat estimate can run. Seats/Wards come from
-- each MEC's seat notice; IndependentWards is entered by hand (the API has no reliable
-- way to tell an independent ward candidate from an unmatched/misspelt party abbreviation
-- on a slip, so this isn't auto-detected — set it to 0 until told otherwise for a
-- municipality, and the seat estimate just won't account for that ward).
CREATE TABLE IF NOT EXISTS CouncilSeats (
    Municipality VARCHAR(100) NOT NULL PRIMARY KEY,
    Seats INT NOT NULL,
    Wards INT NOT NULL,
    IndependentWards INT NOT NULL DEFAULT 0,
    UpdatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Index the summary/coverage queries actually filter and group by. ResultsSlip already
-- has idx_live_vd (VD, IsLive) from results_slip_v2.sql; this one serves the
-- municipality-scoped party/ward aggregations instead of a VD lookup.
CREATE INDEX IF NOT EXISTS idx_results_slip_summary
    ON ResultsSlip (Election, Municipality, IsLive, Ballot);

-- Example seed row (edit before election night, or leave out — a municipality with no
-- row here just gets votes without a seat estimate, per BuildSummary's fallback):
-- INSERT INTO CouncilSeats (Municipality, Seats, Wards, IndependentWards)
-- VALUES ('MAN - Mangaung', 101, 51, 1)
-- ON DUPLICATE KEY UPDATE Seats = VALUES(Seats), Wards = VALUES(Wards),
--     IndependentWards = VALUES(IndependentWards);
