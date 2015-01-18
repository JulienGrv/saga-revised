
--
-- Table structure for table `list_inventory`
--



CREATE TABLE IF NOT EXISTS `list_inventory` (
  `CharId` int(10) unsigned NOT NULL,
  `ContainerMaxStorage` tinyint(3) unsigned NOT NULL,
  `Container` blob,
  PRIMARY KEY (`CharId`)
);


