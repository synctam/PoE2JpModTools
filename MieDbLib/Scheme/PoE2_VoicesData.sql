BEGIN TRANSACTION;
DROP TABLE IF EXISTS `Voices`;
CREATE TABLE IF NOT EXISTS `Voices` (
	`ID`	TEXT NOT NULL,
	`SHA1`	BLOB NOT NULL,
	`Voice`	BLOB NOT NULL,
	PRIMARY KEY(`ID`)
);
COMMIT;
