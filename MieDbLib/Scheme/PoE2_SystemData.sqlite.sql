BEGIN TRANSACTION;
DROP TABLE IF EXISTS `SpeakerAttributes`;
CREATE TABLE IF NOT EXISTS `SpeakerAttributes` (
	`ID`	BLOB NOT NULL,
	`Name`	TEXT,
	`Gender`	INTEGER,
	PRIMARY KEY(`ID`)
);
DROP TABLE IF EXISTS `RaceAttributes`;
CREATE TABLE IF NOT EXISTS `RaceAttributes` (
	`RaceID`	BLOB NOT NULL,
	`Name`	TEXT,
	PRIMARY KEY(`RaceID`)
);
DROP TABLE IF EXISTS `QuestsNodeLinks`;
CREATE TABLE IF NOT EXISTS `QuestsNodeLinks` (
	`FileCode`	INTEGER NOT NULL,
	`FromNodeID`	INTEGER NOT NULL,
	`ToNodeID`	INTEGER NOT NULL,
	PRIMARY KEY(`FileCode`,`FromNodeID`,`ToNodeID`)
);
DROP TABLE IF EXISTS `QuestsNodeEntries`;
CREATE TABLE IF NOT EXISTS `QuestsNodeEntries` (
	`FileCode`	INTEGER NOT NULL,
	`NodeID`	INTEGER NOT NULL,
	`NodeType`	INTEGER,
	`IsRootNode`	INTEGER,
	PRIMARY KEY(`FileCode`,`NodeID`)
);
DROP TABLE IF EXISTS `LanguageEntryHistory`;
CREATE TABLE IF NOT EXISTS `LanguageEntryHistory` (
	`UpdatedAt`	INTEGER NOT NULL,
	`FileCode`	INTEGER NOT NULL,
	`ID`	INTEGER NOT NULL,
	`DefaultText`	TEXT,
	`FemaleText`	TEXT,
	`ProductLine`	INTEGER NOT NULL,
	PRIMARY KEY(`UpdatedAt`,`FileCode`,`ID`)
);
DROP TABLE IF EXISTS `LanguageEntries`;
CREATE TABLE IF NOT EXISTS `LanguageEntries` (
	`FileCode`	INTEGER NOT NULL,
	`ID`	INTEGER NOT NULL,
	`ReferenceID`	INTEGER,
	`ReferenceText`	TEXT,
	`DefaultText`	TEXT,
	`FemaleText`	TEXT,
	`ProductLine`	INTEGER NOT NULL,
	`UpdatedAt`	INTEGER NOT NULL,
	PRIMARY KEY(`ID`,`FileCode`)
);
DROP TABLE IF EXISTS `FileList`;
CREATE TABLE IF NOT EXISTS `FileList` (
	`FileCode`	INTEGER NOT NULL,
	`FileID`	TEXT NOT NULL,
	`LanguageType`	INTEGER,
	`UpdatedAt`	INTEGER,
	PRIMARY KEY(`FileCode`)
);
DROP TABLE IF EXISTS `ConversationNodeLinks`;
CREATE TABLE IF NOT EXISTS `ConversationNodeLinks` (
	`FileCode`	INTEGER NOT NULL,
	`FromNodeID`	INTEGER NOT NULL,
	`ToNodeID`	INTEGER NOT NULL,
	PRIMARY KEY(`FileCode`,`FromNodeID`,`ToNodeID`)
);
DROP TABLE IF EXISTS `ConversationNodeEntries`;
CREATE TABLE IF NOT EXISTS `ConversationNodeEntries` (
	`FileCode`	INTEGER NOT NULL,
	`NodeID`	INTEGER NOT NULL,
	`NodeType`	INTEGER,
	`SpeakerGuid`	BLOB,
	`ListenerGuid`	BLOB,
	`IsRootNode`	INTEGER,
	`IsQuestionNode`	INTEGER,
	PRIMARY KEY(`FileCode`,`NodeID`)
);
DROP TABLE IF EXISTS `ChatterNodeLinks`;
CREATE TABLE IF NOT EXISTS `ChatterNodeLinks` (
	`FileCode`	INTEGER NOT NULL,
	`FromNodeID`	INTEGER NOT NULL,
	`ToNodeID`	INTEGER NOT NULL,
	PRIMARY KEY(`FileCode`,`FromNodeID`,`ToNodeID`)
);
DROP TABLE IF EXISTS `ChatterNodeEntries`;
CREATE TABLE IF NOT EXISTS `ChatterNodeEntries` (
	`FileCode`	INTEGER NOT NULL,
	`NodeID`	INTEGER NOT NULL,
	`NodeType`	INTEGER,
	`IsRootNode`	INTEGER,
	PRIMARY KEY(`FileCode`,`NodeID`)
);
DROP TABLE IF EXISTS `CharacterAttributes`;
CREATE TABLE IF NOT EXISTS `CharacterAttributes` (
	`ID`	BLOB NOT NULL,
	`Name`	TEXT,
	`Gender`	INTEGER,
	`SpeakerID`	BLOB,
	`RaceID`	BLOB,
	PRIMARY KEY(`ID`)
);
DROP VIEW IF EXISTS `v_LanguageEntries`;
CREATE VIEW v_LanguageEntries AS 
SELECT 
  FileList.LanguageType, LanguageEntries.FileCode, FileList.FileID, FileList.LanguageType, 
  LanguageEntries.ReferenceID, LanguageEntries.DefaultText, LanguageEntries.ProductLine
FROM LanguageEntries 
  INNER JOIN FileList ON LanguageEntries.FileCode = FileList.FileCode
WHERE (((FileList.LanguageType)="0"));
COMMIT;
