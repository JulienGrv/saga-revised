
--
-- Table structure for table `list_storage`
--



CREATE TABLE IF NOT EXISTS `list_storage` (
  `CharId` int(10) unsigned NOT NULL,
  `ContainerMaxStorage` tinyint(3) unsigned NOT NULL,
  `Container` blob,
  PRIMARY KEY (`CharId`)
);


