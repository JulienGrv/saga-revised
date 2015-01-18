--
-- Dumping routines for database 'saga'
--

DROP PROCEDURE IF EXISTS `list_availablequests`;
DROP PROCEDURE IF EXISTS `list_availablepersonalrequests`;

DELIMITER $$

CREATE PROCEDURE `list_availablepersonalrequests`(CharacterId int, regionid int, clvl int, jlvl int, qid int)
BEGIN
  SELECT
    `list_quests`.`QuestId`,
    `list_quests`.`NPC`
  FROM
    `list_quests`
  WHERE
    `list_quests`.`QuestId` != qid AND
    `list_quests`.`QuestType`=2 AND
    `list_quests`.`Req_Clvl` <= clvl AND
    `list_quests`.`Req_Jlvl` <= jlvl AND
    `list_quests`.`NPC` > 0 AND
    `list_quests`.`QuestId` NOT IN ( SELECT `list_questhistory`.`QuestId` FROM `list_questhistory` WHERE `list_questhistory`.`CharId`=CharacterId ) AND
    (
      `list_quests`.`Req_Quest`='0' OR
      `list_quests`.`Req_Quest` IN (SELECT `list_questhistory`.`QuestId` FROM `list_questhistory` WHERE `list_questhistory`.`CharId`=CharacterId)
    )
  GROUP BY
    `list_quests`.`NPC`
  LIMIT 50;

END $$

CREATE PROCEDURE `list_availablequests`(CharacterId int, regionid int, clvl int, jlvl int)
BEGIN
  SELECT
    *
  FROM
    `list_quests`
  WHERE
    `list_quests`.`NPC`=regionid AND
    `list_quests`.`QuestType`=1 AND
    `list_quests`.`Req_Clvl` <= clvl AND
    `list_quests`.`Req_Jlvl` <= jlvl AND
    `list_quests`.`QuestId` NOT IN ( SELECT `list_questhistory`.`QuestId` FROM `list_questhistory` WHERE `list_questhistory`.`CharId`=CharacterId ) AND
    (
      `list_quests`.`Req_Quest`='0' OR
      `list_quests`.`Req_Quest` IN (SELECT `list_questhistory`.`QuestId` FROM `list_questhistory` WHERE `list_questhistory`.`CharId`=CharacterId)
    )
  LIMIT 30;
END $$

DELIMITER ;
