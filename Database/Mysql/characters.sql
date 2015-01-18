
--
-- Table structure for table `characters`
--



CREATE TABLE IF NOT EXISTS `characters` (
  `CharId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `CharName` varchar(32) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `CharFace` varbinary(11) DEFAULT NULL,
  `UserId` int(10) unsigned NOT NULL DEFAULT '0',
  `Cexp` int(10) unsigned NOT NULL DEFAULT '1',
  `Jexp` int(10) unsigned NOT NULL DEFAULT '1',
  `Job` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `Map` tinyint(3) unsigned NOT NULL DEFAULT '11',
  `Gender` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `HP` mediumint(8) unsigned NOT NULL DEFAULT '200',
  `SP` mediumint(8) unsigned NOT NULL DEFAULT '210',
  `LP` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `LC` tinyint(3) unsigned NOT NULL DEFAULT '45',
  `Position.x` float NOT NULL DEFAULT '-17208',
  `Position.y` float NOT NULL DEFAULT '9944',
  `Position.z` float NOT NULL DEFAULT '109',
  `Saveposition.x` float NOT NULL DEFAULT '-6558.26',
  `Saveposition.y` float NOT NULL DEFAULT '14842.2',
  `Saveposition.z` float NOT NULL DEFAULT '4322.33',
  `Saveposition.map` tinyint(3) unsigned NOT NULL DEFAULT '3',
  `Stats.Str` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Stats.Dex` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Stats.Int` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Stats.Con` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Stats.Luc` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Stats.Pending` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `Rufi` int(10) unsigned NOT NULL DEFAULT '0',
  `UppercasedCharName` varchar(32) DEFAULT NULL,	
  PRIMARY KEY (`CharId`,`UserId`) USING BTREE
);


