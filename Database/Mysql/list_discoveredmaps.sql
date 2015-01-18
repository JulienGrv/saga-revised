
--
-- Table structure for table `list_discoveredmaps`
--



CREATE TABLE IF NOT EXISTS `list_discoveredmaps` (
  `CharId` int(10) unsigned NOT NULL,
  `MapId` tinyint(3) unsigned NOT NULL,
  `Value` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`CharId`,`MapId`)
);


