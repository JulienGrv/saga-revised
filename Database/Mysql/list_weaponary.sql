
--
-- Table structure for table `list_weaponary`
--



CREATE TABLE IF NOT EXISTS `list_weaponary` (
  `CharId` int(10) unsigned NOT NULL,
  `Weaponary` mediumblob NOT NULL,
  `UnlockedWeaponCount` tinyint(1) unsigned NOT NULL DEFAULT '1',
  `PrimairyWeapon` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `SecondaryWeapoin` tinyint(1) unsigned NOT NULL DEFAULT '255',
  `ActiveWeaponIndex` tinyint(3) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`CharId`)
);


