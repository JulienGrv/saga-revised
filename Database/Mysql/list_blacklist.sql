
--
-- Table structure for table `list_blacklist`
--



CREATE TABLE IF NOT EXISTS `list_blacklist` (
  `CharId` int(10) unsigned NOT NULL DEFAULT '0',
  `FriendName` varchar(32) NOT NULL,
  `Reason` tinyint(3) unsigned NOT NULL,
  KEY `CharId` (`CharId`)
);


