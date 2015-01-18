
--
-- Table structure for table `list_maildata`
--



CREATE TABLE IF NOT EXISTS `list_maildata` (
  `MailId` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Sender` varchar(32) NOT NULL,
  `Receiptent` varchar(32) NOT NULL,
  `Date` datetime NOT NULL,
  `IsRead` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `IsChecked` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `Topic` varchar(128) DEFAULT NULL,
  `Message` text,
  `Attachment` tinyblob,
  `Zeny` int(10) unsigned NOT NULL DEFAULT '0',
  `DateRead` datetime DEFAULT '0000-00-00 00:00:00',
  `IsOutbox` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `IsInbox` tinyint(3) unsigned NOT NULL DEFAULT '0',
  `IsPending` tinyint(3) unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`MailId`),
  KEY `Sender` (`Sender`),
  KEY `Reciever` (`Receiptent`)
);


