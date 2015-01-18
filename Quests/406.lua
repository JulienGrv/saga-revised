-- Saga is Licensed under Creative Commons Attribution-NonCommerial-ShareAlike 3.0 License
-- http://creativecommons.org/licenses/by-nc-sa/3.0/
-- Generated By Quest Extractor on 2/8/2008 3:46:18 PM

local QuestID = 406;
local ReqClv = 1;
local ReqJlv = 0;
local NextQuest = 0;
local RewZeny = 7;
local RewCxp = 9;
local RewJxp = 0;
local RewWxp = 0; 
local RewItem1 = 300000; 
local RewItem2 = 0; 
local RewItemCount1 = 1; 
local RewItemCount2 = 0; 
local StepID = 0;

-- Modify steps below for gameplay

function QUEST_START(cid)
	Saga.AddStep(cid, QuestID, 40601);
	Saga.AddStep(cid, QuestID, 40602);
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
	-- Talk with Kwanto Randal
	Saga.AddWaypoint(cid,QuestID,StepID,1,1063);
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);    
	if ret == 1063 then
		Saga.GeneralDialog(cid, 3933);
		Saga.SubstepComplete(cid, QuestID, StepID, 1);
	end
	
    -- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false then
			return -1;
		 end
    end
	
	-- Clear waypoints
	Saga.ClearWaypoints(cid, QuestID);	
	Saga.StepComplete(cid,QuestID,StepID);	
	return 0;     
end

function QUEST_STEP_2(cid)
	-- Talk with Scacciano Morrigan
	Saga.AddWaypoint(cid,QuestID,StepID,1,1003);
	
	-- Check for completion
	local ret = Saga.GetNPCIndex(cid);    
	if ret == 1003 then
		Saga.GeneralDialog(cid, 3933);
		Saga.SubstepComplete(cid, QuestID, StepID, 1);
	end
	
    -- Check if all substeps are completed
    for i = 1, 1 do
         if Saga.IsSubStepCompleted(cid,QuestID,StepID,i) == false then
			return -1;
		 end
    end
	
	-- Clear waypoints
	Saga.ClearWaypoints(cid, QuestID);	
	Saga.StepComplete(cid,QuestID,StepID);	
	Saga.QuestComplete(cid,QuestID);
	return -1;     
end

function QUEST_CHECK(cid)
	-- Check all steps for progress
	local CurStepID = Saga.GetStepIndex(cid, QuestID );
	local ret = -1;
	StepID = CurStepID;
	
	if CurStepID == 40601 then
		ret = QUEST_STEP_1(cid);
	elseif CurStepID == 40602 then
		ret = QUEST_STEP_2(cid);
	end
	
	if ret == 0 then
		QUEST_CHECK(cid)
	end
	
	return ret;
end
