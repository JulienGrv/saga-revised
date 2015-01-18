
--
-- Table structure for table `list_equipment`
--



CREATE TABLE IF NOT EXISTS `list_equipment` (
  `CharId` int(10) unsigned NOT NULL,
  `Equipement` blob,
  UNIQUE KEY `CharId` (`CharId`) USING BTREE
);


