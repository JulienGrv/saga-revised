
--
-- Table structure for table `list_friends`
--



CREATE TABLE IF NOT EXISTS `list_friends` (
  `CharId` int(10) unsigned NOT NULL DEFAULT '0',
  `FriendName` varchar(32) NOT NULL,
  KEY `CharId` (`CharId`)
);


