-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:16 PM

local QuestID = 263;
local ReqClv = 20;
local ReqJlv = 0;
local NextQuest = 0;
local RewZeny = 556;
local RewCxp = 1950;
local RewJxp = 0;
local RewWxp = 0;
local RewItem1 = 1700114;
local RewItem2 = 0;
local RewItemCount1 = 2;
local RewItemCount2 = 0;
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_START(cid)
	-- Initialize all quest steps
	Saga.AddStep(cid, QuestID, 26301);
	Saga.AddStep(cid, QuestID, 26302);

	Sage.InsertQuest(cid, QuestID, 1);
	return 0;
end

function QUEST_FINISH(cid)
	Saga.GiveZeny(RewZeny);
	Saga.GiveExp( RewCxp, RewJxp, RewWxp);
	Saga.GiveItem(cid, RewItem1, RewItemCount1 );
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
	--Talk to Monika Reynolds
	Saga.AddWaypoint(cid, QuestID, StepID, 1, 1012);

	--Check for completion
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1012 then
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

	if CurStepID == 26301 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 26302 then
		ret = QUEST_STEP_2(cid);
	end

	if ret == 0 then
		QUEST_CHECK(cid)
	end

	return ret;
end
