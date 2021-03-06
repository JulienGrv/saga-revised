-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:18 PM

local QuestID = 402;
local ReqClv = 1;
local ReqJlv = 0;
local NextQuest = 406;
local RewZeny = 10;
local RewCxp = 9;
local RewJxp = 0;
local RewWxp = 0;
local RewItem1 = 1700113;
local RewItem2 = 0;
local RewItemCount1 = 1;
local RewItemCount2 = 0;
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_VERIFY(cid)
	return 0;
end

function QUEST_START(cid)
	Saga.AddStep(cid, QuestID, 40201);
	Saga.AddStep(cid, QuestID, 40202);
	Saga.InsertQuest(cid, QuestID, 1);
	return 0;
end

function QUEST_FINISH(cid)
	-- Gives all rewards
	local freeslots = Saga.FreeInventoryCount(cid, 0);
	if freeslots > 0 then
		Saga.GiveZeny(cid, RewZeny);
		Saga.GiveExp(cid, RewCxp, RewJxp, RewWxp);
		Saga.GiveItem(cid, RewItem1, RewItemCount1);
		return 0;
	else
		Saga.EmptyInventory(cid);
		return -1;
	end
end

function QUEST_CANCEL(cid)
	return 0;
end

function QUEST_STEP_1(cid, StepID)
	-- Get 6 Chonchon Fry Wings
	Saga.FindQuestItem(cid, QuestID, StepID, 10026, 2630, 8000, 3, 1);
	Saga.FindQuestItem(cid, QuestID, StepID, 10027, 2630, 8000, 3, 1);
	Saga.FindQuestItem(cid, QuestID, StepID, 10252, 2630, 8000, 3, 1);
	
	-- Check if all substeps are completed
	for i = 1, 1 do
		if Saga.IsSubStepCompleted(cid, QuestID, StepID, i) == false then
			return -1;
		end
	end
	
	Saga.StepComplete(cid, QuestID, StepID);
	return 0;
end

function QUEST_STEP_2(cid, StepID)
	-- Deliver 'Wings' to Kwan
	Saga.AddWaypoint(cid, QuestID, StepID, 1, 1063);
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1063 then
		Saga.GeneralDialog(cid, 3954);
	
		local ItemCountA = Saga.CheckUserInventory(cid, 2630);
		if ItemCountA > 2 then
			Saga.NpcTakeItem(cid, 2630, 3);
			Saga.SubstepComplete(cid, QuestID, StepID, 1);
		else
			Saga.ItemNotFound(cid);
		end
	end
	
	-- Check if all substeps are completed
	for i = 1, 1 do
		if Saga.IsSubStepCompleted(cid, QuestID, StepID, i) == false then
			return -1;
		end
	end
	
	-- Clear waypoints
	Saga.ClearWaypoints(cid, QuestID);
	Saga.StepComplete(cid, QuestID, StepID);
	Saga.QuestComplete(cid, QuestID);
	return -1;
end

function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID);
	local ret = -1;
	local StepID = CurStepID;
	
	if CurStepID == 40201 then
		ret = QUEST_STEP_1(cid, StepID);
	elseif CurStepID == 40202 then
		ret = QUEST_STEP_2(cid, StepID);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid);
	end
	
	return ret;
end
