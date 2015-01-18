
--
-- Table structure for table `auction_comment`
--



CREATE TABLE IF NOT EXISTS `auction_comment` (
  `CharId` int(10) unsigned NOT NULL,
  `Comment` varchar(128) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`CharId`)
);


