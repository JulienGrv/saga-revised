
--
-- Table structure for table `list_joblinformation`
--



CREATE TABLE IF NOT EXISTS `list_joblinformation` (
  `CharId` int(10) unsigned NOT NULL DEFAULT '0',
  `JobInformation` tinyblob,
  PRIMARY KEY (`CharId`),
  KEY `CharId` (`CharId`)
);


