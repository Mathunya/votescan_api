-- Results slip v2 (28 Oct 2026): photo kept on upload, results saved automatically after
-- the read, one live result per party (duplicate guard), VD taken from the slip.
-- Additive only: existing rows and the old POST /ResultsSlip keep working.

-- Every upload is kept, read or not. VD is filled in from the slip once Claude has read it.
ALTER TABLE ResultsSlipImages
    MODIFY COLUMN VD VARCHAR(20) NOT NULL DEFAULT '',
    ADD COLUMN Ballot VARCHAR(20) NOT NULL DEFAULT '' AFTER Day,
    -- uploaded | read | unreadable | error | saved ('saved' = rows from before v2)
    ADD COLUMN Status VARCHAR(20) NOT NULL DEFAULT 'saved' AFTER Ballot,
    -- the uploader's own VD, for reference only (helpers may upload other VDs' slips)
    ADD COLUMN UserVD VARCHAR(20) NOT NULL DEFAULT '' AFTER UploadedBy,
    -- Claude's full reading, kept for audit
    ADD COLUMN Reading MEDIUMTEXT NULL,
    ADD COLUMN ReadAt TIMESTAMP NULL DEFAULT NULL,
    ADD KEY idx_status (Status),
    ADD KEY idx_uploadedby (UploadedBy);

-- One live row per election + VD + ballot + kind + party. A new save marks the previous
-- row as replaced (IsLive NULL) instead of deleting it. Rows from before v2 have
-- Ballot NULL, so the unique key ignores them; they stay live (IsLive 1).
ALTER TABLE ResultsSlip
    ADD COLUMN Ballot VARCHAR(20) NULL,
    -- slip = from a photographed results slip; special = special votes added later (e.g. IEC data)
    ADD COLUMN Kind VARCHAR(20) NOT NULL DEFAULT 'slip',
    -- auto = saved straight after the read, not yet checked; confirmed = checked and saved by a person
    ADD COLUMN Status VARCHAR(20) NULL,
    ADD COLUMN SlipImageId BIGINT NULL,
    ADD COLUMN SavedBy VARCHAR(20) NULL,
    ADD COLUMN SavedAt TIMESTAMP NULL DEFAULT NULL,
    ADD COLUMN IsLive TINYINT NULL DEFAULT 1,
    ADD COLUMN ReplacedAt TIMESTAMP NULL DEFAULT NULL,
    ADD COLUMN ReplacedBy VARCHAR(20) NULL,
    ADD UNIQUE KEY uq_live_result (Election, VD, Ballot, Kind, Party_name, IsLive),
    ADD KEY idx_live_vd (VD, IsLive);
