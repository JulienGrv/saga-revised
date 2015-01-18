
--
-- Table structure for table `list_eventparticipants`
--



CREATE TABLE IF NOT EXISTS `list_eventparticipants` (
  `EventId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CharId` int(10) unsigned DEFAULT NULL,
  `CharName` varchar(128) NOT NULL,
  PRIMARY KEY (`EventId`)
);


