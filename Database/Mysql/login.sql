
--
-- Table structure for table `login`
--



CREATE TABLE IF NOT EXISTS `login` (
  `UserId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Username` varchar(32) NOT NULL,
  `Password` varchar(32) NOT NULL,
  `Gender` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `LastUserIP` bigint(20) unsigned NOT NULL DEFAULT '0',
  `LastSession` int(10) unsigned NOT NULL DEFAULT '0',
  `ActiveSession` int(10) unsigned NOT NULL DEFAULT '0',
  `LastLoginDate` datetime DEFAULT NULL,
  `IsActivated` tinyint(1) NOT NULL DEFAULT '0',
  `IsBanned` tinyint(1) NOT NULL DEFAULT '0',
  `HasAgreed` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `DateOfBirth` date NOT NULL,
  `DateOfRegistration` date NOT NULL,
  `IsTestAccount` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `LastPlayedWorld` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `GmLevel` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `GameTime` bigint(20) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`UserId`,`Username`),
  UNIQUE KEY `Username` (`Username`)
);


