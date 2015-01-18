
--
-- Table structure for table `list_worldcharacters`
--



CREATE TABLE IF NOT EXISTS `list_worldcharacters` (
  `UserId` tinyint(3) unsigned NOT NULL,
  `WorldId` tinyint(3) unsigned NOT NULL,
  `CharacterCount` tinyint(3) unsigned NOT NULL,
  KEY `Index_1` (`UserId`)
);


