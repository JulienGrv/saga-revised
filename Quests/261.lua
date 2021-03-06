-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:16 PM

local QuestID = 261;
local ReqClv = 20;
local ReqJlv = 0;
local NextQuest = 0;
local RewZeny = 556;
local RewCxp = 2600;
local RewJxp = 1040;
local RewWxp = 0;
local RewItem1 = 1700114;
local RewItem2 = 0;
local RewItemCount1 = 2;
local RewItemCount2 = 0;
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_VERIFY(cid)
	return 0;
end

function QUEST_START(cid)
	-- Initialize all quest steps
	Saga.AddStep(cid, QuestID, 26101);
	Saga.AddStep(cid, QuestID, 26102);
	Saga.AddStep(cid, QuestID, 26103);

	Saga.InsertQuest(cid, QuestID, 1);
	return 0;
end

function QUEST_FINISH(cid)
	Saga.GiveZeny(cid, RewZeny);
	Saga.GiveExp(cid, RewCxp, RewJxp, RewWxp);
	Saga.GiveItem(cid, RewItem1, RewItemCount1);
	return 0;
end

function QUEST_CANCEL(cid)
	return 0;
end

function QUEST_STEP_1(cid, StepID)
	--Talk with Naihong
	Saga.AddWaypoint(cid, QuestID, StepID, 1, 1156);

	--Check for completion
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1156 then
		Saga.GeneralDialog(cid, 2766);
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
	return 0;
end

function QUEST_STEP_2(cid, StepID)
	--Eliminate Gangster Red Cape Cat(4);Eliminate Magician Red Cape Cat(3);
	Saga.Eliminate(cid, QuestID, StepID, 10100, 4, 1);
	Saga.Eliminate(cid, QuestID, StepID, 10101, 4, 1);

	Saga.Eliminate(cid, QuestID, StepID, 10104, 4, 2);
	Saga.Eliminate(cid, QuestID, StepID, 10105, 4, 2);

	-- Check if all substeps are completed
	for i = 1, 2 do
		if Saga.IsSubStepCompleted(cid, QuestID, StepID, i) == false then
			return -1;
		end
	end

	Saga.StepComplete(cid, QuestID, StepID);
	return 0;
end

function QUEST_STEP_3(cid, StepID)
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
	local CurStepID = Saga.GetStepIndex(cid, QuestID);
	local StepID = CurStepID;
	local ret = -1;

	if CurStepID == 26101 then
		ret = QUEST_STEP_1(cid, StepID);
	elseif CurStepID == 26102 then
		ret = QUEST_STEP_2(cid, StepID);
	elseif CurStepID == 26103 then
		ret = QUEST_STEP_3(cid, StepID);
	end

	if ret == 0 then
		QUEST_CHECK(cid);
	end

	return ret;
end

