
--
-- Table structure for table `list_zoneinformation`
--



CREATE TABLE IF NOT EXISTS `list_zoneinformation` (
  `CharId` int(10) unsigned NOT NULL DEFAULT '0',
  `ZoneState` blob,
  PRIMARY KEY (`CharId`)
);


