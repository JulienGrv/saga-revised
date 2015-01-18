
--
-- Table structure for table `list_worlds`
--



CREATE TABLE IF NOT EXISTS `list_worlds` (
  `Id` tinyint(1) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) NOT NULL,
  `Proof` varchar(128) NOT NULL,
  PRIMARY KEY (`Id`)
);


