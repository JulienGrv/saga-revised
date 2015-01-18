-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:14 PM

local QuestID = 19;
local ReqClv = 8;
local ReqJlv = 0;
local NextQuest = 147;
local RewZeny = 165;
local RewCxp = 164;
local RewJxp = 0;
local RewWxp = 0; 
local RewItem1 = 1700113; 
local RewItem2 = 0; 
local RewItemCount1 = 3; 
local RewItemCount2 = 0; 

-- Modify steps below for gameplay

function QUEST_VERIFY(cid)
	Saga.GeneralDialog(cid, 3957);
	return 0;
end

function QUEST_START(cid)
	-- Initialize all quest steps
	-- Initialize all starting navigation points
	Saga.AddStep(cid, QuestID, 1901);
	Saga.AddStep(cid, QuestID, 1902);
    Saga.InsertQuest(cid, QuestID, 2);
	return 0;
end

function QUEST_FINISH(cid)
	local freeslots = Saga.FreeInventoryCount(cid, 0);
	if freeslots > 0 then
	Saga.GiveZeny(cid, RewZeny);
	Saga.GiveExp(cid, RewCxp, RewJxp, RewWxp);
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
	--talk with Zarko Ruzzoli
	Saga.AddWaypoint(cid, QuestID, 1901, 1, 1005);

	--check for completion
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1005 then
	Saga.GeneralDialog(cid, 3936);
	Saga.SubstepComplete(cid,QuestID,StepID,1);
	return 0;
	end
	
	-- check if all substeps are completed
	for i = 1, 1 do
	   if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false then
			return -1;
	end
end
    
	Saga.ClearWaypoints(cid, QuestID);
	Saga.StepComplete(cid,QuestID,StepID);
	return 0;
end

function QUEST_STEP_2(cid)
	--report result to Selphy Adriana
	Saga.AddWaypoint(cid, QuestID, 1902, 1, 1002);
	
	local ret = Saga.GetNPCIndex(cid);
	if ret == 1002 then
	Saga.GeneralDialog(cid, 3936);
	Saga.SubstepComplete(cid,QuestID,StepID,1);
		return 0;
	end
	
		-- check if all substeps are completed
	for i = 1, 1 do
	   if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false then
			return -1;
	end
end

	Saga.ClearWaypoints(cid, QuestID);
	Saga.StepComplete(cid,QuestID,StepID);
	Saga.QuestComplete(cid, QuestID);
	return -1;
end

function QUEST_CHECK(cid)
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	local ret = -1;
	StepID = CurStepID;
	
	if CurStepID == 1901 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 1902 then
		ret = QUEST_STEP_2(cid);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid)
	end
	
	return ret;
end
