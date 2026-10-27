-- Tables for results slip scanning (ResultsSlipController: scan, image, images/{vd}, imagefile/{id}).
-- Run once on the VoteScan database before deploying the results-slip-scan API changes.

-- Backup photos of results slips. Kept in the database, like chat images, so every
-- API node serves the same photos and deploys never remove them. Max 2MB each.
CREATE TABLE IF NOT EXISTS ResultsSlipImages (
    Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    VD VARCHAR(20) NOT NULL,
    Election VARCHAR(255) NOT NULL DEFAULT '',
    Day VARCHAR(20) NOT NULL DEFAULT '',
    Image MEDIUMBLOB NOT NULL,
    ImageMimeType VARCHAR(50) NOT NULL,
    UploadedBy VARCHAR(20) NOT NULL DEFAULT '',
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX IX_ResultsSlipImages_VD (VD, CreatedAt)
);

-- One row per slip scan, used to cap paid Claude calls per user per day
-- (ResultsSlip:MaxScansPerDay, default 20).
CREATE TABLE IF NOT EXISTS ResultsSlipScans (
    Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
    Cell VARCHAR(20) NOT NULL,
    CreatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX IX_ResultsSlipScans_Cell (Cell, CreatedAt)
);
