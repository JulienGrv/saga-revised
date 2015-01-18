
--
-- Table structure for table `list_acl`
--



CREATE TABLE IF NOT EXISTS `list_acl` (
  `RuleId` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT 'Unique id used for delete statements',
  `FilterIp` bigint(20) unsigned NOT NULL COMMENT 'Adress to match',
  `Mask` bigint(20) unsigned NOT NULL COMMENT 'Mask of bits to check',
  `Operation` tinyint(3) unsigned NOT NULL DEFAULT '0' COMMENT '0 For deny and 1 for allow',
  PRIMARY KEY (`RuleId`)
);


