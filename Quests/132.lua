-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:15 PM

local QuestID = 132;
local ReqClv = 6;
local ReqJlv = 0;
local NextQuest = 0;
local RewZeny = 60;
local RewCxp = 224;
local RewJxp = 91;
local RewWxp = 0; 
local RewItem1 = 1700113; 
local RewItem2 = 16118; 
local RewItemCount1 = 1; 
local RewItemCount2 = 1; 

-- Modify steps below for gameplay

function QUEST_START(cid)
	-- Initialize all quest steps
	-- Initialize all starting navigation points
	Saga.Addstep(cid, QuestID, 13201);
	Saga.Addstep(cid, QuestID, 13202);
	Saga.InsertQuest(cid, QuestID, 1);
	return 0;
end
function QUEST_FINISH(cid)
	local freeslots = Saga.FreeInventoryCount(cid, 0);
	if freeslots > 1 then
	Saga.GiveZeny(RewZeny);
	Saga.GiveExp( RewCxp, RewJxp, RewWxp);
	Saga.GiveItem(cid, RewItem1, RewItemCount1 );
	Saga.GiveItem(cid, RewItem2, RewItemCount2 );
	return 0;
else
	return -1;
	end

end
function QUEST_CANCEL(cid)
	return 0;
end

function QUEST_STEP_1(cid)
--Talk with Averro Reinhold

	Saga.AddWaypoint(cid, QuestID, 13201, 1, 1004
--check for completion
	local ret = Saga.GetNpcIndex(cid)
	if ret == 1004
then
	Saga.GeneralDialog(cid, 3936);
	Saga.SubStepComplete(cid, QuestID, 13201, 1);
	Saga.ClearWaypoints(cid, QuestID);
	end

	Saga.FindQuestItem(cid, QuestID, 13201, 10042, 2657, 8000, 5, 2);
	Saga.FindQuestItem(cid, QuestID, 13201, 10043, 2657, 8000, 5, 2); 

--check if all substeps are complete
	for i = 1, 2 do
	if Saga.IsSubStepcompleted(cid, QuestID, 13201, i) == false
then
	return -1;
	end
end 
	Saga.StepComplete(cid, QuestID, 13201);
	return 0;
end

function QUEST_STEP_2(cid)
--Report to Kundi
	Saga.AddWaypoint(cid, QuestID, 13202, 1, 1066);
--check for completion
	local ret = Saga.GetNpcIndex(cid);
	ItemCount = Saga.CheckUserInventory(cid, 2657);
	if ret == 1066 and
	ItemCount > 4
then
	Saga.GeneralDialog(cid, 3936);
	Saga.NpcTakeItem(cid, 2657, 5);
	Saga.SubStepComplete(cid, QuestID, 13202, 1);
	end
end
--check if all substeps are complete
	for i = 1, 1 do
	if Saga.IsSubStepcompleted(cid, QuestID, 13202, i) == false
then
	return -1;
	end
end 
	Saga.StepComplete(cid, QuestID, 13202);
	Saga.ClearWaypoints(cid, QuestID);
	Saga.QuestComplete(cid, QuestID);
end
function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	local ret = -1;

	if CurStepID == 13201 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 13202 then
		ret = QUEST_STEP_2(cid);
	end

	if ret == 0 then
		QUEST_CHECK(cid)
	end

	return ret;
end