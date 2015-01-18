-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:17 PM

local QuestID = 267;
local ReqClv = 17;
local ReqJlv = 0;
local NextQuest = 268;
local RewZeny = 446;
local RewCxp = 1900;
local RewJxp = 770;
local RewWxp = 0;
local RewItem1 = 0;
local RewItem2 = 0;
local RewItemCount1 = 0;
local RewItemCount2 = 0;
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_START(cid)
	-- Initialize all quest steps
	Saga.AddStep(cid, QuestID, 26701);
	Saga.AddStep(cid, QuestID, 26702);
	Saga.AddStep(cid, QuestID, 26703);

	Sage.InsertQuest(cid, QuestID, 1);
	return 0;
end

function QUEST_FINISH(cid)
	Saga.GiveZeny(RewZeny);
	Saga.GiveExp( RewCxp, RewJxp, RewWxp);

	Saga.InsertQuest(cid, NextQuest, 1);
	return 0;
end

function QUEST_CANCEL(cid)
	return 0;
end

function QUEST_STEP_1(cid)
	--Talk with Naihong
	Saga.AddWaypoint(cid, QuestID, StepID, 1, 1156);

	--Check for completion
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1156 then
		Saga.GeneralDialog(cid, 3936);
		Saga.SubstepComplete(cid, QuestID, StepID, 1);
	end

	-- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,StepID, i) == false then
			return -1;
		 end
    end

    Saga.StepComplete(cid, QuestID, StepID);
	Saga.ClearWaypoints(cid, QuestID);
	return 0;
end

function QUEST_STEP_2(cid)
	-- Eliminate Gangster Yellow Cape Cat (4);Eliminate Footpad Yellow Cape Cat (4);
	Saga.Eliminate(cid, QuestID, StepID, 10289, 4, 1);
	Saga.Eliminate(cid, QuestID, StepID, 10290, 4, 1);
	Saga.Eliminate(cid, QuestID, StepID, 10291, 4, 2);
	Saga.Eliminate(cid, QuestID, StepID, 10292, 4, 2);

    -- Check if all substeps are completed
    for i = 1, 2 do
         if Saga.IsSubStepCompleted(cid,QuestID,StepID, i) == false then
			return -1;
		 end
    end

    Saga.StepComplete(cid, QuestID, StepID);
    return 0;
end

function QUEST_STEP_3(cid)
	--Talk with Naihong
	Saga.AddWaypoint(cid, QuestID, StepID, 1, 1156);

	--Check for completion
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1156 then
		Saga.GeneralDialog(cid, 3936);
		Saga.SubstepComplete(cid, QuestID, StepID, 1);
	end

	-- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,StepID, i) == false then
			return -1;
		 end
    end

    Saga.StepComplete(cid, QuestID, StepID);
	Saga.ClearWaypoints(cid, QuestID);
	Sage.QuestComplete(cid, QuestID);
	return -1;
end

function QUEST_CHECK(cid)
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	StepID = CurStepID;
	local ret = -1;

	if CurStepID == 26701 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 26702 then
		ret = QUEST_STEP_2(cid);
	elseif CurStepID == 26703 then
		ret = QUEST_STEP_3(cid);
	end

	if ret == 0 then
		QUEST_CHECK(cid)
	end

	return ret;
end