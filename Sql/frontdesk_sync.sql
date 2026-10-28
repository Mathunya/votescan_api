-- Front Desk sync (review note: B1 duplicate check, B2 ward changes)
-- FOR REVIEW BY FARAI BEFORE RUNNING. Run once on the votescan database (Galera cluster).
-- Only the Sender='add' branch of getFrontDeskRecorded changes; every other branch is
-- copied unchanged from the live procedure (dumped 2026-10-28).

-- B1: store the voter's AllVoters row and allow one capture per voter per election.
-- Existing rows get NULL; a unique key allows any number of NULLs.
ALTER TABLE FrontDeskRecorded
    ADD COLUMN VoterRowID INT NULL AFTER Id,
    ADD UNIQUE KEY uq_voter_election (VoterRowID, Election);

-- B2: per-ward roll version. A ward with no row is version 1. Bump a ward's Version only
-- when its IEC roll changes; phones re-download that ward's roll when it differs.
CREATE TABLE IF NOT EXISTS WardRollVersion (
    WardID VARCHAR(50) NOT NULL PRIMARY KEY,
    Version INT NOT NULL DEFAULT 1,
    UpdatedAt TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- B1: procedure with the new 'add' branch (definer and sql_mode as live).
DROP PROCEDURE IF EXISTS getFrontDeskRecorded;
DELIMITER $$
CREATE DEFINER=`dba_admin`@`10.30.0.%` PROCEDURE `getFrontDeskRecorded`(
    IN Province VARCHAR(50),
    IN Municipality VARCHAR(100),
    IN WardID VARCHAR(50),
    IN VDNumber VARCHAR(50),
    IN VRSection VARCHAR(50),
    IN SeqNo VARCHAR(50),
    IN IDNumber VARCHAR(50),
    IN Surname VARCHAR(100),
    IN FullName VARCHAR(150),
    IN VR_Address VARCHAR(500),
    IN StreetNumber VARCHAR(50),
    IN StreetName VARCHAR(100),
    IN StreetType VARCHAR(50),
    IN PlaceNumber VARCHAR(50),
    IN PlaceName VARCHAR(100),
    IN HouseNumber VARCHAR(50),
    IN FarmName VARCHAR(100),
    IN FarmNumber VARCHAR(50),
    IN Location VARCHAR(100),
    IN Suburb VARCHAR(100),
    IN Town VARCHAR(100),
    IN ResPostalCode VARCHAR(20),
    IN VoterID VARCHAR(100),
    IN Election VARCHAR(100),
    IN ID VARCHAR(50),
    IN `Date` VARCHAR(50),
    IN ScanVD VARCHAR(50),
    IN Sender VARCHAR(50),
    IN region VARCHAR(150),
    IN startDate VARCHAR(50),
    IN endDate VARCHAR(50)
)
BEGIN

IF Sender='add' THEN
    -- One capture per voter per election. The voter is identified by their AllVoters row
    -- (ID), not by name or the masked ID number, and uq_voter_election stops two phones
    -- capturing the same voter at the same moment from both inserting.
    -- Returns Result: 1 = captured now, 2 = already captured (by anyone).
    BEGIN
        DECLARE v_row INT DEFAULT NULL;
        DECLARE v_inserted INT DEFAULT 0;
        DECLARE v_dup INT DEFAULT 0;
        DECLARE CONTINUE HANDLER FOR 1062 SET v_dup = 1;

        IF ID REGEXP '^[0-9]+$' THEN
            SET v_row = CAST(ID AS SIGNED);
        END IF;

        IF v_row IS NOT NULL THEN
            INSERT INTO FrontDeskRecorded(
            VoterRowID,
            Province,
            Municipality,
            WardID,
            VDNumber,
            VRSection,
            SeqNo,
            IDNumber,
            Surname,
            FullName,
            VR_Address,
            StreetNumber,
            StreetName,
            StreetType,
            PlaceNumber,
            PlaceName,
            HouseNumber,
            FarmName,
            FarmNumber,
            Location,
            Suburb,
            Town,
            ResPostalCode,
            VoterID,
            Election,
            ScanVD
            )
            SELECT
            v_row,
            Province,
            Municipality,
            WardID,
            VDNumber,
            VRSection,
            SeqNo,
            IDNumber,
            Surname,
            FullName,
            VR_Address,
            StreetNumber,
            StreetName,
            StreetType,
            PlaceNumber,
            PlaceName,
            HouseNumber,
            FarmName,
            FarmNumber,
            Location,
            Suburb,
            Town,
            ResPostalCode,
            VoterID,
            Election,
            ScanVD
            FROM DUAL
            WHERE NOT EXISTS (
                SELECT 1
                FROM FrontDeskRecorded fr
                WHERE fr.VoterRowID = v_row
                AND fr.Election <=> Election
            );
            SET v_inserted = ROW_COUNT();
            IF v_dup = 1 THEN
                SET v_inserted = 0;
            END IF;
        ELSE
            -- no voter row sent (very old clients): keep the previous name-based check
            IF NOT EXISTS (
                SELECT *
                FROM FrontDeskRecorded fr
                WHERE fr.Surname = Surname
                AND fr.FullName = FullName
                AND fr.IDNumber = IDNumber
                AND fr.WardID = WardID
                AND fr.VDNumber = VDNumber
            ) THEN
                INSERT INTO FrontDeskRecorded(
            Province,
            Municipality,
            WardID,
            VDNumber,
            VRSection,
            SeqNo,
            IDNumber,
            Surname,
            FullName,
            VR_Address,
            StreetNumber,
            StreetName,
            StreetType,
            PlaceNumber,
            PlaceName,
            HouseNumber,
            FarmName,
            FarmNumber,
            Location,
            Suburb,
            Town,
            ResPostalCode,
            VoterID,
            Election,
            ScanVD
                )
                VALUES(
            Province,
            Municipality,
            WardID,
            VDNumber,
            VRSection,
            SeqNo,
            IDNumber,
            Surname,
            FullName,
            VR_Address,
            StreetNumber,
            StreetName,
            StreetType,
            PlaceNumber,
            PlaceName,
            HouseNumber,
            FarmName,
            FarmNumber,
            Location,
            Suburb,
            Town,
            ResPostalCode,
            VoterID,
            Election,
            ScanVD
                );
                SET v_inserted = 1;
            END IF;
        END IF;

        UPDATE AllVoters av
        SET av.Recorded='yes'
        WHERE av.ID = ID;

        SELECT IF(v_inserted = 1, 1, 2) AS Result;
    END;
END IF;


IF Sender='view' THEN

    SELECT
        fr.Province,
        fr.Municipality,
        fr.WardID,
        fr.VDNumber,
        fr.VRSection,
        fr.SeqNo,
        fr.IDNumber,
        fr.Surname,
        fr.FullName,
        fr.VR_Address,
        fr.StreetNumber,
        fr.StreetName,
        fr.StreetType,
        fr.PlaceNumber,
        fr.PlaceName,
        fr.HouseNumber,
        fr.FarmName,
        fr.FarmNumber,
        fr.Location,
        fr.Suburb,
        fr.Town,
        fr.ResPostalCode,
        fr.VoterID,
        fr.`Date`,
        fr.Election,
        fr.ScanVD
    FROM FrontDeskRecorded fr
    ORDER BY fr.Surname ASC;

END IF;


IF Sender='byelection' THEN

    SELECT *
    FROM FrontDeskRecorded fr
    WHERE fr.Election = Election
    ORDER BY fr.Surname ASC;

END IF;


IF Sender='byward' THEN

    SELECT *
    FROM FrontDeskRecorded fr
    WHERE fr.WardID = WardID
    ORDER BY fr.Surname ASC;

END IF;


IF Sender='byvd' THEN

    SELECT *
    FROM FrontDeskRecorded fr
    WHERE fr.VDNumber = VDNumber
    ORDER BY fr.Surname ASC;

END IF;


IF Sender='onewardnumbers' THEN

    SELECT
        aw.WardID,
        aw.total,
        COALESCE(COUNT(fr.WardID),0) AS scanned
    FROM
    (
        SELECT
            av.WardID,
            COUNT(*) AS total
        FROM AllVoters av
        WHERE av.WardID = WardID
        GROUP BY av.WardID
    ) aw

    LEFT JOIN FrontDeskRecorded fr
        ON aw.WardID = fr.WardID
        AND DATE(fr.`Date`)
        BETWEEN DATE('2024-05-27')
        AND DATE('2024-05-29')

    GROUP BY aw.WardID, aw.total
    ORDER BY scanned DESC;

END IF;


IF Sender='vdnumbers' THEN

    SELECT
        fr.VDNumber,
        COUNT(DISTINCT CONCAT(
            fr.FullName,
            fr.Surname,
            fr.IDNumber
        )) AS Total
    FROM FrontDeskRecorded fr
    WHERE DATE(fr.`Date`)
    IN ('2024-02-13','2024-02-14')
    AND fr.WardID = WardID
    GROUP BY fr.VDNumber

    UNION ALL

    SELECT
        'Other' AS WardCategory,
        COUNT(*) AS Total
    FROM FrontDeskRecorded fr
    WHERE DATE(fr.`Date`)
    IN ('2024-02-13','2024-02-14')
    AND fr.WardID != WardID;

END IF;


IF Sender='getmunicipalityprogress' THEN

    SELECT
        fr.Municipality,
        COUNT(*) AS scanned
    FROM FrontDeskRecorded fr
    WHERE fr.Election = Election
    OR DATE(fr.`Date`) = DATE(`Date`)
    GROUP BY fr.Municipality
    ORDER BY scanned DESC;

END IF;


IF Sender='getprovinceprogress' THEN

    SELECT
        (
            SELECT COUNT(*)
            FROM FrontDeskRecorded fr
            WHERE fr.Election = Election
            OR DATE(fr.`Date`) = DATE(`Date`)
        ) AS recorded,

        (
            SELECT COUNT(*)
            FROM AllVoters
        ) AS total,

        (
            SELECT COUNT(*)
            FROM Users
        ) AS users;

END IF;


IF Sender='getprovincialmunicipalities' THEN

    SELECT
        am.Municipality,
        COALESCE(COUNT(fr.Municipality),0) AS scanned
    FROM
    (
        SELECT DISTINCT Municipality
        FROM FrontDeskRecorded
    ) am

    LEFT JOIN FrontDeskRecorded fr
        ON am.Municipality = fr.Municipality
        AND DATE(fr.`Date`)
        BETWEEN DATE('2024-05-27')
        AND DATE('2024-05-29')

    GROUP BY am.Municipality
    ORDER BY scanned DESC;

END IF;


IF Sender='getmunicipalitywards' THEN

    SELECT
        aw.WardID,
        COALESCE(COUNT(fr.WardID),0) AS scanned
    FROM
    (
        SELECT DISTINCT av.WardID
        FROM AllVoters av
        WHERE av.Municipality = Municipality
    ) aw

    LEFT JOIN FrontDeskRecorded fr
        ON aw.WardID = fr.WardID
        AND DATE(fr.`Date`)
        BETWEEN DATE('2024-05-27')
        AND DATE('2024-05-29')

    GROUP BY aw.WardID
    ORDER BY scanned DESC;

END IF;


IF Sender='getwardvds' THEN

    SELECT
        av.VDNumber,
        av.VDName,
        av.total,
        COALESCE(COUNT(fr.VDNumber),0) AS scanned
    FROM
    (
        SELECT
            av2.VDNumber,
            av2.VDName,
            COUNT(av2.VDName) AS total
        FROM AllVoters av2
        WHERE av2.WardID = WardID
        GROUP BY av2.VDNumber,av2.VDName
    ) av

    LEFT JOIN FrontDeskRecorded fr
        ON av.VDNumber = fr.VDNumber
        AND DATE(fr.`Date`)
        BETWEEN DATE('2024-05-27')
        AND DATE('2024-05-29')

    GROUP BY
        av.VDNumber,
        av.VDName,
        av.total

    ORDER BY scanned DESC;

END IF;

IF Sender = 'regionalcount' THEN

    SELECT
        z.rgn AS Region,
        SUM(CASE WHEN z.d = DATE(endDate) THEN 1 ELSE 0 END) AS Total,
        SUM(CASE WHEN z.d = DATE(startDate) THEN 1 ELSE 0 END) AS Total0
    FROM (
        SELECT
            CASE fr.Municipality
        WHEN 'FS161 - Letsemeng' THEN 'Xhariep Region'
        WHEN 'FS162 - Kopanong' THEN 'Xhariep Region'
        WHEN 'FS163 - Mohokare' THEN 'Xhariep Region'
        WHEN 'FS164 - New Local Municipality' THEN 'Xhariep Region'
        WHEN 'FS181 - Masilonyana' THEN 'Lejweleputswa Region'
        WHEN 'FS182 - Tokologo' THEN 'Lejweleputswa Region'
        WHEN 'FS183 - Tswelopele' THEN 'Lejweleputswa Region'
        WHEN 'FS184 - Matjhabeng' THEN 'Lejweleputswa Region'
        WHEN 'FS185 - Nala' THEN 'Lejweleputswa Region'
        WHEN 'FS191 - Setsoto' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS192 - Dihlabeng' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS193 - Nketoana' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS194 - Maluti a Phofung' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS195 - Phumelela' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS196 - Mantsopa' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS201 - Moqhaka' THEN 'Fezile Dabi Region'
        WHEN 'FS203 - Ngwathe' THEN 'Fezile Dabi Region'
        WHEN 'FS204 - Metsimaholo' THEN 'Fezile Dabi Region'
        WHEN 'FS205 - Mafube' THEN 'Fezile Dabi Region'
        WHEN 'MAN - Mangaung' THEN 'Mangaung Region'
        ELSE NULL
    END AS rgn,
            DATE(fr.`Date`) AS d
        FROM FrontDeskRecorded fr
        WHERE DATE(fr.`Date`) BETWEEN DATE(startDate) AND DATE(endDate)
    ) z
    GROUP BY z.rgn
    ORDER BY Total DESC;

END IF;


IF Sender = 'municipalcount' THEN

    SELECT
        fr.Municipality,
        SUM(CASE WHEN DATE(fr.`Date`) = DATE(endDate) THEN 1 ELSE 0 END) AS Total,
        SUM(CASE WHEN DATE(fr.`Date`) = DATE(startDate) THEN 1 ELSE 0 END) AS Total0
    FROM FrontDeskRecorded fr
    WHERE DATE(fr.`Date`) BETWEEN DATE(startDate) AND DATE(endDate)
      AND CASE fr.Municipality
        WHEN 'FS161 - Letsemeng' THEN 'Xhariep Region'
        WHEN 'FS162 - Kopanong' THEN 'Xhariep Region'
        WHEN 'FS163 - Mohokare' THEN 'Xhariep Region'
        WHEN 'FS164 - New Local Municipality' THEN 'Xhariep Region'
        WHEN 'FS181 - Masilonyana' THEN 'Lejweleputswa Region'
        WHEN 'FS182 - Tokologo' THEN 'Lejweleputswa Region'
        WHEN 'FS183 - Tswelopele' THEN 'Lejweleputswa Region'
        WHEN 'FS184 - Matjhabeng' THEN 'Lejweleputswa Region'
        WHEN 'FS185 - Nala' THEN 'Lejweleputswa Region'
        WHEN 'FS191 - Setsoto' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS192 - Dihlabeng' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS193 - Nketoana' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS194 - Maluti a Phofung' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS195 - Phumelela' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS196 - Mantsopa' THEN 'Thabo Mofutsanyane Region'
        WHEN 'FS201 - Moqhaka' THEN 'Fezile Dabi Region'
        WHEN 'FS203 - Ngwathe' THEN 'Fezile Dabi Region'
        WHEN 'FS204 - Metsimaholo' THEN 'Fezile Dabi Region'
        WHEN 'FS205 - Mafube' THEN 'Fezile Dabi Region'
        WHEN 'MAN - Mangaung' THEN 'Mangaung Region'
        ELSE NULL
    END = region
    GROUP BY fr.Municipality
    ORDER BY Total DESC;

END IF;


IF Sender = 'wardcount' THEN

    SELECT
        aw.WardID,
        SUM(CASE WHEN DATE(fr.`Date`) = DATE(endDate) THEN 1 ELSE 0 END) AS Total,
        SUM(CASE WHEN DATE(fr.`Date`) = DATE(startDate) THEN 1 ELSE 0 END) AS Total0
    FROM (
        SELECT DISTINCT av.WardID
        FROM AllVoters av
        WHERE av.Municipality = Municipality
    ) aw
    LEFT JOIN FrontDeskRecorded fr
        ON fr.WardID = aw.WardID
       AND DATE(fr.`Date`) BETWEEN DATE(startDate) AND DATE(endDate)
    GROUP BY aw.WardID
    ORDER BY Total DESC;

END IF;


IF Sender = 'vdcount' THEN

    SELECT
        av.VDNumber,
        av.VDName,
        SUM(CASE WHEN DATE(fr.`Date`) = DATE(endDate) THEN 1 ELSE 0 END) AS Total,
        SUM(CASE WHEN DATE(fr.`Date`) = DATE(startDate) THEN 1 ELSE 0 END) AS Total0
    FROM (
        SELECT DISTINCT av2.VDNumber, av2.VDName
        FROM AllVoters av2
        WHERE av2.WardID = WardID
    ) av
    LEFT JOIN FrontDeskRecorded fr
        ON fr.VDNumber = av.VDNumber
       AND fr.WardID = WardID
       AND DATE(fr.`Date`) BETWEEN DATE(startDate) AND DATE(endDate)
    GROUP BY av.VDNumber, av.VDName
    ORDER BY Total DESC;

END IF;

END$$
DELIMITER ;
