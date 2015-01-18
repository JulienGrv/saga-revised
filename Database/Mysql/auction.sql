
--
-- Table structure for table `auction`
--



CREATE TABLE IF NOT EXISTS `auction` (
  `AuctionId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Categorie` tinyint(3) unsigned NOT NULL,
  `Grade` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `CharId` int(10) unsigned NOT NULL DEFAULT '0',
  `CharName` varchar(20) NOT NULL,
  `ItemName` varchar(50) NOT NULL,
  `ReqClvl` tinyint(3) unsigned NOT NULL,
  `Price` int(10) unsigned NOT NULL,
  `ItemContent` tinyblob NOT NULL,
  `Expires` datetime NOT NULL,
  PRIMARY KEY (`AuctionId`),
  KEY `Categorie` (`Categorie`),
  KEY `ItemName` (`ItemName`),
  KEY `ReqClvl` (`ReqClvl`)
);


