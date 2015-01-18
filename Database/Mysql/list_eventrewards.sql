
--
-- Table structure for table `list_eventrewards`
--



CREATE TABLE IF NOT EXISTS `list_eventrewards` (
  `RewardId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CharId` int(10) unsigned NOT NULL,
  `ItemId` int(10) unsigned NOT NULL,
  `ItemCount` tinyint(3) unsigned DEFAULT '1',
  PRIMARY KEY (`RewardId`)
);


