
--
-- Table structure for table `list_learnedskills`
--



CREATE TABLE IF NOT EXISTS `list_learnedskills` (
  `CharId` int(10) unsigned NOT NULL DEFAULT '0',
  `SkillId` int(10) unsigned NOT NULL DEFAULT '0',
  `SkillExp` int(10) unsigned NOT NULL DEFAULT '0',
  `Job` tinyint(3) unsigned NOT NULL DEFAULT '0',
  KEY `CharId` (`CharId`)
);


