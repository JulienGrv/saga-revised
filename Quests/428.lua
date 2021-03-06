-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:18 PM

local QuestID = 428;
local ReqClv = 31;
local ReqJlv = 0;
local NextQuest = 0;
local RewZeny = 1526;
local RewCxp = 7860;
local RewJxp = 3132;
local RewWxp = 0;
local RewItem1 = 1700115;
local RewItem2 = 0;
local RewItemCount1 = 3;
local RewItemCount2 = 0;
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_VERIFY(cid)
	return 0;
end

function QUEST_START(cid)
	Saga.AddStep(cid, QuestID, 42801);
	Saga.AddStep(cid, QuestID, 42802);
	Saga.AddStep(cid, QuestID, 42803);
	Saga.AddStep(cid, QuestID, 42804);
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
	-- Talk with Aili
	Saga.AddWaypoint(cid, QuestID, StepID, 1, 1028);
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1028 then
		Saga.GeneralDialog(cid, 4727);
		Saga.SubstepComplete(cid, QuestID, StepID, 1);
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
	return 0;
end

function QUEST_STEP_2(cid, StepID)
	-- Gather Needed Bolt (8)
	Saga.FindQuestItem(cid, QuestID, StepID, 10353, 4238, 8000, 8, 1);
	Saga.FindQuestItem(cid, QuestID, StepID, 10354, 4238, 8000, 8, 1);
	
	-- Check if all substeps are completed
	for i = 1, 1 do
		if Saga.IsSubStepCompleted(cid, QuestID, StepID, i) == false then
			return -1;
		end
	end
	
	Saga.StepComplete(cid, QuestID, StepID);
	return 0;
end

function QUEST_STEP_3(cid, StepID)
	-- Deliver item to Moritz Blauvelt
	Saga.AddWaypoint(cid, QuestID, StepID, 1, 1026);
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1026 then
		Saga.GeneralDialog(cid, 4730);

		local ItemCountA = Saga.CheckUserInventory(cid, 4238);
		if ItemCountA > 7 then
			Saga.NpcTakeItem(cid, 4238, 8);
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
	return 0;
end

function QUEST_STEP_4(cid, StepID)
	-- Hand in to Kafra Board Mailbox
	local ret = Saga.GetActionObjectIndex(cid);
	if ret == 1123 then
		Saga.SubstepComplete(cid, QuestID, StepID, 1);
	end
	
	-- Check if all substeps are completed
	for i = 1, 1 do
		if Saga.IsSubStepCompleted(cid, QuestID, StepID, i) == false then
			return -1;
		end
	end
	
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
	
	if CurStepID == 42801 then
		ret = QUEST_STEP_1(cid, StepID);
	elseif CurStepID == 42802 then
		ret = QUEST_STEP_2(cid, StepID);
	elseif CurStepID == 42803 then
		ret = QUEST_STEP_3(cid, StepID);
	elseif CurStepID == 42804 then
		ret = QUEST_STEP_4(cid, StepID);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid);
	end
	
	return ret;
end
