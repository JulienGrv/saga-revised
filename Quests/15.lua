-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:14 PM

local QuestID = 15;
local ReqClv = 11;
local ReqJlv = 0;
local NextQuest = 16;
local RewZeny = 408;
local RewCxp = 929;
local RewJxp = 368;
local RewWxp = 0; 
local RewItem1 = 1700113; 
local RewItem2 = 0; 
local RewItemCount1 = 10; 
local RewItemCount2 = 0; 
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_VERIFY(cid)
	Saga.GeneralDialog(cid, 3957);
	return 0;
end

function QUEST_START(cid)
	Saga.AddStep(cid, QuestID, 1501);
    Saga.AddStep(cid, QuestID, 1502);
    Saga.InsertQuest(cid, QuestID, 1);
	return 0;
end

function QUEST_FINISH(cid)
	-- Gives all rewards
	local freeslots = Saga.FreeInventoryCount(cid, 0);
	if freeslots > 0 then
		Saga.GiveZeny(cid, RewZeny);
		Saga.GiveExp(cid, RewCxp, RewJxp, RewWxp);
		Saga.GiveItem(cid, RewItem1, RewItemCount1 );
		return 0;
	else
		Saga.EmptyInventory(cid);
		return -1;
	end
end

function QUEST_CANCEL(cid)
	return 0;
end

function QUEST_STEP_1(cid)
	--Get Rodha Frog's stomach;Get Cave Puffer's poison sac;Obtain Flatro's organs;Obtain some loot from VadonVadon

	Saga.FindQuestItem(cid,QuestID,StepID,10061,2670,8000,1,1);
	Saga.FindQuestItem(cid,QuestID,StepID,10062,2670,8000,1,1);
	Saga.FindQuestItem(cid,QuestID,StepID,10059,2672,8000,1,2);
	Saga.FindQuestItem(cid,QuestID,StepID,10060,2672,8000,1,2);
	Saga.FindQuestItem(cid,QuestID,StepID,10064,2673,8000,1,3);
	Saga.FindQuestItem(cid,QuestID,StepID,10065,2673,8000,1,3);
	Saga.FindQuestItem(cid,QuestID,StepID,40005,2735,8000,1,4);


--check if all substeps are completed
	for i = 1, 4 do
	  if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false then
		return -1;
	end
end
	Saga.StepComplete(cid,QuestID,StepID);
	return 0;
end

function QUEST_STEP_2(cid)

	--Deliver material Averro Reinhold
	Saga.AddWaypoint(cid, QuestID, 1502, 1, 1004);

	--check for completion

	local ret = Saga.GetNPCIndex(cid);
	if ret == 1004 then
	Saga.GeneralDialog(cid, 3936);
	Saga.NpcTakeItem(cid, 2670, 1);
	Saga.NpcTakeItem(cid, 2672, 1);
	Saga.NpcTakeItem(cid, 2673, 1);
	Saga.NpcTakeItem(cid, 2735, 1);

	Saga.SubstepComplete(cid,QuestID,StepID,1);
      end

	--check if all substeps are complete
	for i = 1, 1 do
	     if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false then

		return -1;
	end
end

	Saga.ClearWaypoints(cid, QuestID);
	Saga.StepComplete(cid,QuestID,StepID)
	Saga.QuestComplete(cid, QuestID);
	return -1;
end
	
function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	local ret = -1;
	StepID = CurStepID;
	
	if CurStepID == 1501 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 1502 then
		ret = QUEST_STEP_2(cid);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid)
	end
	
	return ret;
end
